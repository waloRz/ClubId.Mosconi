using System;
using System.Collections.Generic;

namespace ClubId.Models;

public partial class Torneo
{
    public int IdTorneo { get; set; }

    public string? Nombre { get; set; }

    public int IdCategoria { get; set; }

    public int? CantEquipos { get; set; }

    public int? Estado { get; set; }

    public virtual ICollection<Equipoxtorneo> Equipoxtorneos { get; set; } = new List<Equipoxtorneo>();

    public virtual ICollection<Fecha> Fechas { get; set; } = new List<Fecha>();

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;
}
