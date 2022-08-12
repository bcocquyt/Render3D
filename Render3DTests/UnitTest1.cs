using Render3DLib;

namespace Render3DTests
{
    public class UnitTest1
    {
        string testLine = string.Empty;
        string expected = string.Empty;
        string testResult = string.Empty;


        [Fact]
        public void CanRemoveCommentBetweenBrackets()
        {
            testLine = "%";
            expected = "";
            testResult = GCodeSanitizer.Sanitize(testLine);
            Assert.Equal(expected, testResult);
        }

        [Fact]
        public void CanRemoveCommentAfterPercentage()
        {
            testLine = "%Commentaar abc def";
            expected = "";
            testResult = GCodeSanitizer.Sanitize(testLine);
            Assert.Equal(expected, testResult);
        }

        [Fact]
        public void CanRemoveCommentAfterCommand()
        {
            testLine = "G01 Z-1.000000 F100.0(Penetrate)";
            expected = "G01 Z-1.000000 F100.0";
            testResult = GCodeSanitizer.Sanitize(testLine);
            Assert.Equal(expected, testResult);
        }

        [Fact]
        public void CanRemoveEOLCommentAfterCommand()
        {
            testLine = "G01 Z-1.000000 F100.0%Penetrate";
            expected = "G01 Z-1.000000 F100.0";
            testResult = GCodeSanitizer.Sanitize(testLine);
            Assert.Equal(expected, testResult);
        }
    }
}