using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace KendoUIAppUnitTest
{
    [TestClass]
    public class ShopsFileTest
    {
        private ParseContentRepository _parseContent;
        private const string FileName = "shops.txt";

        [TestMethod]
        public void CreateFile()
        {
            var finalResult = new List<Item>();
            _parseContent = new ParseContentRepository(Website.Ekonika);
            var ekonikaItems = _parseContent.ParseAllPages("http://ekonika.ru/catalog/shoes/");
            finalResult.AddRange(ekonikaItems);

            if (File.Exists("Ekonika")) File.Delete("Ekonika");
            File.WriteAllText("Ekonika", JsonConvert.SerializeObject(finalResult));

            _parseContent = new ParseContentRepository(Website.Bashmag);
            var bashmagItems = _parseContent.ParseAllPages("https://www.bashmag.ru/women/82-obuv_");
            finalResult.AddRange(bashmagItems);

            if (File.Exists("Bashmag")) File.Delete("Bashmag");
            File.WriteAllText("Bashmag", JsonConvert.SerializeObject(finalResult));

            _parseContent = new ParseContentRepository(Website.Sapato);
            var sapatoItems = _parseContent.ParseAllPages("https://www.sapato.ru/woman/");
            finalResult.AddRange(sapatoItems);

            if (File.Exists(FileName)) File.Delete(FileName);
            File.WriteAllText(FileName, JsonConvert.SerializeObject(finalResult));
        }

        [TestMethod]
        public void EkonikaCreateFile()
        {
            _parseContent = new ParseContentRepository(Website.Ekonika);
            var ekonikaItems = _parseContent.ParseAllPages("http://ekonika.ru/catalog/shoes/");
            if (File.Exists("Ekonika.txt")) File.Delete("Ekonika.txt");
            File.WriteAllText("Ekonika.txt", JsonConvert.SerializeObject(ekonikaItems));
        }

        [TestMethod]
        public void BashmagCreateFile()
        {
            _parseContent = new ParseContentRepository(Website.Bashmag);
            var bashmagItems = _parseContent.ParseAllPages("https://www.bashmag.ru/women/82-obuv_");
            if (File.Exists("Bashmag.txt")) File.Delete("Bashmag.txt");
            File.WriteAllText("Bashmag.txt", JsonConvert.SerializeObject(bashmagItems));
        }

        [TestMethod]
        public void SapatoCreateFile()
        {
            _parseContent = new ParseContentRepository(Website.Sapato);
            var sapatoItems = _parseContent.ParseAllPages("https://www.sapato.ru/woman/");
            if (File.Exists("Sapato.txt")) File.Delete("Sapato.txt");
            File.WriteAllText("Sapato.txt", JsonConvert.SerializeObject(sapatoItems));
        }

        [TestMethod]
        public void JsonToCsv()
        {
            const string readFile = "Bashmag.txt";
            if (File.Exists(readFile))
            {
                var jsonResponse = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(readFile));
                string csv = String.Join(Environment.NewLine, jsonResponse.Select(x => x.ToString()).ToArray());

                if (File.Exists("item.csv")) File.Delete("item.csv");
                File.WriteAllText("item.csv", csv);
            }
        }
    }
}