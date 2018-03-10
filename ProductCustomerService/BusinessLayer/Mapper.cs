using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProductCustomerService.Models;
using ProductCustomerService.UIContext;
namespace ProductCustomerService.BusinessLayer
{
    public static class Mapper
    {
        public static Customer Map(this Customer dbCustomer,customer uiCustomer,bool isEdit)
        {
            dbCustomer.CustomerFirstName = uiCustomer.customerFirstName;
            dbCustomer.CustomerLastName = uiCustomer.customerLastName;
            dbCustomer.Email = uiCustomer.email;
            dbCustomer.Keyword = uiCustomer.keyword;
            dbCustomer.Address = uiCustomer.address;
            if (!isEdit)
            {
                dbCustomer.NetOutstandingBalance = uiCustomer.netOutstandingBalance;
                dbCustomer.CreatedDateTime = DateTime.Now;
            }
            else
            {
                dbCustomer.LastUpdatedDateTime = DateTime.Now;
            }
            dbCustomer.MaxLimitAllowed = uiCustomer.maxLimitAmount;
            dbCustomer.MaxWaitingDays = uiCustomer.maxWaitingDays;
            return dbCustomer;
        }

        public static customer Map(this customer uiCustomer,Customer dbCustomer )
        {
            uiCustomer.customerID = dbCustomer.CustomerID;
            uiCustomer.customerFirstName = dbCustomer.CustomerFirstName;
            uiCustomer.customerLastName = dbCustomer.CustomerLastName;
            uiCustomer.email = dbCustomer.Email;
            uiCustomer.keyword = dbCustomer.Keyword;
            uiCustomer.address = dbCustomer.Address;
            uiCustomer.netOutstandingBalance = dbCustomer.NetOutstandingBalance;
            uiCustomer.maxLimitAmount = dbCustomer.MaxLimitAllowed;
            uiCustomer.maxWaitingDays = dbCustomer.MaxWaitingDays;

            var primaryContact = dbCustomer.CustomerContacts?.FirstOrDefault(x => x.IsPrimaryContact == true);
            if (primaryContact != null)
            {
                uiCustomer.primaryContact = new customerContact();
                uiCustomer.primaryContact.Map(primaryContact);
            }

            var secondaryContact = dbCustomer.CustomerContacts?.FirstOrDefault(x => x.IsPrimaryContact == false);
            if (secondaryContact != null)
            {
                uiCustomer.secondaryContact = new customerContact();
                uiCustomer.secondaryContact.Map(secondaryContact);
            }

            return uiCustomer;
        }

        public static CustomerContact Map(this CustomerContact dbContact,customerContact uiContact)
        {
            dbContact.ContactTypeID = uiContact.contactTypeID;
            dbContact.CountryCode = uiContact.countryCode;
            dbContact.CityCode = uiContact.cityCode;
            dbContact.ContactNumber = uiContact.contactNumber;
            return dbContact;
        }


        public static customerContact Map(this customerContact uiContact, CustomerContact dbContact)
        {
            uiContact.customerContactID = dbContact.CustomerContactID;
            uiContact.contactTypeID= dbContact.ContactTypeID;
            uiContact.countryCode = dbContact.CountryCode;
            uiContact.cityCode = dbContact.CityCode;
            uiContact.contactNumber = dbContact.ContactNumber;
            return uiContact;
        }

