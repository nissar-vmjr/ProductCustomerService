using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProductCustomerService.UIContext;
using ProductCustomerService.Models;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net;
using System.Data.OleDb;
using System.IO;
using System.Data;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace ProductCustomerService.BusinessLayer
{
    public class BusinessProvider : BaseProvider, IBusinessProvider
    {

        public BusinessProvider()
        {

        }

        public async Task<RsCustomer> SaveCustomerDetails(RqCustomer rqCustomer)
        {
            var response = new RsCustomer();
            try
            {
                if (rqCustomer.customer == null)
                {
                    ThrowCustomException("Please provide customer details");
                }
                using (UnitOfWork uow = new UnitOfWork())
                {
                    if (!rqCustomer.customer.customerID.HasValue || rqCustomer.customer.customerID.Value <= 0)
                    {
                        ValidateIfContactNumberAlreadyExists(uow, rqCustomer, response);
                        Customer customer = new Customer();
                        customer.CreatedBy = rqCustomer.UserLogin;
                        customer.Map(rqCustomer.customer, false);
                        customer.InitialBalance = rqCustomer.customer.netOutstandingBalance;
                        uow.Context.Customers.Add(customer);
                        try
                        {
                            uow.BeginTransaction();
                            uow.Commit();

                            var primaryContact = new CustomerContact();
                            primaryContact.CustomerID = customer.CustomerID;
                            primaryContact.IsPrimaryContact = true;
                            primaryContact.Map(rqCustomer.customer.primaryContact);


                            uow.CustomerContacts.Add(primaryContact);
                            if (rqCustomer.customer.secondaryContact != null)
                            {
                                var secondaryContact = new CustomerContact();
                                secondaryContact.CustomerID = customer.CustomerID;
                                secondaryContact.IsPrimaryContact = false;
                                secondaryContact.Map(rqCustomer.customer.secondaryContact);
                                uow.CustomerContacts.Add(secondaryContact);
                            }


                            uow.Commit();

                            uow.CommitTransaction();

                        }
                        catch (Exception ex)
                        {
                            uow.RollbackTransaction();
                            throw new Exception(ex.Message, ex);
                        }

                        try
                        {
                            response.customer.Map(customer);
                        }
                        catch (Exception ex)
                        {
                            ThrowWarningException("Customer record saved but fetch failed");
                        }

                    }
                    else
                    {
                        var customer = uow.Context.Customers.Include("CustomerContacts").Where(x => x.CustomerID == rqCustomer.customer.customerID.Value).FirstOrDefault();

                        if (customer == null)
                        {
                            ThrowCustomException("Invalid customer");
                        }

                        customer = customer.Map(rqCustomer.customer, true);

                        var primaryContact = customer.CustomerContacts.FirstOrDefault(x => x.IsPrimaryContact == true);
                        if (primaryContact != null)
                        {
                            ValidateIfContactNumberAlreadyExists(uow, rqCustomer, response, primaryContact.ContactNumber);
                            primaryContact.Map(rqCustomer.customer.primaryContact);
                        }

                        var secondaryContact = customer.CustomerContacts.FirstOrDefault(x => x.IsPrimaryContact == false);
                        if (secondaryContact != null)
                        {
                            if (rqCustomer.customer.secondaryContact == null)
                            {
                                uow.CustomerContacts.Remove(secondaryContact);
                            }
                            else
                            {
                                secondaryContact.Map(rqCustomer.customer.secondaryContact);
                            }
                        }
                        else
                        {
                            if (rqCustomer.customer.secondaryContact != null)
                            {
                                secondaryContact = new CustomerContact();
                                secondaryContact.CustomerID = customer.CustomerID;
                                secondaryContact.IsPrimaryContact = false;
                                secondaryContact.Map(rqCustomer.customer.secondaryContact);
                                uow.CustomerContacts.Add(secondaryContact);
                            }
                        }
                        customer.LastUpdatedBy = rqCustomer.UserLogin;

                        uow.Commit();

                        try
                        {
                            response.customer.Map(customer);
                        }
                        catch (Exception ex)
                        {
                            ThrowWarningException("Customer record saved but fetch failed");
                        }

                    }

                    response.ResponseMessage = "Save customer success";
                    response.ResponseStatus = ResponseStatus.Success;
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Saving customer failed", exception);
            }

            return await Task.FromResult(response);
        }

        private void ValidateIfContactNumberAlreadyExists(UnitOfWork uow, RqCustomer rqCustomer, RsCustomer response, string contactNo = null)
        {
            if (!rqCustomer.isAllowNumberRepeat && rqCustomer.customer.primaryContact != null && !string.IsNullOrWhiteSpace(rqCustomer.customer.primaryContact.contactNumber))
            {
                var contacts = uow.Context.CustomerContacts.Include("Customer").Where(x => x.ContactNumber == rqCustomer.customer.primaryContact.contactNumber).ToList();
                bool flag = true;
                if (contactNo != null && contactNo == rqCustomer.customer.primaryContact.contactNumber)
                {
                    flag = false;
                }
                if (flag && contacts != null && contacts.Count > 0)
                {
                    foreach (var contact in contacts)
                    {
                        response.RuleViolations.Add(new RuleViolation(BusinessHelper.FormatName(contact.Customer.CustomerFirstName, contact.Customer.CustomerLastName, contact.Customer.Keyword)));
                    }
                    ThrowCustomException("Contact Number already exists");

                }
            }
        }
        public RsCustomer GetCustomerDetails(long customerID, string userLogin)
        {
            var response = new RsCustomer();
            try
            {
                if (customerID <= 0)
                {
                    ThrowCustomException("Invalid Customer");
                }
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var customer = uow.Context.Customers.Include("CustomerContacts").Where(x => x.CustomerID == customerID).FirstOrDefault();
                    if (customer == null)
                    {
                        ThrowCustomException("Invalid Customer");
                    }
                    response.customer.Map(customer);
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetching customer success";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching customer failed", exception);
            }

            return response;
        }



        public RsCustomerKeywordSearch GetCustomerSearchResults(string keyword)
        {
            var response = new RsCustomerKeywordSearch();

            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    long result;
                    List<Customer> customers = new List<Customer>(); ;
                    if (long.TryParse(keyword, out result))
                    {
                        var customerIds = uow.Context.CustomerContacts.Include("Customer").Where(x => x.ContactNumber.Contains(keyword))?.Select(x => x.CustomerID).ToList();
                        if (customerIds != null && customerIds.Count > 0)
                            customers = uow.Context.Customers.Where(x => customerIds.Distinct().Contains(x.CustomerID)).ToList();

                    }
                    else
                    {
                        if (keyword.Contains(' '))
                        {
                            keyword = keyword.Split(' ').First();
                        }
                        keyword = keyword.ToLower();
                        customers = uow.Context.Customers.Where(x => x.CustomerFirstName.ToLower().Contains(keyword) || (x.CustomerLastName != null && x.CustomerLastName.ToLower().Contains(keyword)) || (x.Keyword != null && x.Keyword.ToLower().Contains(keyword))).ToList();
                    }
                    foreach (var customer in customers)
                    {
                        response.customers.Add(new customerSearch
                        {
                            customerID = customer.CustomerID,
                            name = BusinessHelper.FormatName(customer.CustomerFirstName, customer.CustomerLastName, customer.Keyword)
                        });
                    }

                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetching customers success";
                }

            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching customers failed", exception);
            }
            return response;
        }

        public RsBalanceTransaction SaveBalanceTransaction(RqBalanceTransaction rqBalanceTransaction)
        {
            var response = new RsBalanceTransaction();
            try
            {
                if (rqBalanceTransaction.balanceTransaction == null)
                {
                    ThrowCustomException("Please provide transaction details");
                }
                if (rqBalanceTransaction.balanceTransaction.incomingAmount <= 0 && rqBalanceTransaction.balanceTransaction.outgoingAmount <= 0)
                {
                    ThrowCustomException("Both incoming and outgoing amount cannot be zero or below");
                }
                using (UnitOfWork uow = new UnitOfWork())
                {
                    if (!rqBalanceTransaction.balanceTransaction.balanceTransactionID.HasValue || rqBalanceTransaction.balanceTransaction.balanceTransactionID <= 0)
                    {
                        var customerRecord = uow.Context.Customers.Where(x => x.CustomerID == rqBalanceTransaction.customerID).FirstOrDefault();
                        if (customerRecord == null)
                        {
                            ThrowCustomException("Invalid customer");
                        }
                        BalanceTransaction dbRecord = new BalanceTransaction();
                        dbRecord.CustomerID = rqBalanceTransaction.customerID;
                        dbRecord.CreatedBy = rqBalanceTransaction.UserLogin;
                        dbRecord.Map(rqBalanceTransaction.balanceTransaction, false);
                        if (rqBalanceTransaction.balanceTransaction.transactionTypeID == (int)TransactionTypeEnum.Cheque)
                        {
                            dbRecord.OutstandingBalance = customerRecord.NetOutstandingBalance + dbRecord.OutgoingAmount;
                            customerRecord.NetOutstandingBalance = dbRecord.OutstandingBalance;
                            customerRecord.LastUpdatedDateTime = DateTime.Now;
                            customerRecord.LastUpdatedBy = rqBalanceTransaction.UserLogin;
                        }
                        else
                        {
                            customerRecord.NetOutstandingBalance = customerRecord.NetOutstandingBalance + dbRecord.OutgoingAmount - dbRecord.IncomingAmount;
                            dbRecord.OutstandingBalance = customerRecord.NetOutstandingBalance;
                            customerRecord.LastUpdatedBy = rqBalanceTransaction.UserLogin;
                            customerRecord.LastUpdatedDateTime = DateTime.Now;
                        }
                        uow.Context.BalanceTransactions.Add(dbRecord);

                        uow.Commit();
                        dbRecord.Customer = customerRecord;

                        try
                        {
                            response.balanceTransaction.Map(dbRecord);
                        }
                        catch (Exception ex)
                        {
                            ThrowWarningException("Transaction saved succesfully but fetch failed");
                        }
                    }
                    else
                    {
                        var dbRecord = uow.Context.BalanceTransactions.Include("Customer").Where(x => x.BalanceTransactionID == rqBalanceTransaction.balanceTransaction.balanceTransactionID.Value).FirstOrDefault();
                        if (dbRecord == null)
                        {
                            ThrowCustomException("Invalid transaction");
                        }
                        //if (dbRecord.TransactionTypeID == (int)TransactionTypeEnum.Cheque && ((dbRecord.ChequeStatusID.HasValue && dbRecord.ChequeStatusID.Value >= (int)ChequeStatusEnum.Pending)))
                        //{
                        //    ThrowCustomException("Cannot edit once cheque is presented to bank. Fail the cheque as an alternative.");
                        //}
                        if (dbRecord.TransactionTypeID == (int)TransactionTypeEnum.Cheque)
                        {
                            ThrowCustomException("Cheque transactions can only be deleted before presenting to bank and cannot be edited.");
                        }
                        if (rqBalanceTransaction.balanceTransaction.transactionTypeID == (int)TransactionTypeEnum.Cheque)
                        {
                            ThrowCustomException("Cannot edit a transaction type to cheque");
                        }
                        //if (BusinessHelper.GetLastUpdatedDateTime(dbRecord.CreatedDateTime, dbRecord.LastUpdatedDateTime).AddDays(3) < DateTime.Now)
                        //{
                        //    ThrowCustomException("This transaction is older than 3 days. Hence cannot be edited.");
                        //}
                        var records = uow.Context.BalanceTransactions.Where(x => x.CustomerID == dbRecord.CustomerID).ToList();
                        foreach (var record in records)
                        {
                            if (BusinessHelper.GetLastUpdatedDateTime(record.CreatedDateTime, record.LastUpdatedDateTime) > BusinessHelper.GetLastUpdatedDateTime(dbRecord.CreatedDateTime, dbRecord.LastUpdatedDateTime) && record.TransactionTypeID != (int)TransactionTypeEnum.Cheque)
                            {
                                ThrowCustomException("This transaction is not latest transaction on this customer");
                            }
                        }

                        dbRecord.LastUpdatedBy = rqBalanceTransaction.UserLogin;

                        double previousOutstandingbalance = dbRecord.OutstandingBalance;
                        int previousTranstype = dbRecord.TransactionTypeID;
                        double previousIncomingAmount = dbRecord.IncomingAmount;
                        double previousOutgoingAmount = dbRecord.OutgoingAmount;

                        if (previousTranstype != (int)TransactionTypeEnum.Cheque && dbRecord.TransactionTypeID != (int)TransactionTypeEnum.Cheque)
                        {
                            previousOutstandingbalance = previousOutstandingbalance - dbRecord.OutgoingAmount + dbRecord.IncomingAmount;
                        }

                        dbRecord.Map(rqBalanceTransaction.balanceTransaction, true);

                        //if(previousTranstype==(int)TransactionTypeEnum.Cheque && dbRecord.TransactionTypeID == (int)TransactionTypeEnum.Cheque)
                        //{
                        //    dbRecord.OutstandingBalance = previousOutstandingbalance - previousOutgoingAmount + dbRecord.OutgoingAmount;                           
                        //}
                        //else if(previousTranstype == (int)TransactionTypeEnum.Cheque && dbRecord.TransactionTypeID != (int)TransactionTypeEnum.Cheque)
                        //{
                        //    dbRecord.OutstandingBalance = previousOutstandingbalance - previousOutgoingAmount + dbRecord.OutgoingAmount - dbRecord.IncomingAmount;                         
                        //}
                        //else if(previousTranstype != (int)TransactionTypeEnum.Cheque && dbRecord.TransactionTypeID == (int)TransactionTypeEnum.Cheque)
                        //{
                        //    dbRecord.OutstandingBalance = previousOutstandingbalance - previousOutgoingAmount+previousIncomingAmount+ dbRecord.OutgoingAmount;                          
                        //}
                        //else
                        //{                           
                        dbRecord.OutstandingBalance = previousOutstandingbalance + dbRecord.OutgoingAmount - dbRecord.IncomingAmount;
                        //}
                        dbRecord.Customer.NetOutstandingBalance = dbRecord.OutstandingBalance;
                        dbRecord.Customer.LastUpdatedBy = rqBalanceTransaction.UserLogin;
                        dbRecord.Customer.LastUpdatedDateTime = DateTime.Now;

                        uow.Commit();

                        try
                        {
                            response.balanceTransaction.Map(dbRecord);
                        }
                        catch (Exception ex)
                        {
                            ThrowWarningException("Transaction saved succesfully but fetch failed");
                        }
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Transaction saved successfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Transaction save failure", exception);
            }
            return response;
        }

        public RsCommon DeleteBalanceTransaction(RqDeleteTransaction rqDeleteTransaction)
        {
            var response = new RsCommon();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var dbRecord = uow.Context.BalanceTransactions.Where(x => x.BalanceTransactionID == rqDeleteTransaction.transactionID).FirstOrDefault();
                    if (dbRecord == null)
                    {
                        ThrowCustomException("Invalid transaction");
                    }
                    if (dbRecord.TransactionTypeID == (int)TransactionTypeEnum.Cheque && ((dbRecord.ChequeStatusID.HasValue && dbRecord.ChequeStatusID.Value >= (int)ChequeStatusEnum.Pending)))
                    {
                        ThrowCustomException("Cannot delete once cheque is presented to bank. Fail the cheque as an alternative.");
                    }

                    if (BusinessHelper.GetLastUpdatedDateTime(dbRecord.CreatedDateTime, dbRecord.LastUpdatedDateTime).AddDays(3) < DateTime.Now && dbRecord.TransactionTypeID != (int)TransactionTypeEnum.Cheque)
                    {
                        ThrowCustomException("You cannot delete this transaction after 3 days");
                    }

                    if (dbRecord.TransactionTypeID != (int)TransactionTypeEnum.Cheque)
                    {
                        var records = uow.Context.BalanceTransactions.Where(x => x.CustomerID == dbRecord.CustomerID).ToList();
                        foreach (var record in records)
                        {
                            if (BusinessHelper.GetLastUpdatedDateTime(record.CreatedDateTime, record.LastUpdatedDateTime) > BusinessHelper.GetLastUpdatedDateTime(dbRecord.CreatedDateTime, dbRecord.LastUpdatedDateTime) &&
                                !(record.TransactionTypeID == (int)TransactionTypeEnum.Cheque && record.ChequeStatusID.HasValue && record.ChequeStatusID.Value >= (int)ChequeStatusEnum.Pending))
                            {
                                ThrowCustomException("This transaction is not latest transaction on this customer");
                            }
                        }
                    }

                    try
                    {
                        uow.BeginTransaction();
                        var customer = uow.Context.Customers.Where(x => x.CustomerID == dbRecord.CustomerID).FirstOrDefault();
                        var productTransactions = uow.Context.ProductTransactions.Where(x => x.BalanceTransactionID == rqDeleteTransaction.transactionID).ToList();
                        productTransactions.ForEach(x =>
                        {
                            x.BalanceTransactionID = null;
                            x.LastUpdatedDateTime = DateTime.Now;
                            x.LastUpdatedBy = rqDeleteTransaction.UserLogin;
                        });
                        uow.Commit();

                        if (dbRecord.TransactionTypeID != (int)TransactionTypeEnum.Cheque)
                        {
                            customer.NetOutstandingBalance = dbRecord.OutstandingBalance - dbRecord.OutgoingAmount + dbRecord.IncomingAmount;
                            customer.LastUpdatedBy = rqDeleteTransaction.UserLogin;
                            customer.LastUpdatedDateTime = DateTime.Now;
                        }
                        uow.Context.BalanceTransactions.Remove(dbRecord);

                        uow.Commit();
                        uow.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        uow.RollbackTransaction();
                        throw ex;
                    }

                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Transaction deleted succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Transaction delete failed", exception);
            }
            return response;
        }

        public RsBalanceTransaction GetBalanceTransaction(long transactionID, string userlogin)
        {
            var response = new RsBalanceTransaction();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var transaction = uow.Context.BalanceTransactions.Include("Customer").Include("Customer.CustomerContacts").Where(x => x.BalanceTransactionID == transactionID).FirstOrDefault();
                    if (transaction == null)
                    {
                        ThrowCustomException("Invalid transaction");
                    }
                    response.balanceTransaction = new balanceTransaction().Map(transaction);
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched details succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching details failed", exception);
            }
            return response;
        }
        public RsSearchCustomerResult GetCustomerRecentTransactions(RqBalanceTransaction rqBalanceTransaction)
        {
            var response = new RsSearchCustomerResult();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var customer = uow.Context.Customers.Where(x => x.CustomerID == rqBalanceTransaction.customerID).FirstOrDefault();
                    if (customer == null)
                    {
                        ThrowCustomException("Invalid customer");
                    }

                    var recentTransactions = uow.Context.BalanceTransactions.Where(x => x.CustomerID == customer.CustomerID).ToList();
                    List<BalanceTransaction> top5Transactions;
                    if (recentTransactions != null && recentTransactions.Count > 0)
                    {
                        top5Transactions = recentTransactions.OrderByDescending(x => BusinessHelper.GetLastUpdatedDateTime(x.CreatedDateTime, x.LastUpdatedDateTime)).Take(5).ToList();
                        foreach (var transaction in top5Transactions)
                        {
                            response.transactions.Add(new balanceTransaction().Map(transaction));
                        }
                    }
                    response.currentBalanceAmount = customer.NetOutstandingBalance;
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Records fetched succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching records failed", exception);
            }

            return response;
        }

        public RsProducts GetProducts()
        {
            var response = new RsProducts();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var products = uow.Context.Products.ToList();
                    products.ForEach(x => response.products.Add(new product().Map(x)));
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Records fetched succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching records failed", exception);
            }
            return response;
        }

        public RsSubProducts GetSubProducts(long productID, string userlogin)
        {
            var response = new RsSubProducts();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var subproducts = uow.Context.SubProducts.Include("Product").Where(x => x.ProductID == productID && x.isActive==true).ToList();
                    subproducts.ForEach(x => response.subProducts.Add(new subProduct().Map(x)));
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Records fetched succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching records failed", exception);
            }
            return response;
        }

        public RsSubproduct GetSubProductDetails(long subProductID, string userlogin)
        {
            var response = new RsSubproduct();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var subproduct = uow.Context.SubProducts.Include("Product").Where(x => x.SubProductID == subProductID && x.isActive==true).FirstOrDefault();
                    if (subproduct == null)
                    {
                        ThrowCustomException("Invalid sub product");
                    }
                    response.subProduct = new subProduct().Map(subproduct);
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Sub product details fetched";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching details failed", exception);
            }
            return response;
        }

        public RsCommonData GetCommonData()
        {
            var response = new RsCommonData();
            try
            {
                response.contactTypes = LookupContext.ContactTypes.Select(x => new lookup() { id = x.ContactTypeID, value = x.ContactTypeName }).ToList();
                response.notificationTypes = LookupContext.NotificationTypes.Select(x => new lookup() { id = x.NotificationTypeID, value = x.NotificationTypeName }).ToList();
                response.transactionTypes = LookupContext.TransactionTypes.Select(x => new lookup() { id = x.TransactionTypeID, value = x.Description }).ToList();
                response.chequeStatusTypes = LookupContext.ChequeStatuses.Select(x => new lookup() { id = x.ChequeStatusID, value = x.StatusName }).ToList();
                response.warehouses = LookupContext.Warehouses.Select(x => new lookup() { id = x.WarehouseID, value = x.WarehouseName }).ToList();
                using (UnitOfWork uow = new UnitOfWork())
                {
                    response.suppliers=uow.Context.Suppliers.Where(x=>x.DisplayInDropdown).Any()? uow.Context.Suppliers.Where(x => x.DisplayInDropdown).Select(x => new lookup() { id = x.SupplierID, value = x.Name }).ToList():new List<lookup>();
                }
                    //response.suppliers = uow.Suppliers.Select(x => new lookup() { id = x.SupplierID, value = x.Name }).ToList();
                response.lineSaleSuppliers = LookupContext.LineSaleSuppliers.Select(x => new lookup() { id = x.LineSaleSupplierID, value = x.Name }).ToList();
                response.ResponseStatus = ResponseStatus.Success;
                response.ResponseMessage = "Fetched records succesfully";
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching records failed", exception);
            }
            return response;
        }

        public RsProductTransaction SaveProductTransaction(RqProductTransact rqProductTransaction)
        {
            var response = new RsProductTransaction();
            try
            {
                if (rqProductTransaction.productTransaction == null)
                {
                    ThrowCustomException("Please input transaction details");
                }

                using (UnitOfWork uow = new UnitOfWork())
                {
                    if (!rqProductTransaction.productTransaction.productTransactionID.HasValue || rqProductTransaction.productTransaction.productTransactionID.Value <= 0)
                    {
                        var subProduct = uow.Context.SubProducts.Include("Product").Where(x => x.SubProductID == rqProductTransaction.productTransaction.subProductID && x.isActive==true).FirstOrDefault();
                        if (subProduct == null)
                        {
                            ThrowCustomException("Invalid product");
                        }
                        var stockCount = subProduct.QuantityAvailable;
                        if (rqProductTransaction.productTransaction.isSellFromWarehouse && rqProductTransaction.productTransaction.sellQuantity > stockCount)
                        {
                            ThrowCustomException("There is no enough stock available");
                        }
                        var dbRecord = new ProductTransaction();
                        try
                        {
                            uow.BeginTransaction();
                            if (rqProductTransaction.productTransaction.balanceTransaction != null)
                            {
                                var customerRecord = uow.Context.Customers.Where(x => x.CustomerID == rqProductTransaction.productTransaction.balanceTransaction.customerID).FirstOrDefault();
                                if (customerRecord == null)
                                {
                                    ThrowCustomException("Invalid customer");
                                }
                                var dbBalanceRecord = new BalanceTransaction();
                                dbBalanceRecord.CreatedBy = rqProductTransaction.UserLogin;
                                dbBalanceRecord.Map(rqProductTransaction.productTransaction.balanceTransaction, false);
                                //if (rqProductTransaction.productTransaction.balanceTransaction.transactionTypeID == (int)TransactionTypeEnum.Cheque)
                                //{
                                //    dbBalanceRecord.OutgoingAmount = customerRecord.NetOutstandingBalance;
                                //}
                                //else
                                //{
                                customerRecord.NetOutstandingBalance = customerRecord.NetOutstandingBalance + dbBalanceRecord.OutgoingAmount - dbBalanceRecord.IncomingAmount;
                                dbBalanceRecord.OutstandingBalance = customerRecord.NetOutstandingBalance;
                                customerRecord.LastUpdatedBy = rqProductTransaction.UserLogin;
                                customerRecord.LastUpdatedDateTime = DateTime.Now;
                                //var subproduct=uow.Context.SubProducts.Include("Product").Where(x => x.SubProductID == rqProductTransaction.productTransaction.subProductID).FirstOrDefault();
                                if (subProduct != null)
                                {
                                    dbBalanceRecord.Description = "Item: " + subProduct.SubProductName + " " + subProduct.Product.ProductName + ", Quantity: " + rqProductTransaction.productTransaction.sellQuantity.ToString() + "."
                                                                   + (!string.IsNullOrWhiteSpace(rqProductTransaction.productTransaction.description) ? Environment.NewLine + rqProductTransaction.productTransaction.description : string.Empty);
                                }

                                dbBalanceRecord.CustomerID = customerRecord.CustomerID;

                                //}
                                uow.Context.BalanceTransactions.Add(dbBalanceRecord);
                                uow.Commit();
                                dbRecord.BalanceTransactionID = dbBalanceRecord.BalanceTransactionID;
                                dbRecord.BalanceTransaction = dbBalanceRecord;
                            }


                            dbRecord.CreatedBy = rqProductTransaction.UserLogin;
                            dbRecord.Map(rqProductTransaction.productTransaction, false);
                            dbRecord.QuantityRemaining = stockCount;
                            if (rqProductTransaction.productTransaction.isSellFromWarehouse)
                            {
                                dbRecord.QuantityRemaining = stockCount - rqProductTransaction.productTransaction.sellQuantity;
                                subProduct.LastUpdatedBy = rqProductTransaction.UserLogin;
                                subProduct.LastUpdatedDatetime = DateTime.Now;
                            }
                            else
                            {
                                if(rqProductTransaction.productTransaction.supplierID!=1)
                                {
                                    var supplier=uow.Context.Suppliers.Where(x => x.SupplierID == rqProductTransaction.productTransaction.supplierID).FirstOrDefault();
                                    if(supplier==null)
                                    {
                                        ThrowCustomException("Invalid supplier");
                                    }
                                    supplier.BalanceAmount = supplier.BalanceAmount + rqProductTransaction.productTransaction.buyingAmount;
                                    supplier.LastUpdatedBy = rqProductTransaction.UserLogin;
                                    supplier.LastUpdatedDatetime = DateTime.Now;
                                }
                            }
                            dbRecord.QuantityRemaining += rqProductTransaction.productTransaction.buyQuantity;
                            subProduct.QuantityAvailable = dbRecord.QuantityRemaining;
                            uow.Context.ProductTransactions.Add(dbRecord);
                            uow.Commit();
                            uow.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            uow.RollbackTransaction();
                            throw ex;
                        }
                        try
                        {
                            response.transaction.Map(dbRecord);
                        }
                        catch (Exception ex)
                        {
                            ThrowWarningException("Transaction saved successfully but fetch failed");
                        }

                    }
                    else
                    {
                        var dbRecord = uow.Context.ProductTransactions.Include("SubProduct").Include("BalanceTransaction").Include("BalanceTransaction.Customer").Where(x => x.ProductTransactionID == rqProductTransaction.productTransaction.productTransactionID).FirstOrDefault();
                        if (dbRecord == null)
                        {
                            ThrowCustomException("Invalid transaction");
                        }
                        //if (BusinessHelper.GetLastUpdatedDateTime(dbRecord.CreatedDateTime, dbRecord.LastUpdatedDateTime).AddDays(3) < DateTime.Now)
                        //{
                        //    ThrowCustomException("You cannot edit this transaction after 3 days");
                        //}

                        dbRecord.LastUpdatedBy = rqProductTransaction.UserLogin;
                        var oldStockCount = dbRecord.QuantityRemaining;
                        var oldIsWarehouseSell = dbRecord.IsSellFromWarehouse;
                        var oldBuy = dbRecord.BuyQuantity;
                        var oldSell = dbRecord.SellQuantity;
                        dbRecord.Map(rqProductTransaction.productTransaction, true);
                        if (oldIsWarehouseSell && rqProductTransaction.productTransaction.isSellFromWarehouse)
                        {
                            dbRecord.QuantityRemaining = oldStockCount - oldSell + rqProductTransaction.productTransaction.sellQuantity;
                        }
                        else if (oldIsWarehouseSell && !rqProductTransaction.productTransaction.isSellFromWarehouse)
                        {
                            dbRecord.QuantityRemaining = oldStockCount - oldSell + rqProductTransaction.productTransaction.buyQuantity;
                        }
                        else if (!oldIsWarehouseSell && rqProductTransaction.productTransaction.isSellFromWarehouse)
                        {
                            dbRecord.QuantityRemaining = oldStockCount - oldBuy + rqProductTransaction.productTransaction.buyQuantity - rqProductTransaction.productTransaction.sellQuantity;
                        }
                        else
                        {
                            dbRecord.QuantityRemaining = oldStockCount - oldBuy + rqProductTransaction.productTransaction.buyQuantity;
                        }
                        dbRecord.SubProduct.QuantityAvailable = dbRecord.QuantityRemaining;

                        uow.Commit();

                        try
                        {
                            response.transaction.Map(dbRecord);
                        }
                        catch (Exception ex)
                        {
                            ThrowWarningException("Transaction saved succesfully but fetch failed");
                        }
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Transaction saved succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Save transaction failed", exception);
            }
            return response;
        }

        public RsCommon DeleteProductTransaction(RqDeleteTransaction rqDeleteTransaction)
        {
            var response = new RsCommon();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var record = uow.Context.ProductTransactions.Where(x => x.ProductTransactionID == rqDeleteTransaction.transactionID).FirstOrDefault();
                    if (record == null)
                    {
                        ThrowCustomException("Invalid transaction");
                    }
                    //if (BusinessHelper.GetLastUpdatedDateTime(record.CreatedDateTime, record.LastUpdatedDateTime).AddDays(3) < DateTime.Now)
                    //{
                    //    ThrowCustomException("You cannot delete this transaction after 3 days");
                    //}
                    //var subproductRecords = uow.Context.ProductTransactions.Where(x => x.SubProductID == record.SubProductID).Select(x => x.SubProductID).ToList();
                    //foreach (var id in subproductRecords)
                    //{
                    //    if (id > rqDeleteTransaction.transactionID)
                    //    {
                    //        ThrowCustomException("This transaction is not latest transaction on this product");
                    //    }
                    //}
                    Supplier supplier = null;
                    if (record.SupplierID != 1)
                    {
                        supplier = uow.Context.Suppliers.Where(x => x.SupplierID == record.SupplierID).FirstOrDefault();
                    }
                    var subproduct = uow.Context.SubProducts.Where(x => x.SubProductID == record.SubProductID && x.isActive==true).FirstOrDefault();
                    if(record.SellQuantity>0 && record.IsSellFromWarehouse)                 
                    {
                        subproduct.QuantityAvailable = subproduct.QuantityAvailable + record.SellQuantity;
                        subproduct.LastUpdatedDatetime = DateTime.Now;
                        subproduct.LastUpdatedBy = rqDeleteTransaction.UserLogin;
                    }
                    else if(record.SellQuantity > 0 && !record.IsSellFromWarehouse)
                    {
                        if(supplier!=null)
                        {
                            supplier.BalanceAmount = supplier.BalanceAmount - record.BuyingAmount;
                            supplier.LastUpdatedBy = rqDeleteTransaction.UserLogin;
                            supplier.LastUpdatedDatetime = DateTime.Now;
                        }
                    }
                    else if(record.BuyQuantity>0)
                    {
                        subproduct.QuantityAvailable = subproduct.QuantityAvailable - record.BuyQuantity;
                        if (supplier != null)
                        {
                            supplier.BalanceAmount = supplier.BalanceAmount - record.BuyingAmount;
                            supplier.LastUpdatedBy = rqDeleteTransaction.UserLogin;
                            supplier.LastUpdatedDatetime = DateTime.Now;
                        }
                    }
                    uow.Context.ProductTransactions.Remove(record);
                    uow.Commit();

                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Delete transaction succesful";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Delete transaction failed", exception);
            }
            return response;
        }

        public RsProductTransaction GetProductTransactionDetail(long transactionID, string userlogin)
        {
            var response = new RsProductTransaction();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var record = uow.Context.ProductTransactions.Include("BalanceTransaction").Include("BalanceTransaction.Customer").Include("SubProduct").Include("SubProduct.Product").Where(x => x.ProductTransactionID == transactionID).FirstOrDefault();
                    if (record == null)
                    {
                        ThrowCustomException("Invalid transaction");
                    }
                    response.transaction.Map(record);
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched details succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching details failed", exception);
            }
            return response;
        }

        public RsNotifications GetNotifications()
        {
            var response = new RsNotifications();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    GetNotifications(uow, response);
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched records succesfully";

                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching records failed", exception);
            }
            return response;
        }

        private void GetNotifications(UnitOfWork uow, RsNotifications response)
        {
            var balanceTransactions = uow.Context.BalanceTransactions.Include("Customer").Where(x => x.TransactionTypeID == (int)TransactionTypeEnum.Cheque
                                           && (x.ChequeStatusID == (int)ChequeStatusEnum.NotInitiated || x.ChequeStatusID == (int)ChequeStatusEnum.Pending ||
                                           x.ChequeStatusID == (int)ChequeStatusEnum.Rejected)).ToList();
            foreach (var transaction in balanceTransactions)
            {
                var chequeAlert = new chequeAlert();
                chequeAlert.balanceTransactionID = transaction.BalanceTransactionID;
                chequeAlert.chequeActionDate = transaction.ChequeActionDate.ToString();
                chequeAlert.chequeDate = transaction.ChequeDate.ToString();
                chequeAlert.chequeDepositDate = transaction.ChequeDepositedDate.ToString();
                chequeAlert.chequeFailureComments = transaction.ChequeFailureComments;
                chequeAlert.chequeIssuerBank = transaction.ChequeIssuerBank;
                chequeAlert.chequeNumber = transaction.ChequeNumber;
                chequeAlert.chequeStatusID = transaction.ChequeStatusID.Value;
                chequeAlert.comments = transaction.Description;
                chequeAlert.chequeCustomerName = transaction.ChequeCustomerName;
                if (transaction.Customer != null)
                {
                    chequeAlert.customerID = transaction.Customer.CustomerID;
                    chequeAlert.customerFirstName = transaction.Customer.CustomerFirstName;
                    chequeAlert.customerLastName = transaction.Customer.CustomerLastName;
                    chequeAlert.customerKeyword = transaction.Customer.Keyword;
                }
                chequeAlert.isHighPriority = IsHighPriorityChequeAlert(transaction);
                chequeAlert.chequeAmount = transaction.IncomingAmount;
                response.chequeAlerts.Add(chequeAlert);
                
            }
            response.chequeAlerts = response.chequeAlerts.OrderByDescending(x => x.isHighPriority).ThenBy(x => Convert.ToDateTime(x.chequeDate)).ToList();

            var customers = uow.Context.Customers.Include("CustomerContacts").Where(x => x.NetOutstandingBalance >= x.MaxLimitAllowed || (x.MaxWaitingDays.HasValue && x.NetOutstandingBalance>0)).ToList();
            foreach (var customer in customers)
            {
                var latestTransaction = uow.Context.BalanceTransactions.Where(x => x.CustomerID == customer.CustomerID)?.OrderByDescending(x => x.TransactionDate).FirstOrDefault();
                if (customer.NetOutstandingBalance >= customer.MaxLimitAllowed ||
                    (latestTransaction != null?latestTransaction.TransactionDate.AddDays(customer.MaxWaitingDays.Value)<DateTime.Now:customer.CreatedDateTime.Value.AddDays(customer.MaxWaitingDays.Value)<DateTime.Now))
                {
                    var customerAlert = new customerAlert();
                    if (latestTransaction == null)
                    {
                        customerAlert.lastTransactionDate = null;
                    }
                    else
                    {
                        customerAlert.lastTransactionDate = latestTransaction.TransactionDate.ToString(); ;
                    }
                    customerAlert.customerFirstName = customer.CustomerFirstName;
                    customerAlert.customerLastName = customer.CustomerLastName;
                    customerAlert.customerID = customer.CustomerID;
                    customerAlert.keyword = customer.Keyword;
                    customerAlert.maxLimitAmount = customer.MaxLimitAllowed;
                    customerAlert.netOutstandingBalance = customer.NetOutstandingBalance;
                    customerAlert.maxWaitingDays = customer.MaxWaitingDays;

                    if (customer.CustomerContacts != null && customer.CustomerContacts.Count > 0)
                    {
                        var dbPrimaryContact = customer.CustomerContacts.FirstOrDefault(x => x.IsPrimaryContact == true);
                        if (dbPrimaryContact != null)
                        {
                            customerAlert.primaryContact = new customerContact().Map(dbPrimaryContact);
                        }
                        var dbSecondaryContact = customer.CustomerContacts.FirstOrDefault(x => x.IsPrimaryContact == false);
                        if (dbSecondaryContact != null)
                        {
                            customerAlert.secondaryContact = new customerContact().Map(dbSecondaryContact);
                        }
                    }
                    response.customerAlerts.Add(customerAlert);
                }
            }

            var subproducts = uow.Context.SubProducts.Include("Product").Where(x => x.QuantityAvailable < x.MinAlertQuantity && x.isActive==true).ToList();

            foreach (var product in subproducts)
            {
                var subproductAlert = new subProductAlert();
                subproductAlert.subProductID = product.SubProductID;
                subproductAlert.subProductName = product.SubProductName;
                subproductAlert.quantityAvailable = product.QuantityAvailable;
                subproductAlert.minAlertQuantity = product.MinAlertQuantity;
                if (product.Product != null)
                {
                    subproductAlert.productID = product.Product.ProductID;
                    subproductAlert.productName = product.Product.ProductName;
                }
                var lastTransacton = uow.Context.ProductTransactions.Where(x => x.SubProductID == product.SubProductID)?.OrderByDescending(x => x.TransactionDate).FirstOrDefault();
                if(lastTransacton!=null)
                {
                    subproductAlert.lastTransactionDate = lastTransacton.TransactionDate.ToString();
                }
                else
                {
                    subproductAlert.lastTransactionDate = null; 
                }
                response.subProductAlerts.Add(subproductAlert);
            }
        }
        private bool IsHighPriorityChequeAlert(BalanceTransaction transaction)
        {
            bool result = false;
            var chequeStatus = (ChequeStatusEnum)transaction.ChequeStatusID;
            var chequeDate = transaction.ChequeDate;
            var chequeDepositedDate = transaction.ChequeDepositedDate;
            if (chequeStatus == ChequeStatusEnum.NotInitiated && (chequeDate.HasValue && chequeDate.Value <= DateTime.Now))
            {
                result = true;
            }
            else if (chequeStatus == ChequeStatusEnum.Pending && (chequeDepositedDate.HasValue && chequeDepositedDate.Value.AddDays(2) <= DateTime.Now))
            {
                result = true;
            }
            else if (chequeStatus == ChequeStatusEnum.Rejected)
            {
                result = true;
            }

            return result;
        }

        public RsCommon ChangeChequeStatus(RqChangeChequeStatus rqTransaction)
        {
            var response = new RsCommon();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var dbRecord = uow.Context.BalanceTransactions.Where(x => x.BalanceTransactionID == rqTransaction.balanceTransactionID && x.TransactionTypeID == (int)TransactionTypeEnum.Cheque).FirstOrDefault();
                    if (dbRecord == null)
                    {
                        ThrowCustomException("Invalid transaction");
                    }
                    var dbCustomerRecord = uow.Context.Customers.Where(x => x.CustomerID == dbRecord.CustomerID).FirstOrDefault();
                    if (dbCustomerRecord == null)
                    {
                        ThrowCustomException("No customer is associated in this transaction");
                    }
                    if (dbRecord.ChequeStatusID.Value == (int)ChequeStatusEnum.NotInitiated)
                    {
                        DateTime depositedDate;
                        if (!(DateTime.TryParse(rqTransaction.chequeDepositedDate, out depositedDate) && rqTransaction.chequeStatusID == (int)ChequeStatusEnum.Pending))
                        {
                            ThrowCustomException("Please provide proper inputs");
                        }
                        dbRecord.ChequeDepositedDate = new Nullable<DateTime>(depositedDate);
                        dbRecord.ChequeStatusID = rqTransaction.chequeStatusID;
                        dbRecord.LastUpdatedBy = rqTransaction.UserLogin;
                        dbRecord.LastUpdatedDateTime = DateTime.Now;

                        uow.Commit();
                    }
                    else if (dbRecord.ChequeStatusID.Value == (int)ChequeStatusEnum.Pending)
                    {
                        DateTime actionDate;
                        if (!((DateTime.TryParse(rqTransaction.chequeActionDate, out actionDate)) &&
                              (rqTransaction.chequeStatusID == (int)ChequeStatusEnum.Passed || rqTransaction.chequeStatusID == (int)ChequeStatusEnum.Rejected)))
                        {
                            ThrowCustomException("Please provide proper inputs");
                        }
                        if (rqTransaction.chequeStatusID == (int)ChequeStatusEnum.Passed)
                        {
                            dbRecord.ChequeActionDate = new Nullable<DateTime>(actionDate);
                            dbRecord.ChequeStatusID = rqTransaction.chequeStatusID;
                            dbRecord.LastUpdatedBy = rqTransaction.UserLogin;
                            dbRecord.LastUpdatedDateTime = DateTime.Now;
                            dbRecord.OutstandingBalance = dbCustomerRecord.NetOutstandingBalance - dbRecord.IncomingAmount;
                            dbCustomerRecord.NetOutstandingBalance = dbRecord.OutstandingBalance;
                            dbCustomerRecord.LastUpdatedBy = rqTransaction.UserLogin;
                            dbCustomerRecord.LastUpdatedDateTime = DateTime.Now;
                        }
                        else if (rqTransaction.chequeStatusID == (int)ChequeStatusEnum.Rejected)
                        {
                            if (string.IsNullOrWhiteSpace(rqTransaction.chequeFailureComments))
                            {
                                ThrowCustomException("Please provide failure comments");
                            }
                            dbRecord.ChequeFailureComments = rqTransaction.chequeFailureComments;
                            dbRecord.ChequeActionDate = new Nullable<DateTime>(actionDate);
                            dbRecord.ChequeStatusID = rqTransaction.chequeStatusID;
                            dbRecord.LastUpdatedBy = rqTransaction.UserLogin;
                            dbRecord.LastUpdatedDateTime = DateTime.Now;
                        }
                        uow.Commit();
                    }
                    else if (dbRecord.ChequeStatusID.Value == (int)ChequeStatusEnum.Rejected)
                    {
                        if (!(rqTransaction.chequeStatusID == (int)ChequeStatusEnum.Passed || rqTransaction.chequeStatusID == (int)ChequeStatusEnum.Closed))
                        {
                            ThrowCustomException("Please provide proper inputs");
                        }
                        if (rqTransaction.chequeStatusID == (int)ChequeStatusEnum.Passed)
                        {
                            DateTime actionDate;
                            if (!DateTime.TryParse(rqTransaction.chequeActionDate, out actionDate))
                            {
                                ThrowCustomException("Please input cheque passed date");
                            }
                            dbRecord.ChequeActionDate = new Nullable<DateTime>(actionDate);
                            dbRecord.ChequeStatusID = rqTransaction.chequeStatusID;
                            dbRecord.LastUpdatedBy = rqTransaction.UserLogin;
                            dbRecord.LastUpdatedDateTime = DateTime.Now;
                            dbRecord.OutstandingBalance = dbCustomerRecord.NetOutstandingBalance - dbRecord.IncomingAmount;
                            dbCustomerRecord.NetOutstandingBalance = dbRecord.OutstandingBalance;
                            dbCustomerRecord.LastUpdatedBy = rqTransaction.UserLogin;
                            dbCustomerRecord.LastUpdatedDateTime = DateTime.Now;
                        }
                        else if (rqTransaction.chequeStatusID == (int)ChequeStatusEnum.Closed)
                        {
                            dbRecord.ChequeStatusID = rqTransaction.chequeStatusID;
                            dbRecord.ChequeClosureComments = rqTransaction.chequeClosureComments;
                            dbRecord.LastUpdatedBy = rqTransaction.UserLogin;
                            dbRecord.LastUpdatedDateTime = DateTime.Now;
                        }
                        uow.Commit();
                    }
                    else
                    {
                        ThrowCustomException("Not able to save record due to the current cheque status");
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Record saved succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Unable to save record", exception);
            }
            return response;
        }

        public RsCommon AddLineSale(RqLineSale rqLineSale)
        {
            var response = new RsCommon();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var supplier = LookupContext.LineSaleSuppliers.Where(x => x.LineSaleSupplierID == rqLineSale.supplierID).FirstOrDefault();
                    if (supplier == null)
                    {
                        ThrowCustomException("Invalid supplier");
                    }
                    var lineSaleRecord = new LineSale();
                    lineSaleRecord.LineSaleSupplierID = supplier.LineSaleSupplierID;
                    lineSaleRecord.SupplierName = rqLineSale.supplierName;
                    DateTime itemsDeliveredDate;
                    if (DateTime.TryParse(rqLineSale.itemRecievedDate, out itemsDeliveredDate))
                    {
                        lineSaleRecord.ItemsDeliveredDate = itemsDeliveredDate;
                    }
                    else
                    {
                        lineSaleRecord.ItemsDeliveredDate = DateTime.Now;
                    }
                    lineSaleRecord.TotalAmount = rqLineSale.totalAmount;
                    lineSaleRecord.BalanceAmount = rqLineSale.totalAmount;
                    lineSaleRecord.Comments = rqLineSale.comments;
                    lineSaleRecord.CreatedBy = rqLineSale.UserLogin;
                    lineSaleRecord.CreatedDateTime = DateTime.Now;
                    try
                    {
                        uow.BeginTransaction();

                        if (rqLineSale.isTrackingRequired)
                        {
                            lineSaleRecord.IsTrackingRequired = true;
                            uow.Context.LineSales.Add(lineSaleRecord);
                            uow.Commit();

                            if (rqLineSale.amountPaid.HasValue && rqLineSale.amountPaid.Value > 0)
                            {
                                var trackingRecord = new LineSaleTracking();
                                trackingRecord.LineSaleID = lineSaleRecord.LineSaleID;
                                trackingRecord.AmountPaid = rqLineSale.amountPaid.Value;
                                trackingRecord.Comments = "This amount paid on items delivered date";
                                DateTime paymentDate;
                                if (DateTime.TryParse(rqLineSale.itemRecievedDate, out paymentDate))
                                {
                                    trackingRecord.PaymentDate = paymentDate;
                                }
                                else
                                {
                                    trackingRecord.PaymentDate = DateTime.Now;
                                }
                                trackingRecord.IsBalanceSettled = false;
                                trackingRecord.CreatedBy = rqLineSale.UserLogin;
                                trackingRecord.CreatedDateTime = DateTime.Now;

                                trackingRecord.BalanceAmount = lineSaleRecord.TotalAmount - trackingRecord.AmountPaid;
                                if (trackingRecord.BalanceAmount < 0)
                                {
                                    ThrowCustomException("You cannot pay more than amount pending");
                                }
                                if (trackingRecord.BalanceAmount == 0)
                                {
                                    trackingRecord.IsBalanceSettled = true;
                                }
                                lineSaleRecord.BalanceAmount = trackingRecord.BalanceAmount;
                                lineSaleRecord.LastUpdatedBy = rqLineSale.UserLogin;
                                lineSaleRecord.LastUpdatedDateTime = DateTime.Now;
                                //lineSaleRecord.LineSaleTrackings.Add(trackingRecord);
                                //trackingRecord.LineSale = lineSaleRecord;
                                uow.Context.LineSaleTrackings.Add(trackingRecord);
                                uow.Commit();
                            }
                        }
                        else
                        {
                            lineSaleRecord.IsTrackingRequired = false;
                            lineSaleRecord.BalanceAmount = 0;
                            uow.Context.LineSales.Add(lineSaleRecord);
                            uow.Commit();
                        }
                        uow.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        uow.RollbackTransaction();
                        throw ex;
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Record saved succesfully";
                }

            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Unable to save record", exception);
            }

            return response;
        }

        public RsCommon AddLineSaleTracking(RqLineSaleTracking rqLineSaleTracking)
        {
            var response = new RsCommon();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var lineSaleRecord = uow.Context.LineSales.Include("LineSaleTrackings").Where(x => x.LineSaleID == rqLineSaleTracking.lineSaleID).FirstOrDefault();
                    if (lineSaleRecord == null)
                    {
                        ThrowCustomException("Invalid line sale");
                    }
                    if (lineSaleRecord.IsTrackingRequired == false || lineSaleRecord.BalanceAmount <= 0)
                    {
                        ThrowCustomException("This line sale is closed");
                    }
                    if (lineSaleRecord.LineSaleTrackings != null && lineSaleRecord.LineSaleTrackings.Count > 0)
                    {
                        var latestTrackingRecord = lineSaleRecord.LineSaleTrackings.ToList().OrderByDescending(x => x.LineSaleTrackingID).FirstOrDefault();
                        if (latestTrackingRecord.IsBalanceSettled == true || latestTrackingRecord.BalanceAmount <= 0)
                        {
                            ThrowCustomException("This line sale is closed");
                        }

                        AddLineSaleTracking(uow, lineSaleRecord, rqLineSaleTracking);
                    }
                    else
                    {
                        AddLineSaleTracking(uow, lineSaleRecord, rqLineSaleTracking);
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Record saved succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Unable to save record", exception);
            }

            return response;
        }

        private void AddLineSaleTracking(UnitOfWork uow, LineSale lineSaleRecord, RqLineSaleTracking rqLineSaleTracking)
        {
            var trackingRecord = new LineSaleTracking();
            trackingRecord.LineSaleID = lineSaleRecord.LineSaleID;
            trackingRecord.AmountPaid = rqLineSaleTracking.amountPaid;
            trackingRecord.Comments = rqLineSaleTracking.comments;
            DateTime paymentDate;
            if (DateTime.TryParse(rqLineSaleTracking.paymentDate, out paymentDate))
            {
                trackingRecord.PaymentDate = paymentDate;
            }
            else
            {
                trackingRecord.PaymentDate = DateTime.Now;
            }
            trackingRecord.IsBalanceSettled = rqLineSaleTracking.isBalanceSettled;
            trackingRecord.CreatedBy = rqLineSaleTracking.UserLogin;
            trackingRecord.CreatedDateTime = DateTime.Now;

            trackingRecord.BalanceAmount = lineSaleRecord.BalanceAmount - trackingRecord.AmountPaid;
            if (trackingRecord.BalanceAmount < 0)
            {
                ThrowCustomException("You cannot pay more than amount pending");
            }
            if (trackingRecord.BalanceAmount == 0)
            {
                trackingRecord.IsBalanceSettled = true;
            }
            lineSaleRecord.BalanceAmount = trackingRecord.BalanceAmount;
            lineSaleRecord.LastUpdatedBy = rqLineSaleTracking.UserLogin;
            lineSaleRecord.LastUpdatedDateTime = DateTime.Now;
            lineSaleRecord.LineSaleTrackings.Add(trackingRecord);
            trackingRecord.LineSale = lineSaleRecord;
            uow.Context.LineSaleTrackings.Add(trackingRecord);
            uow.Commit();
        }

        public RsLineSale GetPendingLineSales(string userlogin)
        {
            var response = new RsLineSale();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var lineSales = uow.Context.LineSales.Include("LineSaleTrackings").Where(x => x.IsTrackingRequired == true && x.BalanceAmount > 0).ToList();
                    if (lineSales.Count > 0)
                    {
                        foreach (var linesale in lineSales.OrderByDescending(x => x.LineSaleID))
                        {
                            if (linesale.LineSaleTrackings != null && linesale.LineSaleTrackings.Count > 0)
                            {
                                var latestTrackingRecord = linesale.LineSaleTrackings.ToList().OrderByDescending(x => x.LineSaleTrackingID).FirstOrDefault();
                                if (latestTrackingRecord.IsBalanceSettled == false && latestTrackingRecord.BalanceAmount > 0)
                                {
                                    response.lineSales.Add(LineSaleMapper(linesale, new lineSale(), latestTrackingRecord));
                                }
                            }
                            else
                            {
                                response.lineSales.Add(LineSaleMapper(linesale, new lineSale(), null));

                            }
                            response.totalPendingAmount += linesale.BalanceAmount;
                        }
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched records succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching records failed", exception);
            }
            return response;
        }

        private lineSale LineSaleMapper(LineSale dbRecord, lineSale uiRecord, LineSaleTracking latestTrackingRecord)
        {
            uiRecord.lineSaleID = dbRecord.LineSaleID;
            uiRecord.supplierID = dbRecord.LineSaleSupplierID;
            uiRecord.balanceAmount = dbRecord.BalanceAmount;
            uiRecord.comments = dbRecord.Comments;
            uiRecord.itemsDeliveredDate = dbRecord.ItemsDeliveredDate.ToString();
            uiRecord.billedAmount = dbRecord.TotalAmount;
            if (latestTrackingRecord != null)
            {
                uiRecord.lastPaymentDate = latestTrackingRecord.PaymentDate.ToString();
            }
            else
            {
                uiRecord.lastPaymentDate = string.Empty;
            }
            uiRecord.supplierName = dbRecord.SupplierName;
            uiRecord.allowPayment = true;
            if (dbRecord.LineSaleTrackings != null && dbRecord.LineSaleTrackings.Count > 0)
            {
                var trackings = dbRecord.LineSaleTrackings.OrderByDescending(x => x.LineSaleTrackingID);
                foreach (var tracking in trackings)
                {
                    var track = new lineSaleTracking();
                    if (trackings.FirstOrDefault() == tracking)
                    {
                        track.isDeletable = true;
                    }
                    uiRecord.lineSaleTrackings.Add(LineSaleTrackingMapper(tracking, track));
                }
                uiRecord.hasTrackings = true;
            }
            else
            {
                uiRecord.hasTrackings = false;
            }
            return uiRecord;
        }

        private lineSaleTracking LineSaleTrackingMapper(LineSaleTracking dbRecord, lineSaleTracking uiRecord)
        {
            uiRecord.lineSaleTrackingID = dbRecord.LineSaleTrackingID;
            uiRecord.lineSaleID = dbRecord.LineSaleID;
            uiRecord.amountPaid = dbRecord.AmountPaid;
            uiRecord.balanceAmount = dbRecord.BalanceAmount;
            uiRecord.comments = dbRecord.Comments;
            uiRecord.paymentDate = dbRecord.PaymentDate.ToString();
            return uiRecord;
        }
        public RsLineSaleTrackings GetLineSaleTrackings(long lineSaleID, string userlogin)
        {
            var response = new RsLineSaleTrackings();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var lineSaleRecord = uow.Context.LineSales.Include("LineSaleTrackings").Where(x => x.LineSaleID == lineSaleID).FirstOrDefault();
                    if (lineSaleRecord == null)
                    {
                        ThrowCustomException("Invalid line sale");
                    }
                    if (lineSaleRecord.LineSaleTrackings != null && lineSaleRecord.LineSaleTrackings.Count > 0)
                    {
                        var trackings = lineSaleRecord.LineSaleTrackings.OrderByDescending(x => x.LineSaleTrackingID);
                        foreach (var tracking in trackings)
                        {
                            var track = new lineSaleTracking();
                            if(trackings.FirstOrDefault()==tracking)
                            {
                                track.isDeletable = true;
                            }
                            response.lineSaleTrackings.Add(LineSaleTrackingMapper(tracking, track));
                        }
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched records succesfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Fetching records failed", exception);
            }
            return response;
        }

        public RsSearchCustomerResult SearchBalanceTransaction(RqSearch rqSearch)
        {
            var response = new RsSearchCustomerResult();
            try
            {
                DateTime startDate = new DateTime();
                 DateTime endDate=new DateTime();
                if(!string.IsNullOrWhiteSpace(rqSearch.startDate))
                {
                    if(!DateTime.TryParse(rqSearch.startDate,out startDate))
                    {
                        ThrowCustomException("Invalid Start date");
                    }
                }
                if (!string.IsNullOrWhiteSpace(rqSearch.endDate))
                {
                    if (!DateTime.TryParse(rqSearch.endDate, out endDate))
                    {
                        ThrowCustomException("Invalid End date");
                    }
                }
                //if(rqSearch.ID==null && (string.IsNullOrWhiteSpace(rqSearch.startDate) || string.IsNullOrWhiteSpace(rqSearch.endDate)))
                //{
                //    ThrowCustomException("Please enter customer or select start and end date");
                //}
                using (UnitOfWork uow = new UnitOfWork())
                {
                    Customer customer = null; 
                    var fromRecord = ((rqSearch.pageIndex-1) * rqSearch.pageSize)+1;
                    var toRecord = rqSearch.pageSize * rqSearch.pageIndex;
                    Func<BalanceTransaction, bool> expression;
                    if(rqSearch.ID.HasValue)
                    {
                        customer = uow.Context.Customers.Where(x => x.CustomerID == rqSearch.ID).FirstOrDefault();
                        if (customer == null)
                        {
                            ThrowCustomException("Invalid Customer");
                        }
                        if (!string.IsNullOrWhiteSpace(rqSearch.startDate) && !string.IsNullOrWhiteSpace(rqSearch.endDate))
                        {
                            expression = x => x.CustomerID == rqSearch.ID && x.TransactionDate >= startDate && x.TransactionDate <= endDate;
                        }
                        else
                        {
                            expression = x => x.CustomerID == rqSearch.ID;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(rqSearch.startDate) && !string.IsNullOrWhiteSpace(rqSearch.endDate))
                        {
                            expression = x => x.TransactionDate >= startDate && x.TransactionDate <= endDate;
                        }
                        else
                            expression = x => true;
                    }
                    response.totalRecords = uow.Context.BalanceTransactions.Where(expression) != null ? uow.Context.BalanceTransactions.Where(expression).Count() : 0;
                    if(response.totalRecords!=0 && response.totalRecords<fromRecord)
                    {
                        ThrowCustomException("The page index exceeds the total records");
                    }
                    var transactions = uow.Context.BalanceTransactions.Include("Customer").Where(expression)?.AsQueryable()
                                      .OrderByDescending(x => BusinessHelper.GetLastUpdatedDateTime(x.CreatedDateTime, x.LastUpdatedDateTime))
                                      .Skip((rqSearch.pageIndex - 1) * rqSearch.pageSize).Take(rqSearch.pageSize).ToList();
                    transactions.ForEach(x => response.transactions.Add(new balanceTransaction().Map(x)));

                    AddEditableDeletableFlagsForBalanceTransactionSearch(uow, rqSearch, response);

                   if (customer!=null)
                    {
                        response.currentBalanceAmount = customer.NetOutstandingBalance;
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched records succesfully";

                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Searching transactions failed", exception);
            }
            return response;
        }

        private void AddEditableDeletableFlagsForBalanceTransactionSearch(UnitOfWork uow, RqSearch request,RsSearchCustomerResult response)
        {
            Dictionary<long, BalanceTransaction> dictionary = new Dictionary<long, BalanceTransaction>();
            foreach(var transaction in response.transactions)
            {
                if(transaction.transactionTypeID==(int)TransactionTypeEnum.Cheque)
                {
                    if(transaction.chequeStatusID == (int)ChequeStatusEnum.NotInitiated)
                        transaction.isDeletable = true;
                }
                else
                {
                    if(dictionary.ContainsKey(transaction.customerID))
                    {
                        if(dictionary[transaction.customerID].BalanceTransactionID==transaction.balanceTransactionID)
                        {
                            transaction.isDeletable = true;
                            transaction.isEditable = true;
                        }
                    }
                    else
                    {
                        var latestTransaction = uow.Context.BalanceTransactions.Where(x => x.CustomerID == transaction.customerID && x.TransactionTypeID != (int)TransactionTypeEnum.Cheque)?.OrderByDescending(x => x.LastUpdatedDateTime.HasValue?x.LastUpdatedDateTime:x.CreatedDateTime).FirstOrDefault();
                        if(latestTransaction!=null)
                            dictionary.Add(transaction.customerID, latestTransaction);

                        if (dictionary[transaction.customerID].BalanceTransactionID == transaction.balanceTransactionID)
                        {
                            transaction.isDeletable = true;
                            transaction.isEditable = true;
                        }
                    }
                  
                }
            }
        }

        public HttpResponseMessage ExportToExcel(RqSearch rqSearch)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK); 
            try
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                if (!string.IsNullOrWhiteSpace(rqSearch.startDate))
                {
                    if (!DateTime.TryParse(rqSearch.startDate, out startDate))
                    {
                        LogHelper.Log(new Exception("Invalid start date"));
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                if (!string.IsNullOrWhiteSpace(rqSearch.endDate))
                {
                    if (!DateTime.TryParse(rqSearch.endDate, out endDate))
                    {
                        LogHelper.Log(new Exception("Invalid end date"));
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                using (UnitOfWork uow = new UnitOfWork())
                {
                    Customer customer = null;
                    Func<BalanceTransaction, bool> expression;
                    if (rqSearch.ID.HasValue)
                    {
                        customer = uow.Context.Customers.Where(x => x.CustomerID == rqSearch.ID).FirstOrDefault();
                        if (customer == null)
                        {
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        if (!string.IsNullOrWhiteSpace(rqSearch.startDate) && !string.IsNullOrWhiteSpace(rqSearch.endDate))
                        {
                            expression = x => x.CustomerID == rqSearch.ID && x.TransactionDate >= startDate && x.TransactionDate <= endDate;
                        }
                        else
                        {
                            expression = x => x.CustomerID == rqSearch.ID;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(rqSearch.startDate) && !string.IsNullOrWhiteSpace(rqSearch.endDate))
                        {
                            expression = x => x.TransactionDate >= startDate && x.TransactionDate <= endDate;
                        }
                        else
                            expression = x => true;
                    }
                    var transactions = uow.Context.BalanceTransactions.Include("Customer").Where(expression)?.AsQueryable()
                                     .OrderByDescending(x => BusinessHelper.GetLastUpdatedDateTime(x.CreatedDateTime, x.LastUpdatedDateTime)).ToList();

                    response=CreateCustomerTransactionSpreadsheet(transactions, response);


                }
            }
            catch(Exception exception)
            {
                LogHelper.Log(exception);
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        private HttpResponseMessage CreateCustomerTransactionSpreadsheet(List<BalanceTransaction> transactions,HttpResponseMessage response)
        {
            var uploadedTime = DateTime.Now;
            string filePath = ConfigurationManager.AppSettings["DataTemplatesPath"];
            OleDbConnection connection;
            string tempFileName = string.Concat(filePath, string.Format("TempCustomerTransactions_{0:yyyy-MM-dd_hh-mm-ss-tt}.xlsx", uploadedTime)); ;//path+file
            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
            
            File.Copy(string.Concat(filePath, "CustomerTransactionsTemplate.xlsx"), tempFileName);


            var connectionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + tempFileName + "; Extended Properties=Excel 12.0;";
            connection = new OleDbConnection(connectionString);
            OleDbCommand command = new OleDbCommand("", connection);

            command.Connection = connection;

            if (connection.State != ConnectionState.Open)
                connection.Open();
            foreach (var transaction in transactions)
            {
                command.CommandText = string.Format("insert into [Sheet1$] ([CustomerName],[CustomerID],[TransactionID],[TransactionDate],[IncomingAmount],[OutgoingAmount],[TransactionType],[Comments],[NetBalance],[ChequeDate],[ChequeStatus]) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')"
                                       ,BusinessHelper.FormatName(transaction.Customer.CustomerFirstName,transaction.Customer.CustomerLastName,transaction.Customer.Keyword)
                                       ,transaction.CustomerID.ToString()
                                       , transaction.BalanceTransactionID.ToString()
                                       , transaction.TransactionDate.ToString("dd-MM-yyyy")
                                       ,transaction.IncomingAmount.ToString()
                                       , transaction.OutgoingAmount.ToString()
                                       , ((TransactionTypeEnum)transaction.TransactionTypeID).ToString()
                                       ,transaction.Description
                                       ,transaction.OutstandingBalance.ToString()
                                       , transaction.ChequeDate.HasValue? transaction.ChequeDate.Value.ToString("dd-MM-yyyy"):string.Empty
                                       , transaction.ChequeStatusID.HasValue?((ChequeStatusEnum)transaction.ChequeStatusID.Value).ToString():string.Empty);
                
                command.ExecuteNonQuery();
            }
            if (connection.State == ConnectionState.Open)
                connection.Close();


            byte[] bytes = File.ReadAllBytes(tempFileName);

            //Convert bytes to memory stream content and bind the response
            response.Content = new StreamContent(new MemoryStream(bytes));

            string filename =  "CustomerTransactions_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt") + ".xlsx";
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = filename;

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");


            //Delete the temporary excel file
            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }

            return response;
        }

        public RsCustomers SearchCustomers(RqSearchCustomers rqSearchCustomers)
        {
            var response = new RsCustomers();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    long result;
                    var fromRecord = ((rqSearchCustomers.pageIndex - 1) * rqSearchCustomers.pageSize) + 1;
                    var toRecord = rqSearchCustomers.pageSize * rqSearchCustomers.pageIndex;
                    var keyword = rqSearchCustomers.keyword;
                    int totalRecords = 0;
                    List<Customer> customers = new List<Customer>();
                    Func<Customer, bool> expression;
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        if (long.TryParse(keyword, out result))
                        {
                            var customerIds = uow.Context.CustomerContacts.Include("Customer").Where(x => x.ContactNumber.Contains(keyword))?.Select(x => x.CustomerID).ToList();
                            if (customerIds != null && customerIds.Count > 0)
                            {
                                expression = x => customerIds.Distinct().Contains(x.CustomerID);
                                //totalRecords = uow.Context.Customers.Include("CustomerContacts").Where(x => customerIds.Distinct().Contains(x.CustomerID)).Count();
                                
                                //customers = uow.Context.Customers.Include("CustomerContacts").Where(x => customerIds.Distinct().Contains(x.CustomerID))?
                                //    .OrderByDescending(x => x.LastUpdatedDateTime.HasValue ? x.LastUpdatedDateTime : x.CreatedDateTime)
                                //    .Skip((rqSearchCustomers.pageIndex - 1) * rqSearchCustomers.pageSize).Take(rqSearchCustomers.pageSize).ToList();
                            }
                            else
                            {
                                expression = x => false;
                            }

                        }
                        else
                        {
                            if (keyword.Contains(' '))
                            {
                                keyword = keyword.Split(' ').First();
                            }
                            keyword = keyword.ToLower();
                            expression = x => x.CustomerFirstName.ToLower().Contains(keyword) || (x.CustomerLastName != null && x.CustomerLastName.ToLower().Contains(keyword)) || (x.Keyword != null && x.Keyword.ToLower().Contains(keyword));
                            //customers = uow.Context.Customers.Include("CustomerContacts").Where(x => x.CustomerFirstName.ToLower().Contains(keyword) || (x.CustomerLastName != null && x.CustomerLastName.ToLower().Contains(keyword)) || (x.Keyword != null && x.Keyword.ToLower().Contains(keyword))).ToList();
                        }
                    }
                    else
                    {
                        expression = x => true;
                       

                    }
                    totalRecords = uow.Context.Customers.Where(expression) != null ? uow.Context.Customers.Where(expression).Count() : 0;
                    if (response.totalRecords != 0 && response.totalRecords < fromRecord)
                    {
                        ThrowCustomException("The page index exceeds the total records");
                    }
                    customers = uow.Context.Customers.Include("CustomerContacts").Where(expression)?.AsQueryable()
                           .OrderByDescending(x=>x.LastUpdatedDateTime.HasValue?x.LastUpdatedDateTime:x.CreatedDateTime)
                           .Skip((rqSearchCustomers.pageIndex - 1) * rqSearchCustomers.pageSize).Take(rqSearchCustomers.pageSize).ToList();
                    if (customers != null)
                    {
                        foreach (var customer in customers)
                        {
                            var latestTransaction = uow.Context.BalanceTransactions.Where(x => x.CustomerID == customer.CustomerID)?.OrderByDescending(x => x.TransactionDate).FirstOrDefault();
                            var cust = new customer();
                            if (latestTransaction != null)
                            {
                                cust.latestTransactionDate = latestTransaction.TransactionDate.ToString();
                            }
                            response.customers.Add(cust.Map(customer));
                           
                        }
                    }

                    response.totalRecords = totalRecords;

                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Search customers success";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Search customers failed", exception);
            }
            return response;
        }

        public async Task<RsSubProducts> GetAllProducts()
        {
            var response = new RsSubProducts();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var subproducts = uow.Context.SubProducts.Include("Product").Where(x=>x.isActive).ToList();
                    subproducts.ForEach(x => response.subProducts.Add(new subProduct().Map(x)));
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched records successfully";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Failed to fetch records", exception);
            }
            return await Task.FromResult(response);
        }

        public async Task<RsCustomers> GetTopBuyingCustomers()
        {
            var response = new RsCustomers();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var customerIDs = uow.Context.Customers.Select(x => x.CustomerID).ToList();
                    response.allCustomersBalance = uow.Context.Customers.Any() ? uow.Context.Customers.Sum(x => x.NetOutstandingBalance):0;
                    Dictionary <long, double> topCustomers = new Dictionary<long, double>();
                    foreach(var id in customerIDs)
                    {
                        var totalOutgoingAmount = uow.Context.BalanceTransactions.Where(x => x.CustomerID == id).Any() ? uow.Context.BalanceTransactions.Where(x => x.CustomerID == id).Sum(x => x.OutgoingAmount) : 0;
                        topCustomers.Add(id, totalOutgoingAmount);
                    }
                    var top10Customers = topCustomers.OrderByDescending(x => x.Value).Take(10).ToList();
                    foreach (var pair in top10Customers)
                    {
                        var customer = new customer();
                        customer.totalOutgoingAmount = pair.Value;
                        response.customers.Add(customer.Map(uow.Context.Customers.Where(x => x.CustomerID == pair.Key).FirstOrDefault()));                   
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched records succesfully";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Failed to fetch records", exception);
            }
            return await Task.FromResult(response);
        }

        public async Task<RsProductTransactions> SearchProductTransactions(RqProductTransactionsSearch rqProductTransactionsSearch)
        {
            var response = new RsProductTransactions();
            try
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                if (!string.IsNullOrWhiteSpace(rqProductTransactionsSearch.startDate))
                {
                    if (!DateTime.TryParse(rqProductTransactionsSearch.startDate, out startDate))
                    {
                        ThrowCustomException("Invalid Start date");
                    }
                }
                if (!string.IsNullOrWhiteSpace(rqProductTransactionsSearch.endDate))
                {
                    if (!DateTime.TryParse(rqProductTransactionsSearch.endDate, out endDate))
                    {
                        ThrowCustomException("Invalid End date");
                    }
                }
                using (UnitOfWork uow = new UnitOfWork())
                {
                    long result;
                    var fromRecord = ((rqProductTransactionsSearch.pageIndex - 1) * rqProductTransactionsSearch.pageSize) + 1;
                    var toRecord = rqProductTransactionsSearch.pageSize * rqProductTransactionsSearch.pageIndex;
                    var productID = rqProductTransactionsSearch.productID;
                    var subProductID = rqProductTransactionsSearch.subProductID;
                    int totalRecords = 0;
                    List<ProductTransaction> transactions = new List<ProductTransaction>();
                    Func<ProductTransaction, bool> expression;
                    if(productID.HasValue && productID.Value>0)
                    {
                        if(!string.IsNullOrWhiteSpace(rqProductTransactionsSearch.startDate) && !string.IsNullOrWhiteSpace(rqProductTransactionsSearch.endDate))
                        {
                            expression = x => subProductID.HasValue && subProductID.Value>0 ? x.SubProductID == subProductID.Value : x.SubProduct.Product.ProductID==productID.Value && x.TransactionDate >= startDate && x.TransactionDate <= endDate;
                        }
                        else
                        {
                            expression = x => subProductID.HasValue && subProductID.Value > 0 ? x.SubProductID == subProductID.Value : x.SubProduct.Product.ProductID == productID.Value;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(rqProductTransactionsSearch.startDate) && !string.IsNullOrWhiteSpace(rqProductTransactionsSearch.endDate))
                        {
                            expression = x => x.TransactionDate >= startDate && x.TransactionDate <= endDate;
                        }
                        else
                        {
                            expression = x => true;
                        }
                    }

                    response.totalRecords = uow.Context.ProductTransactions.Include("SubProduct").Include("SubProduct.Product").Where(expression) != null ? uow.Context.ProductTransactions.Include("SubProduct").Include("SubProduct.Product").Where(expression).Count():0;
                    if (response.totalRecords != 0 && response.totalRecords < fromRecord)
                    {
                        ThrowCustomException("The page index exceeds the total records");
                    }
                    transactions= uow.Context.ProductTransactions.Include("SubProduct").Include("SubProduct.Product").Where(expression)?.AsQueryable()
                        .OrderByDescending(x=>x.LastUpdatedDateTime.HasValue?x.LastUpdatedDateTime.Value:x.CreatedDateTime.Value)
                        .Skip((rqProductTransactionsSearch.pageIndex - 1) * rqProductTransactionsSearch.pageSize).Take(rqProductTransactionsSearch.pageSize).ToList();

                    if(transactions!=null)
                    {
                        foreach(var transaction in transactions)
                        {
                            var tran = new productTransaction();
                            if(transaction.BalanceTransactionID.HasValue)
                            {
                                var balanceTran= uow.Context.BalanceTransactions.Include("Customer").Where(x => x.BalanceTransactionID == transaction.BalanceTransactionID.Value).FirstOrDefault();
                                if (balanceTran != null)
                                    tran.customerName = BusinessHelper.FormatName(balanceTran.Customer.CustomerFirstName, balanceTran.Customer.CustomerLastName, balanceTran.Customer.Keyword);
                            }
                            
                            response.transactions.Add(tran.Map(transaction));
                        }
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched records succesfully";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Failed to fetch records", exception);
            }
            return await Task.FromResult(response);
        }

        private void AddEditDeleteFlagsToProductTransactions(UnitOfWork uow,List<productTransaction> transactions)
        {
            Dictionary<long, ProductTransaction> dictionary = new Dictionary<long, ProductTransaction>();
            foreach(var transaction in transactions)
            {
                if (dictionary.ContainsKey(transaction.subProductID))
                {
                    if (dictionary[transaction.subProductID].ProductTransactionID == transaction.productTransactionID)
                    {
                        transaction.isEditable = true;
                        transaction.isDeletable = true;
                    }
                }
                else
                {
                    var latestTransaction = uow.Context.ProductTransactions.Where(x => x.SubProductID == transaction.subProductID)?
                        .OrderByDescending(x => x.LastUpdatedDateTime.HasValue ? x.LastUpdatedDateTime.Value : x.CreatedDateTime.Value).FirstOrDefault();
                    if (latestTransaction != null)
                        dictionary.Add(transaction.subProductID, latestTransaction);
                    if (dictionary[transaction.subProductID].ProductTransactionID == transaction.productTransactionID)
                    {
                        transaction.isEditable = true;
                        transaction.isDeletable = true;
                    }
                }
            }
            
        }
        public async Task<RsCommon> DeleteLineSaleTracking(RqDeleteTransaction request)
        {
            var response = new RsCommon();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var tracking = uow.Context.LineSaleTrackings.Where(x => x.LineSaleTrackingID == request.transactionID).FirstOrDefault();
                    if(tracking==null)
                    {
                        ThrowCustomException("Invalid transaction ID");
                    }
                    var latestTracking = uow.Context.LineSaleTrackings.Where(x => x.LineSaleID == tracking.LineSaleID)?.OrderByDescending(x => x.CreatedDateTime).FirstOrDefault();
                    if(latestTracking!=null && latestTracking.LineSaleTrackingID!=tracking.LineSaleTrackingID)
                    {
                        ThrowCustomException("This transaction is not latest for this line sale");
                    }
                    var linesale = uow.Context.LineSales.Where(x => x.LineSaleID == tracking.LineSaleID).FirstOrDefault();
                    linesale.LastUpdatedDateTime = DateTime.Now;
                    linesale.LastUpdatedBy = request.UserLogin;
                    linesale.BalanceAmount = linesale.BalanceAmount + tracking.AmountPaid;
                    uow.Context.LineSaleTrackings.Remove(tracking);
                    uow.Commit();
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Delete transaction successful";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Delete transaction failed", exception);
            }
            return await Task.FromResult(response);
        }

        public async Task<RsCommon> DeleteLineSale(RqDeleteTransaction request)
        {
            var response = new RsCommon();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var linesale = uow.Context.LineSales.Where(x => x.LineSaleID == request.transactionID).FirstOrDefault();
                    if(linesale==null)
                    {
                        ThrowCustomException("Invalid transaction");
                    }
                    var linesaletrackings = uow.Context.LineSaleTrackings.Where(x => x.LineSaleID == request.transactionID).ToList();
                    linesaletrackings.ForEach(x => uow.Context.LineSaleTrackings.Remove(x));
                    uow.Context.LineSales.Remove(linesale);
                    uow.Commit();
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Delete transaction successful";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response,"Delete transaction failed", exception);
            }
            return await Task.FromResult(response);
        }

        public HttpResponseMessage ExportToExcel(RqProductTransactionsSearch rqProductTransactionsSearch)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();
                if (!string.IsNullOrWhiteSpace(rqProductTransactionsSearch.startDate))
                {
                    if (!DateTime.TryParse(rqProductTransactionsSearch.startDate, out startDate))
                    {
                        LogHelper.Log(new Exception("Invalid start date"));
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                if (!string.IsNullOrWhiteSpace(rqProductTransactionsSearch.endDate))
                {
                    if (!DateTime.TryParse(rqProductTransactionsSearch.endDate, out endDate))
                    {
                        LogHelper.Log(new Exception("Invalid end date"));
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                using (UnitOfWork uow = new UnitOfWork())
                {
                    long result;
                   
                    var productID = rqProductTransactionsSearch.productID;
                    var subProductID = rqProductTransactionsSearch.subProductID;
                    
                    List<ProductTransaction> transactions = new List<ProductTransaction>();
                    Func<ProductTransaction, bool> expression;
                    if (productID.HasValue)
                    {
                        if (!string.IsNullOrWhiteSpace(rqProductTransactionsSearch.startDate) && !string.IsNullOrWhiteSpace(rqProductTransactionsSearch.endDate))
                        {
                            expression = x => subProductID.HasValue ? x.SubProductID == subProductID.Value : x.SubProduct.Product.ProductID == productID.Value && x.TransactionDate >= startDate && x.TransactionDate <= endDate;
                        }
                        else
                        {
                            expression = x => subProductID.HasValue ? x.SubProductID == subProductID.Value : x.SubProduct.Product.ProductID == productID.Value;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(rqProductTransactionsSearch.startDate) && !string.IsNullOrWhiteSpace(rqProductTransactionsSearch.endDate))
                        {
                            expression = x => x.TransactionDate >= startDate && x.TransactionDate <= endDate;
                        }
                        else
                        {
                            expression = x => true;
                        }
                    }
                    transactions = uow.Context.ProductTransactions.Include("SubProduct").Include("SubProduct.Product").Include("Supplier").Include("Warehouse").Where(expression)?.AsQueryable()
                                     .OrderByDescending(x => BusinessHelper.GetLastUpdatedDateTime(x.CreatedDateTime, x.LastUpdatedDateTime)).ToList();

                    response = CreateProductTransactionSpreadsheet(uow,transactions, response);


                }
            }
            catch (Exception exception)
            {
                LogHelper.Log(exception);
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }




        private HttpResponseMessage CreateProductTransactionSpreadsheet(UnitOfWork uow, List<ProductTransaction> transactions, HttpResponseMessage response)
        {
            var uploadedTime = DateTime.Now;
            
            string filePath =ConfigurationManager.AppSettings["DataTemplatesPath"];
            OleDbConnection connection;
            string tempFileName = string.Concat(filePath, string.Format("TempProductTransactions_{0:yyyy-MM-dd_hh-mm-ss-tt}.xlsx", uploadedTime)); //path+file
            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
            
            File.Copy(string.Concat(filePath, "ProductTransactionsTemplate.xlsx"), tempFileName);
           

            var connectionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + tempFileName + "; Extended Properties=Excel 12.0;";
            connection = new OleDbConnection(connectionString);
            OleDbCommand command = new OleDbCommand("", connection);

            command.Connection = connection;

            if (connection.State != ConnectionState.Open)
                connection.Open();
            foreach (var transaction in transactions)
            {
                string customerName = string.Empty;
                if(transaction.BalanceTransactionID.HasValue)
                {
                    var trans=uow.Context.BalanceTransactions.Include("Customer").Where(x => x.BalanceTransactionID == transaction.BalanceTransactionID.Value).FirstOrDefault();
                    customerName = BusinessHelper.FormatName(trans.Customer.CustomerFirstName, trans.Customer.CustomerLastName, trans.Customer.Keyword);
                }
                try
                {
                    command.CommandText = string.Format("insert into [Sheet1$] ([SubProductName],[ProductName],[TransactionID],[TransactionDate],[BuyQuantity],[SellQuantity],[SupplierName],[WareHouseName],[SellingPrice],[NetAmount],[TaxAmount],[OtherAmount],[GrossAmount],[Comments],[CustomerName]) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')"
                                           , transaction.SubProduct.SubProductName
                                           , transaction.SubProduct.Product.ProductName
                                           , transaction.ProductTransactionID
                                           , transaction.TransactionDate.ToString("dd-MM-yyyy")
                                           , transaction.BuyQuantity.ToString()
                                           , transaction.SellQuantity.ToString()
                                           , transaction.Supplier != null ? transaction.Supplier.Name : string.Empty
                                           , transaction.Warehouse != null ? transaction.Warehouse.WarehouseName : string.Empty
                                           , transaction.SellingPrice.ToString()
                                           , transaction.TotalPrice.ToString()
                                           , transaction.TaxPrice.ToString()
                                           , transaction.MiscellaneousPrice.ToString()
                                           , transaction.TotalPriceIncludingTax.ToString()
                                           , transaction.Description
                                           , customerName);
                    command.ExecuteNonQuery();
                }
                catch(Exception ex)
                {

                }
            }
            if (connection.State == ConnectionState.Open)
                connection.Close();


            byte[] bytes = File.ReadAllBytes(tempFileName);

            //Convert bytes to memory stream content and bind the response
            response.Content = new StreamContent(new MemoryStream(bytes));

            string filename = "ProductTransactions_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt") + ".xlsx";
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = filename;

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");


            //Delete the temporary excel file
            if (File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }

            return response;
        }

        public async Task<RsLinesalesReport> GetLinesalesReport(RqSearch request)
        {
            var response = new RsLinesalesReport();
            DateTime date = new DateTime();
            if(!DateTime.TryParse(request.startDate,out date))
            {
                ThrowCustomException("Invalid date");
            }
            date = date.Date;
            var tomorrowsDate = date.AddDays(1);
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var closedLinesales = uow.Context.LineSales.Where(x => x.BalanceAmount <= 0 && !x.IsTrackingRequired && x.CreatedDateTime.Value >= date && x.CreatedDateTime.Value<tomorrowsDate).ToList();
                    foreach(var linesale in closedLinesales)
                    {
                        var lnsl = new lineSale();
                        LineSaleMapper(linesale, lnsl, null);
                        //if(date.Date==DateTime.Now.Date)
                        //{
                        //    lnsl.isDeletable = true;
                        //}
                        response.closedLineSales.Add(lnsl);
                    }
                    response.totalClosedAmount = closedLinesales.Sum(x => x.TotalAmount);
                    var pendingLinesales = uow.Context.LineSales.Where(x => x.BalanceAmount > 0 && x.CreatedDateTime.Value >= date && x.CreatedDateTime.Value < tomorrowsDate && x.IsTrackingRequired).ToList();
                    foreach(var linesale in pendingLinesales)
                    {
                        var lnsl = new lineSale();
                        LineSaleMapper(linesale, lnsl, null);
                        //if (date.Date == DateTime.Now.Date)
                        //{
                        //    lnsl.isDeletable = true;
                        //}
                        response.pendingLineSales.Add(lnsl);
                    }
                    response.totalNewPendingAmount = pendingLinesales.Sum(x => x.BalanceAmount);
                    var trackingLinesales = uow.Context.LineSales.Include("LineSaleTrackings").Where(x => x.LastUpdatedDateTime.HasValue?( x.LastUpdatedDateTime.Value >= date && x.LastUpdatedDateTime.Value < tomorrowsDate && x.IsTrackingRequired):false).ToList();
                    response.oldBillPaymentAmount = 0;
                    foreach (var linesale in trackingLinesales)
                    {
                        if(linesale.LineSaleTrackings.ToList().Count>0)
                        {
                            if(linesale.LineSaleTrackings.ToList().Any(x=>x.CreatedDateTime.Value >= date && x.CreatedDateTime.Value < tomorrowsDate))
                            {
                                var lnsl = new lineSale();
                                LineSaleMapper(linesale, lnsl, null);
                                response.oldBillPaymentAmount += AddBoldFlagForLinesaleReport(linesale, lnsl,date);
                                response.trackedLineSales.Add(lnsl);
                            }
                        }
                    }
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Fetched records successfully";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Failed to fetch records", exception);
            }
            return await Task.FromResult(response);
        }

        private double AddBoldFlagForLinesaleReport(LineSale dbLnsl,lineSale uiLnsl,DateTime date)
        {
            double sum = 0;
            foreach(var tracking in dbLnsl.LineSaleTrackings.ToList())
            {
                if(tracking.CreatedDateTime.Value.Date==date)
                {
                    var uiTracking = uiLnsl.lineSaleTrackings.Where(x => x.lineSaleTrackingID == tracking.LineSaleTrackingID).FirstOrDefault();
                    if(uiTracking!=null)
                    {
                        uiTracking.isBold = true;
                        sum = sum + uiTracking.amountPaid;
                    }
                }
            }
            return sum;
        }

        public async Task<RsSuppliers> GetSuppliers()
        {
            var response = new RsSuppliers();
            try
            {
               
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var suppliers=uow.Context.Suppliers.Where(x => x.SupplierID != 1).ToList();
                    suppliers.ForEach(x =>
                    {
                        response.suppliers.Add(new supplier().Map(x));
                        response.totalBalanceAmount += x.BalanceAmount;
                        });

                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Records fetched successfuly";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Failed to fetch records", exception);
            }
            return await Task.FromResult(response);
        }

        public async Task<RsCommon> AddSupplierTracking(RqSupplier rqSupplier)
        {
            var response = new RsCommon();
            try
            {
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var supplier = uow.Context.Suppliers.Where(x => x.SupplierID == rqSupplier.supplierID && x.SupplierID!=1).FirstOrDefault();
                    if(supplier==null)
                    {
                        ThrowCustomException("Invalid supplier");
                    }
                    var tracking = new SupplierTracking();
                    tracking.SupplierID = supplier.SupplierID;
                    if (rqSupplier.isPayment)
                    {
                        tracking.PaidAmount = rqSupplier.amount;
                        supplier.BalanceAmount = supplier.BalanceAmount - rqSupplier.amount;
                    }
                    else
                    {
                        tracking.BorrowedAmount = rqSupplier.amount;
                        supplier.BalanceAmount = supplier.BalanceAmount+ rqSupplier.amount;
                    }
                    tracking.comments = rqSupplier.comments;
                    tracking.CreatedDatetime = DateTime.Now;
                    tracking.CreatedBy = rqSupplier.UserLogin;
                    uow.Context.SupplierTrackings.Add(tracking);
                    supplier.LastUpdatedBy = rqSupplier.UserLogin;
                    supplier.LastUpdatedDatetime = DateTime.Now;
                    uow.Commit();
                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Record saved successfuly";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Failed to save record",exception);
            }
            return await Task.FromResult(response);
        }

        public bool AddDataTemplateFile(string filename)
        {
            try
            {
                var file = ConfigurationManager.AppSettings["DataTemplatesPath"] + filename;
                if (!File.Exists(file))
                {
                    return false;
                }

                byte[] bytes = File.ReadAllBytes(file);
                using (UnitOfWork uow = new UnitOfWork())
                {
                    var dataTemplateRecord = new DataTemplate();
                    dataTemplateRecord.FileName = filename;
                    dataTemplateRecord.FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    dataTemplateRecord.File = bytes;

                    uow.Context.DataTemplates.Add(dataTemplateRecord);
                    uow.Commit();
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<RsCommon> AddNewProduct(RqProduct request)
        {
            var response = new RsCommon();
            try
            {
                if(string.IsNullOrWhiteSpace(request.productName))
                {
                    ThrowCustomException("Product name is mandatory");
                }
                using (UnitOfWork uow = new UnitOfWork())
                {
                    if(uow.Products.Any() && uow.Products.Select(x=>x.ProductName.ToLower()).ToList().Contains(request.productName.Trim().ToLower()))
                    {
                        ThrowCustomException("This product already exists");
                    }
                    var product = new Product();
                    product.ProductName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.productName.Trim().ToLower());
                    product.VATPercentage = request.vatPercentage;
                    product.CreatedBy = request.UserLogin;
                    product.CreatedDateTime = DateTime.Now;

                    uow.Context.Products.Add(product);
                    uow.Commit();

                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Record saved successfully";
                }
            }
            catch(Exception exception)
            {
                GetExceptionResponse(response, "Failed to save record", exception);
            }
            return await Task.FromResult(response);
        }
    

    public async Task<RsCommon> AddNewSubProduct(RqSubproduct request)
    {
        var response = new RsCommon();
        try
        {
            if (string.IsNullOrWhiteSpace(request.subProductName))
            {
                ThrowCustomException("SubProduct name is mandatory");
            }
            using (UnitOfWork uow = new UnitOfWork())
            {
                var product = uow.Context.Products.Where(x => x.ProductID == request.productID).FirstOrDefault();
                if (product==null)
                {
                    ThrowCustomException("Invalid product");
                }

                    var subproducts = uow.Context.SubProducts.Where(x => x.ProductID == request.productID);
                    if (subproducts.Any() && subproducts.Select(x => x.SubProductName.ToLower()).ToList().Contains(request.subProductName.Trim().ToLower()))
                    {
                        ThrowCustomException("This sub product already exists");
                    }

                    var subproduct = new SubProduct();
                    subproduct.ProductID = request.productID;
                    subproduct.SubProductName= CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.subProductName.Trim().ToLower());
                    subproduct.QuantityAvailable = request.quantityAvailable;
                    subproduct.MarkedPrice = request.markedPrice;
                    subproduct.MinAlertQuantity = request.minAlertQuantity;
                    subproduct.isActive = true;
                    subproduct.CreatedBy = request.UserLogin;
                    subproduct.CreatedDateTime = DateTime.Now;

                    uow.Context.SubProducts.Add(subproduct);
                    uow.Commit();

                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Record saved successfully";
                }
        }
        catch (Exception exception)
        {
            GetExceptionResponse(response, "Failed to save record", exception);
        }
        return await Task.FromResult(response);
    }

        public async Task<RsCommon> AddNewSupplier(RqSupplierNew request)
        {
            var response = new RsCommon();
            try
            {
                if (string.IsNullOrWhiteSpace(request.supplierName))
                {
                    ThrowCustomException("Supplier name is mandatory");
                }
                using (UnitOfWork uow = new UnitOfWork())
                {
                    if (uow.Suppliers.Any() && uow.Suppliers.Select(x => x.Name.ToLower()).ToList().Contains(request.supplierName.Trim().ToLower()))
                    {
                        ThrowCustomException("This supplier already exists");
                    }
                    var supplier = new Supplier();
                    supplier.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(request.supplierName.Trim().ToLower());
                    supplier.BalanceAmount = request.balanceAmount;
                    supplier.InitialBalance = request.balanceAmount;
                    supplier.DisplayInDropdown = request.displayInDropdown;
                    supplier.CreatedDateTime = DateTime.Now;
                    supplier.CreatedBy = request.UserLogin;

                    uow.Context.Suppliers.Add(supplier);
                    uow.Commit();

                    response.ResponseStatus = ResponseStatus.Success;
                    response.ResponseMessage = "Record saved successfully";
                }
            }
            catch (Exception exception)
            {
                GetExceptionResponse(response, "Failed to save record", exception);
            }
            return await Task.FromResult(response);
        }

    }

}