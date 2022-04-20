namespace Gabo.DotNet.xUnitToJUnit;

internal static class Program
{
    internal const int SuccessExitCode = 0;
    internal const int FailureExitCode = 1;

    internal static int Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Two arguments should be provided:");
            Console.WriteLine(
                "dotnet xunit-to-junit \"path-to-xunit-test-results.xml\" \"desired-path-to-junit-test-results.xml\"");
            return FailureExitCode;
        }

        var xUnitTestResultsFilePath = args[0];
        var jUnitTestResultsFilePath = args[1];

        JUnitTransformer.Transform(xUnitTestResultsFilePath, jUnitTestResultsFilePath);

        Console.WriteLine(
            $"The xUnit test results file \"{xUnitTestResultsFilePath}\" has been converted to the JUnit test results file \"{jUnitTestResultsFilePath}\"");

        return SuccessExitCode;
    }
}
