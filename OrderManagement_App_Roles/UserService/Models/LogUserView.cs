using System;
using System.Collections.Generic;

namespace UserService.Models;

public partial class LogUserView
{
    public int LogId { get; set; }

    public DateTime Date { get; set; }

    public string Thread { get; set; } = null!;

    public string Level { get; set; } = null!;

    public string Logger { get; set; } = null!;

    public string Message { get; set; } = null!;

    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
}
