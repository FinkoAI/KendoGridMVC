using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using WebGrease.Css.Extensions;

namespace KendoUIApp.Models
{
    public class BashmagParsingRepo: IParseContent
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
            item.WebsiteName=Website.Bashmag;
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
                const int fractionalValuePoints = 2;
                item.Discount = (discount > 0)
                    ? Math.Round(100 - ((item.Price/discount)*100), fractionalValuePoints)
                    : discount;
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
            var hasNextPage = "Has Data";
            var finalitemDescriptionUrlList = new List<string>();
            while (!string.IsNullOrEmpty(hasNextPage))
            {
                List<string> itemDescriptionUrlList;
                if (HasItems(rootDocument, out itemDescriptionUrlList))
                {
                    finalitemDescriptionUrlList.AddRange(itemDescriptionUrlList);
                }
                hasNextPage = BashmagHasMoreThenCurrentPageItems(rootDocument);
                if (!string.IsNullOrEmpty(hasNextPage))
                    rootDocument = website.Load(string.Format("https://www.bashmag.ru{0}", hasNextPage));
            }

            finalitemDescriptionUrlList.ForEach(
                item => { itemList.Add(ParseItem(string.Format("https://www.bashmag.ru{0}", item))); });

            return itemList;
        }

        public List<Item> ParseAllPages(string url)
        {
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            var subcategoriesUrl = new List<string>();
            rootDocument = website.Load(url);
            const string subcategoryLinkClass = "//div[@class='category-item ']//a";
            var availableItem = rootDocument.DocumentNode.SelectNodes(subcategoryLinkClass);
            availableItem.ForEach(item => { subcategoriesUrl.Add(item.GetAttributeValue("href", "")); });
            subcategoriesUrl.ForEach(
                subCate => { itemList.AddRange(ParsePage(string.Format("https://www.bashmag.ru{0}", subCate))); });
            return itemList;
        }

        #region Private Methods
        private string BashmagHasMoreThenCurrentPageItems(HtmlDocument rootDocument)
        {
            var nextPageUrl = string.Empty;
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
        private bool HasItems(HtmlDocument rootDocument, out List<string> urlList)
        {
            var newUrlList = new List<string>();

            const string availableItemClass = "//div[@class='product-image-cont']//a";
            var availableItem = rootDocument.DocumentNode.SelectNodes(availableItemClass);
            availableItem.ForEach(item => { newUrlList.Add(item.GetAttributeValue("href", "")); });

            urlList = newUrlList;
            return urlList.Count > 0;
        }
        private bool HasId(HtmlDocument rootDocument, out string id)
        {
            id = string.Empty;

            const string productIdClass = "//input[@name='virtuemart_product_id[]']";
            var productId = rootDocument.DocumentNode.SelectSingleNode(productIdClass);
            if (productId == null) return false;
            id = productId.GetAttributeValue("value", "");

            return true;
        }
        private bool HasProductGallery(HtmlDocument rootDocument, out List<string> imageUrls)
        {
            imageUrls = new List<string>();

            const string productGalleryClass = "//figure[@class='product-image-gallery-cont']//img";
            var imageGallery = rootDocument.DocumentNode.SelectNodes(productGalleryClass);
            if (imageGallery == null || imageGallery.Count == 0) return false;
            imageUrls.AddRange(imageGallery.Select(node => node.GetAttributeValue("src", "")));

            return true;
        }
        private bool HasTitle(HtmlDocument rootDocument, out string type, out string brand,
            out string subType)
        {
            type = string.Empty;
            brand = string.Empty;
            subType = string.Empty;

            const int bashmagTypeIndex = 3;
            const int bashmagSubTypeIndex = 4;
            const int brandImageIndex = 1;
            var productTitleClass = "//ul[@class='breadcrumb']//li";
            var productTitleCollection = rootDocument.DocumentNode.SelectNodes(productTitleClass);
            if (productTitleCollection == null) return false;
            type = productTitleCollection.Count - 1 >= bashmagTypeIndex
                ? productTitleCollection[bashmagTypeIndex].InnerText
                : string.Empty;
            subType = productTitleCollection.Count - 1 >= bashmagSubTypeIndex
                ? productTitleCollection[bashmagSubTypeIndex].InnerText
                : string.Empty;
            productTitleClass = "//div[@class='product-manuf']";
            var productTitle = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
            if (productTitle != null)
            {
                brand = productTitle.InnerText.Replace('\t', ' ').Replace('\n', ' ').Trim();
                if (string.IsNullOrEmpty(brand))
                {
                    brand = productTitle.ChildNodes[brandImageIndex].GetAttributeValue("alt", "");
                }
            }

            return true;
        }
        private bool HasPrice(HtmlDocument rootDocument, out decimal price)
        {
            price = 0;

            const int bashmagPriceIndex = 0;
            const char bashmagPriceSplitter = ' ';
            var productPriceClass = "//div[@class='SalesPriceCat']";
            var productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
            if (productPrice == null)
            {
                productPriceClass = "//div[@class='BasePriceCat']";
                productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
            }
            if (productPrice == null) return false;
            var correctValue = productPrice.InnerText.Split(bashmagPriceSplitter)[bashmagPriceIndex];
            decimal.TryParse(correctValue, out price);

            return true;
        }
        private bool HasDiscount(HtmlDocument rootDocument, out decimal discount)
        {
            discount = 0;

            const int bashmagPriceIndex = 0;
            const char bashmagPriceSplitter = ' ';
            const string productDiscountClass = "//div[@class='OldBasePriceCat']";
            var productDiscountPrice = rootDocument.DocumentNode.SelectSingleNode(productDiscountClass);
            if (productDiscountPrice == null) return false;
            decimal.TryParse(productDiscountPrice.InnerText.Split(bashmagPriceSplitter)[bashmagPriceIndex],
                out discount);

            return true;
        }
        private bool HasSizes(HtmlDocument rootDocument, out List<Size> sizes)
        {
            var sizeList = new List<Size>();

            const int bashmagSizeNodeIndex = 1;
            const string availableSizesClass = "//div[@class='vpf-radio-button']//label";
            var availableSizes = rootDocument.DocumentNode.SelectNodes(availableSizesClass);
            availableSizes.ForEach(node =>
            {
                var availSize = node.ChildNodes[bashmagSizeNodeIndex].InnerText;
                sizeList.Add(new Size {SizeText = availSize, IsAvailable = !node.InnerHtml.Contains("disabled")});
            });

            sizes = sizeList.OrderBy(x => x.SizeText).ToList();
            return sizes.Count > 0;
        }
        private bool HasProperties(HtmlDocument rootDocument,
            out List<KeyValuePair<string, string>> propertiesList)
        {
            var newpropertiesList = new List<KeyValuePair<string, string>>();

            const string propertyClass = "//div[@class='product-cart-variants']";
            var properties = rootDocument.DocumentNode.SelectNodes(propertyClass);
            properties.ForEach(node =>
            {
                if (node.HasChildNodes)
                {
                    node.ChildNodes.ForEach(child =>
                    {
                        if (child.HasChildNodes)
                        {
                            var propertyKey = string.Empty;
                            var propertyValue = string.Empty;

                            child.ChildNodes.ForEach(children =>
                            {
                                if (children.GetAttributeValue("class", "").Contains("product-field-display"))
                                {
                                    propertyValue =
                                        children.InnerText.Replace('\t', ' ').Replace('\n', ' ').Trim();
                                }
                                if (children.GetAttributeValue("class", "").Contains("product-field-desc"))
                                {
                                    propertyKey =
                                        children.InnerText.Replace('\t', ' ').Replace('\n', ' ').Trim();
                                }
                            });

                            newpropertiesList.Add(new KeyValuePair<string, string>(propertyKey, propertyValue));
                        }
                    });
                }
            });

            propertiesList = newpropertiesList;
            return propertiesList.Count > 0;
        }
        #endregion
    }
}