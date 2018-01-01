using System;
using System.IO;
using System.Text;
using System.Xml;
using Xunit;

namespace XsltTests
{
    public class JUnitXsltTests
    {
        [Fact]
        public void GivenValidInput_WhenTransform_ThenSaveWithoutBom()
        {
            // Arrange

            const string inputFileName = "passed-test";

            // Act

            var actual = JUnitXslt.Transform(inputFileName);

            // Assert
            
            Assert.Equal('<', actual[0]);
        }
        
        [Fact]
        public void GivenPassedTest_WhenTransform_ThenGeneratePassedMarkup()
        {
            // Arrange

            const string inputFileName = "passed-test";

            // Act

            var actual = JUnitXslt.Transform(inputFileName);

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

            var actual = JUnitXslt.Transform(inputFileName);

            // Assert

            var expected = GetExpected(inputFileName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GivenFailedTest_WhenTransform_ThenGenerateFaileddMarkup()
        {
            // Arrange

            const string inputFileName = "failed-test";

            // Act

            var actual = JUnitXslt.Transform(inputFileName);

            // Assert

            var expected = GetExpected(inputFileName);

            Assert.Equal(expected, actual);
        }

        private static string GetExpected(string inputFileName)
        {
            return File.ReadAllText($"./expected/{inputFileName}.xml");
        }
    }
}