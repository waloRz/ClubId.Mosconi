using System;
using System.Collections.Generic;

namespace ClubId.Models;

public partial class Equipoxpartido
{
    public int IdExP { get; set; }

    public int IdPartido { get; set; }

    public int IdEquipo { get; set; }

    public int? Resultado { get; set; }

    public int IdEquipo2 { get; set; }

    public int? Resultado2 { get; set; }

    public virtual Equipo IdEquipo2Navigation { get; set; } = null!;

    public virtual Equipo IdEquipoNavigation { get; set; } = null!;

    public virtual Partido IdPartidoNavigation { get; set; } = null!;
}
