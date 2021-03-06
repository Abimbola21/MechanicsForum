﻿using System;
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
using System.Net.Mail;
using System.Threading.Tasks;

namespace MechanicsForum.Controllers
{
    //[Authorize(Roles = "SuperAdmin,Admin")]
    public class AspNetUsersController : Controller
    {
        public AspNetUsersController()
        {
        }
        private MechanicsForumEntities db = new MechanicsForumEntities();   
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        

        public AspNetUsersController(ApplicationUserManager _userManager, ApplicationRoleManager roleManager)
        {
            UserManager = _userManager;
            RoleManager = _roleManager;
        }

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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        // GET: AspNetUsers
        public ActionResult Index()
        {
            //return View(db.AspNetUsers.ToList());
            return View();

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
        //[Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult> Create()
        {
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //Register New Users
        // POST: AspNetUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Create(string UserName, string Email, string Password, params string[] Roles)
        {
            var user = new ApplicationUser { UserName = UserName, Email = Email};
            //user.AccessFailedCount = 0;
            var result = UserManager.Create(user, Password);
           //Send message to user to confirm their email
            if (result.Succeeded)
            {
               // SignInManager.SignIn(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code =  UserManager.GenerateEmailConfirmationToken(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                var body = "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>";
                var message = new MailMessage();
                message.To.Add(Email);
                message.Subject = "Confirm Account";
                message.Body = string.Format(body, "Mechanics Forum", "mechanicsautomation@gmail.com");
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                  smtp.Send(message);
                }

                //Add User to the selected Roles. This block is only accessible to Administrator/Super 
                //while creating users, they can add role
             if (Roles != null)
                    {
                    result =  UserManager.AddToRoles(user.Id, Roles);

                    if (!result.Succeeded)
                        {
                        return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
                    }
                    }
             ////this block is for  a user other than super/administrator, they are assigned Reader roles at registration
             //   else
             //   {
             //       result = UserManager.AddToRoles(user.Id, "Contributor");

             //       if (!result.Succeeded)
             //       {
             //           return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
             //       }
             //   }
                            
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
            }
        }

        //This returns a list of all users
        public JsonResult GetAllUsers()
        {
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
                {
                    var AllUsers = (from a in db.AspNetUsers
                                    select new
                                    {
                                        a.Id,
                                        a.UserName,
                                        a.Email,
                                        Role = a.AspNetRoles.Select(r => r.Name)
                                    });
                return Json(new { result = AllUsers }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        //end All users details

        //this method returns all the roles in the database
        [HttpPost]
        public JsonResult GetRoles()
        {
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("SuperAdmin"))
                {
                    var names = (from a in db.AspNetRoles                               
                                 select new
                                 {
                                     a.Name,
                                 });
                    return Json(new { result = names }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var names = (from a in db.AspNetRoles
                                 where a.Name != "SuperAdmin"
                                 select new
                                 {
                                     a.Name,
                                 });
                    return Json(new { result = names }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var names = (from a in db.AspNetRoles
                             where a.Name != "SuperAdmin"
                             select new
                             {
                                 a.Name,
                             });
                return Json(new { result = names }, JsonRequestBehavior.AllowGet);
            }
 
        }

        //end Get Roles

        //This method populates the edit user page with the selected user details
        public JsonResult LoadUserDetails(string id)
        {
            if (id == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

            var selectedUser = (from a in db.AspNetUsers
                                where a.Id == id
                                select new
                                {
                                    a.Id,
                                    a.UserName,
                                    a.Email,
                                    Role = a.AspNetRoles.Select(r => r.Name)
                                });
            return Json(selectedUser, JsonRequestBehavior.AllowGet);
        }
        //end Load User details

        public ActionResult Edit(string id)
        {
            return View();
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

        //  [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,UserName,Email,PhoneNumber,Role")] AspNetUser aspNetUser)
        //{

        //    var user = new ApplicationUser { UserName = UserName, Email = Email };
        //    user.AccessFailedCount = 0;
        //    var result = UserManager.Create(user, Password);

        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(aspNetUser).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(aspNetUser);
        //}
        //Edit User Profile
        // POST: AspNetUsers/Edit/
        [HttpPost]
        public JsonResult Edit (string Id, string UserName, string Email, string PhoneNumber,List<string> selectedRole)
        {
            //find the user to be edited
            var user = UserManager.FindById(Id);
            
            if(user == null)
            {
                return Json(new { message = "User Not found" }, JsonRequestBehavior.AllowGet);
            }
            //assign new values to user profile
            user.UserName = UserName;
            user.Email = Email;
            user.PhoneNumber = PhoneNumber;
            //get current user roles
            var userRoles = UserManager.GetRoles(user.Id);

            selectedRole = selectedRole ?? new List<string> { };

            //Add newly selected roles to users with the exception of the already assigned roles
            var result = UserManager.AddToRoles(user.Id, selectedRole.Except(userRoles).ToArray<string>());
            if (!result.Succeeded)
            {
                return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
            }
            //Also remove user roles if it is not included in the selected roles
            result = UserManager.RemoveFromRoles(user.Id, userRoles.Except(selectedRole).ToArray<string>());
            if (!result.Succeeded)
            {
                return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
            }
            //Update the user profile
            result = UserManager.Update(user);
            if (result.Succeeded)
            {
                
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
            }
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
