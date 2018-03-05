namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsedReagents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UsedReagents",
                c => new
                    {
                        UsedReagentId = c.Long(nullable: false, identity: true),
                        LabId = c.Long(nullable: false),
                        ReagentId = c.Long(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UsedDate = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.UsedReagentId)
                .ForeignKey("dbo.Labs", t => t.LabId, cascadeDelete: true)
                .ForeignKey("dbo.Reagents", t => t.ReagentId, cascadeDelete: true)
                .Index(t => t.LabId)
                .Index(t => t.ReagentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsedReagents", "ReagentId", "dbo.Reagents");
            DropForeignKey("dbo.UsedReagents", "LabId", "dbo.Labs");
            DropIndex("dbo.UsedReagents", new[] { "ReagentId" });
            DropIndex("dbo.UsedReagents", new[] { "LabId" });
            DropTable("dbo.UsedReagents");
        }
    }
}
