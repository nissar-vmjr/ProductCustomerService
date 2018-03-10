using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService.UIContext
{
    public class productTransaction
    {
        public productTransaction()
        {
            //customer = new customer();
        }
        public long? productTransactionID { get; set; }

        public long subProductID { get; set; }

        public int? wareHouseID { get; set; }

        public int? supplierID { get; set; }

        public long? balanceTransactionID { get; set; }

        public string transactionDate { get; set; }

        public double sellingPrice { get; set; }

        public bool isSellFromWarehouse { get; set; }

        public int buyQuantity { get; set; }
        public int sellQuantity { get; set; }

        public double totalPrice { get; set; }

        public double taxPrice { get; set; }

        public double miscellaneousPrice { get; set; }

        public double buyingAmount { get; set; }
        public string description { get; set; }
        public long totalPriceIncludingTax { get; set; }

        public long quantityRemaining { get; set; }

        //public bool isSelling { get; set; }

        public string supplierName { get; set; }

        //public string isIncludeCustomer { get; set; }

        //public customer customer { get; set; }

        public string customerName { get; set; }

        public bool isEditable { get; set; }
        public bool isDeletable { get; set; }
        public balanceTransaction balanceTransaction { get; set; }
        public subProduct subProduct { get; set; }
        
    }
}