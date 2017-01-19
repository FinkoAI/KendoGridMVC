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
    }
}