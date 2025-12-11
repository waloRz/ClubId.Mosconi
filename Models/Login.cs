using System;
using System.Collections.Generic;

namespace ClubId.Models;

public partial class Login
{
    public int Id { get; set; }

    public string Usuario { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}
