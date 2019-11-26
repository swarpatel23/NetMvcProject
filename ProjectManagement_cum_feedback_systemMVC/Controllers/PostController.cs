using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using ProjectManagement_cum_feedback_systemMVC.Models;
namespace ProjectManagement_cum_feedback_systemMVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private Model1 db = new Model1();
        // GET: Post
        
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(FormCollection form)
        {
            user_post up = new user_post();
            up.post_title = form["post_title"];
            up.post_desc = form["post_desc"];
            up.project_Id = Int32.Parse(form["project_Id"]);
            up.user_Id = User.Identity.GetUserId();
            up.vote = 0;

            try
            {
                db.user_posts.Add(up);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
            }
            return RedirectToAction("question"+"/"+form["project_Id"], "Project", new { area = "" });
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(FormCollection form)
        {
            string post_title = form["post_title_edit"];
            string post_desc = form["post_desc_edit"];
            int post_id = Int32.Parse(form["post_id_edit"]);                        
            try
            {                
                var up = db.user_posts.Where(post => post.post_Id == post_id).FirstOrDefault();                
                if(up!=null)
                {
                    up.post_title = post_title;
                    up.post_desc = post_desc;
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
            }
            return RedirectToAction("question" + "/" + form["project_Id"], "Project", new { area = "" });
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Delete(FormCollection form)
        {            
            int post_id = Int32.Parse(form["post_id_delete"]);
            try
            {
                var up = db.user_posts.Find(post_id);               
               if (up != null)
                {
                    db.user_posts.Remove(up);
                    db.SaveChanges();
                }                                
            }
            catch (Exception ex)
            {

                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
            }
            return RedirectToAction("question" + "/" + form["project_Id"], "Project", new { area = "" });
        }

        public ActionResult Details(int id)
        {
            var up = db.user_posts.Where(p=>p.post_Id == id).FirstOrDefault();
            if(up == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound); 
            }
            Session["post_Id"] = up.post_Id;
            ViewBag.post_model = up;
            ViewBag.comment_model = db.post_comments.Where(c=>c.post_Id == up.post_Id);
            return View();
        }
        [HttpPost]
        public string like(int id)
        {
            string user_id = User.Identity.GetUserId();
            post_user_vote vote = db.Post_User_Votes.Where(v => v.user_id.Equals(user_id) && v.post_id == id).FirstOrDefault();            
            if (vote == null)
            {
                int temp=0;
                try
                {
                    user_post up = db.user_posts.Find(id);
                    up.vote += 1;
                    temp = up.vote;
                    db.Post_User_Votes.Add(new post_user_vote() { user_id = user_id, post_id = id });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                    return ex.ToString();
                }
                return temp.ToString();
            }
            else
            {
                return "already voted";
            }
            
        }
        
        public JsonResult getallissue(int id)
        {

            var results = db.project_issue.Where(p => p.project_Id == id).Select(e => new
            {
                issue_Id = e.issue_Id,
                issue_title = e.issue_title
            }).ToList();

            return new JsonResult() { Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult linkissuepost(FormCollection form)
        {
            try
            {
                int post_id = int.Parse(form["post_Id"]);
                int issue_id = int.Parse(form["issue_Id"]);
                user_post_issue upi = new user_post_issue();
                upi.post_Id = post_id;
                upi.issue_Id = issue_id;
                db.User_Post_Issues.Add(upi);
                db.SaveChanges();
                return RedirectToAction("Details" + "/" + form["post_Id"], "Post", new { area = "" });
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }            
        }
        [HttpGet]
        public string getstatus(int? id)
        {            
            if(id == null)
            {
                return "error";
            }
            try
            {
                var check = db.User_Post_Issues.Where(upi => upi.post_Id == id).FirstOrDefault();
                if(check != null)
                {
                    var post = db.project_issue.Where(i => i.issue_Id == check.issue_Id).FirstOrDefault();
                    if(post!=null)
                    {
                        if(post.issue_status == issue_stat.todo)
                        {
                            return "todo";
                        }
                        else if (post.issue_status == issue_stat.done)
                        {
                            return "done";
                        }
                        else
                        {
                            return "progress";
                        }
                    }
                    else
                    {
                        return "error";
                    }
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception ex)
            {
                return "error";                
            }            
        }
    }
        
}