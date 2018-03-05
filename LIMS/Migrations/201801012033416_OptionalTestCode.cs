namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OptionalTestCode : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Labs", "TestId", "dbo.Tests");
            DropIndex("dbo.Labs", new[] { "TestId" });
            AlterColumn("dbo.Labs", "TestId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Labs", "TestId");
            AddForeignKey("dbo.Labs", "TestId", "dbo.Tests", "TestId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Labs", "TestId", "dbo.Tests");
            DropIndex("dbo.Labs", new[] { "TestId" });
            AlterColumn("dbo.Labs", "TestId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Labs", "TestId");
            AddForeignKey("dbo.Labs", "TestId", "dbo.Tests", "TestId", cascadeDelete: true);
        }
    }
}
