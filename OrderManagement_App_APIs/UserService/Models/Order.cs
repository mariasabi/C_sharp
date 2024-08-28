using System;
using System.Collections.Generic;

namespace UserService.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string? Itemname { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime OrderTime { get; set; }
}
