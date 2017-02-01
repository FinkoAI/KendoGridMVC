using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KendoUIAppUnitTest
{
    [TestClass]
    public class EkonikaParserTest
    {
        private const string ItemParseUrl = @"http://ekonika.ru/catalog/view/10093476";
        private const string PageParseUrl = @"http://ekonika.ru/catalog/shoes/vesna_leto_2017/";
        private const string ParseAllPageUrl = @"http://ekonika.ru/catalog/shoes/";

        private readonly ParseContentRepository _parseContent = new ParseContentRepository(Website.Ekonika);

        [TestMethod]
        public void EkonikaItemParse()
        {
            var itemObj = _parseContent.ParseItem(ItemParseUrl);
            Assert.AreEqual(itemObj.Id, "10093476");
            Assert.AreEqual(itemObj.ImageUrls.Count, 6,"6 images are available");
            Assert.AreEqual(itemObj.Type, "");
            Assert.AreEqual(itemObj.SubType, "Балетки");
            Assert.AreEqual(itemObj.Brand, "RiaRosa");
            Assert.AreEqual(itemObj.Price, 900);
            Assert.AreEqual(itemObj.Discount, -80);
            Assert.AreEqual(itemObj.Sizes.Count, 2);
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("29")).Count > 0,"29 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("32")).Count > 0, "32 number size available");
            Assert.AreEqual(itemObj.Properties.Count, 7, "7 properties are available");
        }

        [TestMethod]
        public void ParsingPage()
        {
            _parseContent.ParsePage(PageParseUrl);
        }

        [TestMethod]
        public void ParsingAllPage()
        {
            _parseContent.ParseAllPages(ParseAllPageUrl);
        }
    }
}

