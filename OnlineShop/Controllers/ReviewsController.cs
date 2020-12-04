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
        [Authorize(Roles = "User,Editor,Admin")]
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
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Review rev = db.Reviews.Find(id);
            return View(rev);
        }

        [HttpPut]
        [Authorize(Roles = "User,Editor,Admin")]
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
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Review rev = db.Reviews.Find(id);
            db.Reviews.Remove(rev);
            db.SaveChanges();
            return Redirect("/Products/Show/" + rev.ProductId);
        }

    }
}