using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using WebGrease.Css.Extensions;

namespace KendoUIApp.Models
{
    public class ParserBase
    {
        protected bool GetAllPages(HtmlDocument rootDocument, out List<string> pageList)
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

        protected bool HasItems(HtmlDocument rootDocument, Website website, out List<string> urlList)
        {
            var newUrlList = new List<string>();
            string availableItemClass;
            HtmlNodeCollection availableItem;
            switch (website)
            {
                case Website.Sapato:
                     availableItemClass = "//div[@class='catalog-items__list catalog__list']//article[@class='catalog__item clearfix']//a[@class='catalog__image']";
                    availableItem = rootDocument.DocumentNode.SelectNodes(availableItemClass);
                    availableItem.ForEach(item => { newUrlList.Add(item.GetAttributeValue("href", "")); });
                    break;
                case Website.Bashmag:
                    availableItemClass = "//div[@class='product-image-cont']//a";
                    availableItem = rootDocument.DocumentNode.SelectNodes(availableItemClass);
                    availableItem.ForEach(item => { newUrlList.Add(item.GetAttributeValue("href", "")); });
                    break;
            }
            urlList = newUrlList;
            return urlList.Count > 0;
        }

        protected bool HasId(HtmlDocument rootDocument, Website website, out string id)
        {
            id = string.Empty;
            string productIdClass;
            HtmlNode productId;
            switch (website)
            {
                case Website.Sapato:
                    productIdClass = "//input[@id='productID']";
                    productId = rootDocument.DocumentNode.SelectSingleNode(productIdClass);
                    if (productId == null) return false;
                    id = productId.GetAttributeValue("value", "");
                    break;
                case Website.Bashmag:
                    productIdClass = "//input[@name='Itemid']";
                    productId = rootDocument.DocumentNode.SelectSingleNode(productIdClass);
                    if (productId == null) return false;
                    id = productId.GetAttributeValue("value", "");
                    break;
            }
            return true;
        }

        protected bool HasProductGallery(HtmlDocument rootDocument, Website website, out List<string> imageUrls)
        {
            imageUrls = new List<string>();
            string productGalleryClass;
            HtmlNodeCollection imageGallery;
            switch (website)
            {
                case Website.Sapato:
                    productGalleryClass = "//div[@class='product-gallery__list-wrapper']//img";
                    imageGallery = rootDocument.DocumentNode.SelectNodes(productGalleryClass);
                    if (imageGallery == null || imageGallery.Count == 0) return false;
                    imageUrls.AddRange(imageGallery.Select(node => node.GetAttributeValue("src", "")));
                    break;
                case Website.Bashmag:
                    productGalleryClass = "//figure[@class='product-image-gallery-cont']//img";
                    imageGallery = rootDocument.DocumentNode.SelectNodes(productGalleryClass);
                    if (imageGallery == null || imageGallery.Count == 0) return false;
                    imageUrls.AddRange(imageGallery.Select(node => node.GetAttributeValue("src", "")));
                    break;
            }
            return true;
        }

        protected bool HasTitle(HtmlDocument rootDocument, Website website, out string type, out string brand,
            out string subType)
        {
            const int typeIndex = 0;
            const int brandIndex = 1;
            const char typeBrandSeperator = ',';
            type = string.Empty;
            brand = string.Empty;
            subType = string.Empty;
            string productTitleClass;
            HtmlNode productTitle;
            switch (website)
            {
                case Website.Sapato:
                    productTitleClass = "//h1[@class='product-info__title']";
                    productTitle = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
                    if (productTitle == null) return false;
                    var data = productTitle.InnerHtml.Split(typeBrandSeperator);
                    type = data[typeIndex];
                    brand = data[brandIndex];
                    subType = productTitle.NextSibling.NextSibling.InnerText;
                    break;
                case Website.Bashmag:
                    const int bashmagTypeIndex = 3;
                    const int bashmagSubTypeIndex = 4;
                    const int brandImageIndex = 1;
                    productTitleClass = "//ul[@class='breadcrumb']//li";
                    var productTitleCollection = rootDocument.DocumentNode.SelectNodes(productTitleClass);
                    if (productTitleCollection == null) return false;
                    type = productTitleCollection.Count - 1 >= bashmagTypeIndex
                        ? productTitleCollection[bashmagTypeIndex].InnerText
                        : string.Empty;
                    subType = productTitleCollection.Count - 1 >= bashmagSubTypeIndex
                        ? productTitleCollection[bashmagSubTypeIndex].InnerText
                        : string.Empty;
                    productTitleClass = "//div[@class='product-manuf']";
                    productTitle = rootDocument.DocumentNode.SelectSingleNode(productTitleClass);
                    brand = productTitle.InnerText.Replace('\t', ' ').Replace('\n', ' ').Trim();
                    if (string.IsNullOrEmpty(brand))
                    {
                        brand = productTitle.ChildNodes[brandImageIndex].GetAttributeValue("alt", "");
                    }
                    break;
            }
            return true;
        }

