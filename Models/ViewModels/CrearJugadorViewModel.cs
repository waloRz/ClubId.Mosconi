using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ClubId.Models.ViewModels;

public class CategoriaViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un equipo.")]
    public int IdCategoria { get; set; }
    public string? NombreCat { get; set; }
}

public class CrearJugadorViewModel
{   
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    
    [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}$", 
    ErrorMessage = "El DNI debe tener el formato XX.XXX.XXX (ej. 12.345.678).")]
    public string Dni { get; set; } = null!;
    public DateTime FechaNac { get; set; }
    public bool Activo { get; set; }
    public string? Foto { get; set; }
    public int IdEquipo { get; set; }
    public int IdCategoria { get; set; }
      public DateTime FechaRecibo { get; set; }    
     public IEnumerable<SelectListItem>? ListaCategoria { get; set; }
    public IEnumerable<SelectListItem>? ListaEquipos { get; set; }
}

