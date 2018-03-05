namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LabMembers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Labs", "LabManager_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Labs", new[] { "LabManager_Id" });
            CreateTable(
                "dbo.LabMembers",
                c => new
                    {
                        LabId = c.Long(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IsLabManager = c.Boolean(nullable: false, defaultValue: false),
                    })
                .PrimaryKey(t => new { t.LabId, t.UserId })
                .ForeignKey("dbo.Labs", t => t.LabId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.LabId)
                .Index(t => t.UserId);
            
            DropColumn("dbo.Labs", "LabManager_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Labs", "LabManager_Id", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.LabMembers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.LabMembers", "LabId", "dbo.Labs");
            DropIndex("dbo.LabMembers", new[] { "UserId" });
            DropIndex("dbo.LabMembers", new[] { "LabId" });
            DropTable("dbo.LabMembers");
            CreateIndex("dbo.Labs", "LabManager_Id");
            AddForeignKey("dbo.Labs", "LabManager_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
