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

        private const string BashmagParseItemUrl =
            "https://www.bashmag.ru/men/36-obuv_/41-botinki/1751-Botinki-liga-detail";

        [TestMethod]
        public void ParsingItem()
        {
            var itemObj2 = _parseContent.ParseItem("https://www.sapato.ru/11155918");
            var itemObj = _parseContent.ParseItem(ParseItemUrl);
            Assert.AreEqual(itemObj.ImageUrls.Count, 4); // Page Shown 4 Images
            Assert.AreEqual(itemObj.Id, "11144426");
        }

        [TestMethod]
        public void ParsingPage()
        {
            _parseContent.ParsePage(ParseOnePageUrl, Website.Sapato);
        }

        [TestMethod]
        public void ParsingSection()
        {
            _parseContent.ParseAllPages(ParseAllPageUrl, Website.Sapato);
        }

        [TestMethod]
        public void ParsingNewItem()
        {
            const string url = @"https://www.sapato.ru/11155918";
            _parseContent.ParseItem(url, Website.Sapato);
        }

        [TestMethod]
        public void ParsingBashmagItem()
        {
            _parseContent.ParseItem(BashmagParseItemUrl, Website.Bashmag);
        }
    }
}