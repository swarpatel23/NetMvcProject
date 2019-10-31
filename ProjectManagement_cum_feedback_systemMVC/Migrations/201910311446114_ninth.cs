namespace ProjectManagement_cum_feedback_systemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ninth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.projects", "project_title", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.projects", "project_title");
        }
    }
}
