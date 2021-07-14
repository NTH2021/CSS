using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectCCS.ViewsModel
{
    public class BillManagerVM
    {
        [Key]
        public int idBill { get; set; }

        public long? Total { get; set; }

        public bool? Status { get; set; }

        public DateTime? Date { get; set; }

        [Required]
        [StringLength(400)]
        public string Address { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}