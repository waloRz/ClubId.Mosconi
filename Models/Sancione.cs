using System;
using System.Collections.Generic;

namespace ClubId.Models;

public partial class Sancione
{
    public int IdSanciones { get; set; }

    public DateTime Fecha { get; set; }

    public int IdCategorias { get; set; }

    public int NroFecha { get; set; }

    public string? Comunicado { get; set; }

    public virtual Categoria IdCategoriasNavigation { get; set; } = null!;

    public virtual ICollection<Jueqxsancion> Jueqxsancions { get; set; } = new List<Jueqxsancion>();
}
