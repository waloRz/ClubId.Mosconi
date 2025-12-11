using System;
using System.Collections.Generic;

namespace ClubId.Models;

public partial class Jueqxsancion
{
    public int IdJexS { get; set; }

    public int IdSanciones { get; set; }

    public int Idjugador { get; set; }

    public int IdEquipo { get; set; }

    public string Sancion { get; set; } = null!;

    public string Informe { get; set; } = null!;

    public virtual Equipo IdEquipoNavigation { get; set; } = null!;

    public virtual Sancione IdSancionesNavigation { get; set; } = null!;

    public virtual Jugadore IdjugadorNavigation { get; set; } = null!;
}
