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
        private readonly ProductModelRepository _productService = new ProductModelRepository();

        public ActionResult Index()
        {
            ViewBag.ProductCategories = _productService.ProductCategories;
            ViewBag.defaultCategory = _productService.ProductCategories.First();
            return View();
        }

        public ActionResult PopUpDemo()
        {
            ViewBag.ProductCategories = _productService.ProductCategories;
            ViewBag.defaultCategory = _productService.ProductCategories.First();
            return View();
        }

        public ActionResult Products_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_productService.Products.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Products_Update([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")] IEnumerable<ProductModel> products)
        {
            var productModels = products as IList<ProductModel> ?? products.ToList();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in productModels.ToList())
                {
                    _productService.Update(product);
                }
            }

            return Json(productModels.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Product_Update([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")] ProductModel product)
        {
            if (product != null && ModelState.IsValid)
            {

                _productService.Update(product);
            }

            return Json(null);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Products_Destroy([DataSourceRequest] DataSourceRequest request, ProductModel product)
        {
            if (product != null)
            {
                _productService.Destroy(product);
            }

            return Json(new[] {product}.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Products_Create([DataSourceRequest] DataSourceRequest request, ProductModel product)
        {
            if (product != null && ModelState.IsValid)
            {
                _productService.Create(product);
            }

            return Json(new[] {product}.ToDataSourceResult(request, ModelState));
        }
    }
}