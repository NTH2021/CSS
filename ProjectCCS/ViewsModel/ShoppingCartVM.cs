using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectCCS.ViewsModel
{
    public class ShoppingCartVM
    {

        public int id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string image { get; set; }

        [Required]
        public long? price { get; set; }

        public int? Amount { get; set; }
    }
}