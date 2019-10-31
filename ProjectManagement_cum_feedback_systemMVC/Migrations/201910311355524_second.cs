namespace ProjectManagement_cum_feedback_systemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.project_issue");
            AlterColumn("dbo.project_issue", "issue_Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.project_issue", "issue_Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.project_issue");
            AlterColumn("dbo.project_issue", "issue_Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.project_issue", new[] { "issue_Id", "project_Id" });
        }
    }
}
