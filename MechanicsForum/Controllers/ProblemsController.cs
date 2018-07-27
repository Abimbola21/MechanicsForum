using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.IO;
using System.Web.Mvc;
using MechanicsForum.Models;
using Microsoft.AspNet.Identity;

namespace MechanicsForum.Controllers
{
    public class ProblemsController : Controller
    {
        private MechanicsForumEntities db = new MechanicsForumEntities();
        enum MediaType { Image, Video, Document };

        //GET: Logged in user
        public string CurrentUser()
        {
           return User.Identity.GetUserName();
           
        }
        // GET: Problems
        public ActionResult Index()
        {
            // return View(db.Problems.ToList());
            return View();
        }

        public ActionResult Search()
        {
            string S = Request["search"];
            if(S == null)
            {
                S = "";
            }
            return View((Object)S);
        }

        //This returns a list of all problems
        public JsonResult GetAllProblems(string id)
        {
            var AllProblems = (from a in db.Problems.AsEnumerable().Distinct()
                         join b in db.Answers on a.Id equals b.Problem_Id into ProblemAnswer
                         from r in ProblemAnswer.DefaultIfEmpty()//.Where(x => x.Problem_Id != 0)
                         orderby a.PostDate descending
                         group new
                         {
                             a.Id,
                             a.UserId,
                             a.Description,
                             a.Summary,
                             a.Status,
                             /*In my Answer table, the Problem_Id field is not null,
                             this query is a result of a left join on the Problems table;
                             this means that all problems will be returned even if they have not been answered. 
                             The problem_Id field will then be null, in the result set.
                             The lambdah expression below returns an array.I'm using .FirstOrDefault to give me the
                             first element in this array or 0 if empty for ease of access in my Javascript
                             */
                             ProblemId = a.Answers.Select(r => r.Problem_Id).FirstOrDefault(),
                             ModifiedDate = a.ModifiedDate.GetValueOrDefault().ToString("MM/dd/yyyy HH:ss"),
                             PostDate = a.PostDate.GetValueOrDefault().ToString("MM/dd/yyyy HH:ss"),
                             a.ClosedBy,
                             DateClosed = a.DateClosed.GetValueOrDefault().ToString("MM/dd/yyyy HH:ss"),
                             latestAnswerBy = a.Answers.Select(r => r.AnsweredBy).FirstOrDefault()
                         }
                         by new { a.Id }
                               into g
                         select new
                         {
                             g,
                             count = g.Count()
                         }).ToList();

            if (id != null)
            {
                 var t = AllProblems.SelectMany(x => x.g).Where(y => y.Summary.IndexOf(id, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                y.Description.IndexOf(id, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
                
                return Json(new { result = t }, JsonRequestBehavior.AllowGet);
            }
            
            if (AllProblems != null)
            { 
                    return Json(new { result = AllProblems }, JsonRequestBehavior.AllowGet);               
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        //end All Problem

           //This returns a key value pair of each problem and its number of answers
        public JsonResult CountAnsweredProblems()
        {
            var count = from a in db.Answers
                        group new
                        {
                            a.Problem_Id
                        }
                        by new { a.Problem_Id }
                         into g
                        select new
                        {
                          g.Key,
                          count=g.Count()
                        };

            return Json(new { result = count }, JsonRequestBehavior.AllowGet);
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
        //[ValidateAntiForgeryToken]
 
        [HttpPost]
        public ActionResult Create([Bind(Include = "Summary,Description,Media Path")] Problem problem)
        { 
            var result = new List<string>();
            var paths = new List<string>();
            ProblemsMedia media = new ProblemsMedia();
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
                    paths.Add(path);
                }
            }
            problem.Status = "posted"; 
            problem.UserId = User.Identity.GetUserName();
            problem.PostDate = DateTime.Now;
            if (ModelState.IsValid)
            {
             db.Problems.Add(problem);
             db.SaveChanges();
             media.ProblemID = problem.Id;  
                foreach(var mPath in paths)
                {
                    media.MediaPath = mPath;
                    db.ProblemsMedias.Add(media);
                    db.SaveChanges();
                }
             }
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
        public ActionResult Edit([Bind(Include = "Id,Description,Status,UserId,MediaPath")] Problem problem)
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
