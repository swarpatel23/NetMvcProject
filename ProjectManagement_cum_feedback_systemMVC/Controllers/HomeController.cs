using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ProjectManagement_cum_feedback_systemMVC.Models;

namespace ProjectManagement_cum_feedback_systemMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Index","_IndexLayout");
        }

        public ActionResult About()
        {
            
            Model1 m = new Model1();

            project p =new project();
            p.user_Id = User.Identity.GetUserId();
            p.project_title = "moneygame";
            m.projects.Add(p);
            m.SaveChanges();
            
            ViewBag.Message = "Your application description page.";
            
            user_post up = new user_post();
            up.user_Id = User.Identity.GetUserId();
            up.project_Id = 1;
            up.post_title = "add review pop up";
            up.post_desc = "display review pop up after 5min of installation";
            up.vote = 0;
            m.user_posts.Add(up);
            m.SaveChanges();

            project_issue pi = new project_issue();
            pi.project_Id = 1;
            pi.assign_status = 0;
            pi.issue_status = issue_stat.todo;
            pi.issue_title = "add review popup";
            pi.issue_desc = "after 10 min of gameplay review popup is displayed";

            m.project_issue.Add(pi);
            m.SaveChanges();

            /*project_user pu = new project_user();
            pu.project_Id = 1;
            pu.user_Id = User.Identity.GetUserId();
            pu.role = Roles.teammember;
            m.project_users.Add(pu);
            m.SaveChanges();*/

            /*project_issue_assign pia =new project_issue_assign();
            pia.user_Id = User.Identity.GetUserId();
            pia.project_Id = 1;
            pia.issue_Id = 1;
            pia.startdate=new DateTime(2011,2,23);
            pia.enddate=new DateTime(2012,2,23);
            m.project_issue_assigns.Add(pia);
            m.SaveChanges();*/

            post_comment pc = new post_comment();
            pc.post_Id = 1;
            pc.project_Id = 1;
            pc.user_Id = User.Identity.GetUserId();
            pc.comment_desc = "it would be better if we display popup after 30min gameplay";
            m.post_comments.Add(pc);
            m.SaveChanges();


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}