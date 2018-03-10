namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration18 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LineSales", "SupplierID", "dbo.Suppliers");
            DropIndex("dbo.LineSales", new[] { "SupplierID" });
            AddColumn("dbo.LineSales", "LineSaleSupplierID", c => c.Int());
            CreateIndex("dbo.LineSales", "LineSaleSupplierID");
            AddForeignKey("dbo.LineSales", "LineSaleSupplierID", "dbo.LineSaleSuppliers", "LineSaleSupplierID");
            DropColumn("dbo.LineSales", "SupplierID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LineSales", "SupplierID", c => c.Int());
            DropForeignKey("dbo.LineSales", "LineSaleSupplierID", "dbo.LineSaleSuppliers");
            DropIndex("dbo.LineSales", new[] { "LineSaleSupplierID" });
            DropColumn("dbo.LineSales", "LineSaleSupplierID");
            CreateIndex("dbo.LineSales", "SupplierID");
            AddForeignKey("dbo.LineSales", "SupplierID", "dbo.Suppliers", "SupplierID");
        }
    }
}
