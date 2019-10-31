namespace ProjectManagement_cum_feedback_systemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.project_user");
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
                .PrimaryKey(t => t.post_Id);
            
            CreateTable(
                "dbo.post_comment",
                c => new
                    {
                        comment_Id = c.Int(nullable: false, identity: true),
                        user_Id = c.String(nullable: false),
                        project_Id = c.Int(nullable: false),
                        comment_desc = c.String(nullable: false),
                        user_post_post_Id = c.Int(),
                    })
                .PrimaryKey(t => t.comment_Id)
                .ForeignKey("dbo.user_post", t => t.user_post_post_Id)
                .Index(t => t.user_post_post_Id);
            
            AlterColumn("dbo.project_user", "user_Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.project_user", new[] { "project_Id", "user_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.post_comment", "user_post_post_Id", "dbo.user_post");
            DropIndex("dbo.post_comment", new[] { "user_post_post_Id" });
            DropPrimaryKey("dbo.project_user");
            AlterColumn("dbo.project_user", "user_Id", c => c.Int(nullable: false));
            DropTable("dbo.post_comment");
            DropTable("dbo.user_post");
            DropTable("dbo.project_issue_assign");
            AddPrimaryKey("dbo.project_user", new[] { "project_Id", "user_Id" });
        }
    }
}