        public static BalanceTransaction Map(this BalanceTransaction dbTransaction,balanceTransaction uiTransaction,bool isEdit)
        {
            DateTime transactionDate;
            if (DateTime.TryParse(uiTransaction.transactionDate, out transactionDate))
                dbTransaction.TransactionDate = transactionDate;
            else
                dbTransaction.TransactionDate = DateTime.Now;
            dbTransaction.IncomingAmount = uiTransaction.incomingAmount;
            dbTransaction.OutgoingAmount = uiTransaction.outgoingAmount;
            dbTransaction.TransactionTypeID = uiTransaction.transactionTypeID;
            dbTransaction.Description = uiTransaction.comments;

            if(uiTransaction.transactionTypeID==(int)TransactionTypeEnum.Cheque)
            {
                DateTime chequeDate;
                if (DateTime.TryParse(uiTransaction.chequeDate, out chequeDate))
                    dbTransaction.ChequeDate = chequeDate;
                dbTransaction.ChequeNumber = uiTransaction.chequeNumber;
                dbTransaction.ChequeCustomerName = uiTransaction.chequeCustomerName;
                dbTransaction.ChequeIssuerBank = uiTransaction.chequeIssuerBank;
                dbTransaction.ChequeStatusID = (int)ChequeStatusEnum.NotInitiated;
                dbTransaction.OnlineReferernceID = null;
            }
            else if(uiTransaction.transactionTypeID==(int)TransactionTypeEnum.Online)
            {
                dbTransaction.OnlineReferernceID = uiTransaction.onlineReferernceID;
                dbTransaction.ChequeDate = null;
                dbTransaction.ChequeNumber = null;
                dbTransaction.ChequeCustomerName = null;
                dbTransaction.ChequeIssuerBank = null;
                dbTransaction.ChequeStatusID = null;
            }
            else
            {
                dbTransaction.ChequeDate = null;
                dbTransaction.ChequeNumber = null;
                dbTransaction.ChequeCustomerName = null;
                dbTransaction.ChequeIssuerBank = null;
                dbTransaction.ChequeStatusID = null;
                dbTransaction.OnlineReferernceID = null;
            }

            if(isEdit)
            {
                dbTransaction.LastUpdatedDateTime = DateTime.Now;
            }
            else
            {
                dbTransaction.CreatedDateTime = DateTime.Now;
                //dbTransaction.CustomerID = uiTransaction.customerID;
            }

            return dbTransaction;
        }

        public static balanceTransaction Map(this balanceTransaction uiTransaction,BalanceTransaction dbTransaction)
        {
            uiTransaction.balanceTransactionID = dbTransaction.BalanceTransactionID;
            uiTransaction.transactionDate = dbTransaction.TransactionDate.ToString();
            uiTransaction.incomingAmount = dbTransaction.IncomingAmount;
            uiTransaction.outgoingAmount = dbTransaction.OutgoingAmount;
            uiTransaction.outstandingBalance = dbTransaction.OutstandingBalance;
            uiTransaction.comments = dbTransaction.Description;
            uiTransaction.transactionTypeID = dbTransaction.TransactionTypeID;
            uiTransaction.customerID = dbTransaction.CustomerID;
            if(dbTransaction.TransactionTypeID==(int)TransactionTypeEnum.Cheque)
            {
                uiTransaction.chequeDate = dbTransaction.ChequeDate.ToString();
                uiTransaction.chequeCustomerName = dbTransaction.ChequeCustomerName;
                uiTransaction.chequeIssuerBank = dbTransaction.ChequeIssuerBank;
                uiTransaction.chequeNumber = dbTransaction.ChequeNumber;
                uiTransaction.chequeStatusID = dbTransaction.ChequeStatusID;
                uiTransaction.chequeDepositedDate = dbTransaction.ChequeDepositedDate.HasValue ? dbTransaction.ChequeDepositedDate.Value.ToString() : string.Empty;
                //uiTransaction.isChequePassed = dbTransaction.IsChequePassed;
                uiTransaction.chequeActionDate = dbTransaction.ChequeActionDate.HasValue? dbTransaction.ChequeActionDate.Value.ToString():string.Empty;
                //uiTransaction.isChequeFailed = dbTransaction.IsChequeFailed;
                uiTransaction.chequeFailureComments = dbTransaction.ChequeFailureComments;
            }
            else if(dbTransaction.TransactionTypeID == (int)TransactionTypeEnum.Online)
            {
                uiTransaction.onlineReferernceID = dbTransaction.OnlineReferernceID;
            }
            if(dbTransaction.Customer!=null)
            {
                uiTransaction.customer=new customer().Map(dbTransaction.Customer);
            }

            return uiTransaction;
        }

        public static product Map(this product uiProduct,Product dbProduct)
        {
            uiProduct.productID = dbProduct.ProductID;
            uiProduct.productName = dbProduct.ProductName;
            uiProduct.productCode = dbProduct.ProductCode;
            uiProduct.vatPercentage = dbProduct.VATPercentage;
            return uiProduct;
        }

        public static subProduct Map(this subProduct uiSubproduct, SubProduct dbSubproduct)
        {
            uiSubproduct.subProductID = dbSubproduct.SubProductID;
            uiSubproduct.subProductName = dbSubproduct.SubProductName;
            uiSubproduct.quantityAvailable = dbSubproduct.QuantityAvailable;
            uiSubproduct.markedPrice = dbSubproduct.MarkedPrice;
            if(dbSubproduct.Product!=null)
            {
                uiSubproduct.product = new product().Map(dbSubproduct.Product);
            }
            return uiSubproduct;
        }

