namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration13 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductTransactions",
                c => new
                    {
                        ProductTransactionID = c.Long(nullable: false, identity: true),
                        SubProductID = c.Long(nullable: false),
                        WarehouseID = c.Int(),
                        SupplierID = c.Int(),
                        BalanceTransactionID = c.Long(),
                        TransactionDate = c.DateTime(nullable: false),
                        BuyQuantity = c.Int(nullable: false),
                        SellQuantity = c.Int(nullable: false),
                        IsSellFromWarehouse = c.Boolean(nullable: false),
                        SellingPrice = c.Double(nullable: false),
                        TotalPrice = c.Double(nullable: false),
                        TaxPrice = c.Double(nullable: false),
                        MiscellaneousPrice = c.Double(nullable: false),
                        Description = c.String(maxLength: 2000),
                        TotalPriceIncludingTax = c.Long(nullable: false),
                        QuantityRemaining = c.Long(nullable: false),
                        SupplierName = c.String(maxLength: 100),
                        CreatedBy = c.String(maxLength: 50),
                        CreatedDateTime = c.DateTime(),
                        LastUpdatedBy = c.String(maxLength: 50),
                        LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductTransactionID)
                .ForeignKey("dbo.BalanceTransactions", t => t.BalanceTransactionID)
                .ForeignKey("dbo.SubProducts", t => t.SubProductID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .ForeignKey("dbo.Warehouses", t => t.WarehouseID)
                .Index(t => t.SubProductID)
                .Index(t => t.WarehouseID)
                .Index(t => t.SupplierID)
                .Index(t => t.BalanceTransactionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductTransactions", "WarehouseID", "dbo.Warehouses");
            DropForeignKey("dbo.ProductTransactions", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.ProductTransactions", "SubProductID", "dbo.SubProducts");
            DropForeignKey("dbo.ProductTransactions", "BalanceTransactionID", "dbo.BalanceTransactions");
            DropIndex("dbo.ProductTransactions", new[] { "BalanceTransactionID" });
            DropIndex("dbo.ProductTransactions", new[] { "SupplierID" });
            DropIndex("dbo.ProductTransactions", new[] { "WarehouseID" });
            DropIndex("dbo.ProductTransactions", new[] { "SubProductID" });
            DropTable("dbo.ProductTransactions");
        }
    }
}
