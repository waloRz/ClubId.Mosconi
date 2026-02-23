using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClubId.Data;
using ClubId.Models;
using ClubId.Models.ViewModels;
using CsvHelper;
using ClubId.Services;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;


namespace ClubId.Controllers
{
    public class ReportesController : Controller
    {
        private readonly LigabdContext _context;
        private readonly IWebHostEnvironment _env;

        public ReportesController(LigabdContext context, IWebHostEnvironment env)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Sancionados(DateTime? desde, DateTime? hasta, int? idCategoria)
        {
            // Llenar el dropdown de categorías
            ViewData["IdCategoria"] = new SelectList(await _context.Categorias.Where(c => c.EstadoCat).ToListAsync(), "IdCategorias", "NombreCat");

            var model = new ReporteSancionadosViewModel();

            if (desde.HasValue && hasta.HasValue && idCategoria.HasValue)
            {
                model.FechaDesde = desde.Value;
                model.FechaHasta = hasta.Value;
                model.IdCategoria = idCategoria.Value;

                // Misma lógica de consulta que usamos para el PDF
                model.Resultados = await _context.Jueqxsancions
                    .Include(j => j.IdSancionesNavigation)
                    .Include(j => j.IdjugadorNavigation)
                    .Include(j => j.IdEquipoNavigation)
                    .Where(j => j.IdSancionesNavigation.Fecha >= desde &&
                                j.IdSancionesNavigation.Fecha <= hasta &&
                                j.IdSancionesNavigation.IdCategorias == idCategoria)
                    .OrderBy(j => j.IdEquipoNavigation.NombreEq)
                    .ThenBy(j => j.IdjugadorNavigation.Apellido)
                    .Select(j => new SancionReporteDto
                    {
                        Equipo = j.IdEquipoNavigation.NombreEq,
                        NombreCompleto = $"{j.IdjugadorNavigation.Apellido} {j.IdjugadorNavigation.Nombre}",
                        Carnet = j.IdjugadorNavigation.Idjugador.ToString(),
                        FechaBoletin = j.IdSancionesNavigation.Fecha.ToString("dd/MM/yy"),
                        NroFecha = j.IdSancionesNavigation.NroFecha,
                        SancionTexto = j.Sancion
                    }).ToListAsync();
            }
            // 📊 Lógica para el gráfico: Agrupamos por equipo y contamos
            model.DatosGrafico = model.Resultados
                .GroupBy(r => r.Equipo)
                .Select(g => new ChartDataDto
                {
                    Etiqueta = g.Key,
                    Valor = g.Count()
                })
                .OrderByDescending(x => x.Valor) // El que más tiene, primero
                .ToList();
            return View(model);
        }

    public async Task<IActionResult> GenerarReporteSancionados(DateTime desde, DateTime hasta, int idCategoria)
{
    var sancionados = await _context.Jueqxsancions // Tabla intermedia
        .Include(j => j.IdSancionesNavigation)
        .Include(j => j.IdjugadorNavigation)
        .Include(j => j.IdEquipoNavigation)
        .Where(j => j.IdSancionesNavigation.Fecha >= desde && 
                    j.IdSancionesNavigation.Fecha <= hasta &&
                    j.IdSancionesNavigation.IdCategorias == idCategoria)
        // Ordenamos por Equipo y luego por el nombre/apellido del jugador
        .OrderBy(j => j.IdEquipoNavigation.NombreEq)
        .ThenBy(j => j.IdjugadorNavigation.Apellido)
        .Select(j => new SancionReporteDto
        {
            Equipo = j.IdEquipoNavigation.NombreEq,
            NombreCompleto = $"{j.IdjugadorNavigation.Apellido} {j.IdjugadorNavigation.Nombre}",
            Carnet = j.IdjugadorNavigation.Idjugador.ToString(),
            FechaBoletin = j.IdSancionesNavigation.Fecha.ToString("dd/MM/yy"),
            NroFecha = j.IdSancionesNavigation.NroFecha,
            SancionTexto = j.Sancion // Ejemplo: "1 (una) fecha art 82"
        })
        .ToListAsync();

    var categoriaNombre = await _context.Categorias
        .Where(c => c.IdCategorias == idCategoria)
        .Select(c => c.NombreCat)
        .FirstOrDefaultAsync();

    // Generar PDF con QuestPDF
    var document = new ReporteSancionadosDocument(sancionados, desde, hasta, categoriaNombre ?? "VETERANOS");
    byte[] pdfBytes = document.GeneratePdf();

    return File(pdfBytes, "application/pdf");
}
    
    }
}
