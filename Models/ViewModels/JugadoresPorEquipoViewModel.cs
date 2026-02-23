using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClubId.Models.ViewModels;

public class JugadorPorEquipoViewModel
{
    public int Idjugador { get; set; }

    public int? NroCarnetOld{ get; set; }

    public int idjugadorxEquipo { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

[RegularExpression(@"^\d{2}\.\d{3}\.\d{3}$", 
    ErrorMessage = "El DNI debe tener el formato XX.XXX.XXX (ej. 12.345.678).")]
    public string Dni { get; set; } = null!;

    public DateOnly FechaNac { get; set; }

    public bool Activo { get; set; }

    public string? Foto { get; set; }
    public int IdEquipo { get; set; }

      public string? NombreEq { get; set; } = null!;

    public string? Delegado { get; set; } = null!;

    public string? Celular { get; set; }

    public ulong? Estado { get; set; }

    public string? FotoEq { get; set; }

    public DateTime FechaRecibo { get; set; }
    public string? MensajeSancion { get; set; } // mensaje de advertencia

    public int IdCategoria { get; set; }
    public string? NombreCat { get; set; } = null!;
    public IEnumerable<SelectListItem>? ListaCategoria { get; set; }
    public IEnumerable<SelectListItem>? ListaEquipos { get; set; }

}