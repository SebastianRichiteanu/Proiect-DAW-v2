using Microsoft.AspNet.Identity;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult AddRequest()
        {
            var products = db.Products.Include("Category").Include("User");
            ViewBag.Products = products;
            return View();
        }

        public ActionResult Index(string sortOrder)
        {
           

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var products = from p in db.Products.Include("Category").Include("User")
                           select p;


           
            switch (sortOrder)
            {
                case "pret_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                case "pret_cresc":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "rating_desc":
                    products = products.OrderByDescending(s => s.Rating);
                    break;
                case "rating_cresc":
                    products = products.OrderBy(s => s.Rating);
                    break;
            }

            ViewBag.Products = products;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Activat(int id)
        {
            Product prod = db.Products.Find(id);
            prod.Categ = GetAllCategories();
            return View(prod);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Activat(int id, Product requestProduct)
        {
            requestProduct.Categ = GetAllCategories();

            try
            {
                    Product product = db.Products.Find(id);
                    product.Activat = requestProduct.Activat;
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost modificat";
                        
                    return RedirectToAction("AddRequest");
            }
            catch (Exception e)
            {
                return View(requestProduct);
            }
        }
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New()
        {
            Product prod = new Product();
            prod.Categ = GetAllCategories();
            prod.UserId = User.Identity.GetUserId();
            prod.Rating = -1;
            if (User.IsInRole("Admin"))
                prod.Activat = true;
            else
                prod.Activat = false;
            return View(prod);
        }
        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New(Product prod,HttpPostedFileBase Image)
        {
            prod.Categ = GetAllCategories();
            prod.UserId = User.Identity.GetUserId();
            prod.Rating = -1;
            if (User.IsInRole("Admin"))
                prod.Activat = true;
            else
                prod.Activat = false;

            try
            {

                if (ModelState.IsValid && Image != null)
                {
                    prod.Picture = new byte[Image.ContentLength];
                    Image.InputStream.Read(prod.Picture, 0, Image.ContentLength);
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

            ViewBag.afisareButoane = false;
            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();

            return View(product);
        }
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Product prod = db.Products.Find(id);
            prod.Categ = GetAllCategories();
            
            if (prod.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                
                return View(prod);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine!";
                return RedirectToAction("Index");
            }

        }
        
        [HttpPut]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id, Product requestProduct)
        {
            requestProduct.Categ = GetAllCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    Product product = db.Products.Find(id);
                    if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
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
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine!";
                        return RedirectToAction("Index");
                    }
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
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Product prod = db.Products.Find(id);
            if (prod.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Products.Remove(prod);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs care nu va apartine!";
                return RedirectToAction("Index");
            }
        }
    }
}