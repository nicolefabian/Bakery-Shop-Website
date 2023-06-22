using System;
using System.Collections.Generic;

namespace Patisserie.Models.DB
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public decimal Total { get; set; }
        public string CustomerId { get; set; } = null!;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
