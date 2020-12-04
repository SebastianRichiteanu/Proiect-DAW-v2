using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            return View();
        }

        public ActionResult New()
        {
            return View();
        }
        [HttpPost]
        public ActionResult New(Category cat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost adaugata";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(cat);
                }
            }
            catch (Exception e)
            {
                return View(cat);
            }
        }
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }
        [HttpPut]
        public ActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                Category category = db.Categories.Find(id);
                if (TryUpdateModel(category))
                {
                    category.CategoryName = requestCategory.CategoryName;
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost modificata";
                    return RedirectToAction("Index");
                }

                return View(requestCategory);
            }
            catch (Exception e)
            {
                return View(requestCategory);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}