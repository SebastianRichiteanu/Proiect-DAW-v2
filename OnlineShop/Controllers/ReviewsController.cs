using Microsoft.AspNet.Identity;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult New(Review rev)
        {
            try
            {
                rev.UserId = User.Identity.GetUserId();
                rev.UserName = db.Users.Find(rev.UserId).UserName;

                db.Reviews.Add(rev);
                db.SaveChanges();
                CalcMedie(rev.ProductId);
                return Redirect("/Products/Show/" + rev.ProductId);
            }

            catch (Exception e)
            {
                return Redirect("/Products/Show/" + rev.ProductId);
            }

        }
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Review rev = db.Reviews.Find(id);

            if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(rev);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Products");
            }
        }

        [HttpPut]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id, Review requestRev)
        {
            try
            {
                Review rev = db.Reviews.Find(id);
                if(rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    if (TryUpdateModel(rev))
                    {
                        rev.ReviewComment = requestRev.ReviewComment;
                        rev.ReviewRating = requestRev.ReviewRating;
                        db.SaveChanges();
                        CalcMedie(rev.ProductId);
                    }
                return Redirect("/Products/Show/" + rev.ProductId);
            }
            catch (Exception e)
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Products");
            }

        }
        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Review rev = db.Reviews.Find(id);
            if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Reviews.Remove(rev);
                db.SaveChanges();
                CalcMedie(rev.ProductId);
                return Redirect("/Products/Show/" + rev.ProductId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Products");
            }
        }
        public void CalcMedie (int id)
        {
            Product prod = db.Products.Find(id);
            float medie = 0;
            int nr = 0;
            foreach (var review in prod.Reviews)
            {
                ++nr;
                medie += review.ReviewRating;
            }
            if (nr == 0)
                prod.Rating = -1;
            else 
                prod.Rating = (float)Math.Round(medie / nr, 2);
            db.SaveChanges();
        }
    }
}