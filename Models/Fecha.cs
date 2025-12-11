using System;
using System.Collections.Generic;

namespace ClubId.Models;

/// <summary>
/// son las fechas de los torneos, su tamaño esta dado por la cantidad de equipos que tenga el torneo
/// 
/// </summary>
public partial class Fecha
{
    public int IdFecha { get; set; }

    public DateOnly? Fechora { get; set; }

    public int IdTorneo { get; set; }

    public int NroFecha { get; set; }

    public virtual Torneo IdTorneoNavigation { get; set; } = null!;

    public virtual ICollection<Partido> Partidos { get; set; } = new List<Partido>();
}
