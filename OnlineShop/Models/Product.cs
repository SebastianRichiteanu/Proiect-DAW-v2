using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
        [Required]
        public float Price { get; set; }
        public float Rating { get; set; }
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public IEnumerable<SelectListItem> Categ { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

}