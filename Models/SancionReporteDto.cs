namespace ClubId.Models
{
    public class SancionReporteDto
    {
        public string Equipo { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Carnet { get; set; } = string.Empty;
        public string FechaBoletin { get; set; } = string.Empty;
        public int NroFecha { get; set; }
        public string SancionTexto { get; set; } = string.Empty;
    }
}