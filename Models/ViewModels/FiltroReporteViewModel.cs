using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClubId.Models.ViewModels
{
    public class FiltroReporteViewModel
    {
        public DateTime FechaDesde { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime FechaHasta { get; set; } = DateTime.Now;
        public int IdCategoria { get; set; }
        public List<SelectListItem>? ListaCategorias { get; set; }
    }
}