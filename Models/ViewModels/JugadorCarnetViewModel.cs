namespace ClubId.Models.ViewModels;

public class JugadorCarnetViewModel
{
    public int JugadorId { get; set; }   
    public string? Nombre { get; set; }
    public string Apellido { get; set; } = null!;
    public string? Dni { get; set; }
    public string? Foto { get; set; }
    public DateOnly FechaNac { get; set; }    
     public bool Activo { get; set; }
     public int EquipoId { get; set; }
    public string? NombreEquipo { get; set; }
    public int idJxE { get; set; }    
    public string nombreCat { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public DateTime FechaRecibo { get; set; }
}

