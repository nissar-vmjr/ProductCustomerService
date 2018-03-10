using ProductCustomerService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProductCustomerService
{
    public class ProdCustContext: ApplicationDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<SubProduct> SubProducts { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }

        public DbSet<ProductTransaction> ProductTransactions { get; set; }

        public DbSet<BalanceTransaction> BalanceTransactions { get; set; }

        public DbSet<TransactionType> TransactionTypes { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<NotificationType> NotificationTypes { get; set; }

        public DbSet<ContactType> ContactTypes { get; set; }

        public DbSet<LineSale> LineSales { get; set; }

        public DbSet<LineSaleTracking> LineSaleTrackings { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<ChequeStatus> ChequeStatuses { get; set; }

        public DbSet<LineSaleSupplier> LineSaleSuppliers { get; set; }

        public DbSet<SupplierTracking> SupplierTrackings { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<DataTemplate> DataTemplates { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
    }
}