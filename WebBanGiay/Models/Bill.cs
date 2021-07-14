namespace WebBanGiay.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bill")]
    public partial class Bill
    {
        [Key]
        public int idBill { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public long? Total { get; set; }

        public bool? Status { get; set; }

        public DateTime? Date { get; set; }
    }
}
