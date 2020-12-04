using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var products = db.Products.Include("Category");
            ViewBag.Products = products;
            return View();
        }
        public ActionResult New()
        {
            Product prod = new Product();
            prod.Categ = GetAllCategories();
            return View(prod);
        }
        [HttpPost]
        public ActionResult New(Product prod)
        {
            prod.Categ = GetAllCategories();
            
            try
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(prod);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost adaugat";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(prod);
                }
            }
            catch (Exception e)
            {
                return View(prod);
            }
        }
        public ActionResult Show(int id)
        {
            Product product = db.Products.Find(id);
            return View(product);
        }
        public ActionResult Edit(int id)
        {
            Product prod = db.Products.Find(id);
            prod.Categ = GetAllCategories();
            return View(prod);
        }

        [HttpPut]
        public ActionResult Edit(int id, Product requestProduct)
        {
            requestProduct.Categ = GetAllCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    Product product = db.Products.Find(id);

                    if (TryUpdateModel(product))
                    {
                        product.Title = requestProduct.Title;
                        product.Description = requestProduct.Description;
                        product.Price = requestProduct.Price;
                        product.Rating = requestProduct.Rating;
                        product.CategoryId = requestProduct.CategoryId;
                        db.SaveChanges();
                        TempData["message"] = "Produsul a fost modificat";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestProduct);
                }

            }
            catch (Exception e)
            {
                return View(requestProduct);
            }
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
           
            return selectList;
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Product prod = db.Products.Find(id);
            db.Products.Remove(prod);
            db.SaveChanges();
            TempData["message"] = "Produsul a fost sters";
            return RedirectToAction("Index");
        }
    }
}