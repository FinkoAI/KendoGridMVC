using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KendoUIAppUnitTest
{
    [TestClass]
    public class SpvtomskeParserTest
    {
        private const string ItemParseUrl = @"http://spvtomske.ru/sp/newCatalog/index/id/63275239/";
        private const string PageParseUrl = @"http://spvtomske.ru/sp/catalog/index/brendId/1732/?page=2";
        private const string ParseAllPageUrl = @"http://spvtomske.ru/sp/catalog/index/brendId/1403/";

        private readonly ParseContentRepository _parseContent = new ParseContentRepository(Website.Spvtomske);

        [TestMethod]
        public void SpvtomskeItemParse()
        {
            var itemObj = _parseContent.ParseItem(ItemParseUrl);
            Assert.AreEqual(itemObj.Id, "63275239");
            Assert.AreEqual(itemObj.Title, "Сандалии (иск. кожа)");
            Assert.AreEqual(itemObj.ImageUrls.Count, 5,"5 images are available");
            Assert.AreEqual(itemObj.Type, "");
            Assert.AreEqual(itemObj.SubType, " Сандалии, босоножки, сабо");
            Assert.AreEqual(itemObj.Brand, "OБУВЬ - SАLE");
            Assert.AreEqual(itemObj.Price, 851);
            Assert.AreEqual(itemObj.Discount, 0m);
            Assert.AreEqual(itemObj.Sizes.Count, 6);
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("36")).Count > 0,"36 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("37")).Count > 0, "37 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("38")).Count > 0, "38 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("39")).Count > 0, "39 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("40")).Count > 0, "40 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("41")).Count > 0, "41 number size available");
            Assert.AreEqual(itemObj.Properties.Count, 6, "6 properties are available");
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

        [TestMethod]
        public void SpvtomskeItem2Parse()
        {
            const string url = "http://spvtomske.ru/sp/newCatalog/index/id/39531373";
            var itemObj = _parseContent.ParseItem(url);
            Assert.AreEqual(itemObj.Id, "39531373");
            Assert.AreEqual(itemObj.Title, "Бсоножки жен. (иск. кожа)");
            Assert.AreEqual(itemObj.ImageUrls.Count, 13, "13 images are available");
            Assert.AreEqual(itemObj.Type, "");
            Assert.AreEqual(itemObj.SubType, " Босоножки, сандалии, сабо");
            Assert.AreEqual(itemObj.Brand, "ТУФЛИ МОЕЙ МЕЧТЫ");
            Assert.AreEqual(itemObj.Price, 1067);
            Assert.AreEqual(itemObj.Discount, 0m);
            Assert.AreEqual(itemObj.Sizes.Count, 1);
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("39")).Count > 0, "39 number size available");
            Assert.AreEqual(itemObj.Properties.Count, 6, "6 properties are available");
        }
    }
}

