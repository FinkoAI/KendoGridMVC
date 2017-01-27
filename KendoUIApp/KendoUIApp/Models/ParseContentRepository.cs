using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using WebGrease.Css.Extensions;

namespace KendoUIApp.Models
{
    public class ParseContentRepository
    {
        public Item ParseItem(string url)
        {
            var item = new Item();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return item;
            string id;
            if (HasId(rootDocument, out id))
            {
                item.Id = id;
            }

            List<string> imageUrls;
            if (HasProductGallery(rootDocument, out imageUrls))
            {
                item.ImageUrls = imageUrls;
            }
            string type;
            string brand;
            string subType;
            if (HasTitle(rootDocument, out type, out brand, out subType))
            {
                item.Brand = brand;
                item.Type = type;
                item.SubType = subType;
            }
            decimal price;
            if (HasPrice(rootDocument, out price))
            {
                item.Price = price;
            }
            List<Size> itemSize;
            if (HasSizes(rootDocument, out itemSize))
            {
                item.Sizes = itemSize;
            }
            List<KeyValuePair<string, string>> properties;
            if (HasProperties(rootDocument, out properties))
            {
                item.Properties = properties;
            }
            return item;
        }

        public List<Item> ParsePage(string url)
        {
            return null;
        }

        public List<Item> ParseAllPages(string url)
        {
            return null;
        }

        #region Parsing Item Methods

        private bool HasId(HtmlDocument rootDocument, out string id)
        {
            id = string.Empty;
            const string productIdClass = "//input[@id='productID']";
            var productId = rootDocument.DocumentNode.SelectSingleNode(productIdClass);
            if (productId == null) return false;
            id = productId.GetAttributeValue("value", "");
            return true;
        }

        private bool HasProductGallery(HtmlDocument rootDocument, out List<string> imageUrls)
        {
            imageUrls = new List<string>();
            const string productGalleryClass = "//div[@class='product-gallery__list-wrapper']//img";
            var imageGallery = rootDocument.DocumentNode.SelectNodes(productGalleryClass);
            if (imageGallery == null || imageGallery.Count == 0) return false;
            imageUrls.AddRange(imageGallery.Select(node => node.GetAttributeValue("src", "")));
            return true;
        }

        private bool HasTitle(HtmlDocument rootDocument, out string type, out string brand, out string subType)
        {
            const int typeIndex = 0;
            const int brandIndex = 1;
            const char typeBrandSeperator = ',';
            type = string.Empty;
            brand = string.Empty;
            subType = string.Empty;
            const string productTitleClass = "//h1[@class='product-info__title']";
            var productTitle = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
            if (productTitle == null) return false;
            var data = productTitle.InnerHtml.Split(typeBrandSeperator);
            type = data[typeIndex];
            brand = data[brandIndex];
            subType = productTitle.NextSibling.NextSibling.InnerText;
            return true;
        }

        private bool HasPrice(HtmlDocument rootDocument, out decimal price)
        {
            price = 0;
            const string productPriceClass = "//span[@class='product-info__new-price']/text()";
            var productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
            if (productPrice == null) return false;
            var correctValue = Regex.Replace(productPrice.InnerText, @"\s+", "");
            decimal.TryParse(correctValue, out price);
            return true;
        }

        private bool HasSizes(HtmlDocument rootDocument, out List<Size> sizes)
        {
            const string availableSizesClass = "//li[@class='radio__item radio__item_size']";
            const string unavailableSizesClass = "//li[@class='radio__item radio__item_size disabled']";
            var sizeList = new List<Size>();
            var availableSizes = rootDocument.DocumentNode.SelectNodes(availableSizesClass);
            var unavailableSizes = rootDocument.DocumentNode.SelectNodes(unavailableSizesClass);

            availableSizes.ForEach(node =>
            {
                var availSize = node.GetAttributeValue("id", "").Replace("size_", "");
                sizeList.Add(new Size {SizeText = availSize, IsAvailable = true});
            });
            unavailableSizes.ForEach(node =>
            {
                var availSize = node.GetAttributeValue("id", "").Replace("size_", "");
                sizeList.Add(new Size {SizeText = availSize, IsAvailable = false});
            });

            sizes = sizeList.OrderBy(x => x.SizeText).ToList();
            return sizes.Count > 0;
        }

        private bool HasProperties(HtmlDocument rootDocument, out List<KeyValuePair<string, string>> propertiesList)
        {
            var newpropertiesList = new List<KeyValuePair<string, string>>();
            const string propertyClass = "//ul[@class='product-chars__list']//li[@class='product-chars__item']";
            var properties = rootDocument.DocumentNode.SelectNodes(propertyClass);
            properties.ForEach(node =>
            {
                var propertyKey = string.Empty;
                var propertyValue = string.Empty;
                node.ChildNodes.ForEach(child =>
                {
                    if (child.GetAttributeValue("class", "").Contains("left"))
                    {
                        propertyKey = child.InnerText.Replace('\n', ' ').Trim();
                    }
                    if (child.GetAttributeValue("class", "").Contains("right"))
                    {
                        propertyValue = child.InnerText.Replace('\n', ' ').Trim();
                    }
                });

                newpropertiesList.Add(new KeyValuePair<string, string>(propertyKey, propertyValue));
            });
            propertiesList = newpropertiesList;
            return propertiesList.Count > 0;
        }

        #endregion
    }
}