using Kendo.Mvc.UI;
using KendoUIApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;

namespace KendoUIApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        ProductModelRepository productService = new ProductModelRepository();
        public ActionResult Index()
        {
            ViewBag.ProductCategories = productService.ProductCategories;
            ViewBag.defaultCategory = productService.ProductCategories.First();
            return View();
        }

        public ActionResult PopUpDemo()
        {
            ViewBag.ProductCategories = productService.ProductCategories;
            ViewBag.defaultCategory = productService.ProductCategories.First();
            return View();
        }

        public ActionResult Products_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(productService.Products.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Products_Update([DataSourceRequest] DataSourceRequest request,
           [Bind(Prefix = "models")]IEnumerable<ProductModel> products)
        {
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    productService.Update(product);
                }
            }

            return Json(products.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Product_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]ProductModel product)
        {
            if (product != null && ModelState.IsValid)
            {

                productService.Update(product);
            }

            return Json(null);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Products_Destroy([DataSourceRequest] DataSourceRequest request, ProductModel product)
        {
            if (product != null)
            {
                productService.Destroy(product);
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Products_Create([DataSourceRequest] DataSourceRequest request, ProductModel product)
        {
            if (product != null && ModelState.IsValid)
            {
                productService.Create(product);
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

      public ActionResult PopupDataForm(IEnumerable<int> prodIds)
        {
            if (prodIds != null)
                ViewBag.Message = string.Format("{0} records selected for product category change", prodIds.Count());
            else
                ViewBag.Message = "No Record is select for product category change";
            TempData["prodIds"] = prodIds;
            ViewBag.ProductCategories = productService.ProductCategories;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public string UpdateCategoryBatch(int prodCatId)
        {
            string message = "No Record is update";
            if (TempData["prodIds"] != null)
            {
                var prodIds = (IEnumerable<int>)TempData["prodIds"];
                message = string.Format("{0} record updated", prodIds.Count());
                var prodCategory = productService.ProductCategories.ToList().Find(y => y.ProductCategoryId.Equals(prodCatId));
                prodIds.ToList().ForEach(x =>
                {
                    var product = productService.Products.ToList().Find(y => y.ProductId.Equals(x));
                    product.ProductCategory = prodCategory;
                });
            }           
            return message;
        }
    }
}