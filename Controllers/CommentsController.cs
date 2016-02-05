using Blog.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    [RequireHttps]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            return View(db.Comments.OrderBy(c=>c.Created).ToList());
        }

        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Admin()
        {
            return View(db.Comments.OrderBy(c =>c.Created).ToList());
        }

        // POST: Comments/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create([Bind(Include = "Id,PostId,Body")] Comment comment)
        {
            var slug = db.Posts.Find(comment.PostId).Slug;
            if (ModelState.IsValid)
            {
                comment.Created = System.DateTimeOffset.Now;
                comment.AuthorId = User.Identity.GetUserId();
                db.Comments.Add(comment);
                db.SaveChanges();

            }

            return RedirectToAction("Details","Posts", new { Slug = slug });
        }

        // GET: Comments/Edit/5
        [Authorize(Roles ="Admin, Moderator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,PostId,AuthorId,Body,Created")] Comment comment)
        {
            var slug = db.Posts.Find(comment.PostId).Slug;

            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Posts", new { Slug = slug });
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Comment comment = db.Comments.Find(id);
            var slug = db.Posts.Find(comment.PostId).Slug;
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Posts", new { Slug = slug });
        }
    }
}
