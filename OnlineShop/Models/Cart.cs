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
        public int Quantity { get; set; }
    }
}