using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using KendoUIApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using  System.Data;

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
            const string readFile = "Bashmag.txt";
            const int idIndex = 3;
            const int imageUrlsIndex = 4;
            const int priceIndex = 5;
            const int discountIndex = 6;
            const int typeIndex = 7;
            const int subTypeIndex = 8;
            const int brandIndex = 9;
            const int sizeIndex = 10;
            const int propertiesIndex = 11;
            #endregion

            if (File.Exists(readFile))
            {
                var jsonResponse = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(readFile));
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Bashmag");
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
                jsonResponse.ForEach(item =>
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
                if (File.Exists("Bashmag.xlsx")) File.Delete("Bashmag.xlsx");
                workbook.SaveAs("Bashmag.xlsx");
            }
        }
    }
}