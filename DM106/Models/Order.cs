using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DM106.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
        public int Id { get; set; }
        public string userEmail { get; set; }
        public DateTime? dataPedido { get; set; }
        public DateTime? dataEntrega { get; set; }
        public string status { get; set; }
        public decimal preco { get; set; }
        public decimal peso { get; set; }
        public decimal frete { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}