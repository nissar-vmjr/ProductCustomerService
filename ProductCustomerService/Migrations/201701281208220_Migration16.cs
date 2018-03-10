namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration16 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LineSaleSuppliers",
                c => new
                    {
                        LineSaleSupplierID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.LineSaleSupplierID);
            
            AddColumn("dbo.Suppliers", "LastUpdatedDatetime", c => c.DateTime());
            AddColumn("dbo.Suppliers", "LastUpdatedBy", c => c.String(maxLength: 100));
            AddColumn("dbo.SubProducts", "LastUpdatedDatetime", c => c.DateTime());
            AddColumn("dbo.SubProducts", "LastUpdatedBy", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubProducts", "LastUpdatedBy");
            DropColumn("dbo.SubProducts", "LastUpdatedDatetime");
            DropColumn("dbo.Suppliers", "LastUpdatedBy");
            DropColumn("dbo.Suppliers", "LastUpdatedDatetime");
            DropTable("dbo.LineSaleSuppliers");
        }
    }
}
