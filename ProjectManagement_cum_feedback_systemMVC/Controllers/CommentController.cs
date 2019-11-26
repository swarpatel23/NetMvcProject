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
    public class CommentController : Controller
    {
        // GET: Comment
        private Model1 db = new Model1();
        // GET: Post

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(FormCollection form)
        {
            post_comment pc = new post_comment();
            pc.user_Id = User.Identity.GetUserId();
            pc.project_Id = Int32.Parse(form["project_Id"]);
            pc.post_Id = Int32.Parse(form["post_Id"]);
            pc.comment_desc = form["comment_desc"];            

            try
            {
                db.post_comments.Add(pc);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
            }
            return RedirectToAction("Details" + "/" + form["post_Id"], "Post", new { area = "" });
        }
    }
}