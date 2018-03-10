namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration19 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductTransactions", "BuyingAmount", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductTransactions", "BuyingAmount");
        }
    }
}
