using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class Cart
    {
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }
        [Key]
        [Column(Order = 1)]
        public int ProductId { get; set; }

        [Range(1, 10, ErrorMessage = "Cantitatea este intre 1 si 10")]
        public int Quantity { get; set; }
    }
}