        public static ProductTransaction Map(this ProductTransaction dbTransaction,productTransaction uiTransaction,bool isEdit)
        {
            DateTime transactionDate;
            if (DateTime.TryParse(uiTransaction.transactionDate, out transactionDate))
                dbTransaction.TransactionDate = transactionDate;
            else
                dbTransaction.TransactionDate = DateTime.Now;

            dbTransaction.SupplierID = uiTransaction.supplierID;
            dbTransaction.IsSellFromWarehouse = uiTransaction.isSellFromWarehouse;
            dbTransaction.SupplierName = uiTransaction.supplierName;
            dbTransaction.WarehouseID = uiTransaction.wareHouseID;
            dbTransaction.Description = uiTransaction.description;
            dbTransaction.BuyQuantity = uiTransaction.buyQuantity;
            dbTransaction.SellQuantity = uiTransaction.sellQuantity;
            dbTransaction.SellingPrice = uiTransaction.sellingPrice;
            dbTransaction.TotalPrice = uiTransaction.totalPrice;
            dbTransaction.TaxPrice = uiTransaction.taxPrice;
            dbTransaction.TotalPriceIncludingTax = uiTransaction.totalPriceIncludingTax;
            dbTransaction.BuyingAmount = uiTransaction.buyingAmount;
            dbTransaction.MiscellaneousPrice = uiTransaction.miscellaneousPrice;
            if(isEdit)
            {
                dbTransaction.LastUpdatedDateTime = DateTime.Now;
            }
            else
            {
                dbTransaction.CreatedDateTime = DateTime.Now;
                dbTransaction.SubProductID = uiTransaction.subProductID;
                dbTransaction.BalanceTransactionID = uiTransaction.balanceTransactionID;
            }


            return dbTransaction;
        }

        public static productTransaction Map(this productTransaction uiTransaction,ProductTransaction dbTransaction)
        {
            uiTransaction.productTransactionID = dbTransaction.ProductTransactionID;
            uiTransaction.subProductID = dbTransaction.SubProductID;
            uiTransaction.wareHouseID = dbTransaction.WarehouseID;
            uiTransaction.balanceTransactionID = dbTransaction.BalanceTransactionID;
            uiTransaction.transactionDate = dbTransaction.TransactionDate.ToString();
            uiTransaction.buyQuantity = dbTransaction.BuyQuantity;
            uiTransaction.sellQuantity = dbTransaction.SellQuantity;
            uiTransaction.isSellFromWarehouse = dbTransaction.IsSellFromWarehouse;
            uiTransaction.sellingPrice = dbTransaction.SellingPrice;
            uiTransaction.totalPrice = dbTransaction.TotalPrice;
            uiTransaction.taxPrice = dbTransaction.TaxPrice;
            uiTransaction.totalPriceIncludingTax = dbTransaction.TotalPriceIncludingTax;
            uiTransaction.miscellaneousPrice = dbTransaction.MiscellaneousPrice;
            uiTransaction.description = dbTransaction.Description;
            uiTransaction.quantityRemaining = dbTransaction.QuantityRemaining;
            uiTransaction.supplierName = dbTransaction.SupplierName;
            uiTransaction.supplierID = dbTransaction.SupplierID;
            uiTransaction.buyingAmount = dbTransaction.BuyingAmount;

            if(dbTransaction.BalanceTransaction!=null)
            {
                uiTransaction.balanceTransaction = new balanceTransaction();
                uiTransaction.balanceTransaction.Map(dbTransaction.BalanceTransaction);
            }
            if(dbTransaction.SubProduct!=null)
            {
                uiTransaction.subProduct = new subProduct();
                uiTransaction.subProduct.Map(dbTransaction.SubProduct);
            }
            
            return uiTransaction;
        }

       

        public static supplier Map(this supplier uiSupplier, Supplier dbSupplier)
        {
            uiSupplier.supplierID = dbSupplier.SupplierID;
            uiSupplier.supplierName = dbSupplier.Name;
            uiSupplier.balanceAmount = dbSupplier.BalanceAmount;
            uiSupplier.lastUpdatedDate = dbSupplier.LastUpdatedDatetime.ToString();
            return uiSupplier;
        }

    }
}