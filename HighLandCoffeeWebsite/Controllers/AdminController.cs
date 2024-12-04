using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HighLandCoffeeWebsite.Models;

namespace HighLandCoffeeWebsite.Controllers
{
    public class AdminController : Controller
    {
        private CoffeeDataContext db = new CoffeeDataContext();
        // GET: AdminHome]
        public ActionResult Customer(string searchTerm, string sortOrder, int page = 1, int pageSize = 5)
        {
            // Lấy tất cả người dùng từ cơ sở dữ liệu
            var users = db.Users.AsQueryable();

            // Tìm kiếm theo từ khóa (nếu có)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => u.UserName.Contains(searchTerm)
                                      || u.FullName.Contains(searchTerm));
            }

            // Sắp xếp theo tên tài khoản
            switch (sortOrder)
            {
                case "username_asc":
                    users = users.OrderBy(u => u.UserName);
                    break;
                case "username_desc":
                    users = users.OrderByDescending(u => u.UserName);
                    break;
                default:
                    users = users.OrderBy(u => u.UserId); // Sắp xếp theo UserId mặc định
                    break;
            }

            // Tính tổng số người dùng
            var totalUsers = users.Count();
            var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

            // Phân trang
            users = users.Skip((page - 1) * pageSize).Take(pageSize);

            // Trả về View với danh sách người dùng
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortOrder = sortOrder;

            return View(users.ToList());
        }
        public ActionResult Dashboard()
        {
            // Lấy danh sách 5 sản phẩm bán chạy
            var topProducts = (from orderItem in db.OrderItems
                               group orderItem by orderItem.ProductId into g
                               join product in db.Products on g.Key equals product.ProductId
                               orderby g.Sum(x => x.Quantity) descending
                               select new
                               {
                                   Name = product.Name,
                                   ImageUrl = product.ImageUrl,
                                   TotalSold = g.Sum(x => x.Quantity)
                               }).Take(5).ToList();
            // Chuyển đổi anonymous type thành ExpandoObject
            var topProductsDynamic = topProducts.Select(p => {
                dynamic item = new ExpandoObject();
                item.Name = p.Name;
                item.ImageUrl = p.ImageUrl;
                item.TotalSold = p.TotalSold;
                return item;
            }).ToList();

            ViewBag.TopProducts = topProductsDynamic;

            // Tổng doanh thu
            var totalRevenue = db.Orders.Sum(order => order.TotalAmount);

            // Tổng số người dùng
            int totalUsers = db.Users.Count();

            // Tổng số sản phẩm
            int totalProducts = db.Products.Count();

            // Tổng số đơn hàng
            int totalOrders = db.Orders.Count();

            // Doanh thu hàng tháng
            var monthlyRevenue = db.Orders
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(o => o.TotalAmount) })
                .ToList();

            // Chuyển dữ liệu qua ViewBag
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.MonthlyRevenue = monthlyRevenue;

            return View();
        }
        public ActionResult Signout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }
        public ActionResult EditUser(int id)
        {
            var user = db.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Xử lý cập nhật thông tin người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(User model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.UserId == model.UserId);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Password = model.Password;
                    user.Address = model.Address;
                    db.SubmitChanges();
                    return RedirectToAction("Customer");
                }
            }
            return View(model);
        }

        // Xóa người dùng
        public ActionResult DeleteUser(int id)
        {
            var user = db.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return HttpNotFound();
            }

            db.Users.DeleteOnSubmit(user);
            db.SubmitChanges();

            return RedirectToAction("Customer");
        }
    }
}