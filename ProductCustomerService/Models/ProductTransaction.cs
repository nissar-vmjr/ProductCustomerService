using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class ProductTransaction
    {
        public long ProductTransactionID { get; set; }

        public long SubProductID { get; set; }

        public int? WarehouseID { get; set; }

        public int? SupplierID { get; set; }

        public long? BalanceTransactionID { get; set; }

        public DateTime TransactionDate { get; set; }
        public int BuyQuantity { get; set; }

        public int SellQuantity { get; set; }

        public bool IsSellFromWarehouse { get; set; }

        public double SellingPrice { get; set; }
        public double TotalPrice { get; set; }

        public double TaxPrice { get; set; }

        public double MiscellaneousPrice { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }
        public long TotalPriceIncludingTax { get; set; }

        public double BuyingAmount { get; set; }

        public long QuantityRemaining { get; set; }

        //public bool IsSelling { get; set; }


        [MaxLength(100)]
        public string SupplierName { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; }
       
        public DateTime? CreatedDateTime { get; set; }

        [MaxLength(50)]
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedDateTime { get; set; }

        public virtual SubProduct SubProduct { get; set; }
        public virtual Warehouse Warehouse { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual BalanceTransaction BalanceTransaction { get; set; }
    }
}