using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectManagement_cum_feedback_systemMVC.Models;

namespace ProjectManagement_cum_feedback_systemMVC.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        // GET: Task1
        private Model1 db = new Model1();

        public ActionResult Index(int id)
        {
            ViewBag.projectid = id;                        

            return View(db.tasks.Where(t => t.Project_Id == id).ToList());

        }

        public ActionResult Create(int id)
        {
            ViewBag.Project_id = id;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(FormCollection form)
        {
            ViewBag.projectid = form["Project_id"];
            task t = new task();
            t.Project_Id = Int32.Parse(form["Project_id"]);
            t.task_Id_toshow = form["task_Id_toshow"];
            t.task_name = form["task_name"];
            /*t.start_date = DateTime.Parse(form["start_date"]);
            t.end_date = DateTime.Parse(form["end_date"]);
            t.duration = Int32.Parse(form["duration"]);*/
            if (form["start_date"] == "") 
            {
                t.start_date = null;
            }
            else
            {
                t.start_date = DateTime.Parse(form["start_date"]);
            }
            if (form["end_date"] == "")
            {
                t.end_date = null;
            }
            else
            {
                t.end_date = DateTime.Parse(form["end_date"]);
            }
            if (form["duration"].Equals(""))
            {
                t.duration = null;
            }
            else
            {
                t.duration = Int32.Parse(form["duration"]);
            }
            t.percentage = Int32.Parse(form["percentage"]);
            t.dependencies = form["dependencies"];
            try
            {
                db.tasks.Add(t);
                db.SaveChanges();                
            }
            catch (Exception ex)
            {
                
            }
            return RedirectToAction("Index/" + form["Project_id"]);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            task task1 = db.tasks.Find(id);
            ViewBag.Project_id = task1.Project_Id;
            if (task1 == null)
            {
                return HttpNotFound();
            }
            return View(task1);
        }

        // POST: tasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "task_id,Project_id,task_Id_toshow,task_name,start_date,end_date,duration,percentage,dependencies")] task task)
        {


            db.Entry(task).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index/" + task.Project_Id.ToString());

            
        }

        public ActionResult Delete(string id,string pid)
        {
            int t_id = Int32.Parse(id);
            task task1 = db.tasks.Find(t_id);
            if(task1 !=null)
            {
                db.tasks.Remove(task1);
                db.SaveChanges();
            }
            return RedirectToAction("Index/" + pid);

        }

    }
}
