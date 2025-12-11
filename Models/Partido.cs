using System;
using System.Collections.Generic;

namespace ClubId.Models;

/// <summary>
/// todos los partido de la fecha
/// </summary>
public partial class Partido
{
    public int IdPartido { get; set; }

    public int IdFecha { get; set; }

    public int? NroPartido { get; set; }

    public TimeOnly? Horario { get; set; }

    public int? Cancha { get; set; }

    public virtual ICollection<Equipoxpartido> Equipoxpartidos { get; set; } = new List<Equipoxpartido>();

    public virtual Fecha IdFechaNavigation { get; set; } = null!;
}
