
namespace ClubId.Models.ViewModels
{
    public class PlanillaViewModel
    {
        public string NombreEquipo { get; set; }= null!;
        public string Categoria { get; set; }= null!;
        
        // Lista que viaja desde la Vista al Controlador al generar el PDF
        public List<PlanillaJugadorViewModel> Jugadores { get; set; } = new List<PlanillaJugadorViewModel>();
    }

    public class PlanillaJugadorViewModel
    {
        public int Idjugador { get; set; }
        public int Nro { get; set; }
        public string? NroCarnet { get; set; }
        public string ApellidoYNombre { get; set; } =null!;
        public string? Dni { get; set; }
        public string? FechaNacimiento { get; set; }
        public string? Foto { get; set; }
    }
}