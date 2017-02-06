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
        public void LamodaCreateFile()
        {
            _parseContent = new ParseContentRepository(Website.Lamoda);
            var lamodaItems = _parseContent.ParseAllPages("http://www.lamoda.ru/c/15/shoes-women/?genders=women");
            if (File.Exists("Lamoda.txt")) File.Delete("Lamoda.txt");
            File.WriteAllText("Lamoda.txt", JsonConvert.SerializeObject(lamodaItems));
        }

        [TestMethod]
        public void SpvtomskeCreateFile()
        {
            _parseContent = new ParseContentRepository(Website.Spvtomske);
            var spvtomskeItems = _parseContent.ParseAllPages("http://spvtomske.ru/sp/catalog/index/brendId/1403/");
            if (File.Exists("Spvtomske.txt")) File.Delete("Spvtomske.txt");
            File.WriteAllText("Spvtomske.txt", JsonConvert.SerializeObject(spvtomskeItems));
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

        [TestMethod]
        public void LamodaJsonToExcel()
        {
            #region Constants
            const string seperator = ",";
            const string readLamodaFile = "Lamoda.txt";
            const int websiteIndex = 1;
            const int idIndex = 2;
            const int titleIndex = 3;
            const int itemUrlIndex = 4;
            const int imageUrlsIndex = 5;
            const int priceIndex = 6;
            const int discountIndex = 7;
            const int typeIndex = 8;
            const int subTypeIndex = 9;
            const int brandIndex = 10;
            const int sizeIndex = 11;
            const int propertiesIndex = 12;
            #endregion

            if (File.Exists(readLamodaFile))
            {
                var jsonLamodaResponse = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(readLamodaFile));
                
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("ShopItems");
                var row = 1;
                #region Header Row
                worksheet.Cell(row, websiteIndex).Value = "Website";
                worksheet.Cell(row, idIndex).Value = "Id";
                worksheet.Cell(row, titleIndex).Value = "Title";
                worksheet.Cell(row, itemUrlIndex).Value = "Item Url";
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
                jsonLamodaResponse.ForEach(item =>
                {
                    worksheet.Cell(row, websiteIndex).Value = item.WebsiteName.ToString("G");
                    worksheet.Cell(row, idIndex).Value = item.Id;
                    worksheet.Cell(row, titleIndex).Value = item.Title;
                    worksheet.Cell(row, itemUrlIndex).Value = item.Url;
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
                if (File.Exists("LamodaShop.xlsx")) File.Delete("LamodaShop.xlsx");
                workbook.SaveAs("LamodaShop.xlsx");
            }
        }

        [TestMethod]
        public void SpvtomskeJsonToExcel()
        {
            #region Constants
            const string seperator = ",";
            const string readLamodaFile = "Spvtomske.txt";
            const int websiteIndex = 1;
            const int idIndex = 2;
            const int titleIndex = 3;
            const int itemUrlIndex = 4;
            const int imageUrlsIndex = 5;
            const int priceIndex = 6;
            const int discountIndex = 7;
            const int typeIndex = 8;
            const int subTypeIndex = 9;
            const int brandIndex = 10;
            const int sizeIndex = 11;
            const int propertiesIndex = 12;
            #endregion

            if (File.Exists(readLamodaFile))
            {
                var jsonLamodaResponse = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(readLamodaFile));

                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("ShopItems");
                var row = 1;
                #region Header Row
                worksheet.Cell(row, websiteIndex).Value = "Website";
                worksheet.Cell(row, idIndex).Value = "Id";
                worksheet.Cell(row, titleIndex).Value = "Title";
                worksheet.Cell(row, itemUrlIndex).Value = "Item Url";
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
                jsonLamodaResponse.ForEach(item =>
                {
                    worksheet.Cell(row, websiteIndex).Value = item.WebsiteName.ToString("G");
                    worksheet.Cell(row, idIndex).Value = item.Id;
                    worksheet.Cell(row, titleIndex).Value = item.Title;
                    worksheet.Cell(row, itemUrlIndex).Value = item.Url;
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
                if (File.Exists("SpvtomskeShop.xlsx")) File.Delete("SpvtomskeShop.xlsx");
                workbook.SaveAs("SpvtomskeShop.xlsx");
            }
        }
    }
}