using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
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


        public JsonResult getUserEmail(string term)
        {
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            List<string> useremail = au.Users.Where(x => x.Email.StartsWith(term)).Select(y => y.Email).ToList();

            return Json(useremail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getTeamEmail(string term)
        {
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            List<string> useremail = au.Users.Where(x => x.Email.StartsWith(term)).Select(y => y.Email).ToList();

            List<string> teamemail = new List<string>();
            int pid = (int) Session["project_id"];
            foreach (string email in useremail)
            {
                string uid = au.FindByEmail(email).Id;

                var x = m.project_users.Find(pid, uid);
                if (x != null)
                {
                    if (x.role != Roles.customer)
                    {
                        teamemail.Add(email);
                    }
                }

            }

            return Json(teamemail, JsonRequestBehavior.AllowGet);
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
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var x = au.FindById(User.Identity.GetUserId());
            Session["userpic"] = x.UserPhoto;
            return View();
        }


        public ActionResult editprofile()
        {
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var x = au.FindById(User.Identity.GetUserId());
            ViewBag.user = x;
            Session["userpic"] = x.UserPhoto;

            return PartialView();
        }

        public ActionResult updateuser(FormCollection form, HttpPostedFileBase userpic)
        {
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var x = au.FindById(User.Identity.GetUserId());

            x.FirstName = form["firstname"];
            x.LastName = form["lastname"];
            x.UserName = form["username"]; 
            x.PhoneNumber = form["phone"];
            //x.UserPhoto = form["userpic"];
            if (userpic != null)
            {
                string filename = Path.GetFileNameWithoutExtension(userpic.FileName);
                string extension = Path.GetExtension(userpic.FileName);
                filename = filename + DateTime.Now.ToString("yymmssff") + extension;
                x.UserPhoto = filename;
                string filepath = Path.Combine(Server.MapPath("~/Content/userphotos"), filename);
                userpic.SaveAs(filepath);
            }


            /*if (file.ContentLength > 0)
            {
                string filename = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                filename = filename + DateTime.Now.ToString("yymmssff") + extension;
                x.UserPhoto = filename;
                string filepath = Path.Combine(Server.MapPath("~/Content/userphotos"),filename);
                file.SaveAs(filepath);
            }
*/

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
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var z = au.FindById(User.Identity.GetUserId());
            Session["userpic"] = z.UserPhoto;
            var p = m.projects.First(x => x.Project_Id == projectid);
            ViewBag.project_title = p.project_title;
            Session["project_title"] = p.project_title;
            Session["project_story_status"] = p.story_status;
            Session["user_project_role"] = role;

            var previousmsgs = m.project_messages.Where(x => x.project_Id == projectid);
            ViewBag.previousmsgs = previousmsgs;

            var teammodel = m.project_users.Where(x => x.project_Id == projectid && x.role == Roles.teammember);

            var usermodel = m.project_users.Where(x => x.project_Id == projectid && x.role == Roles.customer);

            ViewBag.teammodel = teammodel;
            ViewBag.usermodel = usermodel;
            return View();
        }


        public EmptyResult chatsave(string message)
        {

            //Console.Write(userId, project_Id, username, messages);
            // ViewBag.Records = "Name : " + Messages.username + " City:  " + Messages.messages + " Addreess: " + Messages.Message_Id;
            project_message pm = new project_message();
            pm.project_Id = (int)Session["project_id"];
            pm.userId = User.Identity.GetUserId();
            pm.username = User.Identity.GetUserName();
            pm.messages = message;
            pm.msgtime = DateTime.Now;
            m.project_messages.Add(pm);
            m.SaveChanges();
            return new EmptyResult();
        }

        //----------------------------------------------------
        //----------------------------------------------------
        [HttpGet]
        public ActionResult issues(string id, string role) {
            ViewBag.project_id = id;
            Session["project_id"] = id;
            int projectid = Int32.Parse(id);
            Session["project_id"] = projectid;
            Session["userId"] = User.Identity.GetUserId();
            var p = m.projects.First(x => x.Project_Id == projectid);
            var issuemodal = m.project_issue.Where(x => x.project_Id == projectid);
            ViewBag.issuemodal = issuemodal;
            var teammodel = m.project_users.Where(x => x.project_Id == projectid && x.role == Roles.teammember);
            var usermodel = m.project_users.Where(x => x.project_Id == projectid && x.role == Roles.customer);
            Session["user_project_role"] = role;
            ViewBag.user_project_role = role; 
            ViewBag.rolef = role;
            Session["issuemodal"] = issuemodal;
            return View();
        }

        [HttpPost]
        public ActionResult issues(FormCollection form)
        {
            int projid = Int32.Parse(form["projectid"]);
            int prio = Int32.Parse(form["priority"]);
            int issueassstatus = Int32.Parse(form["issueassstatus"]);
            string issuetitle = form["issuetitle"];
            string issuedesc = form["issuedesc"];
            int issuestatus = Int32.Parse(form["issuestatus"]);
            int issuetype = Int32.Parse(form["issuetype"]);
            project_issue pi = new project_issue();
            pi.project_Id = projid;
            pi.priority = prio;
            pi.assign_status = issueassstatus;
            pi.issue_title = issuetitle;
            pi.issue_desc = issuedesc;
            pi.creationtime = DateTime.Now;
            if (issuestatus==0)
                pi.issue_status = issue_stat.todo;
            else if (issuestatus==1)
                pi.issue_status = issue_stat.inprogress;
            else
                pi.issue_status = issue_stat.done;

            if (issuetype == 0)
                pi.issue_type = issue_type.newfeature;
            else if (issuetype == 1)
                pi.issue_type = issue_type.bug;
            else
                pi.issue_type = issue_type.improvement;
            
            m.project_issue.Add(pi);
            m.SaveChanges();
            TempData["project_id"] = projid;
            Session["project_id"] = projid;
            ViewData["project_id"] = projid;
            Session["user_project_role"] = form["role"];
            ViewData["user_project_role"] = form["role"];
            var issuemodal = m.project_issue.Where(x => x.project_Id == projid);
            ViewBag.issuemodal = issuemodal;
            return RedirectToAction("issues", "Project", new { id = projid,role = form["role"] });
        }

        public EmptyResult changePriority(string issueid,string issuePriority,string pid,string role)
        {
            int projid = Int32.Parse(pid);
            int issid = Int32.Parse(issueid);
            int prionew = Int32.Parse(issuePriority);
            project_issue x = m.project_issue.Find(issid);
            x.priority = prionew;
            m.SaveChanges();
            TempData["project_id"] = projid;
            Session["project_id"] = projid;
            ViewData["project_id"] = projid;
            Session["user_project_role"] = role;
            ViewData["user_project_role"] = role;
            return new EmptyResult();
        }

        public EmptyResult removeIssue(string issueid)
        {
            //int pid = Int32.Parse(projid);
            int iid = Int32.Parse(issueid);
            var x = m.project_issue.Find(iid);
            m.project_issue.Remove(x);
            m.SaveChanges();
            //TempData["project_id"] = projid;
            //Session["project_id"] = projid;
            //ViewData["project_id"] = projid;
            //Session["user_project_role"] = urole;
            //ViewData["user_project_role"] = urole;
            return new EmptyResult();
        }

        
        public ActionResult issuesFilter(string pidf, string rolef,string issstatusf,string isstypef,string priorityf,string datef)
        {
            ViewBag.project_id = pidf;
            Session["project_id"] = pidf;
            int projectid = Int32.Parse(pidf);
            Session["project_id"] = projectid;
            int issstatusf1 = Int32.Parse(issstatusf);
            int isstypef1 = Int32.Parse(isstypef);
            int priorityf1 = Int32.Parse(priorityf);
            int datef1 = Int32.Parse(datef);
           // ViewBag.pidf = pidf;
            ViewBag.rolef = rolef;
            ViewBag.isstatusf = issstatusf;
            ViewBag.isstypef = isstypef;
            ViewBag.priorityf = priorityf;
           IQueryable<project_issue>  issuemodal = m.project_issue.Where(x => x.project_Id == projectid);
            if (issstatusf1 == 0)
            {
                if (isstypef1 == 0)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.newfeature).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.newfeature); }
                    }
                }
                else if (isstypef1 == 1)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.bug).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.bug).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.bug).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.bug).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.bug).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.bug); }
                    }
                }
                else if (isstypef1 == 2)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.improvement).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.improvement).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.improvement).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.improvement).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.improvement).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo && x.issue_type == issue_type.improvement); }
                    }
                }
                else
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo); }
                    }
                }
            }
            else if(issstatusf1 == 1)
            {
                if (isstypef1 == 0)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.newfeature).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.newfeature); }
                    }
                }
                else if (isstypef1 == 1)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.bug).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.bug).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.bug).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.bug).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.bug).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.bug); }
                    }
                }
                else if (isstypef1 == 2)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement); }
                    }
                }
                else
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress); }
                    }
                }
            }
            else if(issstatusf1 == 2)
            {
                if (isstypef1 == 0)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.newfeature).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.newfeature); }
                    }
                }
                else if (isstypef1 == 1)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.bug).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.bug).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.bug).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.bug).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.bug).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.bug); }
                    }
                }
                else if (isstypef1 == 2)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.improvement).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.improvement).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.improvement).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.improvement).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.improvement).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done && x.issue_type == issue_type.improvement); }
                    }
                }
                else
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done); }
                    }
                }
            }
            else
            {
                if (isstypef1 == 0)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  && x.issue_type == issue_type.newfeature).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  && x.issue_type == issue_type.newfeature).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  && x.issue_type == issue_type.newfeature).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.newfeature); }
                    }
                }
                else if (isstypef1 == 1)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.bug).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.bug).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.bug).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.bug).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.bug).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.bug).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_type == issue_type.bug); }
                    }
                }
                else if (isstypef1 == 2)
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid   && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid   && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress && x.issue_type == issue_type.improvement).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid   && x.issue_type == issue_type.improvement).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid   && x.issue_type == issue_type.improvement).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid   && x.issue_type == issue_type.improvement).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid   && x.issue_type == issue_type.improvement).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid   && x.issue_type == issue_type.improvement).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid   && x.issue_type == issue_type.improvement); }
                    }
                }
                else
                {
                    if (priorityf1 == 0)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  ).OrderByDescending(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  ).OrderByDescending(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  ).OrderByDescending(x => x.priority); }
                    }
                    else if (priorityf1 == 1)
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  ).OrderBy(x => x.priority).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  ).OrderBy(x => x.priority).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  ).OrderBy(x => x.priority); }
                    }
                    else
                    {
                        if (datef1 == 0)
                        { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  ).OrderByDescending(x => x.creationtime); }
                        else if (datef1 == 1) { issuemodal = m.project_issue.Where(x => x.project_Id == projectid  ).OrderBy(x => x.creationtime); }
                        else { issuemodal = m.project_issue.Where(x => x.project_Id == projectid); }
                    }
                }
            }
                  

            ViewData["issuemodal"] = issuemodal;
            Session["user_project_role"] = rolef;
            ViewData["issuemodal"] = issuemodal;
            Session["issuemodal"] = issuemodal;
            ViewBag.issuemodal = issuemodal;
            return PartialView("_issueresult");
        }

        public EmptyResult assignIssue(string tmail,DateTime sdate,DateTime edate,string issueid1)
        {
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string uid = au.FindByEmail(tmail).Id;
            project_issue_assign pia = new project_issue_assign();
            pia.project_Id = (int)Session["project_id"];
            pia.issue_Id = Int32.Parse(issueid1);
            pia.user_Id = uid;
            var y = m.project_issue_assigns.Find(pia.issue_Id, uid);
            if (y == null)
            {
                
                pia.startdate = sdate;
                pia.enddate = edate;
                m.project_issue_assigns.Add(pia);
            }
            else
            {
                y.startdate = sdate;
                y.enddate = edate;
            }

            m.SaveChanges();

            var x=m.project_issue.Find(pia.issue_Id);
            x.assign_status = 1;
            m.SaveChanges();

            return new EmptyResult();
        }
        //----------------------------------------------

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
                tmail.Body = User.Identity.GetUserName() + " Updated Users Story Board for " + p.project_title;
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



        [HttpGet]
        public ActionResult createSrs(string id)
        {
            ViewBag.projectid = id;
            Session["project_id"] = id;
            int pid = Int32.Parse(id);
            project p = m.projects.Find(pid);
            Session["project_srs_status"] = p.srs_status;
            if (p.srs_acceptance_status == "accepted")
            {
                Session["srs_acceptance"] = "accept";
            }

            var comments = m.srs_comments.Where(x => x.project_id == pid);
            ViewBag.comments = comments;
            
            return View();
        }

        [HttpGet]
        public ActionResult viewSrs(string id)
        {
            ViewBag.projectid = id;
            Session["project_id"] = id;
            project p = m.projects.Find(Int32.Parse((id)));
            if (p.srs_acceptance_status == "accepted")
            {
                ViewBag.accept = true;
            }
            else
            {
                ViewBag.accept = false;
            }
            return View();
        }

        [HttpPost]
        public ActionResult viewSrs(FormCollection form)
        {
            ViewBag.projectid = form["projectid"];
            Session["project_id"] = form["projectid"];
            srs_comment sc = new srs_comment();
            sc.project_id = Int32.Parse(form["projectid"]);
            if (form["comment_desc"] != "")
            {
               
                sc.user_id = User.Identity.GetUserId();
                sc.cdate = DateTime.Today;
                sc.comment_desc = form["comment_desc"];
                m.srs_comments.Add(sc);
            }

            project p = m.projects.Find(sc.project_id);
            
            if (form["srs_acceptance"] == "Accept")
            {
                p.srs_acceptance_status = "accepted";
                ViewBag.srs_acceptance = "accepted";

            }
            else
            {
                p.srs_acceptance_status = "improvement needed";
                ViewBag.srs_acceptance = "improvement needed";

            }

            if (p.srs_acceptance_status == "accepted")
            {
                ViewBag.accept = true;
            }
            else
            {
                ViewBag.accept = false;
            }

            m.SaveChanges();
            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult createSrs(FormCollection form)
        {
            ViewBag.projectid = form["projectid"];
            int pid = Int32.Parse(form["projectid"]);
            project p = m.projects.Find(pid);
            p.srs_desc = form["srs"];
            if (p.srs_acceptance_status == "accepted")
            {
                Session["srs_acceptance"] = "accept";
            }
            var comments = m.srs_comments.Where(x => x.project_id == pid);
            ViewBag.comments = comments;
            MailMessage umail = new MailMessage();
            MailMessage tmail = new MailMessage();


            if (p.srs_status == 0)
            {
                umail.Body = User.Identity.GetUserName() + " Created Software Requirement Specification for " + p.project_title + ".";
                tmail.Body = User.Identity.GetUserName() + " Created Software Requirement Specification for " + p.project_title;
            }
            else
            {
                umail.Body = User.Identity.GetUserName() + " Updated Software Requirement Specification for" + p.project_title + ".";
                tmail.Body = User.Identity.GetUserName() + " Updated Software Requirement Specification for " + p.project_title;
            }

            p.srs_status = 1;
            ViewBag.srs_updated = true;
            Session["project_srs_status"] = p.srs_status;
            m.SaveChanges();

            umail.From = new MailAddress("ddumvc@gmail.com");
            umail.Subject = "Notification From Canny";
            tmail.From = new MailAddress("ddumvc@gmail.com");
            tmail.Subject = "Notification From Canny";
            ApplicationUserManager au = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var users = m.project_users.Where(x => x.project_Id == pid);
            foreach (project_user t in users)
            {

                if (t.role != Roles.customer)
                {
                    tmail.To.Add(au.GetEmail(t.user_Id));
                    tmail.Body +=
                        ".As now srs is ready you can create issues and start working on it or admin will assign issues.";
                }
                else
                {
                    umail.Body +=
                        ".If you have any conflicts or any part that you don't understand feel free to talk with your admin or teammates'.";
                    umail.To.Add(au.GetEmail(t.user_Id));
                }
            }
            //smtp.Send(umail);
            //smtp.Send(tmail);
            return View();
        }
        [HttpGet]
        public ActionResult roadmap(string id, string role)
        {
            int projectid = Int32.Parse(id);
            ViewBag.projectid = projectid;
            var check = m.tasks.Where(t => t.Project_Id == projectid).FirstOrDefault();

            if (check != null)
            {
                ViewBag.tasks_model = m.tasks.Where(t => t.Project_Id == projectid).ToList();
            }
            else
            {
                ViewBag.tasks_model = null;
            }

            ViewBag.todo_model = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo);
            ViewBag.progress_model = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress);
            ViewBag.done_model = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done);
            return View();
        }
        public ActionResult question(string id)
        {
            int projectid = Int32.Parse(id);
            ViewBag.projectid = projectid;
            ViewBag.post_model = m.user_posts.Where(x => x.project_Id == projectid);
            return View();
        }
        [HttpGet]
        public JsonResult getallpost(int id)
        {
            /*var results = m.user_posts.Where(x => x.project_Id == id).Select(p => new
            {
                post_id = p.post_Id,
                post_title = p.post_title,
                post_desc = p.post_desc,
                post_vote = p.vote,                

            }).ToList();
            */
            var results = (from p in m.user_posts
                           join upi in m.User_Post_Issues on p.post_Id equals upi.post_Id into upis
                           from upi in upis.DefaultIfEmpty()
                           join i in m.project_issue on upi.issue_Id equals i.issue_Id into _is
                           from i in _is.DefaultIfEmpty()
                           where p.project_Id == id
                           select new
                           {
                               id = p.post_Id,
                               title = p.post_title,
                               desc = p.post_desc,
                               vote = p.vote,
                               status = (m.User_Post_Issues.Where(x => x.post_Id == p.post_Id).FirstOrDefault() != null) ? i.issue_status.ToString() : "notreviwed"
                           })
                           .ToList();

            return new JsonResult() { Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult getpostwithparameter(int id, string paramater)
        {
            /*var results = m.user_posts.Where(x => x.project_Id == id).Select(p => new
            {
                post_id = p.post_Id,
                post_title = p.post_title,
                post_desc = p.post_desc,
                post_vote = p.vote,                

            }).ToList();
            */
            var results = (from p in m.user_posts
                           join upi in m.User_Post_Issues on p.post_Id equals upi.post_Id into upis
                           from upi in upis.DefaultIfEmpty()
                           join i in m.project_issue on upi.issue_Id equals i.issue_Id into _is
                           from i in _is.DefaultIfEmpty()
                           where i.issue_status.ToString().Equals(paramater)
                           where p.project_Id == id
                           select new
                           {
                               id = p.post_Id,
                               title = p.post_title,
                               desc = p.post_desc,
                               vote = p.vote,
                               status = (m.User_Post_Issues.Where(x => x.post_Id == p.post_Id).FirstOrDefault() != null) ? i.issue_status.ToString() : "notreviwed"
                           })
                           .ToList();

            return new JsonResult() { Data = results, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpGet]
        public ActionResult board(string id, string role)
        {
            int projectid = Int32.Parse(id);
            ViewBag.projectid = projectid;       
            ViewBag.todo_model = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.todo);
            ViewBag.progress_model = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.inprogress);
            ViewBag.done_model = m.project_issue.Where(x => x.project_Id == projectid && x.issue_status == issue_stat.done);
            return View();
        }
    }



}