using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ICMLookup
{
    /// <summary>
    /// Provides a service for handling embedded resources in the assembly.
    /// </summary>
    public static class EmbeddedService
    {
        /// <summary>
        /// Loads the content of an embedded resource file from the assembly.
        /// </summary>
        /// <param name="fileName">The name of the embedded resource file to load.</param>
        /// <returns>A string representing the content of the embedded resource file.</returns>
        /// <exception cref="Exception">Thrown when the specified resource file is not found in the assembly.</exception>
        /// <example>
        /// <code>
        /// var fileContent = EmbeddedService.LoadFile("ICMCodes.csv");
        /// Console.WriteLine(fileContent);
        /// </code>
        /// </example>
        public static string LoadFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.Contains(fileName));

            if (resourceName == null)
            {
                throw new Exception($"Resource {fileName} not found.");
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}