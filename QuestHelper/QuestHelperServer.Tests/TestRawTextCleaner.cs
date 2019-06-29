using QuestHelper.Server.Integration;
using Xunit;

namespace QuestHelperServer.Tests
{
    public class TestRawTextCleaner
    {
        [Theory]
        [InlineData("this is example of text block comma where we must find commas and set to up first letter and point at end dot")]
        public void TestMust_CleanTextWithComma(string rawText)
        {
            RawTextCleaner cleaner = new RawTextCleaner();
            string cleanText = cleaner.Clean(rawText);
            Assert.True(cleanText.StartsWith('T'));
            Assert.Contains(",", cleanText);
            Assert.True(cleanText.EndsWith('.'));
        }

        [Theory]
        [InlineData("one comma two comma three comma and hyphen as example")]
        public void TestMust_CleanTextWithMultipleCommas(string rawText)
        {
            RawTextCleaner cleaner = new RawTextCleaner();
            string cleanText = cleaner.Clean(rawText);
            Assert.True(cleanText.StartsWith('O'));
            Assert.Contains(",", cleanText);
            Assert.True(cleanText.EndsWith('.'));
        }

        [Theory]
        [InlineData("?????? ??????????? ??????? ? ???????")]
        public void TestMust_CleanTextWithCyrillicComma(string rawText)
        {
            RawTextCleaner cleaner = new RawTextCleaner();
            string cleanText = cleaner.Clean(rawText);
            Assert.True(cleanText.StartsWith('?'));
            Assert.Contains(",", cleanText);
            Assert.True(cleanText.EndsWith('.'));
        }
        [Fact]
        public void TestMust_Work()
        {

        }
    }
}
