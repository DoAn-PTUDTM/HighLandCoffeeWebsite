﻿using HighLandCoffeeWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HighLandCoffeeWebsite.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        CoffeeDataContext db = new CoffeeDataContext();

        public ActionResult ViewCart()
        {
            User user = Session["user"] as User;

            // Kiểm tra nếu user chưa đăng nhập
            if (user == null)
            {
                ViewData["NoProduct"] = "Bạn chưa đăng nhập, vui lòng đăng nhập khi mua hàng";
                return RedirectToAction("Login", "Account"); // Điều hướng đến trang đăng nhập
            }

            // Sử dụng LINQ để lấy giỏ hàng theo UserId
            var cartList = db.ShoppingCarts.Where(c => c.UserId == user.UserId).ToList();

            // Kiểm tra nếu giỏ hàng trống và gán thông báo
            if (!cartList.Any())
            {
                ViewData["NoProduct"] = "Chưa có sản phẩm nào trong giỏ hàng";
            }

            return View(cartList);
        }

        [HttpPost]
        public ActionResult AddCart(int prodID, string size, int num)
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            User user = Session["user"] as User;
            if (user == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra nếu có sản phẩm và size hợp lệ
            if (prodID <= 0 || string.IsNullOrEmpty(size) || num <= 0)
            {
                // Nếu không có sản phẩm hoặc size hoặc số lượng không hợp lệ, trả lại lỗi hoặc thông báo
                ViewData["ErrorMessage"] = "Dữ liệu không hợp lệ";
                return RedirectToAction("ViewProduct", new { id = prodID });
            }

            // Lấy sản phẩm từ cơ sở dữ liệu (hoặc nếu cần thì bạn có thể bỏ qua nếu chỉ cần add sản phẩm vào giỏ)
            var product = db.Products.FirstOrDefault(p => p.ProductId == prodID);
            if (product == null)
            {
                ViewData["ErrorMessage"] = "Sản phẩm không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra giỏ hàng của người dùng
            var cartItem = db.ShoppingCarts.FirstOrDefault(c => c.UserId == user.UserId && c.ProductId == prodID);

            if (cartItem == null)
            {
                // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới
                db.ShoppingCarts.InsertOnSubmit(new ShoppingCart
                {
                    UserId = user.UserId,
                    ProductId = prodID,
                    Size = size,
                    Quantity = num,
                });
            }
            else
            {
                // Nếu sản phẩm đã có trong giỏ hàng, cập nhật số lượng
                cartItem.Quantity += num;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SubmitChanges();

            // Quay lại trang giỏ hàng
            return RedirectToAction("ViewCart");
        }

        public ActionResult Deleted(int prodID)
        {
            // Lấy thông tin người dùng từ session
            User user = Session["user"] as User;

            // Kiểm tra nếu người dùng chưa đăng nhập
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            // Tìm sản phẩm trong giỏ hàng của người dùng bằng LINQ
            var cartItem = db.ShoppingCarts.FirstOrDefault(c => c.UserId == user.UserId && c.ProductId == prodID);

            if (cartItem != null)
            {
                // Xóa sản phẩm khỏi giỏ hàng
                db.ShoppingCarts.DeleteOnSubmit(cartItem);
                db.SubmitChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            // Quay lại trang giỏ hàng
            return RedirectToAction("ViewCart", "Cart");
        }

        public ActionResult CheckOut()
        {
            User currentUser = Session["user"] as User;
            if (currentUser == null)
            {
                // Nếu người dùng chưa đăng nhập, điều hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

                // Lấy danh sách sản phẩm trong giỏ hàng của người dùng
                var cartItems = db.ShoppingCarts.Where(c => c.UserId == currentUser.UserId).ToList();

                if (cartItems.Count > 0)
                {
                var newOrder = new Order
                {
                    UserId = currentUser.UserId,
                    State = "Pending",
                    Address = currentUser.Address,
                    Phone = currentUser.PhoneNumber,
                    TotalAmount = (double)cartItems.Sum(c => c.Quantity * c.Product.Price),
                    OrderDate = DateTime.Now
                };

                db.Orders.InsertOnSubmit(newOrder);  
                db.SubmitChanges();

                // Thêm từng sản phẩm từ giỏ hàng vào bảng OrderItems
                foreach (var cartItem in cartItems)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = newOrder.OrderId, // Liên kết OrderId từ đơn hàng mới
                            ProductId = cartItem.ProductId,
                            Quantity = cartItem.Quantity,
                            Size = cartItem.Size, // Nếu có trường Size trong giỏ hàng
                            Price = (double)(cartItem.Quantity * cartItem.Product.Price) // Giá của sản phẩm
                        };

                        db.OrderItems.InsertOnSubmit(orderItem);
                    }

                db.SubmitChanges(); // Lưu các sản phẩm vào bảng OrderItems

                // Sau khi chuyển giỏ hàng sang đơn hàng, bạn có thể xóa giỏ hàng
                foreach (var cartItem in cartItems)
                {
                    db.ShoppingCarts.DeleteOnSubmit(cartItem);
                }

                db.SubmitChanges();  // Commit the changes to the database


                TempData["checkoutSuccess"] = "Checkout success!";

            }
                else
                {
                    // Nếu giỏ hàng trống, thông báo lỗi hoặc chuyển hướng khác
                    TempData["checkoutError"] = "Your cart is empty.";
                }

            // Điều hướng về trang chủ sau khi thanh toán
            return RedirectToAction("Index", "Order");
        }

    }
}