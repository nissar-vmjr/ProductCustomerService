namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BalanceTransactions", "ChequeClosureComments", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BalanceTransactions", "ChequeClosureComments");
        }
    }
}
