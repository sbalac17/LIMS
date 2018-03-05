namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Comments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LabSampleComments",
                c => new
                    {
                        LabSampleCommentId = c.Long(nullable: false, identity: true),
                        Message = c.String(),
                        NewStatus = c.Int(),
                        LabSample_LabId = c.Long(nullable: false),
                        LabSample_SampleId = c.Long(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.LabSampleCommentId)
                .ForeignKey("dbo.LabSamples", t => new { t.LabSample_LabId, t.LabSample_SampleId }, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => new { t.LabSample_LabId, t.LabSample_SampleId })
                .Index(t => t.User_Id);
            
            AddColumn("dbo.LabSamples", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LabSampleComments", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.LabSampleComments", new[] { "LabSample_LabId", "LabSample_SampleId" }, "dbo.LabSamples");
            DropIndex("dbo.LabSampleComments", new[] { "User_Id" });
            DropIndex("dbo.LabSampleComments", new[] { "LabSample_LabId", "LabSample_SampleId" });
            DropColumn("dbo.LabSamples", "Status");
            DropTable("dbo.LabSampleComments");
        }
    }
}
