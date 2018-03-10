using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductCustomerService.Models
{
    public class DataTemplate
    {
        public int DataTemplateID { get; set; }

        [MaxLength(200)]
        public string FileName { get; set; }

        public byte[] File { get; set; }

        [MaxLength(200)]
        public string FileType { get; set; }

        [MaxLength(200)]
        public string LastDownloadedBy { get; set; }

        public DateTime? LastDownloadedDateTime { get; set; }
    
    }
}