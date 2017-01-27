using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KendoUIAppUnitTest
{
    [TestClass]
    public class ParsingTest
    {
        private const string ParseItemUrl = "https://www.sapato.ru/11144426";
        private const string ParseOnePageUrl = "https://www.sapato.ru/woman/?page=2&page_size=60";
        private const string ParseAllPageUrl = "https://www.sapato.ru/woman/";
        private readonly ParseContentRepository _parseContent = new ParseContentRepository();

        [TestMethod]
        public void ParsingItem()
        {
            var itemObj = _parseContent.ParseItem(ParseItemUrl);
            Assert.AreEqual(itemObj.ImageUrls.Count, 4); // Page Shown 4 Images
            Assert.AreEqual(itemObj.Id, "11144426");
        }

        [TestMethod]
        public void ParsingPage()
        {
            var itemObj = _parseContent.ParsePage(ParseOnePageUrl);
        }

        [TestMethod]
        public void ParsingSection()
        {
            var itemObj = _parseContent.ParseAllPages(ParseAllPageUrl);
        }

        [TestMethod]
        public void ParsingNewItem()
        {
            const string url = @"https://www.sapato.ru/11155918";
            var itemObj = _parseContent.ParseItem(url);
        }
    }
}