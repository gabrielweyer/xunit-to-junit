namespace Gabo.DotNet.xUnitToJUnit.Tests;

public class ProgramTests : IDisposable
{
    private const string ExistingInputFilePath = "input/passed-test.xml";
    private const string OutputDirectory = "output";

    [Fact]
    public void GivenNoArgumentProvided_ThenFailureExitCode()
    {
        // Arrange
        var input = Array.Empty<string>();

        // Act
        var actualExitCode = Program.Main(input);

        // Assert
        actualExitCode.Should().Be(Program.FailureExitCode);
    }

    [Fact]
    public void GivenSingleArgumentProvided_ThenFailureExitCode()
    {
        // Arrange
        var input = new[] { ExistingInputFilePath };

        // Act
        var actualExitCode = Program.Main(input);

        // Assert
        actualExitCode.Should().Be(Program.FailureExitCode);
    }

    [Fact]
    public void GivenMoreThanTwoArgumentsProvided_ThenFailureExitCode()
    {
        // Arrange
        var input = new[] { ExistingInputFilePath, ExistingInputFilePath, ExistingInputFilePath };

        // Act
        var actualExitCode = Program.Main(input);

        // Assert
        actualExitCode.Should().Be(Program.FailureExitCode);
    }

    [Fact]
    public void GivenTwoValidArgumentsProvided_ThenSuccessExitCode()
    {
        // Arrange
        var input = new[] { ExistingInputFilePath, "valid-arguments.xml" };

        // Act
        var actualExitCode = Program.Main(input);

        // Assert
        actualExitCode.Should().Be(Program.SuccessExitCode);
    }

    public void Dispose()
    {
        if (Directory.Exists(OutputDirectory))
        {
            Directory.Delete(OutputDirectory, true);
        }
    }
}
