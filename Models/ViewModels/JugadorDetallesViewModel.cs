using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ClubId.Models.ViewModels;

public class JugadorDetallesViewModel
{
    public int JugadorId { get; set; }    
    public string? Nombre { get; set; }
    public int idJxEdit   { get; set; }
    public string Apellido { get; set; } = null!;
    public string? Dni { get; set; }
    public string? Foto { get; set; }
    public DateOnly FechaNac { get; set; }    
     public bool Activo { get; set; }
    public ICollection<EquipoDetallesViewModel> HistorialEquipos { get; set; }
}

public class EquipoDetallesViewModel
{
    public int EquipoId { get; set; }
    public int idJxE { get; set; }
    public string NombreEquipo { get; set; }
    public string nombreCat { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public DateTime FechaRecibo { get; set; }
}