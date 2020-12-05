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
        public string ReviewComment { get; set; }
        [Required]
        [Range(1,5,ErrorMessage = "Ratingul este intre 1 si 5 stele!")]
        public int ReviewRating { get; set; } 
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}