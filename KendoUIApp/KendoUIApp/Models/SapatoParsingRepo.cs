using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using WebGrease.Css.Extensions;

namespace KendoUIApp.Models
{
    public class SapatoParsingRepo: IParseContent
    {
        public Item ParseItem(string url)
        {
            var item = new Item();
            var website = new HtmlWeb
            {
                PreRequest = delegate(HttpWebRequest webRequest)
                {
                    webRequest.Timeout = 30000;
                    return true;
                }
            };
            var rootDocument = website.Load(url);
            if (rootDocument == null) return item;
            item.Url = url;
            item.WebsiteName = Website.Sapato;
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
                    item => { itemList.Add(ParseItem(string.Format("https://www.sapato.ru{0}", item))); });
            }

            return itemList;
        }

        public List<Item> ParseAllPages(string url)
        {
            var itemList = new List<Item>();
            var website = new HtmlWeb();
            var rootDocument = website.Load(url);
            if (rootDocument == null) return itemList;
            List<string> pageFilterUrlList;
            if (GetAllPages(rootDocument, out pageFilterUrlList))
            {
                pageFilterUrlList.ForEach(
                    page => { itemList.AddRange(ParsePage(page)); });
            }
            return itemList;
        }

        #region Private Methods
        private bool GetAllPages(HtmlDocument rootDocument, out List<string> pageList)
        {
            var newPageList = new List<string>();
            const char finalPageSplitChar = '=';
            const int finalQtyIndex = 1;
            var pageSizeSelection = string.Empty;
            var totalItemPages = 0;
            const string pageSizeClass = "//select[@class='page-count']/option";
            const string totalPagesClass = "//a[@class='page-nav__link page-nav__link_next-all']";
            var pageSize = rootDocument.DocumentNode.SelectNodes(pageSizeClass);
            var totalPages = rootDocument.DocumentNode.SelectSingleNode(totalPagesClass);
            if (pageSize != null && pageSize.Count > 0)
            {
                pageSize.ForEach(node =>
                {
                    if (node.Attributes.Count > 1)
                    {
                        pageSizeSelection = node.GetAttributeValue("value", "");
                    }
                });
            }
            if (totalPages != null)
            {
                var finalValue = totalPages.GetAttributeValue("href", "").Split(finalPageSplitChar)[finalQtyIndex];
                int.TryParse(finalValue, out totalItemPages);
            }
            Enumerable.Range(1, totalItemPages).ForEach(x =>
            {
                newPageList.Add(string.Format("https://www.sapato.ru/woman/?page={0}&{1}", x,
                    pageSizeSelection));
            }
                );

            pageList = newPageList;
            return pageList.Count > 0;
        }

        protected bool HasItems(HtmlDocument rootDocument, out List<string> urlList)
        {
            var newUrlList = new List<string>();
            const string availableItemClass =
                "//div[@class='catalog-items__list catalog__list']//article[@class='catalog__item clearfix']//a[@class='catalog__image']";
            var availableItem = rootDocument.DocumentNode.SelectNodes(availableItemClass);
            availableItem.ForEach(item => { newUrlList.Add(item.GetAttributeValue("href", "")); });
            urlList = newUrlList;
            return urlList.Count > 0;
        }

        protected bool HasId(HtmlDocument rootDocument, out string id)
        {
            id = string.Empty;
            const string productIdClass = "//input[@id='productID']";
            var productId = rootDocument.DocumentNode.SelectSingleNode(productIdClass);
            if (productId == null) return false;
            id = productId.GetAttributeValue("value", "");

            return true;
        }

        protected bool HasProductGallery(HtmlDocument rootDocument, out List<string> imageUrls)
        {
            imageUrls = new List<string>();
            const string productGalleryClass = "//div[@class='product-gallery__list-wrapper']//img";
            var imageGallery = rootDocument.DocumentNode.SelectNodes(productGalleryClass);
            if (imageGallery == null || imageGallery.Count == 0) return false;
            imageUrls.AddRange(imageGallery.Select(node => node.GetAttributeValue("src", "")));
            return true;
        }

        protected bool HasTitle(HtmlDocument rootDocument, out string type, out string brand,
            out string subType)
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

        protected bool HasPrice(HtmlDocument rootDocument, out decimal price)
        {
            price = 0;
            const string productPriceClass = "//span[@class='product-info__new-price']/text()";
            var productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
            if (productPrice == null) return false;
            var correctValue = Regex.Replace(productPrice.InnerText, @"\s+", "");
            decimal.TryParse(correctValue, out price);
            return true;
        }

        protected bool HasDiscount(HtmlDocument rootDocument, out decimal discount)
        {
            discount = 0;
            const string productDiscountClass = "//div[@class='product-gallery__discount']";
            var productDiscountPrice = rootDocument.DocumentNode.SelectSingleNode(productDiscountClass);
            if (productDiscountPrice == null) return false;
            decimal.TryParse(productDiscountPrice.InnerText.Replace('\n',' ').Replace('%',' ').Trim(), out discount);
            return true;
        }

        protected bool HasSizes(HtmlDocument rootDocument, out List<Size> sizes)
        {
            var sizeList = new List<Size>();

            const string availableSizesClass = "//li[contains(@class, 'radio__item radio__item_size')]";
            var availableSizes = rootDocument.DocumentNode.SelectNodes(availableSizesClass);
            availableSizes.ForEach(node =>
            {
                var availSize = node.GetAttributeValue("id", "").Replace("size_", "");
                sizeList.Add(new Size {SizeText = availSize, IsAvailable = !node.InnerHtml.Contains("disabled")});
            });

            sizes = sizeList.OrderBy(x => x.SizeText).ToList();
            return sizes.Count > 0;
        }

        protected bool HasProperties(HtmlDocument rootDocument,
            out List<KeyValuePair<string, string>> propertiesList)
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