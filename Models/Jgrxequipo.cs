using System;
using System.Collections.Generic;

namespace ClubId.Models;

/// <summary>
/// el jugador Y en el equipo X con FK de las talblas equipos y de jugadores
/// </summary>
public partial class Jgrxequipo
{
    public int IdJxE { get; set; }

    public int Idjugador { get; set; }

    public int IdEquipo { get; set; }

    public DateTime FechaRecibo { get; set; }

    public int IdCategorias { get; set; }

    public virtual Categoria IdCategoriasNavigation { get; set; } = null!;

    public virtual Equipo IdEquipoNavigation { get; set; } = null!;

    public virtual Jugadore IdjugadorNavigation { get; set; } = null!;
}
