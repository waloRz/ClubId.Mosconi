namespace ClubId.Models.ViewModels
{
    public class JugadorHistorialViewModel
    {
        // --- Cabecera: Datos del Jugador ---
        public int IdJugador { get; set; }
        public string NombreCompleto { get; set; } // Nombre + Apellido
        public string Dni { get; set; }
        public string Carnet { get; set; } // NroCarnet
        public DateTime? FechaNacimiento { get; set; }
        public string? FotoUrl { get; set; } // Si guardas la ruta, sino usaremos un placeholder

        // --- Cuerpo: Historial de Sanciones ---
        public List<ItemHistorialSancion> Historial { get; set; } = new List<ItemHistorialSancion>();
    }

    public class ItemHistorialSancion
    {
        public int IdSancion { get; set; } // ID del Boletín para generar el reporte
        public DateTime FechaSancion { get; set; }
        public string NombreEquipo { get; set; }
        public string DetalleSancion { get; set; } // "2 FECHAS"
        public string Informe { get; set; } // El motivo
    }
}