namespace ProjectManagement_cum_feedback_systemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.projects",
                c => new
                    {
                        Project_Id = c.Int(nullable: false, identity: true),
                        user_Id = c.String(nullable: false),
                        story_title = c.String(),
                        story_desc = c.String(),
                        story_status = c.Int(nullable: false),
                        srs_title = c.String(),
                        srs_desc = c.String(),
                        srs_status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Project_Id);
            
            CreateTable(
                "dbo.project_issue",
                c => new
                    {
                        issue_Id = c.Int(nullable: false),
                        project_Id = c.Int(nullable: false),
                        issue_title = c.String(nullable: false),
                        issue_desc = c.String(nullable: false),
                        priority = c.Int(nullable: false),
                        assign_status = c.Int(nullable: false),
                        issue_status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.issue_Id, t.project_Id })
                .ForeignKey("dbo.projects", t => t.project_Id, cascadeDelete: true)
                .Index(t => t.project_Id);
            
            CreateTable(
                "dbo.project_user",
                c => new
                    {
                        project_Id = c.Int(nullable: false),
                        user_Id = c.Int(nullable: false),
                        role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.project_Id, t.user_Id })
                .ForeignKey("dbo.projects", t => t.project_Id, cascadeDelete: true)
                .Index(t => t.project_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.project_user", "project_Id", "dbo.projects");
            DropForeignKey("dbo.project_issue", "project_Id", "dbo.projects");
            DropIndex("dbo.project_user", new[] { "project_Id" });
            DropIndex("dbo.project_issue", new[] { "project_Id" });
            DropTable("dbo.project_user");
            DropTable("dbo.project_issue");
            DropTable("dbo.projects");
        }
    }
}
