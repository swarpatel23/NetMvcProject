namespace ProjectManagement_cum_feedback_systemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.post_comment",
                c => new
                    {
                        comment_Id = c.Int(nullable: false, identity: true),
                        user_Id = c.String(nullable: false),
                        project_Id = c.Int(nullable: false),
                        post_Id = c.Int(nullable: false),
                        comment_desc = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.comment_Id)
                .ForeignKey("dbo.user_post", t => t.post_Id, cascadeDelete: true)
                .ForeignKey("dbo.projects", t => t.project_Id, cascadeDelete: false)
                .Index(t => t.project_Id)
                .Index(t => t.post_Id);
            
            CreateTable(
                "dbo.user_post",
                c => new
                    {
                        post_Id = c.Int(nullable: false, identity: true),
                        user_Id = c.String(nullable: false),
                        project_Id = c.Int(nullable: false),
                        vote = c.Int(nullable: false),
                        post_title = c.String(nullable: false),
                        post_desc = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.post_Id)
                .ForeignKey("dbo.projects", t => t.project_Id, cascadeDelete: true)
                .Index(t => t.project_Id);
            
            CreateTable(
                "dbo.projects",
                c => new
                    {
                        Project_Id = c.Int(nullable: false, identity: true),
                        user_Id = c.String(nullable: false),
                        project_title = c.String(nullable: false),
                        story_title = c.String(),
                        story_desc = c.String(),
                        story_status = c.Int(nullable: false),
                        srs_acceptance_status = c.String(),
                        srs_desc = c.String(),
                        srs_status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Project_Id);
            
            CreateTable(
                "dbo.project_issue",
                c => new
                    {
                        issue_Id = c.Int(nullable: false, identity: true),
                        project_Id = c.Int(nullable: false),
                        issue_title = c.String(nullable: false),
                        issue_desc = c.String(nullable: false),
                        priority = c.Int(nullable: false),
                        assign_status = c.Int(nullable: false),
                        creationtime = c.DateTime(nullable: false),
                        issue_status = c.Int(nullable: false),
                        issue_type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.issue_Id)
                .ForeignKey("dbo.projects", t => t.project_Id, cascadeDelete: true)
                .Index(t => t.project_Id);
            
            CreateTable(
                "dbo.project_user",
                c => new
                    {
                        project_Id = c.Int(nullable: false),
                        user_Id = c.String(nullable: false, maxLength: 128),
                        role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.project_Id, t.user_Id })
                .ForeignKey("dbo.projects", t => t.project_Id, cascadeDelete: true)
                .Index(t => t.project_Id);
            
            CreateTable(
                "dbo.project_message",
                c => new
                    {
                        Message_Id = c.Int(nullable: false, identity: true),
                        messages = c.String(),
                        project_Id = c.Int(nullable: false),
                        userId = c.String(nullable: false),
                        username = c.String(),
                        msgtime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Message_Id)
                .ForeignKey("dbo.projects", t => t.project_Id, cascadeDelete: true)
                .Index(t => t.project_Id);
            
            CreateTable(
                "dbo.srs_comment",
                c => new
                    {
                        comment_id = c.Int(nullable: false, identity: true),
                        project_id = c.Int(nullable: false),
                        user_id = c.String(nullable: false),
                        cdate = c.DateTime(nullable: false),
                        comment_desc = c.String(),
                    })
                .PrimaryKey(t => t.comment_id)
                .ForeignKey("dbo.projects", t => t.project_id, cascadeDelete: true)
                .Index(t => t.project_id);
            
            CreateTable(
                "dbo.project_issue_assign",
                c => new
                    {
                        issue_Id = c.Int(nullable: false),
                        user_Id = c.String(nullable: false, maxLength: 128),
                        project_Id = c.Int(nullable: false),
                        startdate = c.DateTime(nullable: false),
                        enddate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.issue_Id, t.user_Id });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.srs_comment", "project_id", "dbo.projects");
            DropForeignKey("dbo.post_comment", "project_Id", "dbo.projects");
            DropForeignKey("dbo.user_post", "project_Id", "dbo.projects");
            DropForeignKey("dbo.project_message", "project_Id", "dbo.projects");
            DropForeignKey("dbo.project_user", "project_Id", "dbo.projects");
            DropForeignKey("dbo.project_issue", "project_Id", "dbo.projects");
            DropForeignKey("dbo.post_comment", "post_Id", "dbo.user_post");
            DropIndex("dbo.srs_comment", new[] { "project_id" });
            DropIndex("dbo.project_message", new[] { "project_Id" });
            DropIndex("dbo.project_user", new[] { "project_Id" });
            DropIndex("dbo.project_issue", new[] { "project_Id" });
            DropIndex("dbo.user_post", new[] { "project_Id" });
            DropIndex("dbo.post_comment", new[] { "post_Id" });
            DropIndex("dbo.post_comment", new[] { "project_Id" });
            DropTable("dbo.project_issue_assign");
            DropTable("dbo.srs_comment");
            DropTable("dbo.project_message");
            DropTable("dbo.project_user");
            DropTable("dbo.project_issue");
            DropTable("dbo.projects");
            DropTable("dbo.user_post");
            DropTable("dbo.post_comment");
        }
    }
}
