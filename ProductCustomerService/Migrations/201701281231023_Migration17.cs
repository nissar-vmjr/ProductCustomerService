namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration17 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SupplierTrackings",
                c => new
                    {
                        SupplierTrackingID = c.Long(nullable: false, identity: true),
                        SupplierID = c.Int(nullable: false),
                        PaidAmount = c.Double(nullable: false),
                        BorrowedAmount = c.Double(nullable: false),
                        comments = c.String(maxLength: 2000),
                        CreatedDatetime = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.SupplierTrackingID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .Index(t => t.SupplierID);
            
            AddColumn("dbo.Suppliers", "BalanceAmount", c => c.Double(nullable: false));
            AddColumn("dbo.Suppliers", "InitialBalance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupplierTrackings", "SupplierID", "dbo.Suppliers");
            DropIndex("dbo.SupplierTrackings", new[] { "SupplierID" });
            DropColumn("dbo.Suppliers", "InitialBalance");
            DropColumn("dbo.Suppliers", "BalanceAmount");
            DropTable("dbo.SupplierTrackings");
        }
    }
}
