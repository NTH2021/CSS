namespace ProjectCCS.Models
{
    using ProjectCCS.ViewsModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [Key]
        [StringLength(100)]
        [ValidEmail]
        public string Email { get; set; }

        [StringLength(50)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(11)]
        [ValidPhone(ErrorMessage = "Phone number is not valid")]
        public string Phone { get; set; }

        [Required]
        [StringLength(400)]
        public string Address { get; set; }
    }
}
