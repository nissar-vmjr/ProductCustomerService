using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCustomerService.Models
{
    public class Warehouse
    {
      
        public int WarehouseID { get; set; }
        [MaxLength(100)]
        public string WarehouseName { get; set; }

        public int StockCount { get; set; }
    }
}