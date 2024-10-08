﻿using System;
using System.Collections.Generic;

namespace Patisserie.Models.DB
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public decimal TotalAmount { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; } 
        public decimal Price { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
    }
}
