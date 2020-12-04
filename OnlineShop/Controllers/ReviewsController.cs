using OnlineShop.Models;
using System;
using System.Collections.Generic;
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
        public ActionResult New(Review rev)
        {
            try
            {
                db.Reviews.Add(rev);
                db.SaveChanges();
                return Redirect("/Products/Show/" + rev.ProductId);
            }

            catch (Exception e)
            {
                return Redirect("/Products/Show/" + rev.ProductId);
            }

        }
        public ActionResult Edit(int id)
        {
            Review rev = db.Reviews.Find(id);
            return View(rev);
        }

        [HttpPut]
        public ActionResult Edit(int id, Review requestRev)
        {
            try
            {
                Review rev = db.Reviews.Find(id);
                if (TryUpdateModel(rev))
                {
                    rev.ReviewComment = requestRev.ReviewComment;
                    rev.ReviewRating = requestRev.ReviewRating;
                    db.SaveChanges();
                }
                return Redirect("/Products/Show/" + rev.ProductId);
            }
            catch (Exception e)
            {
                return View();
            }

        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Review rev = db.Reviews.Find(id);
            db.Reviews.Remove(rev);
            db.SaveChanges();
            return Redirect("/Products/Show/" + rev.ProductId);
        }

    }
}