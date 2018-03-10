namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration10 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductTransactions", new[] { "WareHouse_WarehouseID" });
            CreateIndex("dbo.ProductTransactions", "Warehouse_WarehouseID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductTransactions", new[] { "Warehouse_WarehouseID" });
            CreateIndex("dbo.ProductTransactions", "WareHouse_WarehouseID");
        }
    }
}
