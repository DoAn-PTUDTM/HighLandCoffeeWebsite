using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HighLandCoffeeWebsite.Models;

namespace HighLandCoffeeWebsite.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult ViewProduct()
        {
            return View();
        }
        private static List<Product> _products = new List<Product>
    {
        new Product { ProductId = 1, Name = "Cà phê sữa đá", Price = 30000, ImageUrl = "/Content/img/background.jpg", Description = "Cà phê đậm đà, thơm ngon, phù hợp cho mọi người yêu thích cà phê.", CategoryId = 1 },
        new Product { ProductId = 2, Name = "Cà phê đen", Price = 25000, ImageUrl ="/Content/img/background.jpg", Description = "Cà phê đen nguyên chất, thích hợp cho những ai yêu thích vị đắng mạnh mẽ.", CategoryId = 1 },
        new Product { ProductId = 3, Name = "Sinh tố trái cây", Price = 35000, ImageUrl = "/Content/img/banner-img.png", Description = "Sinh tố tươi ngon với nhiều loại trái cây, bổ dưỡng và mát lạnh.", CategoryId = 2 },
        new Product { ProductId = 4, Name = "Trà sữa", Price = 20000, ImageUrl = "/Content/img/blog-img1.png", Description = "Trà sữa thơm ngon, ngọt ngào với hương vị trà và sữa đặc biệt.", CategoryId = 3 }
    };
        // GET: Product
        public ActionResult ProductDetails(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var relatedProducts = _products.Where(p => p.CategoryId == product.CategoryId && p.ProductId != id).Take(4).ToList();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}