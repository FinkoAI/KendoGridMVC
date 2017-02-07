using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using WebGrease.Css.Extensions;

namespace KendoUIApp.Models
{
    public class SpvtomskeParsingRepo : IParseContent
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
            var isloaded = false;
            HtmlDocument rootDocument = null;
            while (!isloaded)
            {
                try
                {
                    rootDocument = website.Load(url);
                    isloaded = true;
                }
                catch
                {
                    isloaded = false;
                }
            }

            if (rootDocument == null) return item;
            item.Url = url;
            item.WebsiteName = Website.Spvtomske;
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
            var isloaded = false;
            HtmlDocument rootDocument = null;
            while (!isloaded)
            {
                try
                {
                    rootDocument = website.Load(url);
                    isloaded = true;
                }
                catch
                {
                    isloaded = false;
                }
            }

            if (rootDocument == null) return itemList;
            List<string> itemDescriptionUrlList;
            if (HasItems(rootDocument, out itemDescriptionUrlList))
            {
                itemDescriptionUrlList.AsParallel().ForEach(
                    item => { itemList.Add(ParseItem(string.Format("http://spvtomske.ru/sp/newCatalog/index/id/{0}", item))); });
            }
            return itemList;
        }

        public List<Item> ParseAllPages(string url)
        {
            var itemList = new List<Item>();
            List<string> itemDescriptionUrlList;
            int pageNum = 1;
            while (IsValidPage(url, pageNum, out itemDescriptionUrlList))
            {
                itemDescriptionUrlList.ForEach(
                    item => { itemList.AddRange(ParsePage(item)); });
                pageNum++;
                Debug.WriteLine(pageNum);
            }
            return itemList;
        }

        #region Private Methods

        private bool IsValidPage(string url,int pageNum, out List<string> urlList)
        {
            var newUrlList = new List<string>();
            var website = new HtmlWeb();
            var loadUrl = string.Format("{0}?page={1}", url, pageNum);
            var rootDocument = website.Load(loadUrl);
            urlList = newUrlList;
            if (rootDocument == null) return false;
            const string nextPageLinkClass = "//table[@id='datatable']/tbody/tr";
            var nextPage = rootDocument.DocumentNode.SelectNodes(nextPageLinkClass);
            if (nextPage != null &&  nextPage.Count > 0)
            {
                newUrlList.Add(loadUrl);
                urlList = newUrlList;
            }
            return urlList.Count > 0;
        }

        private bool HasItems(HtmlDocument rootDocument, out List<string> urlList)
        {
            var newUrlList = new List<string>();
            const string availableItemClass = "//table[@id='datatable']/tbody/tr/td/div";
            var availableItem = rootDocument.DocumentNode.SelectNodes(availableItemClass);
            availableItem.ForEach(item =>
            {
                if(!string.IsNullOrEmpty(item.GetAttributeValue("tovarid", "")))
                newUrlList.Add(item.GetAttributeValue("tovarid", ""));
            });
            urlList = newUrlList;
            return urlList.Count > 0;
        }

        private bool HasId(HtmlDocument rootDocument, out string id)
        {
            id = string.Empty;
            const string productIdClass = "//input[@name='tovarId']";
            var productId = rootDocument.DocumentNode.SelectSingleNode(productIdClass);
            if (productId == null) return false;
            id = productId.GetAttributeValue("value", "");
            return true;
        }

        private bool HasTitle(HtmlDocument rootDocument, out string title)
        {
            title = string.Empty;
            const string productTitleClass = "//h2[@itemprop='name']/text()";
            var productTitle = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
            if (productTitle == null) return false;
            title = productTitle.InnerText;
            return true;
        }

        private bool HasProductGallery(HtmlDocument rootDocument, out List<string> imageUrls)
        {
            imageUrls = new List<string>();
            string productGalleryClass = "//a[@id='luu_a']";
            var imageGallery = rootDocument.DocumentNode.SelectSingleNode(productGalleryClass);
            if (imageGallery == null) return false;
            var className = imageGallery.GetAttributeValue("class", "");
            productGalleryClass = "//a[@class='" + className + "']";
            var images = rootDocument.DocumentNode.SelectNodes(productGalleryClass);
            if (images == null || images.Count == 0) return false;
            imageUrls.AddRange(images.Select(node => node.GetAttributeValue("href", "")));
            return true;
        }

        private bool HasTitle(HtmlDocument rootDocument, out string type, out string brand,
            out string subType)
        {
            type = string.Empty;
            brand = string.Empty;
            const int subTypeIndex = 1;
            var calcSubType = string.Empty;
            var productTitleClass = "//meta[@name='description']";
            var productTitle = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
            if (productTitle != null)
            {
                var metaContent = productTitle.GetAttributeValue("content", "");
                metaContent.Split(';').ForEach(x =>
                {
                    if (x.Contains("Тип товара"))
                    {
                        calcSubType = x.Split(':')[subTypeIndex];
                    }
                });
            }
            subType = calcSubType;
            productTitleClass = "//div[@itemprop='brand']/text()";
            var productBrand = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
            if (productBrand != null)
            {
                brand = productBrand.InnerText;
            }
            return true;
        }

        private bool HasPrice(HtmlDocument rootDocument, out decimal price)
        {
            price = 0;
            const int priceIndex=0;
            const string productPriceClass = "//h3[@itemprop='price']/text()";
            var productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
            if (productPrice == null) return false;
            var data = productPrice.InnerText.Split(' ')[priceIndex];
            decimal.TryParse(data, out price);
            return true;
        }

        private bool HasDiscount(HtmlDocument rootDocument, out decimal discount)
        {
            discount = 0;
            return false;
        }

        private bool HasSizes(HtmlDocument rootDocument, out List<Size> sizes)
        {
            var sizeList = new List<Size>();
            const int sizesIndex = 1;
            const string availableSizesClass = "//meta[@name='description']";
            var availableSizes = rootDocument.DocumentNode.SelectSingleNode(availableSizesClass);
            if (availableSizes != null)
            {
                var metaContent = availableSizes.GetAttributeValue("content", "");
                metaContent.Split(';').ForEach(x =>
                {
                    if (x.Contains("Размер"))
                    {
                        x.Split(':')[sizesIndex].Split(',').ForEach(y =>
                        {
                            if (!string.IsNullOrEmpty(y))
                                sizeList.Add(new Size {SizeText = y, IsAvailable = true});
                        });
                    }
                });
            }
            sizes = sizeList.OrderBy(x => x.SizeText).ToList();
            return sizes.Count > 0;
        }

        private bool HasProperties(HtmlDocument rootDocument,
            out List<KeyValuePair<string, string>> propertiesList)
        {
            var newpropertiesList = new List<KeyValuePair<string, string>>();
            const string propertyClass = "//div[@class='formd_i']";
            var properties = rootDocument.DocumentNode.SelectNodes(propertyClass);
            properties.ForEach(node =>
            {
                var propertyKey = string.Empty;
                var propertyValue = string.Empty;
                node.ChildNodes.ForEach(child =>
                {
                    if (child.GetAttributeValue("class", "").Equals("formd_t"))
                    {
                        propertyKey = child.InnerText.TrimEnd(':');
                    }
                    if (child.GetAttributeValue("class", "").Equals("formd_param"))
                    {
                        propertyValue = child.InnerText;
                    }
                    if (child.GetAttributeValue("class", "").Equals("formd_param radio"))
                    {
                        var data =new List<string>();
                        child.ChildNodes.ForEach(radio =>
                        {
                            if(radio.Name.Equals("p"))
                            data.Add(radio.InnerText.Replace("\r\n","").Trim());
                        });
                        propertyValue = String.Join(",", data);
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