using System.Collections.Generic;
using HtmlAgilityPack;

namespace KendoUIApp.Models
{
    public partial class ParseContentRepository
    {
        private Item ParseItemSaptro(string url)
        {
            var item = new Item();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return item;
            string id;
            if (HasId(rootDocument, Website.Sapato, out id))
            {
                item.Id = id;
            }

            List<string> imageUrls;
            if (HasProductGallery(rootDocument, Website.Sapato, out imageUrls))
            {
                item.ImageUrls = imageUrls;
            }
            string type;
            string brand;
            string subType;
            if (HasTitle(rootDocument, Website.Sapato, out type, out brand, out subType))
            {
                item.Brand = brand;
                item.Type = type;
                item.SubType = subType;
            }
            decimal price;
            if (HasPrice(rootDocument, Website.Sapato, out price))
            {
                item.Price = price;
            }
            decimal discount;
            if (HasDiscount(rootDocument, Website.Sapato, out discount))
            {
                item.Discount = discount;
            }

            List<Size> itemSize;
            if (HasSizes(rootDocument, Website.Sapato, out itemSize))
            {
                item.Sizes = itemSize;
            }
            List<KeyValuePair<string, string>> properties;
            if (HasProperties(rootDocument, Website.Sapato, out properties))
            {
                item.Properties = properties;
            }
            return item;
        }

        private List<Item> ParsePageSaptro(string url)
        {
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            List<string> itemDescriptionUrlList;
            if (HasItems(rootDocument, out itemDescriptionUrlList))
            {
                itemDescriptionUrlList.ForEach(
                    item => { itemList.Add(ParseItemSaptro(string.Format("https://www.sapato.ru{0}", item))); });
            }

            return itemList;
        }

        private List<Item> ParseAllPagesSaptro(string url)
        {
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            List<string> pageFilterUrlList;
            if (GetAllPages(rootDocument, out pageFilterUrlList))
            {
                pageFilterUrlList.ForEach(
                    page => { itemList.AddRange(ParsePageSaptro(page)); });
            }
            return itemList;
        }
    }
}