namespace ClubId.Models.ViewModels;

public class JugadorCarnetViewModel
{
    public int JugadorId { get; set; }   
    public required string Nombre { get; set; }
    public string Apellido { get; set; } = null!;
    public string? Dni { get; set; }
    public string? Foto { get; set; }
    public DateOnly FechaNac { get; set; }    
     public bool Activo { get; set; }
     public int EquipoId { get; set; }
    public required string NombreEquipo { get; set; }
    public int idJxE { get; set; }    
    public string NombreCat { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public DateTime FechaRecibo { get; set; }
}

