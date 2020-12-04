using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        [Required] // ???
        public string ReviewComment { get; set; }
        [Required] // ???
        public int ReviewRating { get; set; } // int? rating = {0,1,2,3,4,5}
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}