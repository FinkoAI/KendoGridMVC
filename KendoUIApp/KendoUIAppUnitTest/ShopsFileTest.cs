using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
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

        [TestMethod]
        public void JsonToExcel()
        {
            #region Constants
            const string seperator = ",";
            const string readSapatoFile = "Sapato.txt";
            const string readBashmagFile = "Bashmag.txt";
            const string readEkonikaFile = "Ekonika.txt";
            const int idIndex = 1;
            const int imageUrlsIndex = 2;
            const int priceIndex = 3;
            const int discountIndex = 4;
            const int typeIndex = 5;
            const int subTypeIndex = 6;
            const int brandIndex = 7;
            const int sizeIndex = 8;
            const int propertiesIndex = 9;
            #endregion

            if (File.Exists(readSapatoFile) && File.Exists(readBashmagFile) && File.Exists(readEkonikaFile))
            {
                var finalList = new List<Item>();

                var jsonSapatoResponse = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(readSapatoFile));
                var jsonBashmagResponse = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(readBashmagFile));
                var jsonEkonikaResponse = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(readEkonikaFile));

                finalList.AddRange(jsonSapatoResponse);
                finalList.AddRange(jsonBashmagResponse);
                finalList.AddRange(jsonEkonikaResponse);

                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("ShopItems");
                var row = 1;
                #region Header Row
                worksheet.Cell(row, idIndex).Value = "Id";
                worksheet.Cell(row, imageUrlsIndex).Value = "ImageUrls";
                worksheet.Cell(row, priceIndex).Value = "Price";
                worksheet.Cell(row, discountIndex).Value = "Discount";
                worksheet.Cell(row, typeIndex).Value = "Type";
                worksheet.Cell(row, subTypeIndex).Value = "SubType";
                worksheet.Cell(row, brandIndex).Value = "Brand";
                worksheet.Cell(row, sizeIndex).Value = "Sizes";
                worksheet.Cell(row, propertiesIndex).Value = "Properties";
                row++;
                #endregion
                finalList.ForEach(item =>
                {
                    worksheet.Cell(row, idIndex).Value = item.Id;
                    worksheet.Cell(row, imageUrlsIndex).Value = item.ImageUrls != null ? String.Join(seperator, item.ImageUrls) : string.Empty;
                    worksheet.Cell(row, priceIndex).Value = item.Price;
                    worksheet.Cell(row, discountIndex).Value = item.Discount;
                    worksheet.Cell(row, typeIndex).Value = item.Type;
                    worksheet.Cell(row, subTypeIndex).Value = item.SubType;
                    worksheet.Cell(row, brandIndex).Value = item.Brand;
                    worksheet.Cell(row, sizeIndex).Value = item.SizeString;
                    worksheet.Cell(row, propertiesIndex).Value = item.PropertiesString;
                    row++;
                });
                if (File.Exists("Shop.xlsx")) File.Delete("Shop.xlsx");
                workbook.SaveAs("Shop.xlsx");
            }
        }
    }
}