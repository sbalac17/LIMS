namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoggingUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogEntries", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.LogEntries", "UserId");
            AddForeignKey("dbo.LogEntries", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LogEntries", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.LogEntries", new[] { "UserId" });
            DropColumn("dbo.LogEntries", "UserId");
        }
    }
}
