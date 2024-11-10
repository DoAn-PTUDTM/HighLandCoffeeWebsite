using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HighLandCoffeeWebsite.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult ViewProduct()
        {
            return View();
        }
    }
}