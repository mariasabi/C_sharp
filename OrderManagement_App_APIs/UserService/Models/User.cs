using System;
using System.Collections.Generic;

namespace UserService.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool Deleted { get; set; }

    public string HindiName { get; set; } = null!;

    public DateTime? DeletedAt { get; set; }

    public string DeletedBy { get; set; } = null!;

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
