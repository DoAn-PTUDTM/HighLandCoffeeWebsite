using System;
using System.Collections.Generic;
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
        public ActionResult Customer()
        {
            // Lấy tất cả người dùng từ cơ sở dữ liệu
            var users = db.Users.ToList();

            // Trả về view kèm theo danh sách người dùng
            return View(users);
        }
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}