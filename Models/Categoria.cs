using System;
using System.Collections.Generic;

namespace ClubId.Models;

public partial class Categoria
{
    public int IdCategorias { get; set; }

    public string NombreCat { get; set; } = null!;

    public bool EstadoCat { get; set; }

    public virtual ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();

    public virtual ICollection<Jgrxequipo> Jgrxequipos { get; set; } = new List<Jgrxequipo>();

    public virtual ICollection<Sancione> Sanciones { get; set; } = new List<Sancione>();

    public virtual ICollection<Torneo> Torneos { get; set; } = new List<Torneo>();
}
