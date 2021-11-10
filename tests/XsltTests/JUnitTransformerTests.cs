using System;
using System.IO;
using System.Text;
using xUnitToJUnit;
using Xunit;

namespace XsltTests
{
    public class JUnitTransformerTests : IDisposable
    {
        private const string ExistingInputFilePath = "input/passed-test.xml";
        private const string OutputDirectory = "output";

        [Fact]
        public void GivenOnlyFileNameForOutputPath_WhenTransform_ThenCreateFile()
        {
            // Act

            JUnitTransformer.Transform(ExistingInputFilePath, "junit.xml");

            // Assert

            Assert.True(File.Exists("junit.xml"));
        }

        [Fact]
        public void GivenNonExistingOutputDirectory_WhenTransform_ThenCreateDirectory()
        {
            // Arrange

            var outputFilePath = $"{OutputDirectory}/circle-ci/junit.xml";
            Assert.False(Directory.Exists(OutputDirectory));

            // Act

            JUnitTransformer.Transform(ExistingInputFilePath, outputFilePath);

            // Assert

            Assert.True(File.Exists(outputFilePath));
        }

        [Fact]
        public void GivenValidInput_WhenTransform_ThenSaveWithoutBom()
        {
            // Arrange

            const string inputFileName = "passed-test";

            // Act

            var actual = Transform(inputFileName);

            // Assert

            Assert.Equal('<', actual[0]);
        }

        [Fact]
        public void GivenPassedTest_WhenTransform_ThenGeneratePassedMarkup()
        {
            // Arrange

            const string inputFileName = "passed-test";

            // Act

            var actual = Transform(inputFileName);

            // Assert

            var expected = GetExpected(inputFileName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GivenInlineData_WhenTransform_ThenIncludeInlineDataInName()
        {
            // Arrange

            const string inputFileName = "inline-data-test";

            // Act

            var actual = Transform(inputFileName);

            // Assert

            var expected = GetExpected(inputFileName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GivenDisplayName_WhenTransform_ThenUseDisplayNameInsteadOfEmptyString()
        {
            // Arrange

            const string inputFileName = "display-name-test";

            // Act

            var actual = Transform(inputFileName);

            // Assert

            var expected = GetExpected(inputFileName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GivenSkippedTest_WhenTransform_ThenGenerateSkippedMarkup()
        {
            // Arrange

            const string inputFileName = "skipped-test";

            // Act

            var actual = Transform(inputFileName);

            // Assert

            var expected = GetExpected(inputFileName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GivenFailedTest_WhenTransform_ThenGenerateFailedMarkup()
        {
            // Arrange

            const string inputFileName = "failed-test";

            // Act

            var actual = Transform(inputFileName);

            // Assert

            var expected = GetExpected(inputFileName);

            Assert.Equal(expected, actual);
        }

        private static string Transform(string inputFileName)
        {
            using (var stream = new MemoryStream())
            {
                JUnitTransformer.Transform($"./input/{inputFileName}.xml", stream);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static string GetExpected(string inputFileName)
        {
            return File.ReadAllText($"./expected/{inputFileName}.xml");
        }

        public void Dispose()
        {
            if (Directory.Exists(OutputDirectory))
            {
                Directory.Delete(OutputDirectory, true);
            }
        }
    }
}
