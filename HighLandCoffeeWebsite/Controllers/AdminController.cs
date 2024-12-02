using HighLandCoffeeWebsite.Models;
using HighLandCoffeeWebsite.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HighLandCoffeeWebsite.Controllers
{
    public class AdminController : Controller
    {
        AdminService adminService = new AdminService();
        // GET: AdminHome
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewProduct()
        {
            try
            {

                ViewData["Err"] = TempData["Err"];
                List<Admin_Product> p = adminService.ExcuteSQL();
                return View(p);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(Admin_Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    adminService.AddSql(product.productName, product.productDescription, product.price, product.images, product.productTypeID);
                    return RedirectToAction("ViewProduct");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        public ActionResult DeleteProduct(int id)
        {
            try
            {
                if (!adminService.checkProductInCart(id))
                {
                    adminService.DeleteSql(id);
                    return RedirectToAction("ViewProduct");
                }
                else
                {
                    TempData["Err"] = "Hiện tại chưa xoá được sản phẩm";
                    return RedirectToAction("ViewProduct");
                }

            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

    }
}