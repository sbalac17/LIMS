namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LabSamples : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.LabSamples", new[] { "SampleId" });
            AddColumn("dbo.LabSamples", "Notes", c => c.String());
            CreateIndex("dbo.LabSamples", "SampleId", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.LabSamples", new[] { "SampleId" });
            DropColumn("dbo.LabSamples", "Notes");
            CreateIndex("dbo.LabSamples", "SampleId");
        }
    }
}
