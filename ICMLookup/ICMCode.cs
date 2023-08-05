using System.Linq;

namespace ICMLookup
{
    public class ICMCode
    {
        public ICMCode(string code, string description)
        {
            Code = code;
            Description = description;
        }

        public string Code { get; set; }
        public string Description { get; set; }
        public string NormalizedCode => new string(Code.Where(char.IsLetterOrDigit).ToArray()).ToUpper();

        /// <summary>
        /// Returns a string that represents the current <see cref="ICMCode"/>.
        /// </summary>
        /// <returns>A string in the format: "Code:Description".</returns>
        public override string ToString() => $"{Code}:{Description}";
    }
}