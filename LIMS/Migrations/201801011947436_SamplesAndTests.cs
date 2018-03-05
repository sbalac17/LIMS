namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SamplesAndTests : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        TestId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TestId);
            
            CreateTable(
                "dbo.LabSamples",
                c => new
                    {
                        LabId = c.Long(nullable: false),
                        SampleId = c.Long(nullable: false),
                        AssignedDate = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => new { t.LabId, t.SampleId })
                .ForeignKey("dbo.Labs", t => t.LabId, cascadeDelete: true)
                .ForeignKey("dbo.Samples", t => t.SampleId, cascadeDelete: true)
                .Index(t => t.LabId)
                .Index(t => t.SampleId);
            
            CreateTable(
                "dbo.Samples",
                c => new
                    {
                        SampleId = c.Long(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        AddedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        TestId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SampleId)
                .ForeignKey("dbo.Tests", t => t.TestId)
                .Index(t => t.TestId);
            
            AddColumn("dbo.Labs", "TestId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Labs", "TestId");
            AddForeignKey("dbo.Labs", "TestId", "dbo.Tests", "TestId", cascadeDelete: true);
            DropColumn("dbo.Labs", "TestCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Labs", "TestCode", c => c.String(nullable: false));
            DropForeignKey("dbo.LabSamples", "SampleId", "dbo.Samples");
            DropForeignKey("dbo.Samples", "TestId", "dbo.Tests");
            DropForeignKey("dbo.LabSamples", "LabId", "dbo.Labs");
            DropForeignKey("dbo.Labs", "TestId", "dbo.Tests");
            DropIndex("dbo.Samples", new[] { "TestId" });
            DropIndex("dbo.LabSamples", new[] { "SampleId" });
            DropIndex("dbo.LabSamples", new[] { "LabId" });
            DropIndex("dbo.Labs", new[] { "TestId" });
            DropColumn("dbo.Labs", "TestId");
            DropTable("dbo.Samples");
            DropTable("dbo.LabSamples");
            DropTable("dbo.Tests");
        }
    }
}
