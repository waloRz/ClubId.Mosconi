using System;
using System.Collections.Generic;

namespace ClubId.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Avatar { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Planeta { get; set; } = null!;

    public string? Correo { get; set; }
}
