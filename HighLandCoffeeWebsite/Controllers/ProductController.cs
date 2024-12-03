using HighLandCoffeeWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HighLandCoffeeWebsite.Controllers
{
    public class ProductController : Controller
    {
        
        CoffeeDataContext db = new CoffeeDataContext();
        // GET: Product
        public ActionResult ViewProduct()
        {
            var product = db.Products.ToList();

            return View(product);
        }
 

    }
}