using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using WebGrease.Css.Extensions;

namespace KendoUIApp.Models
{
    public class LamodaParsingRepo : IParseContent
    {
        public Item ParseItem(string url)
        {
            var item = new Item();
            var website = new HtmlWeb
            {
                PreRequest = delegate (HttpWebRequest webRequest)
                {
                    webRequest.Timeout = 30000;
                    return true;
                }
            };
            var rootDocument = website.Load(url);
            if (rootDocument == null) return item;
            item.Url = url;
            item.WebsiteName = Website.Lamoda;
            string id;
            if (HasId(rootDocument, out id))
            {
                item.Id = id;
            }
            string title;
            if (HasTitle(rootDocument, out title))
            {
                item.Title = title;
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
            decimal discount;
            if (HasDiscount(rootDocument, out discount))
            {
                item.Discount = discount;
            }
            List<Size> itemSize;
            if (HasSizes(rootDocument,out itemSize))
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
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            List<string> itemDescriptionUrlList;
            if (HasItems(rootDocument, out itemDescriptionUrlList))
            {
                itemDescriptionUrlList.ForEach(
                    item => { itemList.Add(ParseItem(string.Format("http://www.lamoda.ru{0}", item))); });
            }
            return itemList;
        }

        public List<Item> ParseAllPages(string url)
        {
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            List<string> itemDescriptionUrlList;
            if (GetLamodaAllPageUrl(rootDocument, url, out itemDescriptionUrlList))
            {
                itemDescriptionUrlList.ForEach(
               item => { itemList.AddRange(ParsePage(item)); });
            }
            return itemList;
        }

        #region Private Methods

        private bool GetLamodaAllPageUrl(HtmlDocument rootDocument,string url, out List<string> urlList)
        {
            var newUrlList = new List<string>();
            var totalItemPages = 0;
            const string nextPageLinkClass = @"//div[@class='paginator']";
            var nextPage = rootDocument.DocumentNode.SelectSingleNode(nextPageLinkClass);
            if (nextPage != null)
                int.TryParse(nextPage.GetAttributeValue("data-pages",""),out totalItemPages);

            Enumerable.Range(1, totalItemPages).ForEach(x =>
            {
                newUrlList.Add(string.Format("{0}&page={1}", url, x));
            });

            urlList = newUrlList;
            return urlList.Count > 0;
        }

        private bool HasItems(HtmlDocument rootDocument, out List<string> urlList)
        {
            var newUrlList = new List<string>();
            const string availableItemClass = "//a[@class='products-list-item__link link']";
            var availableItem = rootDocument.DocumentNode.SelectNodes(availableItemClass);
            availableItem.ForEach(item => { newUrlList.Add(item.GetAttributeValue("href", "")); });
            urlList = newUrlList;
            return urlList.Count > 0;
        }

        private bool HasId(HtmlDocument rootDocument, out string id)
        {
            id = string.Empty;
            const char splitIdChar = ' ';
            const string productIdClass = "//div[@class='breadcrumbs__sku']/text()";
            var productId = rootDocument.DocumentNode.SelectSingleNode(productIdClass);
            if (productId == null) return false;
            var data= productId.InnerText.Split(splitIdChar);
            id = data[data.Length - 1];
            return true;
        }

        private bool HasTitle(HtmlDocument rootDocument, out string title)
        {
            title = string.Empty;
            const string productTitleClass = "//h1[@itemprop='name']/text()";
            var productTitle = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
            if (productTitle == null) return false;
            title = productTitle.InnerText;
            return true;
        }

        private bool HasProductGallery(HtmlDocument rootDocument, out List<string> imageUrls)
        {
            imageUrls = new List<string>();
            const string productGalleryClass = "//img[@class='showcase__slide-image']";
            var imageGallery = rootDocument.DocumentNode.SelectNodes(productGalleryClass);
            if (imageGallery == null || imageGallery.Count == 0) return false;
            imageUrls.AddRange(imageGallery.Select(node => node.GetAttributeValue("src", "")));
            return true;
        }

        private bool HasTitle(HtmlDocument rootDocument, out string type, out string brand,
            out string subType)
        {
            const int typeIndex = 3;
            const int subtypeIndex = 4;
            type = string.Empty;
            brand = string.Empty;
            subType = string.Empty;
            string productTitleClass = "//span[@itemprop='title']";
            var productTitle = rootDocument.DocumentNode.SelectNodes(productTitleClass);
            if (productTitle != null)
            {
                type = productTitle.Count() - 1 >= typeIndex ? productTitle[typeIndex].InnerText : string.Empty;
                subType = productTitle.Count() - 1 >= subtypeIndex ? productTitle[subtypeIndex].InnerText : string.Empty;
            }
            productTitleClass = "//div[@class='ii-product__brand']";
            var productBrand = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
            if (productBrand != null)
            {
                brand = productBrand.GetAttributeValue("data-name", "");
            }
            return true;
        }

        private bool HasPrice(HtmlDocument rootDocument, out decimal price)
        {
            price = 0;
            const string productPriceClass = "//meta[@itemprop='price']";
            var productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
            if (productPrice == null) return false;
            string data = productPrice.GetAttributeValue("content", "");
            decimal.TryParse(data, out price);
            return true;
        }

        private bool HasDiscount(HtmlDocument rootDocument, out decimal discount)
        {
            discount = 0;
            const string productDiscountClass = "//span[@class='product__badge_m  product__badge_sale']";
            var productDiscountPrice = rootDocument.DocumentNode.SelectSingleNode(productDiscountClass);
            if (productDiscountPrice == null) return false;
            decimal.TryParse(productDiscountPrice.InnerText, out discount);
            return true;
        }

        private bool HasSizes(HtmlDocument rootDocument, out List<Size> sizes)
        {
            var sizeList = new List<Size>();
            const char splitIdChar = ' ';
            const string availableSizesClass = "//div[@class='ii-select__column ii-select__column_native']/div";
            var availableSizes = rootDocument.DocumentNode.SelectNodes(availableSizesClass);
            availableSizes.ForEach(node =>
            {
                if (node.GetAttributeValue("class", "").Contains("ii-select__option"))
                {
                    var availSize = node.InnerText.Split(splitIdChar).First();
                    var availability = !node.OuterHtml.Contains("disabled");
                    sizeList.Add(new Size {SizeText = availSize, IsAvailable = availability});
                }
            });
            sizes = sizeList.OrderBy(x => x.SizeText).ToList();
            return sizes.Count > 0;
        }

        private bool HasProperties(HtmlDocument rootDocument,
            out List<KeyValuePair<string, string>> propertiesList)
        {
            var newpropertiesList = new List<KeyValuePair<string, string>>();
            const int keyIndex = 1;
            const int valueIndex = 3;
            const string propertyClass = "//div[@class='ii-product__attribute']";
            var properties = rootDocument.DocumentNode.SelectNodes(propertyClass);
            properties.ForEach(node =>
            {
                var propertyKey = node.ChildNodes[keyIndex].InnerText;
                var propertyValue = node.ChildNodes[valueIndex].InnerText;
                newpropertiesList.Add(new KeyValuePair<string, string>(propertyKey, propertyValue));
            });

            propertiesList = newpropertiesList;
            return propertiesList.Count > 0;
        }

        #endregion
    }
}