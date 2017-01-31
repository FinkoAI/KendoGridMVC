using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KendoUIAppUnitTest
{
    [TestClass]
    public class SapatoParserTest
    {
        private const string ParseItemUrl = "https://www.sapato.ru/11144426";
        private const string ParseOnePageUrl = "https://www.sapato.ru/woman/?page=2&page_size=60";
        private const string ParseAllPageUrl = "https://www.sapato.ru/woman/";

        private readonly ParseContentRepository _parseContent = new ParseContentRepository(Website.Sapato);

        [TestMethod]
        public void SapatoItemParse()
        {
            var itemObj = _parseContent.ParseItem(ParseItemUrl);
            Assert.AreEqual(itemObj.Id, "11144426");
            Assert.AreEqual(itemObj.ImageUrls.Count, 4,"4 images are available");
            Assert.AreEqual(itemObj.Type, "Полусапоги");
            Assert.AreEqual(itemObj.SubType, "Сапоги");
            Assert.AreEqual(itemObj.Brand, " BON TON");
            Assert.AreEqual(itemObj.Price, 6450);
            Assert.AreEqual(itemObj.Discount, -45);
            Assert.AreEqual(itemObj.Sizes.Count, 6);
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("36")).Count > 0,"36 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("37")).Count > 0, "37 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("38")).Count > 0, "38 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("39")).Count > 0, "39 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("40")).Count > 0, "40 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("41")).Count > 0, "41 number size available");
            Assert.AreEqual(itemObj.Properties.Count, 23, "23 properties are available");
        }

        [TestMethod]
        public void ParsingPage()
        {
            _parseContent.ParsePage(ParseOnePageUrl);
        }

        [TestMethod]
        public void ParsingAllPage()
        {
            _parseContent.ParseAllPages(ParseAllPageUrl);
        }
    }
}

