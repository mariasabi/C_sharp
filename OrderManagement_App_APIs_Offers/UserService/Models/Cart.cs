using System;
using System.Collections.Generic;

namespace UserService.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public decimal? CartValue { get; set; }
    public decimal? VoucherAmount { get; set; } = 0;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User? User { get; set; }
}
