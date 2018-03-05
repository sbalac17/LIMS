namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LabOpenDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LabMembers", "LastOpened", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LabMembers", "LastOpened");
        }
    }
}
