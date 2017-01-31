using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KendoUIAppUnitTest
{
    [TestClass]
    public class BashmagParserTest
    {
        private const string BashmagParseItemUrl = @"https://www.bashmag.ru/men/36-obuv_/41-botinki/3565-Botinki-magelan-detail";
        private const string BashmagParseItemUrlWithBrandImage = @"https://www.bashmag.ru/men/36-obuv_/41-botinki/4373-Botinki-strobbs-detail";
        private const string BashmagParseOnePageUrl = @"https://www.bashmag.ru/women/82-obuv_/86-baletki";
        private const string ParseAllPageUrl = @"https://www.bashmag.ru/women";

        private readonly ParseContentRepository _parseContent = new ParseContentRepository(Website.Bashmag);

        [TestMethod]
        public void BashmagItemParse()
        {
            var itemObj = _parseContent.ParseItem(BashmagParseItemUrl);
            Assert.AreEqual(itemObj.Id, "3565");
            Assert.AreEqual(itemObj.ImageUrls.Count, 8,"8 images are available");
            Assert.AreEqual(itemObj.Type, "Ботинки");
            Assert.AreEqual(itemObj.SubType, "Ботинки Магелан");
            Assert.AreEqual(itemObj.Brand, "Магелан");
            Assert.AreEqual(itemObj.Price, 1940);
            Assert.AreEqual(itemObj.Discount, 69.59m);
            Assert.AreEqual(itemObj.Sizes.Count, 6);
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("40")).Count > 0,"40 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("41")).Count > 0, "41 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("42")).Count > 0, "42 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("43")).Count > 0, "43 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("44")).Count > 0, "44 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("45")).Count > 0, "45 number size available");
            Assert.AreEqual(itemObj.Properties.Count, 9, "9 properties are available");
        }

        [TestMethod]
        public void BashmagItemParseWithBrandImage()
        {
            var itemObj = _parseContent.ParseItem(BashmagParseItemUrlWithBrandImage);
            Assert.AreEqual(itemObj.Id, "4373");
            Assert.AreEqual(itemObj.ImageUrls.Count, 8, "8 images are available");
            Assert.AreEqual(itemObj.Type, "Ботинки");
            Assert.AreEqual(itemObj.SubType, "Ботинки STROBBS");
            Assert.AreEqual(itemObj.Brand, "STROBBS");
            Assert.AreEqual(itemObj.Price, 3090);
            Assert.AreEqual(itemObj.Discount, 40.92m);
            Assert.AreEqual(itemObj.Sizes.Count, 6);
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("40")).Count > 0, "40 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("41")).Count > 0, "41 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("42")).Count > 0, "42 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("43")).Count > 0, "43 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("44")).Count > 0, "44 number size available");
            Assert.IsTrue(itemObj.Sizes.FindAll(x => x.SizeText.Contains("45")).Count > 0, "45 number size available");
            Assert.AreEqual(itemObj.Properties.Count, 9, "9 properties are available");
        }

        [TestMethod]
        public void ParsingPage()
        {
            _parseContent.ParsePage(BashmagParseOnePageUrl);
        }

        [TestMethod]
        public void ParsingAllPage()
        {
            _parseContent.ParseAllPages(ParseAllPageUrl);
        }
    }
}

