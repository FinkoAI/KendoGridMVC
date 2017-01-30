using System.Collections.Generic;
using HtmlAgilityPack;

namespace KendoUIApp.Models
{
    public partial class ParseContentRepository
    {
        private Item ParseItemEkonika(string url)
        {
            var item = new Item();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return item;
            string id;
            if (HasId(rootDocument, Website.Ekonika, out id))
            {
                item.Id = id;
            }
            List<string> imageUrls;
            if (HasProductGallery(rootDocument, Website.Ekonika, out imageUrls))
            {
                item.ImageUrls = imageUrls;
            }
            string type;
            string brand;
            string subType;
            if (HasTitle(rootDocument, Website.Ekonika, out type, out brand, out subType))
            {
                item.Brand = brand;
                item.Type = type;
                item.SubType = subType;
            }
            decimal price;
            if (HasPrice(rootDocument, Website.Ekonika, out price))
            {
                item.Price = price;
            }
            decimal discount;
            if (HasDiscount(rootDocument, Website.Ekonika, out discount))
            {
                item.Discount = discount;
            }
            List<Size> itemSize;
            if (HasSizes(rootDocument, Website.Ekonika, out itemSize))
            {
                item.Sizes = itemSize;
            }
            List<KeyValuePair<string, string>> properties;
            if (HasProperties(rootDocument, Website.Ekonika, out properties))
            {
                item.Properties = properties;
            }
            return item;
        }
        private List<Item> ParsePageEkonika(string url)
        {
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            List<string> itemDescriptionUrlList;
            if (HasItems(rootDocument, Website.Ekonika, out itemDescriptionUrlList))
            {
                itemDescriptionUrlList.ForEach(
                    item => { itemList.Add(ParseItemEkonika(string.Format("http://ekonika.ru/catalog/view/{0}", item))); });
            }
            return itemList;
        }
        private List<Item> ParseAllPagesEkonika(string url)
        {
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            var hasElements = "Data Available";

            List<string> pageFilterUrlList = new List<string>();

            while (!string.IsNullOrEmpty(hasElements))
            {
                List<string> itemDescriptionUrlList;
                if (HasItems(rootDocument, Website.Ekonika, out itemDescriptionUrlList))
                {
                    pageFilterUrlList.AddRange(itemDescriptionUrlList);
                }
                hasElements = GetEkonikaNextPageUrl(rootDocument);
                if (!string.IsNullOrEmpty(hasElements))
                    rootDocument = website.Load(string.Format("{0}{1}", url, hasElements));
            }

            pageFilterUrlList.ForEach(
                item => { itemList.Add(ParseItemEkonika(string.Format("http://ekonika.ru/catalog/view/{0}", item))); });
            return itemList;
        }
        private string GetEkonikaNextPageUrl(HtmlDocument rootDocument)
        {
            string result = string.Empty;
            const string nextPageLinkClass = @"//div[@class='num-paging']//a[starts-with(., 'Вперед')]";
            var nextPage = rootDocument.DocumentNode.SelectSingleNode(nextPageLinkClass);
            if (nextPage != null)
                result = nextPage.GetAttributeValue("href", "");
            return result;
        }
    }
}