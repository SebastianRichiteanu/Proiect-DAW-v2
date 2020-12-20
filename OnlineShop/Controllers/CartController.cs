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
            string usr_id = User.Identity.GetUserId();
            var products = db.Carts.Where(c => c.UserId.Equals(usr_id));
            ViewBag.carts = products;
            var prod = db.Products.Include("Category").Include("User");
            ViewBag.Prod = prod;

            return View();
        }
        [Authorize(Roles = "User,Editor,Admin")]
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
        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            string usr_id = User.Identity.GetUserId();
            Cart cart = db.Carts.Find(usr_id, id);
            db.Carts.Remove(cart);
            db.SaveChanges();
     
            return RedirectToAction("Index");
      
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Cantitate(int id)
        {
            string usr = User.Identity.GetUserId();
            Cart cart = db.Carts.Find(usr, id);
            
            return View(cart);
        }

        [HttpPut]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Cantitate(int id, Cart requestCart)
        {

            try
            {
                string usr = User.Identity.GetUserId();
                Cart cart = db.Carts.Find(usr, id);
                cart.Quantity = requestCart.Quantity;
                db.SaveChanges();
                TempData["message"] = "Produsul a fost modificat";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(requestCart);
            }
        }
    }
}