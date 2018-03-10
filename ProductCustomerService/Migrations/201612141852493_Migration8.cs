namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LineSaleTrackingHistories", "LineSaleTrackingID", "dbo.LineSaleTrackings");
            DropIndex("dbo.LineSaleTrackingHistories", new[] { "LineSaleTrackingID" });
            CreateTable(
                "dbo.LineSales",
                c => new
                    {
                        LineSaleID = c.Long(nullable: false, identity: true),
                        SupplierID = c.Int(),
                        SupplierName = c.String(maxLength: 200),
                        ItemsDeliveredDate = c.DateTime(nullable: false),
                        TotalAmount = c.Double(nullable: false),
                        BalanceAmount = c.Double(nullable: false),
                        Comments = c.String(maxLength: 2000),
                        IsTrackingRequired = c.Boolean(nullable: false),
                        CreatedBy = c.String(maxLength: 50),
                        CreatedDateTime = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 50),
                        LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.LineSaleID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .Index(t => t.SupplierID);
            
            AddColumn("dbo.LineSaleTrackings", "LineSaleID", c => c.Long(nullable: false));
            AddColumn("dbo.LineSaleTrackings", "BalanceAmount", c => c.Double(nullable: false));
            AddColumn("dbo.LineSaleTrackings", "IsBalanceSettled", c => c.Boolean(nullable: false));
            AddColumn("dbo.LineSaleTrackings", "CreatedBy", c => c.String(maxLength: 50));
            AddColumn("dbo.LineSaleTrackings", "CreatedDateTime", c => c.DateTime());
            AlterColumn("dbo.LineSaleTrackings", "Comments", c => c.String(maxLength: 1000));
            AlterColumn("dbo.LineSaleTrackings", "PaymentDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.LineSaleTrackings", "LineSaleID");
            AddForeignKey("dbo.LineSaleTrackings", "LineSaleID", "dbo.LineSales", "LineSaleID", cascadeDelete: true);
            DropColumn("dbo.LineSaleTrackings", "SupplierName");
            DropColumn("dbo.LineSaleTrackings", "ItemsDeliveredDate");
            DropColumn("dbo.LineSaleTrackings", "IsTrackingRequired");
            DropTable("dbo.LineSaleTrackingHistories");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.LineSaleTrackingHistories",
                c => new
                    {
                        LineSaleTrackingHistoryID = c.Long(nullable: false, identity: true),
                        LineSaleTrackingID = c.Long(nullable: false),
                        PaymentDate = c.DateTime(nullable: false),
                        AmountPaid = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.LineSaleTrackingHistoryID);
            
            AddColumn("dbo.LineSaleTrackings", "IsTrackingRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.LineSaleTrackings", "ItemsDeliveredDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.LineSaleTrackings", "SupplierName", c => c.String(maxLength: 200));
            DropForeignKey("dbo.LineSaleTrackings", "LineSaleID", "dbo.LineSales");
            DropForeignKey("dbo.LineSales", "SupplierID", "dbo.Suppliers");
            DropIndex("dbo.LineSaleTrackings", new[] { "LineSaleID" });
            DropIndex("dbo.LineSales", new[] { "SupplierID" });
            AlterColumn("dbo.LineSaleTrackings", "PaymentDate", c => c.DateTime());
            AlterColumn("dbo.LineSaleTrackings", "Comments", c => c.String(maxLength: 2000));
            DropColumn("dbo.LineSaleTrackings", "CreatedDateTime");
            DropColumn("dbo.LineSaleTrackings", "CreatedBy");
            DropColumn("dbo.LineSaleTrackings", "IsBalanceSettled");
            DropColumn("dbo.LineSaleTrackings", "BalanceAmount");
            DropColumn("dbo.LineSaleTrackings", "LineSaleID");
            DropTable("dbo.LineSales");
            CreateIndex("dbo.LineSaleTrackingHistories", "LineSaleTrackingID");
            AddForeignKey("dbo.LineSaleTrackingHistories", "LineSaleTrackingID", "dbo.LineSaleTrackings", "LineSaleTrackingID", cascadeDelete: true);
        }
    }
}
