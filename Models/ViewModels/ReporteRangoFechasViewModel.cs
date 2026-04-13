using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class ReporteRangoFechasViewModel
{
    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaDesde { get; set; } = DateTime.Now.AddMonths(-1);

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaHasta { get; set; } = DateTime.Now;

    public List<JugadorReporteItem>? Resultados { get; set; }
}

public class JugadorReporteItem
{
    public int NroOrden { get; set; }
    public string Foto { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Equipo { get; set; } = string.Empty;
    public DateTime FechaRecibo { get; set; }
}