using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ICMLookup
{
    public class Lookup
    {
        private static readonly Dictionary<CodeType, HashSet<ICMCode>> codes;

        static Lookup()
        {
            codes = new Dictionary<CodeType, HashSet<ICMCode>>
            {
                { CodeType.ICM9Diag, LoadCodes(EmbeddedService.LoadFile(@"ICM-9-Diagnosis.csv")) },
                { CodeType.ICM10Diag, LoadCodes(EmbeddedService.LoadFile(@"ICD10 diag.csv")) },
                { CodeType.ICM9Proc, LoadCodes(EmbeddedService.LoadFile(@"ICM-9-Procedures.csv")) },
                { CodeType.ICM10Proc, LoadCodes(EmbeddedService.LoadFile(@"ICD10 procs.csv")) }
            };
        }

        /// <summary>
        /// Parses the lines from the CSV file and creates ICMCode objects.
        /// </summary>
        /// <param name="lines">The lines from the CSV file.</param>
        /// <returns>A HashSet of ICMCode objects.</returns>
        private static HashSet<ICMCode> LoadCodes(string fileContents)
        {
            var set = new HashSet<ICMCode>();

            using (var reader = new StringReader(fileContents))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var index = line.IndexOf(',');
                    var code = line.Substring(0, index).Trim();
                    var description = line.Substring(index + 1).Trim(' ', '"');
                    set.Add(new ICMCode(code, description));
                }
            }

            return set;
        }

        /// <summary>
        /// Finds an ICM code in the specified code set or in all code sets if no code set is specified.
        /// </summary>
        /// <param name="code">The ICM code to find.</param>
        /// <param name="codeType">The type of the code set to search in, or null to search in all code sets.</param>
        /// <returns>The ICM code if found, or null if not found.</returns>
        /// <example>
        /// <code>
        /// var lookup = new ICMLookup();
        /// var code = lookup.Find("A000", CodeType.ICM10Diag);
        /// </code>
        /// </example>
        public ICMCode Find(string code, CodeType? codeType = null)
        {
            string normalizedCode = new string(code.Where(char.IsLetterOrDigit).ToArray()).ToUpper();

            return codeType.HasValue ? codes[codeType.Value].FirstOrDefault(icmCode => icmCode.NormalizedCode == normalizedCode) 
                : codes.Values.SelectMany(set => set).FirstOrDefault(icmCode => icmCode.NormalizedCode == normalizedCode);
        }


        /// <summary>
        /// Finds the top 10 ICMCodes that are closest to the specified code using the Levenshtein distance.
        /// </summary>
        /// <param name="code">The code to search for.</param>
        /// <returns>The top 10 ICMCodes that are closest to the specified code. If no codes are found, returns an empty list.</returns>
        public List<ICMCode> Search(string code)
        {
            string normalizedCode = new string(code.Where(char.IsLetterOrDigit).ToArray()).ToUpper();
            return codes.Values.SelectMany(set => set)
                .Select(icmCode => new { ICMCode = icmCode, Distance = ComputeLevenshteinDistance(icmCode.NormalizedCode, normalizedCode) })
                .OrderBy(x => x.Distance)
                .Take(10)
                .Select(x => x.ICMCode)
                .ToList();
        }

        /// <summary>
        /// Computes the Levenshtein distance between two strings.
        /// </summary>
        /// <param name="a">The first string.</param>
        /// <param name="b">The second string.</param>
        /// <returns>The Levenshtein distance between the two strings.</returns>
        private static int ComputeLevenshteinDistance(string a, string b)
        {
            var matrix = new int[a.Length + 1, b.Length + 1];

            for (var i = 0; i <= a.Length; i++)
                matrix[i, 0] = i;

            for (var j = 0; j <= b.Length; j++)
                matrix[0, j] = j;

            for (var i = 1; i <= a.Length; i++)
                for (var j = 1; j <= b.Length; j++)
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + ((a[i - 1] == b[j - 1]) ? 0 : 1));

            return matrix[a.Length, b.Length];
        }

        /// <summary>
        /// Returns a specified number of random ICM codes from the specified code set.
        /// </summary>
        /// <param name="codeType">The type of the code set to get samples from.</param>
        /// <param name="count">The number of random codes to return.</param>
        /// <returns>A list of random ICM codes from the specified code set.</returns>
        /// <exception cref="ArgumentException">Thrown when the count is greater than the number of codes in the specified code set.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the specified code set does not exist.</exception>
        /// <example>
        /// <code>
        /// var lookup = new ICMLookup();
        /// var samples = lookup.GetSamples(CodeType.ICM10Diag, 5);
        /// </code>
        /// </example>
        public List<ICMCode> GetSamples(CodeType codeType, int count)
        {
            if (!codes.ContainsKey(codeType))
            {
                throw new KeyNotFoundException($"The specified code set {codeType} does not exist.");
            }

            var codeSet = codes[codeType];

            if (count > codeSet.Count)
            {
                throw new ArgumentException($"The specified count {count} is greater than the number of codes in the specified code set.");
            }

            var random = new Random();
            return codeSet.OrderBy(code => random.Next()).Take(count).ToList();
        }
    }
}
