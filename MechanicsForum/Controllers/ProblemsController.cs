using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.IO;
using System.Web;
using System.Web.Mvc;
using MechanicsForum.Models;
using Microsoft.AspNet.Identity;

namespace MechanicsForum.Controllers
{
    public class ProblemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        enum MediaType { Image, Video, Document };

        // GET: Problems
        public ActionResult Index()
        {
            return View(db.Problems.ToList());
        }

        // GET: Problems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Problem problem = db.Problems.Find(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            return View(problem);
        }

        // GET: Problems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Problems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
         [HttpPost]
         //[ValidateAntiForgeryToken]
        // public ActionResult Create([Bind(Include = "ProblemId,Description,Status,UserId,MediaPath")] Problem problem, FileStream file)
        // {
        //
        //if (file != null)
        //{
        //    var fileName = Path.GetFileName(file.Name);
        //    var path = _env + "\\uploads\\" + fileName;

        //    using (var stream = new FileStream(path, FileMode.Create))
        //    {
        //         file.CopyToAsync(stream);
        //    }
        //    //update media path
        //    problem.MediaPath = "uploads/" + fileName;

        // var fileName = Path.GetFileName(file.Name);
        //  var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
        // problem.MediaPath = path;
        //  file.SaveAs(path);
        //   if (ModelState.IsValid)
        //        {
        //      db.Problems.Add(problem);
        //      db.SaveChanges();
        //      return RedirectToAction("Index");
        //    }

        //    return View(problem);                 
        //    }

        public ActionResult Create([Bind(Include = "Description,Media Path")] Problem problem)
        { 
            var result = new List<string>();
           // Problem problem = new Problem();
            var path = "";

            foreach (string file in Request.Files)
            {
                //Checking file is available to save.  
                if (Request.Files[file].FileName != "")
                {
                    var filename = Path.GetFileName(Request.Files[file].FileName);
                    string[] extension = filename.Split('.');
                    MediaType mediatype = new MediaType();
                   
                    switch (extension[extension.Length-1])
                    {
                        case "gif": 
                        case "jpg": 
                        case "bmp": 
                        case "jpeg": 
                        case "png": mediatype = MediaType.Image; break;
                        case "pdf": 
                        case "doc": 
                        case "docx": mediatype = MediaType.Document; break;
                        case "mp4":
                        case "mpg":
                        case "wmv":
                        case "mov":
                        case "mpeg":mediatype = MediaType.Video; break;
                        default: break;                                             
                    }
                   
                    if (mediatype == MediaType.Image)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/images/"));
                        path = Path.Combine(Server.MapPath("~/Uploads/images/"), filename);
                    }
                    else if(mediatype == MediaType.Document)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/documents/"));
                         path = Path.Combine(Server.MapPath("~/Uploads/documents/"), filename);
                    }
                    else if(mediatype == MediaType.Video)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/videos/"));
                        path = Path.Combine(Server.MapPath("~/Uploads/videos/"), filename);
                    }
                    else
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/media/"));
                        path = Path.Combine(Server.MapPath("~/Uploads/media/"), filename);
                    }
                    Request.Files[file].SaveAs(path);
                    result.Add(filename);
                }
            }
            problem.MediaPath = path;
           // problem.Description = Description;
            problem.Status = "Pending"; 
            problem.UserId = User.Identity.GetUserName();
            if (ModelState.IsValid)
            {
             db.Problems.Add(problem);
             db.SaveChanges();
             }

            //var result = Json(new { result = "Sucess" });
            //var jsonResult = (from f in result
            //                  select
            //                  new
            //                  {
            //                      file = f,
            //                      Description,
                                  
            //                  });

            //return Json(jsonResult, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Index");
        }

        // GET: Problems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Problem problem = db.Problems.Find(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            return View(problem);
        }

        // POST: Problems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProblemId,Description,Status,UserId,MediaPath")] Problem problem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(problem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(problem);
        }

        // GET: Problems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Problem problem = db.Problems.Find(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            return View(problem);
        }

        // POST: Problems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Problem problem = db.Problems.Find(id);
            db.Problems.Remove(problem);
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
