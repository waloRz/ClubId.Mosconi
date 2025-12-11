using System;
using System.Collections.Generic;

namespace ClubId.Models;

/// <summary>
/// Equipos de la Liga, el cual tiene una foranea que me lleva a la categoria donde juega
/// </summary>
public partial class Equipo
{
    public int IdEquipo { get; set; }

    public int IdCategoria { get; set; }

    public string NombreEq { get; set; } = null!;

    public string Delegado { get; set; } = null!;

    public string? Celular { get; set; }

    public bool Estado { get; set; }

    public string? FotoEq { get; set; }

    public virtual ICollection<Equipoxpartido> EquipoxpartidoIdEquipo2Navigations { get; set; } = new List<Equipoxpartido>();

    public virtual ICollection<Equipoxpartido> EquipoxpartidoIdEquipoNavigations { get; set; } = new List<Equipoxpartido>();

    public virtual ICollection<Equipoxtorneo> Equipoxtorneos { get; set; } = new List<Equipoxtorneo>();

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Jgrxequipo> Jgrxequipos { get; set; } = new List<Jgrxequipo>();

    public virtual ICollection<Jueqxsancion> Jueqxsancions { get; set; } = new List<Jueqxsancion>();
}
