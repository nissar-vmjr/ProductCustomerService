namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BalanceTransactions", "TransactionTypeID", "dbo.TransactionTypes");
            DropIndex("dbo.BalanceTransactions", new[] { "TransactionTypeID" });
            DropColumn("dbo.BalanceTransactions", "TransactionTypeID");
            DropTable("dbo.TransactionTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TransactionTypes",
                c => new
                    {
                        TransactionTypeID = c.Int(nullable: false),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.TransactionTypeID);
            
            AddColumn("dbo.BalanceTransactions", "TransactionTypeID", c => c.Int(nullable: false));
            CreateIndex("dbo.BalanceTransactions", "TransactionTypeID");
            AddForeignKey("dbo.BalanceTransactions", "TransactionTypeID", "dbo.TransactionTypes", "TransactionTypeID", cascadeDelete: true);
        }
    }
}
