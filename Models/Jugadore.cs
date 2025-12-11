using System;
using System.Collections.Generic;

namespace ClubId.Models;

/// <summary>
/// datos del jugador
/// </summary>
public partial class Jugadore
{
    public int Idjugador { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Dni { get; set; } = null!;

    public DateTime FechaNac { get; set; }

    public bool Activo { get; set; }

    public string? Foto { get; set; }

    public virtual ICollection<Jgrxequipo> Jgrxequipos { get; set; } = new List<Jgrxequipo>();

    public virtual ICollection<Jueqxsancion> Jueqxsancions { get; set; } = new List<Jueqxsancion>();
}
