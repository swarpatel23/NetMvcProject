namespace ProjectManagement_cum_feedback_systemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.post_user_vote",
                c => new
                    {
                        post_id = c.Int(nullable: false),
                        user_id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.post_id, t.user_id });
            
            CreateTable(
                "dbo.tasks",
                c => new
                    {
                        task_id = c.Int(nullable: false, identity: true),
                        Project_Id = c.Int(nullable: false),
                        task_Id_toshow = c.String(nullable: false),
                        task_name = c.String(nullable: false),
                        start_date = c.DateTime(),
                        end_date = c.DateTime(),
                        duration = c.Int(),
                        percentage = c.Int(nullable: false),
                        dependencies = c.String(),
                    })
                .PrimaryKey(t => t.task_id);
            
            CreateTable(
                "dbo.user_post_issue",
                c => new
                    {
                        post_Id = c.Int(nullable: false),
                        issue_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.post_Id, t.issue_Id });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.user_post_issue");
            DropTable("dbo.tasks");
            DropTable("dbo.post_user_vote");
        }
    }
}
