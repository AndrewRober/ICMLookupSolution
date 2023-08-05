using System;
using System.Linq;
using System.Reflection.Emit;

using ICMLookup;

namespace ICMLookupConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var icmLookup = new Lookup();

            Console.WriteLine("ICM Lookup Console App");
            Console.WriteLine("----------------------");

            while (true)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Find an ICM Code");
                Console.WriteLine("2. Search for ICM Codes");
                Console.WriteLine("3. Get ICM Code Samples");
                Console.WriteLine("4. Exit");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.WriteLine("Enter the ICM Code:");
                        var code = Console.ReadLine();
                        Console.WriteLine("Enter the Code Type (0 for ICM9Diag, 1 for ICM10Diag, 2 for ICM9Proc, 3 for ICM10Proc):");
                        var codeType = (CodeType)int.Parse(Console.ReadLine());

                        var result = icmLookup.Find(code, codeType);

                        if (result != null)
                        {
                            Console.WriteLine($"Found ICM Code: {result.Code}, Description: {result.Description}");
                        }
                        else
                        {
                            Console.WriteLine("ICM Code not found.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("Enter the search term:");
                        var searchTerm = Console.ReadLine();

                        var results = icmLookup.Search(searchTerm);

                        if (results.Count > 0)
                        {
                            Console.WriteLine("Found ICM Codes:");
                            foreach (var icmCode in results)
                            {
                                Console.WriteLine($"Code: {icmCode.Code}, Description: {icmCode.Description}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No ICM Codes found.");
                        }
                        break;

                    case "3":
                        Console.WriteLine("Enter the Code Type (0 for ICM9Diag, 1 for ICM10Diag, 2 for ICM9Proc, 3 for ICM10Proc):");
                        var sampleCodeType = (CodeType)int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the number of samples:");
                        var count = int.Parse(Console.ReadLine());

                        var samples = icmLookup.GetSamples(sampleCodeType, count);

                        Console.WriteLine("ICM Code Samples:");
                        foreach (var icmCode in samples)
                        {
                            Console.WriteLine($"Code: {icmCode.Code}, Description: {icmCode.Description}");
                        }
                        break;

                    case "4":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.WriteLine();
                        break;
                }

                Console.WriteLine();
            }
        }
    }
}