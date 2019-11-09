using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProjectManagement_cum_feedback_systemMVC.Models;

namespace ProjectManagement_cum_feedback_systemMVC.Controllers
{
    public class ProjectController : Controller
    {
        private Model1 m = new Model1();
        private SmtpClient smtp;
        public ProjectController()
        {
            smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("ddumvc@gmail.com", "ddumvc123"); // Enter seders User name and password   
            smtp.EnableSsl = true;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateProject(string title)
        {
            
                project p = new project();
                p.project_title = title;
                p.user_Id = User.Identity.GetUserId();
                m.projects.Add(p);
                m.SaveChanges();
                int id = p.Project_Id;

                project_user pu = new project_user();
                pu.user_Id = User.Identity.GetUserId();
                pu.project_Id = id;
                pu.role = Roles.admin;
                m.project_users.Add(pu);
                m.SaveChanges();
            

            return RedirectToAction("allUserProject");
        }

        [HttpGet]
        public ActionResult allUserProject()
        {
            string s = User.Identity.GetUserId();
            var model = m.project_users.Where(x => x.user_Id == s).ToList();

            ViewBag.userproject = model;
            return View();
        }

        [HttpGet]
        public ActionResult addMember(string id,string role)
        {
            ViewBag.projectid = id;

            int projectid = Int32.Parse(id);
            Session["project_id"] = projectid;

            var p = m.projects.First(x => x.Project_Id == projectid);
            ViewBag.project_title = p.project_title;
            Session["project_title"] = p.project_title;
            Session["project_story_status"] = p.story_status;
            Session["user_project_role"] = role;



            var teammodel = m.project_users.Where(x => x.project_Id == projectid && x.role == Roles.teammember);

            var usermodel = m.project_users.Where(x => x.project_Id == projectid && x.role == Roles.customer);

            ViewBag.teammodel = teammodel;
            ViewBag.usermodel = usermodel;
            
            return View();
        }

        [HttpPost]
        public ActionResult addMember(FormCollection form)
        {
            int projectid = Int32.Parse(form["projectid"]);

            int nteam = Int32.Parse(form["no_of_teammember"]);
            int ncust = Int32.Parse(form["no_of_customer"]);

            var p = m.projects.First(x => x.Project_Id == projectid);
            ViewBag.project_title = p.project_title;

            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            MailMessage mail = new MailMessage();
            for (int i = 0; i <= nteam; i++)
            {
                string temp = "mem" + i;
                string email = form[temp];
                if (email != "" && email!=null)
                {
                    project_user pu = new project_user();
                    var x=au.FindByEmail(email);
                    project_user user_is_exist = m.project_users.Find( projectid,x.Id);
                    if (x != null && user_is_exist==null)
                    {
                        pu.user_Id = x.Id;
                        pu.project_Id = Int32.Parse(form["projectid"]);
                        pu.role = Roles.teammember;
                        m.project_users.Add(pu);
                        m.SaveChanges();

                       
                        mail.To.Add(x.Email);
                        
                        ViewBag.successMessage = "success";

                    }
                }
            }

            if (mail.To.Count != 0)
            {
                mail.From = new MailAddress("ddumvc@gmail.com");
                mail.Subject = "Notification From Canny";
                mail.Body = "You added as team member in project- " + ViewBag.project_title +
                            " .In future admin will assign you work. get ready for work.";
                smtp.Send(mail);
            }

            MailMessage umail = new MailMessage();
            for (int i = 0; i <= ncust; i++)
            {
                string temp = "cust" + i;
                string email = form[temp];
                if (email != "" && email!=null)
                {
                    project_user pu = new project_user();
                    var x = au.FindByEmail(email);
                    project_user user_is_exist = m.project_users.Find(projectid, x.Id);
                    if (x != null && user_is_exist == null)
                    {
                        pu.user_Id = x.Id;
                        pu.project_Id = Int32.Parse(form["projectid"]);
                        pu.role = Roles.customer;
                        m.project_users.Add(pu);
                        m.SaveChanges();
                        umail.To.Add(x.Email);
                        ViewBag.successMessage = "success";

                    }
                }
            }

            if (umail.To.Count != 0)
            {
                umail.From = new MailAddress("ddumvc@gmail.com");
                umail.Subject = "Notification From Canny";
                umail.Body = "You added as Customer in project- " + ViewBag.project_title +
                             " .You are now able to add user stories and post for any features or bug.";
                smtp.Send(umail);

            }

            var teammodel = m.project_users.Where(x => x.project_Id == projectid && x.role==Roles.teammember);

            var usermodel = m.project_users.Where(x => x.project_Id == projectid && x.role == Roles.customer);

            ViewBag.teammodel = teammodel;
            ViewBag.usermodel = usermodel;


            ViewBag.projectid = projectid;
            Session["project_id"] = projectid;
            TempData["project_id"] = projectid;
            return View();
        }


        public ActionResult removeMember(string pid,string uid)
        {
            int projectid = Int32.Parse(pid);
            var x = m.project_users.Find(projectid, uid);
            m.project_users.Remove(x);
            m.SaveChanges();

            return RedirectToAction("addMember", "Project",new {id=pid});
            
        }

        public ActionResult removeProject(string id)
        {
            int projectid = Int32.Parse(id);
            var x = m.projects.Find(projectid);
            m.projects.Remove(x);
            m.SaveChanges();
            return RedirectToAction("allUserProject");
        }

        public ActionResult profile()
        {
            return View();
        }


        public ActionResult editprofile()
        {
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var x = au.FindById(User.Identity.GetUserId());
            ViewBag.user = x;
            return PartialView();
        }

        public ActionResult updateuser(string firstname,string lastname,string username,string Phone)
        {
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var x = au.FindById(User.Identity.GetUserId());

            x.FirstName = firstname;
            x.LastName = lastname;
            x.UserName = username;
            x.PhoneNumber = Phone;
            au.Update(x);
            ApplicationSignInManager SignInM = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
           
            SignInM.SignIn(x,true,true);
            Session["username"] = x.UserName;
            ViewBag.user = x;
            return PartialView("editprofile");
        }


        [HttpGet]
        public ActionResult createStory(string id)
        {
            ViewBag.projectid = id;
            Session["project_id"] = id;
            int pid = Int32.Parse(id);
            project p = m.projects.Find(pid);
            Session["project_story_status"] = p.story_status;
            return View();
        }

        [HttpGet]
        public ActionResult viewStory(string id)
        {
            ViewBag.projectid = id;
            Session["project_id"] = id;
            return View();
        }

        //----------------------------------------------------
        [HttpGet]
        public ActionResult chat(string id,string role)
        {
            ViewBag.projectid = id;

            int projectid = Int32.Parse(id);
            Session["project_id"] = projectid;
            Session["userId"] = User.Identity.GetUserId();
            var p = m.projects.First(x => x.Project_Id == projectid);
            ViewBag.project_title = p.project_title;
            Session["project_title"] = p.project_title;
            Session["project_story_status"] = p.story_status;
            Session["user_project_role"] = role;



            var teammodel = m.project_users.Where(x => x.project_Id == projectid && x.role == Roles.teammember);

            var usermodel = m.project_users.Where(x => x.project_Id == projectid && x.role == Roles.customer);

            ViewBag.teammodel = teammodel;
            ViewBag.usermodel = usermodel;
            return View();
        }

        public ActionResult chatsave(string userId, string project_Id, string username, string messages)
        {
            Console.Write(userId, project_Id, username, messages);
            // ViewBag.Records = "Name : " + Messages.username + " City:  " + Messages.messages + " Addreess: " + Messages.Message_Id;
            return Content("succes");
        }

        //----------------------------------------------------


        [HttpPost, ValidateInput(false)]
        public ActionResult createStory(FormCollection form)
        {
            ViewBag.projectid = form["projectid"];
            int pid = Int32.Parse(form["projectid"]);
            project p = m.projects.Find(pid);
            p.story_desc = form["userstory"];
            MailMessage umail = new MailMessage();
            MailMessage tmail = new MailMessage();


            if (p.story_status == 0)
            {
                umail.Body = User.Identity.GetUserName() + " Created Users Story Board for " + p.project_title+".";
                tmail.Body = User.Identity.GetUserName() + " Created Users Story Board for " + p.project_title;
            }
            else
            {
                umail.Body = User.Identity.GetUserName() + " Updated Users Story Board for " + p.project_title+".";
                tmail.Body = User.Identity.GetUserName() + " Created Users Story Board for " + p.project_title;
            }

            p.story_status = 1;
            ViewBag.story_updated = true;
            Session["project_story_status"] = p.story_status;
            m.SaveChanges();

            umail.From = new MailAddress("ddumvc@gmail.com");
            umail.Subject = "Notification From Canny";
            tmail.From = new MailAddress("ddumvc@gmail.com");
            tmail.Subject = "Notification From Canny";
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var users=m.project_users.Where(x => x.project_Id == pid);
            foreach (project_user t in users)
            {
               
                if (t.role != Roles.customer)
                {
                    tmail.To.Add(au.GetEmail(t.user_Id));
                    tmail.Body +=
                        ".Now It's time to create Software Requirement Specification. Start dicussing with your team member.";
                }
                else
                {
                    umail.To.Add(au.GetEmail(t.user_Id));
                }
            }
            //smtp.Send(umail);
            //smtp.Send(tmail);
            return View();
        }
    }



}