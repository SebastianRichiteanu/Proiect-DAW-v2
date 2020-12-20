using Microsoft.AspNet.Identity;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            Product prod = db.Products.Find(34);
            string usr_id = User.Identity.GetUserId();
            var products = db.Carts.Where(c => c.UserId.Equals(usr_id));
            ViewBag.carts = products;

            return View();
        }

        public ActionResult New(int id)
        {
            Product prod = db.Products.Find(id);
            string usr_id = User.Identity.GetUserId();
            Cart exista = db.Carts.Find(usr_id, prod.Id);
          //  var exista = db.Carts.Where(c => c.UserId.Equals(usr_id) && c.ProductId.Equals(prod.Id));
            if (exista == null)
            {
                Cart cart = new Cart();
                cart.UserId = usr_id;
                cart.ProductId = prod.Id;
                cart.Quantity = 1;
                db.Carts.Add(cart);
            }
            else
            {
                exista.Quantity++;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}