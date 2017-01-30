using System;
using System.Collections.Generic;
using HtmlAgilityPack;

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
    }
}