namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BalanceTransactions", "Description", c => c.String(maxLength: 2000));
            AlterColumn("dbo.BalanceTransactions", "CreatedBy", c => c.String(maxLength: 50));
            AlterColumn("dbo.BalanceTransactions", "LastUpdatedBy", c => c.String(maxLength: 50));
            AlterColumn("dbo.Customers", "CustomerName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Customers", "Address", c => c.String(maxLength: 2000));
            AlterColumn("dbo.CustomerContacts", "ContactNumber", c => c.String(maxLength: 50));
            AlterColumn("dbo.TransactionTypes", "Description", c => c.String(maxLength: 200));
            AlterColumn("dbo.Products", "ProductName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Products", "ProductCode", c => c.String(maxLength: 50));
            AlterColumn("dbo.ProductTransactions", "Description", c => c.String(maxLength: 2000));
            AlterColumn("dbo.ProductTransactions", "CreatedBy", c => c.String(maxLength: 50));
            AlterColumn("dbo.ProductTransactions", "LastUpdatedBy", c => c.String(maxLength: 50));
            AlterColumn("dbo.SubProducts", "SubProductName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SubProducts", "SubProductName", c => c.String());
            AlterColumn("dbo.ProductTransactions", "LastUpdatedBy", c => c.String());
            AlterColumn("dbo.ProductTransactions", "CreatedBy", c => c.String());
            AlterColumn("dbo.ProductTransactions", "Description", c => c.String());
            AlterColumn("dbo.Products", "ProductCode", c => c.String());
            AlterColumn("dbo.Products", "ProductName", c => c.String());
            AlterColumn("dbo.TransactionTypes", "Description", c => c.String());
            AlterColumn("dbo.CustomerContacts", "ContactNumber", c => c.String());
            AlterColumn("dbo.Customers", "Address", c => c.String());
            AlterColumn("dbo.Customers", "CustomerName", c => c.String());
            AlterColumn("dbo.BalanceTransactions", "LastUpdatedBy", c => c.String());
            AlterColumn("dbo.BalanceTransactions", "CreatedBy", c => c.String());
            AlterColumn("dbo.BalanceTransactions", "Description", c => c.String());
        }
    }
}
