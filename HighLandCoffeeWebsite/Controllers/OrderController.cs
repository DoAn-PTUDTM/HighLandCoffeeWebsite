using HighLandCoffeeWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HighLandCoffeeWebsite.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        CoffeeDataContext db = new CoffeeDataContext();
        public ActionResult Index()
        {

            // Get the currently logged-in user
            User currentUser = Session["user"] as User;
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not logged in
            }

            // Fetch only the most recent order for the logged-in user
            var latestOrder = db.Orders
                                .Where(o => o.UserId == currentUser.UserId)  // Filter by current user
                                .OrderByDescending(o => o.OrderDate)  // Order by most recent first
                                .FirstOrDefault();  // Get only the most recent order

            return View(latestOrder); // Pass the most recent order to the view

        }
    }
}