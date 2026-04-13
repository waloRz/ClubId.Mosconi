public class ExpulsadoItem
{
    public string Equipo { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Sancion { get; set; } = string.Empty;
    
    // Agregamos estos dos como STRING para evitar conflictos de tipos
    public string RondaFecha { get; set; } = string.Empty; // Ejemplo: "Fecha 4"
    public string FechaBoletin { get; set; } = string.Empty; // Ejemplo: "15/03/2024"
}

public class ReporteExpulsadosModel
{
    public string CategoriaFiltro { get; set; } = "GENERAL";
    public List<ExpulsadoItem> Jugadores { get; set; } = new List<ExpulsadoItem>();
}