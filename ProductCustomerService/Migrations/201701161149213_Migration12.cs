namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration12 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductTransactions", "BalanceTransactionID", "dbo.BalanceTransactions");
            DropForeignKey("dbo.ProductTransactions", "SubProductID", "dbo.SubProducts");
            DropForeignKey("dbo.ProductTransactions", "Supplier_SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.ProductTransactions", "Warehouse_WarehouseID", "dbo.Warehouses");
            DropIndex("dbo.ProductTransactions", new[] { "SubProductID" });
            DropIndex("dbo.ProductTransactions", new[] { "BalanceTransactionID" });
            DropIndex("dbo.ProductTransactions", new[] { "Supplier_SupplierID" });
            DropIndex("dbo.ProductTransactions", new[] { "Warehouse_WarehouseID" });
            DropTable("dbo.ProductTransactions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductTransactions",
                c => new
                    {
                        ProductTransactionID = c.Long(nullable: false, identity: true),
                        SubProductID = c.Long(nullable: false),
                        WarehouseID = c.Long(),
                        SupplierID = c.Long(),
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
                        Supplier_SupplierID = c.Int(),
                        Warehouse_WarehouseID = c.Int(),
                    })
                .PrimaryKey(t => t.ProductTransactionID);
            
            CreateIndex("dbo.ProductTransactions", "Warehouse_WarehouseID");
            CreateIndex("dbo.ProductTransactions", "Supplier_SupplierID");
            CreateIndex("dbo.ProductTransactions", "BalanceTransactionID");
            CreateIndex("dbo.ProductTransactions", "SubProductID");
            AddForeignKey("dbo.ProductTransactions", "Warehouse_WarehouseID", "dbo.Warehouses", "WarehouseID");
            AddForeignKey("dbo.ProductTransactions", "Supplier_SupplierID", "dbo.Suppliers", "SupplierID");
            AddForeignKey("dbo.ProductTransactions", "SubProductID", "dbo.SubProducts", "SubProductID", cascadeDelete: true);
            AddForeignKey("dbo.ProductTransactions", "BalanceTransactionID", "dbo.BalanceTransactions", "BalanceTransactionID");
        }
    }
}
