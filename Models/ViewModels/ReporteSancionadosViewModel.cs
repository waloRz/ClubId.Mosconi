namespace ClubId.Models.ViewModels;
public class ReporteSancionadosViewModel
{
    // Parámetros de búsqueda
    public DateTime FechaDesde { get; set; } = DateTime.Now.AddMonths(-1);
    public DateTime FechaHasta { get; set; } = DateTime.Now;
    public int IdCategoria { get; set; }

    // Resultados para la tabla
    public List<SancionReporteDto> Resultados { get; set; } = new();

    // Datos para el gráfico
      public List<ChartDataDto> DatosGrafico { get; set; } = new();
}

public class ChartDataDto
{
    public string Etiqueta { get; set; } = string.Empty;
    public int Valor { get; set; }
}