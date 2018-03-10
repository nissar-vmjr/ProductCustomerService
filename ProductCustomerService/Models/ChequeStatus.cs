using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCustomerService.Models
{
    public class ChequeStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ChequeStatusID { get; set; }

        [MaxLength(50)]
        public string StatusName { get; set; }
    }
}