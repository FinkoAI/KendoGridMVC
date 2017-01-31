using System.IO;
using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace KendoUIAppUnitTest
{
    [TestClass]
    public class ShopsFileTest
    {
        private ParseContentRepository _parseContent;

        [TestMethod]
        public void CreateFile()
        {
            _parseContent = new ParseContentRepository(Website.Bashmag);
            var items = _parseContent.ParsePage("https://www.bashmag.ru/women/82-obuv_/86-baletki");
            File.WriteAllText("shops.txt", JsonConvert.SerializeObject(items));
        }
    }
}