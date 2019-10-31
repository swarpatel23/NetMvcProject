namespace ProjectManagement_cum_feedback_systemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fifth : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.post_comment", "user_post_post_Id", "dbo.user_post");
            DropIndex("dbo.post_comment", new[] { "user_post_post_Id" });
            RenameColumn(table: "dbo.post_comment", name: "user_post_post_Id", newName: "post_Id");
            AlterColumn("dbo.post_comment", "post_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.post_comment", "project_Id");
            CreateIndex("dbo.post_comment", "post_Id");
            CreateIndex("dbo.user_post", "project_Id");
            AddForeignKey("dbo.user_post", "project_Id", "dbo.projects", "Project_Id", cascadeDelete: true);
            AddForeignKey("dbo.post_comment", "project_Id", "dbo.projects", "Project_Id", cascadeDelete: false);
            AddForeignKey("dbo.post_comment", "post_Id", "dbo.user_post", "post_Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.post_comment", "post_Id", "dbo.user_post");
            DropForeignKey("dbo.post_comment", "project_Id", "dbo.projects");
            DropForeignKey("dbo.user_post", "project_Id", "dbo.projects");
            DropIndex("dbo.user_post", new[] { "project_Id" });
            DropIndex("dbo.post_comment", new[] { "post_Id" });
            DropIndex("dbo.post_comment", new[] { "project_Id" });
            AlterColumn("dbo.post_comment", "post_Id", c => c.Int());
            RenameColumn(table: "dbo.post_comment", name: "post_Id", newName: "user_post_post_Id");
            CreateIndex("dbo.post_comment", "user_post_post_Id");
            AddForeignKey("dbo.post_comment", "user_post_post_Id", "dbo.user_post", "post_Id");
        }
    }
}
