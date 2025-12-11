using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClubId.Models.ViewModels;

public class JugadorSancionadoViewModel1
{
    //**************SANCIONES
    public int IdSanciones { get; set; }

    //************** JUGADOR X SANCIONES
   
    //public int IdJexS { get; set; } // SE CREA SOLO 
     public string Sancion { get; set; } = null!;
    public string Informe { get; set; } = null!;
   
    //************** JUGADOR X EQUIPO 
    public int IdJxE { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? NombreEq { get; set; } = null!;
    public string? Foto { get; set; } = null!;
    public int Idjugador { get; set; }  // nroCarnet
    public virtual Jgrxequipo IdJxENavigation { get; set; } = null!;

    // public int IdCategorias { get; set; }
    // public string? NombreCat { get; set; } = null!;
    // public IEnumerable<SelectListItem>? ListaCategoria { get; set; }
    // public IEnumerable<SelectListItem>? ListaEquipos { get; set; }

}