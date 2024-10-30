using System;
using System.Collections.Generic;

namespace UserService.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public string? ItemName { get; set; }

    public int? Quantity { get; set; }

    public int? CartId { get; set; }

    public decimal? Price { get; set; }

    public virtual Cart? Cart { get; set; }
}
