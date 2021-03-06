﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace Blog.Controllers
{

    [RequireHttps]
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Posts
        public ActionResult Index(int? page, string userSearch)
        {
                ViewBag.userSearch = userSearch;
                int pageSize = 3;
                int pageNumber = (page ?? 1);

                var allPosts = from p in db.Posts select p;

                if (!string.IsNullOrWhiteSpace(userSearch))
                {
                    allPosts = allPosts.Where(p => p.Title.Contains(userSearch)
                         || p.Body.Contains(userSearch)
                         || p.Category.Contains(userSearch)
                         || p.Comments.Any(c => c.Body.Contains(userSearch)));

                    return View(allPosts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));

            }

            return View(db.Posts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }


        [Authorize(Roles ="Admin")]
        public ActionResult Admin()
        {
            return View(db.Posts.OrderByDescending(p => p.Created).ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(string slug)
        {
            if (String.IsNullOrWhiteSpace (slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.FirstOrDefault (p=>p.Slug== slug);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize (Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Category,Body,MediaURL")] Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(post.Title);
                if(String.IsNullOrWhiteSpace (Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(post);
                }
                if(db.Posts.Any(p=>p.Slug==Slug))
                {
                    ModelState.AddModelError("Title", "the title must be unique.");
                    return View(post);
                }
                if(ImageUploadValidator.IsWebFriendlyImage (image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/img/MediaUrl/"), fileName));
                    post.MediaURL = "~/img/MediaUrl/" + fileName;
                }
                post.Slug = Slug;

                post.Created = System.DateTimeOffset.Now;
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);


        }


        // GET: Posts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Category,Body,MediaURL,Slug")] Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/img/MediaUrl/"), fileName));
                    post.MediaURL = "~/img/MediaUrl/" + fileName;
                }
                post.Updated = System.DateTimeOffset.Now;
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Posts", new { slug = @post.Slug });
            }

            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