        protected bool HasPrice(HtmlDocument rootDocument, Website website, out decimal price)
        {
            price = 0;
            string productPriceClass;
            HtmlNode productPrice;
            string correctValue;
            switch (website)
            {
                case Website.Sapato:
                    productPriceClass = "//span[@class='product-info__new-price']/text()";
                    productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
                    if (productPrice == null) return false;
                    correctValue = Regex.Replace(productPrice.InnerText, @"\s+", "");
                    decimal.TryParse(correctValue, out price);
                    break;
                case Website.Bashmag:
                    const int bashmagPriceIndex = 0;
                    const char bashmagPriceSplitter = ' ';
                    productPriceClass = "//div[@class='SalesPriceCat']";
                    productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
                    if (productPrice == null)
                    {
                        productPriceClass = "//div[@class='BasePriceCat']";
                        productPrice = rootDocument.DocumentNode.SelectSingleNode(productPriceClass);
                    }
                    if (productPrice == null) return false;
                    correctValue = productPrice.InnerText.Split(bashmagPriceSplitter)[bashmagPriceIndex];
                    decimal.TryParse(correctValue, out price);
                    break;
            }
            return true;
        }

        protected bool HasDiscount(HtmlDocument rootDocument, Website website, out decimal discount)
        {
            discount = 0;
            HtmlNode productDiscountPrice;
            string productDiscountClass;
            switch (website)
            {
                case Website.Sapato:
                    productDiscountClass = "//span[@class='subnav-product__discount']";
                    productDiscountPrice = rootDocument.DocumentNode.SelectSingleNode(productDiscountClass);
                    if (productDiscountPrice == null) return false;
                    decimal.TryParse(productDiscountPrice.InnerText.TrimEnd('%'), out discount);
                    break;
                case Website.Bashmag:
                    const int bashmagPriceIndex = 0;
                    const char bashmagPriceSplitter = ' ';
                    productDiscountClass = "//div[@class='OldBasePriceCat']";
                    productDiscountPrice = rootDocument.DocumentNode.SelectSingleNode(productDiscountClass);
                    if (productDiscountPrice == null) return false;
                    decimal.TryParse(productDiscountPrice.InnerText.Split(bashmagPriceSplitter)[bashmagPriceIndex],
                        out discount);
                    break;
            }
            return true;
        }

        protected bool HasSizes(HtmlDocument rootDocument, Website website, out List<Size> sizes)
        {
            string availableSizesClass;
            var sizeList = new List<Size>();
            HtmlNodeCollection availableSizes;
            switch (website)
            {
                case Website.Sapato:
                    availableSizesClass = "//li[contains(@class, 'radio__item radio__item_size')]";
                    availableSizes = rootDocument.DocumentNode.SelectNodes(availableSizesClass);
                    availableSizes.ForEach(node =>
                    {
                        var availSize = node.GetAttributeValue("id", "").Replace("size_", "");
                        sizeList.Add(new Size {SizeText = availSize, IsAvailable = !node.InnerHtml.Contains("disabled")});
                    });
                    break;
                case Website.Bashmag:
                    const int bashmagSizeNodeIndex = 1;
                    availableSizesClass = "//div[@class='vpf-radio-button']//label";
                    availableSizes = rootDocument.DocumentNode.SelectNodes(availableSizesClass);
                    availableSizes.ForEach(node =>
                    {
                        var availSize = node.ChildNodes[bashmagSizeNodeIndex].InnerText;
                        sizeList.Add(new Size {SizeText = availSize, IsAvailable = !node.InnerHtml.Contains("disabled")});
                    });
                    break;
            }
            sizes = sizeList.OrderBy(x => x.SizeText).ToList();
            return sizes.Count > 0;
        }

        protected bool HasProperties(HtmlDocument rootDocument, Website website,
            out List<KeyValuePair<string, string>> propertiesList)
        {
            var newpropertiesList = new List<KeyValuePair<string, string>>();
            string propertyClass;
            HtmlNodeCollection properties;
            switch (website)
            {
                case Website.Sapato:
                    propertyClass = "//ul[@class='product-chars__list']//li[@class='product-chars__item']";
                    properties = rootDocument.DocumentNode.SelectNodes(propertyClass);
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
                    break;
                case Website.Bashmag:
                    propertyClass = "//div[@class='product-cart-variants']";
                    properties = rootDocument.DocumentNode.SelectNodes(propertyClass);
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
                    break;
            }
            propertiesList = newpropertiesList;
            return propertiesList.Count > 0;
        }
    }
}