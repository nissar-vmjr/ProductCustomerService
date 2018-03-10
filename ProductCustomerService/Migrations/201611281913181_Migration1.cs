namespace ProductCustomerService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BalanceTransactions",
                c => new
                    {
                        BalanceTransactionID = c.Long(nullable: false, identity: true),
                        CustomerID = c.Long(nullable: false),
                        TransactionAmount = c.Double(nullable: false),
                        IsDebt = c.Boolean(nullable: false),
                        TransactionTypeID = c.Int(nullable: false),
                        Description = c.String(),
                        OutstandingBalance = c.Double(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDateTime = c.DateTime(),
                        LastUpdatedBy = c.String(),
                        LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.BalanceTransactionID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .ForeignKey("dbo.TransactionTypes", t => t.TransactionTypeID, cascadeDelete: true)
                .Index(t => t.CustomerID)
                .Index(t => t.TransactionTypeID);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.Long(nullable: false, identity: true),
                        CustomerName = c.String(),
                        Address = c.String(),
                        NetOutstandingBalance = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateTable(
                "dbo.CustomerContacts",
                c => new
                    {
                        CustomerContactID = c.Long(nullable: false, identity: true),
                        CustomerID = c.Long(nullable: false),
                        ContactNumber = c.String(),
                    })
                .PrimaryKey(t => t.CustomerContactID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.TransactionTypes",
                c => new
                    {
                        TransactionTypeID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.TransactionTypeID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductID = c.Long(nullable: false, identity: true),
                        ProductName = c.String(),
                        ProductCode = c.String(),
                        VATPercentage = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID);
            
            CreateTable(
                "dbo.ProductTransactions",
                c => new
                    {
                        ProductTransactionID = c.Long(nullable: false, identity: true),
                        SubProductID = c.Long(nullable: false),
                        QuantityPurchased = c.Long(nullable: false),
                        TotalPrice = c.Double(nullable: false),
                        TaxPrice = c.Double(nullable: false),
                        MiscellaneousPrice = c.Double(nullable: false),
                        Description = c.String(),
                        TotalPriceIncludingTax = c.Long(nullable: false),
                        QuantityRemaining = c.Long(nullable: false),
                        IsSelling = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDateTime = c.DateTime(),
                        LastUpdatedBy = c.String(),
                        LastUpdatedDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ProductTransactionID)
                .ForeignKey("dbo.SubProducts", t => t.SubProductID, cascadeDelete: true)
                .Index(t => t.SubProductID);
            
            CreateTable(
                "dbo.SubProducts",
                c => new
                    {
                        SubProductID = c.Long(nullable: false, identity: true),
                        ProductID = c.Long(nullable: false),
                        SubProductName = c.String(),
                        QuantityAvailable = c.Long(nullable: false),
                        MarkedPrice = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.SubProductID)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID);
            
            //CreateTable(
            //    "dbo.AspNetRoles",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Name = c.String(nullable: false, maxLength: 256),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            //CreateTable(
            //    "dbo.AspNetUserRoles",
            //    c => new
            //        {
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            RoleId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.UserId, t.RoleId })
            //    .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId)
            //    .Index(t => t.RoleId);
            
            //CreateTable(
            //    "dbo.AspNetUsers",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Email = c.String(maxLength: 256),
            //            EmailConfirmed = c.Boolean(nullable: false),
            //            PasswordHash = c.String(),
            //            SecurityStamp = c.String(),
            //            PhoneNumber = c.String(),
            //            PhoneNumberConfirmed = c.Boolean(nullable: false),
            //            TwoFactorEnabled = c.Boolean(nullable: false),
            //            LockoutEndDateUtc = c.DateTime(),
            //            LockoutEnabled = c.Boolean(nullable: false),
            //            AccessFailedCount = c.Int(nullable: false),
            //            UserName = c.String(nullable: false, maxLength: 256),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            //CreateTable(
            //    "dbo.AspNetUserClaims",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //            ClaimType = c.String(),
            //            ClaimValue = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);
            
            //CreateTable(
            //    "dbo.AspNetUserLogins",
            //    c => new
            //        {
            //            LoginProvider = c.String(nullable: false, maxLength: 128),
            //            ProviderKey = c.String(nullable: false, maxLength: 128),
            //            UserId = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
            //    .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            //DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProductTransactions", "SubProductID", "dbo.SubProducts");
            DropForeignKey("dbo.SubProducts", "ProductID", "dbo.Products");
            DropForeignKey("dbo.BalanceTransactions", "TransactionTypeID", "dbo.TransactionTypes");
            DropForeignKey("dbo.BalanceTransactions", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.CustomerContacts", "CustomerID", "dbo.Customers");
            //DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            //DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            //DropIndex("dbo.AspNetUsers", "UserNameIndex");
            //DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            //DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            //DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.SubProducts", new[] { "ProductID" });
            DropIndex("dbo.ProductTransactions", new[] { "SubProductID" });
            DropIndex("dbo.CustomerContacts", new[] { "CustomerID" });
            DropIndex("dbo.BalanceTransactions", new[] { "TransactionTypeID" });
            DropIndex("dbo.BalanceTransactions", new[] { "CustomerID" });
            //DropTable("dbo.AspNetUserLogins");
            //DropTable("dbo.AspNetUserClaims");
            //DropTable("dbo.AspNetUsers");
            //DropTable("dbo.AspNetUserRoles");
            //DropTable("dbo.AspNetRoles");
            DropTable("dbo.SubProducts");
            DropTable("dbo.ProductTransactions");
            DropTable("dbo.Products");
            DropTable("dbo.TransactionTypes");
            DropTable("dbo.CustomerContacts");
            DropTable("dbo.Customers");
            DropTable("dbo.BalanceTransactions");
        }
    }
}
