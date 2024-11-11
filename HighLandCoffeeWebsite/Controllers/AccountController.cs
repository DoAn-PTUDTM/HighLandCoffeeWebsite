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
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult User_info()
        {
            var user = new User
            {
                userName = "johndoe",
                password = "123456",
                CustomerName = "John Doe",
                Email = "johndoe@example.com",
                PhoneNumber = "123456789",
                Address = "123 Main St",
                Admin = 0
            };

            return View(user);
        }
    }
}