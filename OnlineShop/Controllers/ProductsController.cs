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
        private int _perPage = 4;

        [Authorize(Roles = "Admin")]
        public ActionResult AddRequest()
        {
            var products = db.Products.Include("Category").Include("User");
            ViewBag.Products = products;
            return View();
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            

            var sortOrder = Request.Params.Get("Sortare") ;
            var search = "";
            var products = from p in db.Products.Include("Category").Include("User").OrderBy(p => p.Title) select p;

            Debug.Write(sortOrder);

            List<SelectListItem> sort = new List<SelectListItem>();
            string def = sortOrder;

            sort.Add(new SelectListItem() { Text = "Normal", Value = "0", Selected = ("0" == def ? true : false) });
            sort.Add(new SelectListItem() { Text = "Pret Crescator", Value = "1", Selected = ("1" == def ? true : false) });
            sort.Add(new SelectListItem() { Text = "Pret Descrescator", Value = "2", Selected = ("2" == def ? true : false) });
            sort.Add(new SelectListItem() { Text = "Rating Crescator", Value = "3", Selected = ("3" == def ? true : false) });
            sort.Add(new SelectListItem() { Text = "Rating Descrescator", Value = "4", Selected = ("4" == def ? true : false) });

            ViewBag.sort = sort;


            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                products = products.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
            }

            switch (sortOrder)
            {
                case "1":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "2":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "3":
                    products = products.OrderBy(p => p.Rating);
                    break;
                case "4":
                    products = products.OrderByDescending(p => p.Rating);
                    break;
                default:
                    break;
            }
        
            var totalItems = products.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedProducts = products.Skip(offset).Take(this._perPage);

            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Products = paginatedProducts;
            ViewBag.SearchString = search;
            ViewBag.Sortare = def;

            
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


            List<SelectListItem> ReviewRating = new List<SelectListItem>();

            ReviewRating.Add(new SelectListItem() { Text = "1", Value = "1"});
            ReviewRating.Add(new SelectListItem() { Text = "2", Value = "2"});
            ReviewRating.Add(new SelectListItem() { Text = "3", Value = "3"});
            ReviewRating.Add(new SelectListItem() { Text = "4", Value = "4"});
            ReviewRating.Add(new SelectListItem() { Text = "5", Value = "5"});

            ViewBag.ReviewRating = ReviewRating;

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