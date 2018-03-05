namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reagents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reagents",
                c => new
                    {
                        ReagentId = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Quantity = c.Int(nullable: false),
                        AddedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        ExpiryDate = c.DateTimeOffset(nullable: false, precision: 7),
                        ManufacturerCode = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ReagentId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reagents");
        }
    }
}
