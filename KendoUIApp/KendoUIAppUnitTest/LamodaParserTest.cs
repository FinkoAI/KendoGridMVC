using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KendoUIAppUnitTest
{
    [TestClass]
    public class LamodaParserTest
    {
        private const string ItemParseUrl = @"http://www.lamoda.ru/p/in029awnjd38/shoes-inario-botforty/";
        private const string PageParseUrl = @"http://www.lamoda.ru/c/15/shoes-women/?genders=women&page=1";
        private const string ParseAllPageUrl = @"http://www.lamoda.ru/c/15/shoes-women/?genders=women";

        private readonly ParseContentRepository _parseContent = new ParseContentRepository(Website.Lamoda);

        [TestMethod]
        public void LamodaItemParse()
        {
            var itemObj = _parseContent.ParseItem(ItemParseUrl);
            Assert.AreEqual(itemObj.Id, "IN029AWNJD38");
            Assert.AreEqual(itemObj.Title, "Ботфорты  Inario");
            Assert.AreEqual(itemObj.ImageUrls.Count, 6,"6 images are available");
            Assert.AreEqual(itemObj.Type, "Сапоги");
            Assert.AreEqual(itemObj.SubType, "Ботфорты");
            Assert.AreEqual(itemObj.Brand, "Inario");
            Assert.AreEqual(itemObj.Price, 7799);
            Assert.AreEqual(itemObj.Discount, 0m);
            Assert.AreEqual(itemObj.Sizes.Count, 5);
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("36")).Count > 0,"36 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("37")).Count > 0, "37 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("38")).Count > 0, "38 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("39")).Count > 0, "39 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("40")).Count > 0, "40 number size available");
            Assert.AreEqual(itemObj.Properties.Count, 18, "18 properties are available");
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

