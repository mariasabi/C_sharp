﻿using System;
using System.Collections.Generic;

namespace UserService.Model;

public partial class Item
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }
}
