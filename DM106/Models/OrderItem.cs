using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DM106.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int quantidade { get; set; }
        // Foreign Key
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        // Navigation property
        public virtual Product Product { get; set; }
    }
}