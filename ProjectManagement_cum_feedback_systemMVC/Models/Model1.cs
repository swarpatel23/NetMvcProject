namespace ProjectManagement_cum_feedback_systemMVC.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class Model1 : DbContext
    {
        // Your context has been configured to use a 'Model1' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'ProjectManagement_cum_feedback_systemMVC.Models.Model1' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model1' 
        // connection string in the application configuration file.
        public Model1()
            : base("name=Model11")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<post_comment> post_comments { get; set; }
        public virtual DbSet<project> projects { get; set; }
        public virtual DbSet<project_issue> project_issue { get; set; }
        public virtual DbSet<project_user> project_users { get; set; }
        public virtual DbSet<project_issue_assign> project_issue_assigns { get; set; }
        public virtual DbSet<user_post> user_posts { get; set; }
        public virtual DbSet<srs_comment> srs_comments { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}