namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration6 : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.LineSaleTrackingHistoryID)
                .ForeignKey("dbo.LineSaleTrackings", t => t.LineSaleTrackingID, cascadeDelete: true)
                .Index(t => t.LineSaleTrackingID);
            
            CreateTable(
                "dbo.LineSaleTrackings",
                c => new
                    {
                        LineSaleTrackingID = c.Long(nullable: false, identity: true),
                        SupplierName = c.String(maxLength: 200),
                        ItemsDeliveredDate = c.DateTime(nullable: false),
                        AmountPaid = c.Double(nullable: false),
                        Comments = c.String(maxLength: 2000),
                        PaymentDate = c.DateTime(),
                        IsTrackingRequired = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LineSaleTrackingID);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.SupplierID);
            
            AddColumn("dbo.BalanceTransactions", "IncomingAmount", c => c.Double(nullable: false));
            AddColumn("dbo.BalanceTransactions", "OutgoingAmount", c => c.Double(nullable: false));
            AddColumn("dbo.Customers", "Email", c => c.String(maxLength: 100));
            AddColumn("dbo.Customers", "Keyword", c => c.String(maxLength: 100));
            AddColumn("dbo.Customers", "MaxLimitAllowed", c => c.Double(nullable: false));
            AddColumn("dbo.Customers", "MaxWaitingDays", c => c.Int());
            AddColumn("dbo.CustomerContacts", "ContactTypeID", c => c.Int(nullable: false));
            AddColumn("dbo.CustomerContacts", "IsPrimaryContact", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductTransactions", "SupplierID", c => c.Long());
            AddColumn("dbo.ProductTransactions", "Supplier_SupplierID", c => c.Int());
            AddColumn("dbo.SubProducts", "MinAlertQuantity", c => c.Long());
            AlterColumn("dbo.BalanceTransactions", "TransactionDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ProductTransactions", "WareHouseID", c => c.Long());
            AlterColumn("dbo.SubProducts", "MarkedPrice", c => c.Double(nullable: false));
            CreateIndex("dbo.CustomerContacts", "ContactTypeID");
            CreateIndex("dbo.ProductTransactions", "Supplier_SupplierID");
            AddForeignKey("dbo.CustomerContacts", "ContactTypeID", "dbo.ContactTypes", "ContactTypeID", cascadeDelete: true);
            AddForeignKey("dbo.ProductTransactions", "Supplier_SupplierID", "dbo.Suppliers", "SupplierID");
            DropColumn("dbo.BalanceTransactions", "TransactionAmount");
            DropColumn("dbo.BalanceTransactions", "IsDebt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BalanceTransactions", "IsDebt", c => c.Boolean(nullable: false));
            AddColumn("dbo.BalanceTransactions", "TransactionAmount", c => c.Double(nullable: false));
            DropForeignKey("dbo.ProductTransactions", "Supplier_SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.LineSaleTrackingHistories", "LineSaleTrackingID", "dbo.LineSaleTrackings");
            DropForeignKey("dbo.CustomerContacts", "ContactTypeID", "dbo.ContactTypes");
            DropIndex("dbo.ProductTransactions", new[] { "Supplier_SupplierID" });
            DropIndex("dbo.LineSaleTrackingHistories", new[] { "LineSaleTrackingID" });
            DropIndex("dbo.CustomerContacts", new[] { "ContactTypeID" });
            AlterColumn("dbo.SubProducts", "MarkedPrice", c => c.Long(nullable: false));
            AlterColumn("dbo.ProductTransactions", "WareHouseID", c => c.Long(nullable: false));
            AlterColumn("dbo.BalanceTransactions", "TransactionDate", c => c.DateTime());
            DropColumn("dbo.SubProducts", "MinAlertQuantity");
            DropColumn("dbo.ProductTransactions", "Supplier_SupplierID");
            DropColumn("dbo.ProductTransactions", "SupplierID");
            DropColumn("dbo.CustomerContacts", "IsPrimaryContact");
            DropColumn("dbo.CustomerContacts", "ContactTypeID");
            DropColumn("dbo.Customers", "MaxWaitingDays");
            DropColumn("dbo.Customers", "MaxLimitAllowed");
            DropColumn("dbo.Customers", "Keyword");
            DropColumn("dbo.Customers", "Email");
            DropColumn("dbo.BalanceTransactions", "OutgoingAmount");
            DropColumn("dbo.BalanceTransactions", "IncomingAmount");
            DropTable("dbo.Suppliers");
            DropTable("dbo.LineSaleTrackings");
            DropTable("dbo.LineSaleTrackingHistories");
        }
    }
}
