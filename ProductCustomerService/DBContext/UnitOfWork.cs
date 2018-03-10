using ProductCustomerService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace ProductCustomerService
{
    public class UnitOfWork:IDisposable
    {
        private readonly ProdCustContext context;
        private bool disposed = false;

        public UnitOfWork()
        {
            context = new ProdCustContext();
            //Do not enabled proxied entities, else serialization fails
            context.Configuration.ProxyCreationEnabled = false;
            //Load Navigation properties explicitly
            context.Configuration.LazyLoadingEnabled = false;
        }

        private DbContextTransaction transaction = null;
        public UnitOfWork(ProdCustContext context)
        {
            this.context = context;
        }

        public ProdCustContext Context
        {
            get { return context; }
        }

        public IDbSet<Product> Products
        {
            get { return context.Products; }
        }

        public IDbSet<SubProduct> SubProducts
        {
            get { return context.SubProducts; }
        }


        public IDbSet<Customer> Customers
        {
            get { return context.Customers; }
        }

        public IDbSet<BalanceTransaction> BalanceTransactions
        {
            get { return context.BalanceTransactions; }
        }
        public IDbSet<CustomerContact> CustomerContacts
        {
            get { return context.CustomerContacts; }
        }
        public IDbSet<ProductTransaction> ProductTransactions
        {
            get { return context.ProductTransactions; }
        }

        public IDbSet<TransactionType> TransactionTypes
        {
            get { return context.TransactionTypes; }
        }

        public IDbSet<Notification> Notification
        {
            get { return context.Notifications; }
        }

        public IDbSet<NotificationType> NotificationTypes
        {
            get { return context.NotificationTypes; }
        }

        public IDbSet<Warehouse> Warehouses
        {
            get { return context.Warehouses; }
        }

        public IDbSet<ContactType> ContactTypes
        {
            get { return context.ContactTypes; }
        }

        public IDbSet<LineSale> LineSales
        {
            get { return context.LineSales; }
        }

        public IDbSet<LineSaleTracking> LineSaleTrackings
        {
            get { return context.LineSaleTrackings; }
        }

        public IDbSet<Supplier> Suppliers
        {
            get { return context.Suppliers; }
        }

        public IDbSet<ChequeStatus> ChequeStatuses
        {
            get { return context.ChequeStatuses; }
        }

        public IDbSet<LineSaleSupplier> LineSaleSuppliers
        {
            get { return context.LineSaleSuppliers; }
        }

        public IDbSet<SupplierTracking> SupplierTrackings
        {
            get { return context.SupplierTrackings; }
        }

        public IDbSet<DataTemplate> DataTemplates
        {
            get { return context.DataTemplates; }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Commit()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception entityValiationException)
            {
                var entityException = new Exception(entityValiationException.Message);
                throw entityException;
            }
        }


        public void BeginTransaction()
        {
            if (transaction == null)
            {
                transaction = context.Database.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            if (transaction != null)
            {
                //AuditLogData();
                transaction.Commit();
            }
        }
        public void RollbackTransaction()
        {
            if (transaction != null)
            {
                transaction.Rollback();
            }
        }
    }
}