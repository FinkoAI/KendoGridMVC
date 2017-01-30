using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using WebGrease.Css.Extensions;

namespace KendoUIApp.Models
{
    public partial class ParseContentRepository
    {
        private Item ParseItemBashmag(string url)
        {
            var item = new Item();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return item;
            string id;
            if (HasId(rootDocument, Website.Bashmag, out id))
            {
                item.Id = id;
            }
            List<string> imageUrls;
            if (HasProductGallery(rootDocument, Website.Bashmag, out imageUrls))
            {
                item.ImageUrls = imageUrls;
            }
            string type;
            string brand;
            string subType;
            if (HasTitle(rootDocument, Website.Bashmag, out type, out brand, out subType))
            {
                item.Brand = brand;
                item.Type = type;
                item.SubType = subType;
            }
            decimal price;
            if (HasPrice(rootDocument, Website.Bashmag, out price))
            {
                item.Price = price;
            }
            decimal discount;
            if (HasDiscount(rootDocument, Website.Bashmag, out discount))
            {
                const int fractionalValuePoints = 2;
                item.Discount = (discount > 0)
                    ? Math.Round(100 - ((item.Price/discount)*100), fractionalValuePoints)
                    : discount;
            }
            List<Size> itemSize;
            if (HasSizes(rootDocument, Website.Bashmag, out itemSize))
            {
                item.Sizes = itemSize;
            }
            List<KeyValuePair<string, string>> properties;
            if (HasProperties(rootDocument, Website.Bashmag, out properties))
            {
                item.Properties = properties;
            }
            return item;
        }

        private List<Item> ParsePageBashmag(string url)
        {
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            var hasNextPage = "Has Data";
            var finalitemDescriptionUrlList = new List<string>();
            while (!string.IsNullOrEmpty(hasNextPage))
            {
                List<string> itemDescriptionUrlList;
                if (HasItems(rootDocument, Website.Bashmag, out itemDescriptionUrlList))
                {
                    finalitemDescriptionUrlList.AddRange(itemDescriptionUrlList);
                }
                hasNextPage = BashmagHasMoreThenCurrentPageItems(rootDocument);
                if (!string.IsNullOrEmpty(hasNextPage))
                    rootDocument = website.Load(string.Format("https://www.bashmag.ru{0}", hasNextPage));
            }

            finalitemDescriptionUrlList.ForEach(
                item => { itemList.Add(ParseItemBashmag(string.Format("https://www.bashmag.ru{0}", item))); });

            return itemList;
        }

        private string BashmagHasMoreThenCurrentPageItems(HtmlDocument rootDocument)
        {
            string nextPageUrl = string.Empty;
            const string nextPageSignCode = "&rsaquo;";
            const string nextPageLinkClass = "//ul[@class='pagination']//li";
            var nextPageUrlItems = rootDocument.DocumentNode.SelectNodes(nextPageLinkClass);
            nextPageUrlItems.ForEach(pager =>
            {
                if (pager.InnerText.Equals(nextPageSignCode))
                {
                    nextPageUrl = pager.FirstChild.GetAttributeValue("href", "");
                }
            });
            return nextPageUrl;
        }
    }
}