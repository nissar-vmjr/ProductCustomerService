namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChequeStatus",
                c => new
                    {
                        ChequeStatusID = c.Int(nullable: false),
                        StatusName = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ChequeStatusID);
            
            AddColumn("dbo.BalanceTransactions", "ChequeDepositedDate", c => c.DateTime());
            AddColumn("dbo.BalanceTransactions", "ChequeActionDate", c => c.DateTime());
            AddColumn("dbo.BalanceTransactions", "ChequeStatusID", c => c.Int());
            AddColumn("dbo.BalanceTransactions", "ChequeFailureComments", c => c.String(maxLength: 2000));
            AddColumn("dbo.Customers", "InitialBalance", c => c.Double());
            AddColumn("dbo.ProductTransactions", "BalanceTransactionID", c => c.Long());
            AddColumn("dbo.ProductTransactions", "TransactionDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProductTransactions", "BuyQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.ProductTransactions", "SellQuantity", c => c.Int(nullable: false));
            AddColumn("dbo.ProductTransactions", "IsSellFromWarehouse", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductTransactions", "SellingPrice", c => c.Double(nullable: false));
            CreateIndex("dbo.BalanceTransactions", "ChequeStatusID");
            CreateIndex("dbo.ProductTransactions", "BalanceTransactionID");
            AddForeignKey("dbo.BalanceTransactions", "ChequeStatusID", "dbo.ChequeStatus", "ChequeStatusID");
            AddForeignKey("dbo.ProductTransactions", "BalanceTransactionID", "dbo.BalanceTransactions", "BalanceTransactionID");
            DropColumn("dbo.BalanceTransactions", "ChequePassingDate");
            DropColumn("dbo.BalanceTransactions", "IsChequePassed");
            DropColumn("dbo.ProductTransactions", "QuantityPurchased");
            DropColumn("dbo.ProductTransactions", "IsSelling");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductTransactions", "IsSelling", c => c.Boolean(nullable: false));
            AddColumn("dbo.ProductTransactions", "QuantityPurchased", c => c.Long(nullable: false));
            AddColumn("dbo.BalanceTransactions", "IsChequePassed", c => c.Boolean());
            AddColumn("dbo.BalanceTransactions", "ChequePassingDate", c => c.DateTime());
            DropForeignKey("dbo.ProductTransactions", "BalanceTransactionID", "dbo.BalanceTransactions");
            DropForeignKey("dbo.BalanceTransactions", "ChequeStatusID", "dbo.ChequeStatus");
            DropIndex("dbo.ProductTransactions", new[] { "BalanceTransactionID" });
            DropIndex("dbo.BalanceTransactions", new[] { "ChequeStatusID" });
            DropColumn("dbo.ProductTransactions", "SellingPrice");
            DropColumn("dbo.ProductTransactions", "IsSellFromWarehouse");
            DropColumn("dbo.ProductTransactions", "SellQuantity");
            DropColumn("dbo.ProductTransactions", "BuyQuantity");
            DropColumn("dbo.ProductTransactions", "TransactionDate");
            DropColumn("dbo.ProductTransactions", "BalanceTransactionID");
            DropColumn("dbo.Customers", "InitialBalance");
            DropColumn("dbo.BalanceTransactions", "ChequeFailureComments");
            DropColumn("dbo.BalanceTransactions", "ChequeStatusID");
            DropColumn("dbo.BalanceTransactions", "ChequeActionDate");
            DropColumn("dbo.BalanceTransactions", "ChequeDepositedDate");
            DropTable("dbo.ChequeStatus");
        }
    }
}
