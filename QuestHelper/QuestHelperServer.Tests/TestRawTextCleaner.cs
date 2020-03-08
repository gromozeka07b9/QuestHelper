using QuestHelper.Server.Integration;
//using Xunit;

namespace QuestHelperServer.Tests
{
    public class TestRawTextCleaner
    {
        /*[Theory]
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
        [InlineData("Проверка ввода запятая где используются запятые дефис но дефисы так же")]
        public void TestMust_CleanTextWithCyrillicComma(string rawText)
        {
            RawTextCleaner cleaner = new RawTextCleaner();
            string cleanText = cleaner.Clean(rawText);
            Assert.True(cleanText.StartsWith('П'));
            Assert.Contains(",", cleanText);
            Assert.True(cleanText.EndsWith('.'));
        }

        [Theory]
        [InlineData("Recognize text without do_ts and com_mas")]
        public void TestMust_CleanTextWithoutDotAndComma(string rawText)
        {
            RawTextCleaner cleaner = new RawTextCleaner();
            string cleanText = cleaner.Clean(rawText);
            Assert.True(cleanText.StartsWith('R'));
            Assert.True(cleanText.EndsWith('.'));
        }

        [Theory]
        [InlineData("Recognize text with two sentences dot and it second sentece comma with comm")]
        public void TestMust_CleanTextTwoSentenses(string rawText)
        {
            RawTextCleaner cleaner = new RawTextCleaner();
            string cleanText = cleaner.Clean(rawText);
            Assert.True(cleanText.StartsWith('R'));
            Assert.True(cleanText.EndsWith('.'));
        }

        [Theory]
        [InlineData("")]
        public void TestMust_CleanTextWithEmpty(string rawText)
        {
            RawTextCleaner cleaner = new RawTextCleaner();
            string cleanText = cleaner.Clean(rawText);
            Assert.True(string.IsNullOrEmpty(cleanText));
        }

        [Fact]
        public void TestMust_Work()
        {

        }*/
    }
}
