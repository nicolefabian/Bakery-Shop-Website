using System;
using System.Collections.Generic;

namespace Patisserie.Models.DB
{
    public partial class CartItem
    {
        public int CartItemId { get; set; }
        public decimal TotalAmount { get; set; }
        public int ProductId { get; set; }
        public int? CartId { get; set; }
        public int Quantity { get; set; }

        public virtual Cart? Cart { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
