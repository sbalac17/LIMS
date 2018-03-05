namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLabModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Labs",
                c => new
                    {
                        LabId = c.Long(nullable: false, identity: true),
                        CollegeName = c.String(nullable: false),
                        CourseCode = c.String(nullable: false),
                        WeekNumber = c.Int(nullable: false),
                        TestCode = c.String(nullable: false),
                        Location = c.String(nullable: false),
                        LabManager_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.LabId)
                .ForeignKey("dbo.AspNetUsers", t => t.LabManager_Id, cascadeDelete: true)
                .Index(t => t.LabManager_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Labs", "LabManager_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Labs", new[] { "LabManager_Id" });
            DropTable("dbo.Labs");
        }
    }
}
