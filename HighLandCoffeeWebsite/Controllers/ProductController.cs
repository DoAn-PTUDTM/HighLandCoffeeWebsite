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
        // private CoffeeDataContext db = new CoffeeDataContext();

        // public ActionResult ViewProduct()
        // {
        //     var products = db.Products.ToList();
        //     return View(products);
        // }

        public ActionResult ProductDetails(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var relatedProducts = db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != id)
                .Take(5)
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity, string size, int additionalPrice)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            var user = Session["User"] as User;  // Cast session về kiểu User

            // Kiểm tra nếu người dùng chưa đăng nhập
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập trước khi thêm sản phẩm vào giỏ hàng" });
            }

            // Lấy UserId từ người dùng
            var userId = user.UserId;

            // Kiểm tra xem người dùng đã có sản phẩm này trong giỏ hàng chưa
            var existingItem = db.ShoppingCarts.FirstOrDefault(s => s.UserId == userId && s.ProductId == productId && s.Size == size);

            if (existingItem != null)
            {
                // Nếu có, cập nhật số lượng
                existingItem.Quantity += quantity;  // Cộng thêm số lượng vào
                db.SubmitChanges();  // Lưu lại thay đổi
                return Json(new { success = true, message = "Sản phẩm đã được cập nhật số lượng trong giỏ hàng" });
            }
            else
            {
                // Nếu không có, thêm sản phẩm mới vào giỏ hàng
                var cartItem = new ShoppingCart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    Size = size
                };

                db.ShoppingCarts.InsertOnSubmit(cartItem);
                db.SubmitChanges();

                return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng" });
            }
        }
 

    }
}