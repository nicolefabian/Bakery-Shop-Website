using System;
using System.Collections.Generic;

namespace Patisserie.Models.DB
{
    public partial class Cart
    {
        public Cart()
        {
            CartItems = new HashSet<CartItem>();
        }

        public int CartId { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? ProductId { get; set; }
        public string? ShoppingId { get; set; }

        public virtual Product? Product { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
