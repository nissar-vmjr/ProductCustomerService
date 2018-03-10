using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProductCustomerService.Models;
using System.Data.Entity;

namespace ProductCustomerService
{
    public class LookupContext
    {
        public LookupContext()
        {

        }

        private static object syncLock = new object();

        private static LookupContext lookupContext;

        private static List<NotificationType> notificationTypes;

        private static List<ContactType> contactTypes;

        private static List<TransactionType> transactionTypes;

        private static List<ChequeStatus> chequeStatuses;
        private static List<Warehouse> warehouses;
        private static List<Supplier> suppliers;
        private static List<LineSaleSupplier> lineSaleSuppliers;
        public static List<NotificationType> NotificationTypes
        {
            get
            {
                if (lookupContext == null)
                    lookupContext = CreateInstance();
                if (notificationTypes == null)
                {
                    notificationTypes = new List<NotificationType>();
                }
                return notificationTypes;
            }
        }

        public static List<ContactType> ContactTypes
        {
            get
            {
                if (lookupContext == null)
                    lookupContext = CreateInstance();
                if (contactTypes == null)
                {
                    contactTypes = new List<ContactType>();
                }
                return contactTypes;
            }
        }

        public static List<TransactionType> TransactionTypes
        {
            get
            {
                if (lookupContext == null)
                    lookupContext = CreateInstance();
                if (transactionTypes == null)
                {
                    transactionTypes = new List<TransactionType>();
                }
                return transactionTypes;
            }
        }

        public static List<ChequeStatus> ChequeStatuses
        {
            get
            {
                if (lookupContext == null)
                    lookupContext = CreateInstance();
                if(chequeStatuses == null)
                {
                    chequeStatuses = new List<ChequeStatus>();
                }
                return chequeStatuses;
            }
            
        }

        public static List<Warehouse> Warehouses
        {
            get
            {
                if (lookupContext == null)
                    lookupContext = CreateInstance();
                if (warehouses == null)
                {
                    warehouses = new List<Warehouse>();
                }
                return warehouses;
            }

        }

       

        public static List<Supplier> Suppliers
        {
            get
            {
                if (lookupContext == null)
                    lookupContext = CreateInstance();
                if (suppliers == null)
                {
                    suppliers = new List<Supplier>();
                }
                return suppliers;
            }

        }

        public static List<LineSaleSupplier> LineSaleSuppliers
        {
            get
            {
                if (lookupContext == null)
                    lookupContext = CreateInstance();
                if (lineSaleSuppliers == null)
                {
                    lineSaleSuppliers = new List<LineSaleSupplier>();
                }
                return lineSaleSuppliers;
            }

        }
        public static LookupContext CreateInstance()
        {
            if (lookupContext == null)
            {
                lock (syncLock)
                {
                    if (lookupContext == null)
                    {
                        lookupContext = new LookupContext();
                        using (UnitOfWork uow = new UnitOfWork())
                        {
                            uow.Context.Configuration.AutoDetectChangesEnabled = false;

                            contactTypes = uow.ContactTypes.AsNoTracking().ToList();
                            transactionTypes = uow.TransactionTypes.AsNoTracking().ToList();
                            notificationTypes = uow.NotificationTypes.AsNoTracking().ToList();
                            chequeStatuses = uow.ChequeStatuses.AsNoTracking().ToList();
                            warehouses = uow.Warehouses.AsNoTracking().ToList();
                            suppliers = uow.Suppliers.AsNoTracking().ToList();
                            lineSaleSuppliers = uow.LineSaleSuppliers.AsNoTracking().ToList();
                        }
                    }
                }
            }
            return lookupContext;

        }
    }
}