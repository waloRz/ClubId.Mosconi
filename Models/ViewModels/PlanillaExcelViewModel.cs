namespace ClubId.Models.ViewModels
{
   public class PlanillaExcelViewModel
{
    public string Dni { get; set; }=null!;
    public string ApellidoNombre { get; set; }=null!;
    
    // Nueva estructura de 3 columnas para la sanción
    public string Sancion { get; set; } =null!;
    public string Debe { get; set; }=null!;
    public string Total { get; set; }=null!;
    
    public string Categoria { get; set; }=null!;
    public string Equipo { get; set; }=null!;
    public string FechaBoletin { get; set; }=null!;
}
}