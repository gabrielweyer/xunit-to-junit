using System;

namespace xUnitToJUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Two arguments should be provided:");
                Console.WriteLine(
                    "dotnet ./xunit-to-junit.dll \"path-to-xunit-test-results.xml\" \"desired-path-to-junit-test-results.xml\"");
                return;
            }

            var xUnitTestResultsFilePath = args[0];
            var jUnitTestResultsFilePath = args[1];
            
            JUnitTransformer.Transform(xUnitTestResultsFilePath, jUnitTestResultsFilePath);

            Console.WriteLine(
                $"The xUnit test results file \"{xUnitTestResultsFilePath}\" has been converted to the JUnit test results file \"{jUnitTestResultsFilePath}\"");
        }
    }
}