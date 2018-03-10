namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration20 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataTemplates",
                c => new
                    {
                        DataTemplateID = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 200),
                        File = c.Binary(),
                        FileType = c.String(maxLength: 200),
                        LastDownloadedBy = c.String(maxLength: 200),
                        LastDownloadedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.DataTemplateID);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        LogID = c.Long(nullable: false, identity: true),
                        Message = c.String(maxLength: 1000),
                        InnerException = c.String(maxLength: 2000),
                        StackTrace = c.String(maxLength: 4000),
                        LoggedUser = c.String(maxLength: 100),
                        LoggedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogID);
            
            AddColumn("dbo.Products", "CreatedBy", c => c.String(maxLength: 100));
            AddColumn("dbo.Products", "CreatedDateTime", c => c.DateTime());
            AddColumn("dbo.SubProducts", "CreatedBy", c => c.String(maxLength: 100));
            AddColumn("dbo.SubProducts", "CreatedDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubProducts", "CreatedDateTime");
            DropColumn("dbo.SubProducts", "CreatedBy");
            DropColumn("dbo.Products", "CreatedDateTime");
            DropColumn("dbo.Products", "CreatedBy");
            DropTable("dbo.Logs");
            DropTable("dbo.DataTemplates");
        }
    }
}
