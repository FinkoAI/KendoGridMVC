using System;
using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KendoUIAppUnitTest
{
    [TestClass]
    public class ParsingTest
    {
        readonly ParseContentRepository _parseContent=new ParseContentRepository();
        private const string ParseItemUrl = "https://www.sapato.ru/11144426";
        private const string ParseOnePageUrl = "https://www.sapato.ru/woman/?page=2&page_size=60";
        private const string ParseAllPageUrl = "https://www.sapato.ru/woman/";

        [TestMethod]
        public void ParsingItem()
        {
            var itemObj2 = _parseContent.ParseItem("https://www.sapato.ru/11155918");
            var itemObj = _parseContent.ParseItem(ParseItemUrl);
            Assert.AreEqual(itemObj.ImageUrls.Count, 4); // Page Shown 4 Images
            Assert.AreEqual(itemObj.Id, "11144426");
        }
    }
}
