namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suppliers", "DisplayInDropdown", c => c.Boolean(nullable: false));
            AddColumn("dbo.Suppliers", "CreatedBy", c => c.String());
            AddColumn("dbo.Suppliers", "CreatedDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "CreatedDateTime");
            DropColumn("dbo.Suppliers", "CreatedBy");
            DropColumn("dbo.Suppliers", "DisplayInDropdown");
        }
    }
}
