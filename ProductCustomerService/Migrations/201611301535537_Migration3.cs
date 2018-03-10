namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BalanceTransactions", "TransactionTypeID", "dbo.TransactionTypes");
            DropPrimaryKey("dbo.TransactionTypes");
            CreateTable(
                "dbo.ContactTypes",
                c => new
                    {
                        ContactTypeID = c.Int(nullable: false),
                        ContactTypeName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ContactTypeID);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationID = c.Long(nullable: false, identity: true),
                        NotificationTypeID = c.Int(nullable: false),
                        Message = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        IsPriorityAlert = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationID)
                .ForeignKey("dbo.NotificationTypes", t => t.NotificationTypeID, cascadeDelete: true)
                .Index(t => t.NotificationTypeID);
            
            CreateTable(
                "dbo.NotificationTypes",
                c => new
                    {
                        NotificationTypeID = c.Int(nullable: false),
                        NotificationTypeName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.NotificationTypeID);
            
            CreateTable(
                "dbo.Warehouses",
                c => new
                    {
                        WarehouseID = c.Int(nullable: false, identity: true),
                        WarehouseName = c.String(maxLength: 100),
                        StockCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WarehouseID);
            
            AddColumn("dbo.BalanceTransactions", "TransactionDate", c => c.DateTime());
            AddColumn("dbo.BalanceTransactions", "ChequeDate", c => c.DateTime());
            AddColumn("dbo.BalanceTransactions", "ChequePassingDate", c => c.DateTime());
            AddColumn("dbo.BalanceTransactions", "ChequeNumber", c => c.String(maxLength: 200));
            AddColumn("dbo.BalanceTransactions", "ChequeCustomerName", c => c.String(maxLength: 100));
            AddColumn("dbo.BalanceTransactions", "ChequeIssuerBank", c => c.String(maxLength: 100));
            AddColumn("dbo.BalanceTransactions", "IsChequePassed", c => c.Boolean());
            AddColumn("dbo.BalanceTransactions", "OnlineReferernceID", c => c.String(maxLength: 100));
            AddColumn("dbo.Customers", "CustomerFirstName", c => c.String(maxLength: 100));
            AddColumn("dbo.Customers", "CustomerLastName", c => c.String(maxLength: 100));
            AddColumn("dbo.Customers", "CreatedBy", c => c.String(maxLength: 50));
            AddColumn("dbo.Customers", "CreatedDateTime", c => c.DateTime());
            AddColumn("dbo.Customers", "LastUpdatedBy", c => c.String(maxLength: 50));
            AddColumn("dbo.Customers", "LastUpdatedDateTime", c => c.DateTime());
            AddColumn("dbo.CustomerContacts", "CountryCode", c => c.String(maxLength: 50));
            AddColumn("dbo.CustomerContacts", "CityCode", c => c.String(maxLength: 50));
            AddColumn("dbo.ProductTransactions", "WareHouseID", c => c.Long(nullable: false));
            AddColumn("dbo.ProductTransactions", "SupplierName", c => c.String(maxLength: 100));
            AddColumn("dbo.ProductTransactions", "WareHouse_WarehouseID", c => c.Int());
            AlterColumn("dbo.TransactionTypes", "TransactionTypeID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TransactionTypes", "TransactionTypeID");
            CreateIndex("dbo.ProductTransactions", "WareHouse_WarehouseID");
            AddForeignKey("dbo.ProductTransactions", "WareHouse_WarehouseID", "dbo.Warehouses", "WarehouseID");
            AddForeignKey("dbo.BalanceTransactions", "TransactionTypeID", "dbo.TransactionTypes", "TransactionTypeID", cascadeDelete: true);
            DropColumn("dbo.Customers", "CustomerName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "CustomerName", c => c.String(maxLength: 100));
            DropForeignKey("dbo.BalanceTransactions", "TransactionTypeID", "dbo.TransactionTypes");
            DropForeignKey("dbo.ProductTransactions", "WareHouse_WarehouseID", "dbo.Warehouses");
            DropForeignKey("dbo.Notifications", "NotificationTypeID", "dbo.NotificationTypes");
            DropIndex("dbo.ProductTransactions", new[] { "WareHouse_WarehouseID" });
            DropIndex("dbo.Notifications", new[] { "NotificationTypeID" });
            DropPrimaryKey("dbo.TransactionTypes");
            AlterColumn("dbo.TransactionTypes", "TransactionTypeID", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.ProductTransactions", "WareHouse_WarehouseID");
            DropColumn("dbo.ProductTransactions", "SupplierName");
            DropColumn("dbo.ProductTransactions", "WareHouseID");
            DropColumn("dbo.CustomerContacts", "CityCode");
            DropColumn("dbo.CustomerContacts", "CountryCode");
            DropColumn("dbo.Customers", "LastUpdatedDateTime");
            DropColumn("dbo.Customers", "LastUpdatedBy");
            DropColumn("dbo.Customers", "CreatedDateTime");
            DropColumn("dbo.Customers", "CreatedBy");
            DropColumn("dbo.Customers", "CustomerLastName");
            DropColumn("dbo.Customers", "CustomerFirstName");
            DropColumn("dbo.BalanceTransactions", "OnlineReferernceID");
            DropColumn("dbo.BalanceTransactions", "IsChequePassed");
            DropColumn("dbo.BalanceTransactions", "ChequeIssuerBank");
            DropColumn("dbo.BalanceTransactions", "ChequeCustomerName");
            DropColumn("dbo.BalanceTransactions", "ChequeNumber");
            DropColumn("dbo.BalanceTransactions", "ChequePassingDate");
            DropColumn("dbo.BalanceTransactions", "ChequeDate");
            DropColumn("dbo.BalanceTransactions", "TransactionDate");
            DropTable("dbo.Warehouses");
            DropTable("dbo.NotificationTypes");
            DropTable("dbo.Notifications");
            DropTable("dbo.ContactTypes");
            AddPrimaryKey("dbo.TransactionTypes", "TransactionTypeID");
            AddForeignKey("dbo.BalanceTransactions", "TransactionTypeID", "dbo.TransactionTypes", "TransactionTypeID", cascadeDelete: true);
        }
    }
}
