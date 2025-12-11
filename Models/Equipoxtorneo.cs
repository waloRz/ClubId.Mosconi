using System;
using System.Collections.Generic;

namespace ClubId.Models;

/// <summary>
/// los equipos que jugan X torneo
/// </summary>
public partial class Equipoxtorneo
{
    public int IdExT { get; set; }

    public int Idequipo { get; set; }

    public int IdTorneo { get; set; }

    public int? Jugados { get; set; }

    public int? Ganados { get; set; }

    public int? Empatados { get; set; }

    public int? Perdidos { get; set; }

    public int? GolesFavor { get; set; }

    public int? GolesContra { get; set; }

    public int? Puntos { get; set; }

    public virtual Torneo IdTorneoNavigation { get; set; } = null!;

    public virtual Equipo IdequipoNavigation { get; set; } = null!;
}
