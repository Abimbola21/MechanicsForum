using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MechanicsForum.Models;
using Microsoft.AspNet.Identity;

namespace MechanicsForum.Controllers
{
    public class AnswersController : Controller
    {
        private MechanicsForumEntities db = new MechanicsForumEntities();
        enum MediaType { Image, Video, Document };

        // GET: Answer
        public ActionResult Index()
        {
            return View(db.Answers.ToList());
        }
        //GET: Logged in user
        public string CurrentUser()
        {
            return User.Identity.GetUserName();

        }

        //GET: Answer/Details/5
        public ActionResult Details(int? id)
        {
           //this calls the Details View page and once this page is loaded,
           // the below method, GetProblemAnswers is called to return all Answers 
           //found to the problem whose id is the same as the Id passed in the argument.
            return View();
        }

        public JsonResult GetProblemAnswers(int? id)
        {
            if (id == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Bad Request" }, JsonRequestBehavior.AllowGet);
            }
            //fromAnswers select record from the answers table whose problem id
            //matches the Id parameter
            var fromAnswers = (from a in db.Answers
                               where a.Problem_Id == id
                               select new
                               {
                                   a.AnswerDesc,
                                   a.AnsweredBy,
                                   AnswerDate = (a.AnswerDate !=null)?((DateTime) a.AnswerDate) : a.AnswerDate,
                                   a.MediaPath,
                                   a.Problem_Id
                               });
            //fromAnswers select record from the Problem table whose problem id
            //matches the Id parameter
            var fromProblems = (from b in db.Problems
                                where b.Id == id
                                select new
                                {
                                    b.Id,
                                    b.Summary,
                                    b.Description,
                                    b.UserId,
                                    PostDate = (b.PostDate != null) ? ((DateTime)b.PostDate) : b.PostDate
                                 });

            //both queries from above are joined in one query
            var join = (from p in fromProblems
                        join q in fromAnswers on p.Id equals q.Problem_Id into ProblemAnswer
                        from r in ProblemAnswer.DefaultIfEmpty()
                        select new
                        {
                            r.AnswerDesc,
                            r.AnsweredBy,
                            r.AnswerDate,
                            r.MediaPath,
                            p.Summary,
                            p.Description,
                            p.UserId,
                            p.PostDate

                        });
            if (join != null)
            {
                return Json(new { result = join }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SubmitAnswer([Bind(Include = "Problem_Id, AnswerDesc,MediaPath")] Answer answer)
        {
            var result = new List<string>();
            var path = "";
            AnswersMedia media = new AnswersMedia();
            var paths = new List<string>();

            foreach (string file in Request.Files)
            {
                //Checking file is available to save.  
                if (Request.Files[file].FileName != "")
                {
                    var filename = Path.GetFileName(Request.Files[file].FileName);
                    string[] extension = filename.Split('.');
                    MediaType mediatype = new MediaType();

                    switch (extension[extension.Length - 1])
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
                        case "mpeg": mediatype = MediaType.Video; break;
                        default: break;
                    }

                    if (mediatype == MediaType.Image)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/answers/images/"));
                        path = Path.Combine(Server.MapPath("~/Uploads/answers/images/"), filename);
                    }
                    else if (mediatype == MediaType.Document)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/answers/documents/"));
                        path = Path.Combine(Server.MapPath("~/Uploads/answers/documents/"), filename);
                    }
                    else if (mediatype == MediaType.Video)
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/answers/videos/"));
                        path = Path.Combine(Server.MapPath("~/Uploads/answers/videos/"), filename);
                    }
                    else
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/answers/media/"));
                        path = Path.Combine(Server.MapPath("~/Uploads/answers/media/"), filename);
                    }
                    Request.Files[file].SaveAs(path);
                    result.Add(filename);
                    paths.Add(path);
                }
            }
            answer.AnsweredBy = User.Identity.GetUserName();
            answer.AnswerDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                media.AnswerID = answer.Id;
                foreach (var mPath in paths)
                {
                    media.MediaPath = mPath;
                    db.AnswersMedias.Add(media);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Details/"+answer.Problem_Id);
        }
        [Authorize(Roles = "Moderator")]
        public JsonResult CloseProblem(int? id)
        {
            
            if (id == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Bad Request" }, JsonRequestBehavior.AllowGet);
            }
            if (Request.IsAuthenticated)
            {
               // if (User.IsInRole("Moderator"))
               // {
                    // Query the database for the row to be updated.
                    var query =
                        from prob in db.Problems
                        where prob.Id == id
                        select prob;

                    // Execute the query, and change the column values
                    // you want to change.
                    if (query != null)
                    {
                        foreach ( var p in query)
                        {
                            p.ClosedBy = CurrentUser();
                            p.DateClosed = DateTime.Now;
                            p.Status = "closed";
                        }
                      
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(new { message = "Bad Request" }, JsonRequestBehavior.AllowGet);
                    }

                    // Submit the changes to the database.
                    try
                    {
                        db.SaveChanges();
                        return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        // Provide for exceptions.
                        return Json(new { result = e }, JsonRequestBehavior.AllowGet);
                    }
               // }
                
            }
            return Json(new { result = false}, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "Contributor,Mechanic")]
        public JsonResult CountVote(int? id)
        {
            if (id == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = "Bad Request" }, JsonRequestBehavior.AllowGet);
            }
            if (Request.IsAuthenticated)
            {
                var query = from ans in db.Answers
                            where ans.Id == id
                            select ans;
                // Execute the query, and change the column values
                // you want to change.
                if (query != null)
                {
               //   var q = from votes in db.VotesUserAnswer

                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { message = "Bad Request" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { result = true}, JsonRequestBehavior.AllowGet);
        }


        // GET: Answer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Answer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProblemId,AnsweredBy,Answer,MediaPath,AnswerDate,ModifiedDate")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(answer);
        }

        // GET: Answer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer Answer = db.Answers.Find(id);
            if (Answer == null)
            {
                return HttpNotFound();
            }
            return View(Answer);
        }

        // POST: Answer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProblemId,AnsweredBy,Answer,MediaPath,AnswerDate,ModifiedDate")] Answer Answer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Answer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Answer);
        }

        // GET: Answer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer Answer = db.Answers.Find(id);
            if (Answer == null)
            {
                return HttpNotFound();
            }
            return View(Answer);
        }

        // POST: Answer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Answer Answer = db.Answers.Find(id);
            db.Answers.Remove(Answer);
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
