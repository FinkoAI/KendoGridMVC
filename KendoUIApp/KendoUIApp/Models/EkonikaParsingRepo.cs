using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using WebGrease.Css.Extensions;

namespace KendoUIApp.Models
{
    public class EkonikaParsingRepo: IParseContent
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
            item.WebsiteName = Website.Ekonika;
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
            decimal discount;
            if (HasDiscount(rootDocument, out discount))
            {
                item.Discount = discount;
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
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            List<string> itemDescriptionUrlList;
            if (HasItems(rootDocument, out itemDescriptionUrlList))
            {
                itemDescriptionUrlList.ForEach(
                    item => { itemList.Add(ParseItem(string.Format("http://ekonika.ru/catalog/view/{0}", item))); });
            }
            return itemList;
        }

        public List<Item> ParseAllPages(string url)
        {
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            var hasElements = "Data Available";

            var pageFilterUrlList = new List<string>();

            while (!string.IsNullOrEmpty(hasElements))
            {
                List<string> itemDescriptionUrlList;
                if (HasItems(rootDocument, out itemDescriptionUrlList))
                {
                    pageFilterUrlList.AddRange(itemDescriptionUrlList);
                }
                hasElements = GetEkonikaNextPageUrl(rootDocument);
                if (!string.IsNullOrEmpty(hasElements))
                    rootDocument = website.Load(string.Format("{0}{1}", url, hasElements));
            }

            pageFilterUrlList.ForEach(
                item => { itemList.Add(ParseItem(string.Format("http://ekonika.ru/catalog/view/{0}", item))); });
            return itemList;
        }

        #region Private Methods

        private string GetEkonikaNextPageUrl(HtmlDocument rootDocument)
        {
            var result = string.Empty;
            const string nextPageLinkClass = @"//div[@class='num-paging']//a[starts-with(., 'Вперед')]";
            var nextPage = rootDocument.DocumentNode.SelectSingleNode(nextPageLinkClass);
            if (nextPage != null)
                result = nextPage.GetAttributeValue("href", "");
            return result;
        }

        private bool HasItems(HtmlDocument rootDocument, out List<string> urlList)
        {
            var newUrlList = new List<string>();

            const string availableItemClass = "//div[@class='catalog-items']//ul//li";
            var availableItem = rootDocument.DocumentNode.SelectNodes(availableItemClass);
            availableItem.ForEach(item => { newUrlList.Add(item.GetAttributeValue("id", "")); });

            urlList = newUrlList;
            return urlList.Count > 0;
        }

        private bool HasId(HtmlDocument rootDocument, out string id)
        {
            id = string.Empty;

            const string productIdClass = "//div[@class='item-fav']";
            var productId = rootDocument.DocumentNode.SelectSingleNode(productIdClass);
            if (productId == null) return false;
            id = productId.GetAttributeValue("data-attr", "");

            return true;
        }

        private bool HasProductGallery(HtmlDocument rootDocument, out List<string> imageUrls)
        {
            imageUrls = new List<string>();

            const string productGalleryClass = "//ul[@class='item-photo-list']//img";
            var imageGallery = rootDocument.DocumentNode.SelectNodes(productGalleryClass);
            if (imageGallery == null || imageGallery.Count == 0) return false;
            imageUrls.AddRange(imageGallery.Select(node => node.GetAttributeValue("src", "")));

            return true;
        }

        private bool HasTitle(HtmlDocument rootDocument, out string type, out string brand,
            out string subType)
        {
            const int typeIndex = 0;
            const int brandIndex = 1;
            type = string.Empty;
            brand = string.Empty;
            subType = string.Empty;

            const char ekonikaSubtypeBrandSplitter = ' ';
            const string productTitleClass = "//div[@class='catalog-title-center-i']//h1";
            var productTitle = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
            if (productTitle == null) return false;
            var data = productTitle.InnerText.Replace("&nbsp;", " ").Split(ekonikaSubtypeBrandSplitter);
            subType = data.Count() - 1 >= typeIndex ? data[typeIndex] : string.Empty;
            brand = data.Count() - 1 >= brandIndex ? data[brandIndex] : string.Empty;

            return true;
        }

        private bool HasPrice(HtmlDocument rootDocument, out decimal price)
        {
            price = 0;
            string productPriceClass = "//span[@class='price-current price-fix']/text()";
            var productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
            if (productPrice == null)
            {
                productPriceClass = "//span[@class='price-current']/text()";
                productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
                if (productPrice == null) return false;
            }
            decimal.TryParse(productPrice.InnerText.Replace("&nbsp;", ""), out price);
            return true;
        }

        private bool HasDiscount(HtmlDocument rootDocument, out decimal discount)
        {
            discount = 0;

            const string productDiscountClass = "//span[@class='price-discount']//b/text()";
            var productDiscountPrice = rootDocument.DocumentNode.SelectSingleNode(productDiscountClass);
            if (productDiscountPrice == null) return false;
            decimal.TryParse(productDiscountPrice.InnerText, out discount);

            return true;
        }

        private bool HasSizes(HtmlDocument rootDocument, out List<Size> sizes)
        {
            var sizeList = new List<Size>();

            const string availableSizesClass = "//select[@id='item-size-select']//option";
            var availableSizes = rootDocument.DocumentNode.SelectNodes(availableSizesClass);
            availableSizes.ForEach(node =>
            {
                var availSize = node.GetAttributeValue("value", "");
                var availability = node.NextSibling.InnerHtml.Contains("пара") ||
                                   node.NextSibling.InnerHtml.Contains("пары");
                sizeList.Add(new Size {SizeText = availSize, IsAvailable = availability});
            });

            sizes = sizeList.OrderBy(x => x.SizeText).ToList();
            return sizes.Count > 0;
        }

        private bool HasProperties(HtmlDocument rootDocument,
            out List<KeyValuePair<string, string>> propertiesList)
        {
            var newpropertiesList = new List<KeyValuePair<string, string>>();

            const string propertyClass = "//div[@class='item-attr']//dl/dt";
            var properties = rootDocument.DocumentNode.SelectNodes(propertyClass);
            properties.ForEach(node =>
            {
                var propertyKey = node.InnerText;
                var propertyValue = node.NextSibling.InnerText;
                newpropertiesList.Add(new KeyValuePair<string, string>(propertyKey, propertyValue));
            });

            propertiesList = newpropertiesList;
            return propertiesList.Count > 0;
        }

        #endregion
    }
}