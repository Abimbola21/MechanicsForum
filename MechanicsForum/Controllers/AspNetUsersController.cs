using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MechanicsForum.Models;

namespace MechanicsForum.Controllers
{
    public class AspNetUsersController : Controller
    {
        
        private MechanicsForumEntities db = new MechanicsForumEntities();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: AspNetUsers
        public ActionResult Index()
        {
            return View(db.AspNetUsers.ToList());
        }

        // GET: AspNetUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Create(string UserName, string Email, string Password)
        {
            var user = new ApplicationUser { UserName = UserName, Email = Email };
            //user.AccessFailedCount = 0;
            var result = UserManager.Create(user, Password);
            if (result.Succeeded)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetRoles()
        {
            var roles = db.AspNetRoles.ToList();
            return Json(new { roles }, JsonRequestBehavior.AllowGet) ;
        }


       //public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,UserName")] AspNetUser aspNetUser)
       //{
       //    if (ModelState.IsValid)
       //    {
       //        aspNetUser.AccessFailedCount = 0;
       //        db.AspNetUsers.Add(aspNetUser);
       //        db.SaveChanges();
       //        return RedirectToAction("Index");
       //    }

       //    return View(aspNetUser);
       //}

       //// GET: AspNetUsers/Edit/5
       //public ActionResult Edit(string id)
       //{
       //    if (id == null)
       //    {
       //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
       //    }
       //    AspNetUser aspNetUser = db.AspNetUsers.Find(id);
       //    if (aspNetUser == null)
       //    {
       //        return HttpNotFound();
       //    }
       //    return View(aspNetUser);
       //}

       // POST: AspNetUsers/Edit/5
       // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
       // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
       [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
