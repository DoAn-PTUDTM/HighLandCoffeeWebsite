using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HighLandCoffeeWebsite.Models;

namespace HighLandCoffeeWebsite.Controllers
{
    public class AccountController : Controller
    {
        private CoffeeDataContext db = new CoffeeDataContext();

        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            // Kiểm tra tài khoản đăng nhập
            var user = db.Users.FirstOrDefault(u => u.UserName == Username && u.Password == Password);
            if (user != null)
            {
                // Lưu thông tin người dùng vào Session
                Session["User"] = user;

                // Hiển thị thông báo đăng nhập thành công
                //TempData["SuccessMessage"] = "Đăng nhập thành công!";

                // Kiểm tra xem người dùng có phải là admin hay không
                if (user.IsAdmin == true)
                {
                    // Điều hướng đến trang Admin nếu là Admin
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    // Điều hướng đến trang chủ nếu không phải admin
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                // Hiển thị thông báo lỗi nếu đăng nhập không thành công
                TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu sai.";
                return RedirectToAction("Login");
            }
        }

        // GET: Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public ActionResult Register(string Username, string Email, string Password)
        {
            // Kiểm tra nếu tên đăng nhập đã tồn tại trong cơ sở dữ liệu
            var existingUser = db.Users.FirstOrDefault(u => u.UserName == Username);
            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "Tên đăng nhập đã tồn tại.";
                return RedirectToAction("Register");
            }

            // Kiểm tra nếu email đã tồn tại
            var existingEmail = db.Users.FirstOrDefault(u => u.Email == Email);
            if (existingEmail != null)
            {
                TempData["ErrorMessage"] = "Email đã tồn tại.";
                return RedirectToAction("Register");
            }

            // Tạo đối tượng User mới và lưu vào cơ sở dữ liệu
            var newUser = new User
            {
                UserName = Username,
                Email = Email,
                Password = Password, // Trong thực tế, bạn nên mã hóa mật khẩu trước khi lưu
                FullName = "", // Để trống hoặc có thể lấy từ form nếu có
                PhoneNumber = "", // Cũng tương tự
                Address = "",
                IsAdmin = false // Mặc định là người dùng thường, nếu là admin thì thay đổi sau
            };

            db.Users.InsertOnSubmit(newUser); // Thêm người dùng vào bảng Users
            db.SubmitChanges(); // Lưu người dùng vào cơ sở dữ liệu

            TempData["SuccessMessage"] = "Đăng ký thành công!";
            return RedirectToAction("Login"); // Chuyển hướng đến trang đăng nhập
        }

        // GET: Logout
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa thông tin người dùng trong Session
            //TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công!";
            return RedirectToAction("Login");
        }

        // Hiển thị thông tin người dùng
        public ActionResult User_info()
        {
            var user = Session["User"] as User;
            if (user != null)
            {
                return View(user);
            }
            TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem thông tin!";
            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult UpdateUserInfo(User model)
        {
            try
            {
                // Lấy thông tin người dùng từ cơ sở dữ liệu
                var user = db.Users.FirstOrDefault(u => u.UserName == model.UserName);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Người dùng không tồn tại!";
                    return RedirectToAction("User_info"); // Nếu không tìm thấy người dùng
                }

                // Cập nhật thông tin người dùng
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SubmitChanges();

                // Cập nhật lại session với thông tin người dùng mới (nếu cần)
                Session["User"] = user;

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";

                // Load lại trang "User_info" với thông tin người dùng mới
                return RedirectToAction("User_info");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("User_info");
            }
        }
        [HttpGet]
        public JsonResult GetOrderHistory()
        {
            var userId = (Session["User"] as User)?.UserId;
            if (userId == null)
            {
                return Json(new { success = false, message = "Người dùng không hợp lệ." }, JsonRequestBehavior.AllowGet);
            }

            var orders = db.Orders
                           .Where(o => o.UserId == userId)
                           .Select(o => new
                           {
                               o.OrderId,
                               o.OrderDate,
                               o.TotalAmount,
                               o.Address,
                               Items = o.OrderItems.Select(d => new
                               {
                                   d.Product.Name,
                                   d.Product.ImageUrl,
                                   d.Quantity,
                                   d.Price
                               })
                           })
                           .ToList();

            return Json(orders, JsonRequestBehavior.AllowGet);
        }
    }
}