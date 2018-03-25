namespace LIMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LabSampleCommentIdFields : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.LabSampleComments", name: "LabSample_LabId", newName: "LabId");
            RenameColumn(table: "dbo.LabSampleComments", name: "LabSample_SampleId", newName: "SampleId");
            RenameColumn(table: "dbo.LabSampleComments", name: "User_Id", newName: "UserId");
            RenameIndex(table: "dbo.LabSampleComments", name: "IX_LabSample_LabId_LabSample_SampleId", newName: "IX_LabId_SampleId");
            RenameIndex(table: "dbo.LabSampleComments", name: "IX_User_Id", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.LabSampleComments", name: "IX_UserId", newName: "IX_User_Id");
            RenameIndex(table: "dbo.LabSampleComments", name: "IX_LabId_SampleId", newName: "IX_LabSample_LabId_LabSample_SampleId");
            RenameColumn(table: "dbo.LabSampleComments", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.LabSampleComments", name: "SampleId", newName: "LabSample_SampleId");
            RenameColumn(table: "dbo.LabSampleComments", name: "LabId", newName: "LabSample_LabId");
        }
    }
}
