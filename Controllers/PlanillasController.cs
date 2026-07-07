using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClubId.Data;
using ClubId.Models;
using ClubId.Models.ViewModels;
using CsvHelper;
using ClubId.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClubId.Controllers
{
    public class PlanillasController : Controller
    {
        private readonly LigabdContext _context;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _env;

        public PlanillasController(IImageService imageService, LigabdContext context, IWebHostEnvironment env)
        {
            _context = context;
            _imageService = imageService;
            _env = env;
             QuestPDF.Settings.License = LicenseType.Community;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new PlanillaViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> BuscarJugadores(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new { results = new object[] { } });

            // 1. NORMALIZACIÓN INICIAL DEL INPUT:
            // - Pasamos a minúsculas para ignorar mayúsculas/minúsculas.
            // - Eliminamos los puntos "." para que "40.123.456" pase a ser "40123456".
            // - Limpiamos espacios al inicio y final con Trim().
            var termNormalizado = term.ToLower().Replace(".", "").Trim();

            // - Dividimos por espacios eliminando fragmentos vacíos. 
            // Esto destruye automáticamente los espacios dobles o triples (ej: "Juan    Perez" -> ["juan", "perez"])
            var palabras = termNormalizado.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!palabras.Any())
                return Json(new { results = new object[] { } });

            var query = _context.Jugadores.AsQueryable();

            // 2. DETERMINAR EL TIPO DE BÚSQUEDA:
            // Evaluamos el término completo sin puntos ni espacios intermedios.
            bool isNumeric = long.TryParse(termNormalizado.Replace(" ", ""), out _);

            if (isNumeric)
            {
                // El input limpio es un número (DNI). 
                // Eliminamos también los puntos de la columna en la BD por si están guardados con formato.
                var dniBuscar = termNormalizado.Replace(" ", "");
                query = query.Where(j => j.Dni.Replace(".", "").Contains(dniBuscar));
            }
            else
            {
                // El input es texto (Nombre / Apellido).
                // Buscamos palabra por palabra para que no importe el orden (ej: "Perez Juan" o "Juan Perez")
                foreach (var palabra in palabras)
                {
                    var p = palabra; // Variable local para evitar problemas de clausura en la expresión lambda
                    query = query.Where(j => j.Apellido.ToLower().Contains(p) || j.Nombre.ToLower().Contains(p));
                }
            }

            // 3. EJECUCIÓN Y PROYECCIÓN:
            var jugadores = await query
                .Select(j => new
                {
                    id = j.Idjugador,
                    text = $"{j.Apellido} {j.Nombre} - DNI: {j.Dni}",
                    apellidoYNombre = $"{j.Apellido} {j.Nombre}",
                    dni = j.Dni,
                    fechaNacimiento = j.FechaNac.ToString("dd/MM/yy"),
                    nroCarnet = $"{j.Idjugador}"
                })
                .Take(10)
                .ToListAsync();

            return Json(new { results = jugadores });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerarPdf(PlanillaViewModel model)
        {
            if (model.Jugadores == null || !model.Jugadores.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un jugador a la planilla.");
                return View("Crear", model);
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Text($"Planilla de Equipo: {(model.NombreEquipo ?? "S/N").ToUpper()} - Categoría: {(model.Categoria ?? "S/C").ToUpper()}")
                        .SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        // Definición de columnas
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // Nro
                            columns.ConstantColumn(60); // Nro Carnet
                            columns.RelativeColumn();   // Apellido y Nombre
                            columns.ConstantColumn(80); // DNI
                            columns.ConstantColumn(80); // Fecha Nac
                        });

                        // Encabezados
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Nro").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Carnet").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("Apellido y Nombre").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("D.N.I.").SemiBold();
                            header.Cell().BorderBottom(1).PaddingBottom(5).Text("F. Nac").SemiBold();
                        });

                        // Filas de jugadores
                        foreach (var jugador in model.Jugadores)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(jugador.Nro.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(jugador.NroCarnet);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(jugador.ApellidoYNombre.ToUpper());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(jugador.Dni);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(jugador.FechaNacimiento);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Planilla_{model.NombreEquipo ?? "Equipo"}.pdf");
        }
    
    [HttpPost]
        public async Task<IActionResult> ImprimirCarnets(List<int> idsJugadores)
        {
            if (idsJugadores == null || !idsJugadores.Any())
            {
                return RedirectToAction("Crear"); // O manejar el error
            }

            // Buscamos los jugadores seleccionados en la base de datos
            var listaJugadores = await _context.Jugadores
                .Where(j => idsJugadores.Contains(j.Idjugador))
                .Select(j => new JugadorCarnetDto
                {
                    IdJugador = j.Idjugador,
                    NroCarnet = $"{j.Idjugador}", // Ajustá a tu lógica de numeración
                    ApellidoYNombre = $"{j.Apellido} {j.Nombre}",
                    Dni = j.Dni,
                    FechaNacimiento = j.FechaNac.ToString("dd/MM/yyyy"),
                    Foto = j.Foto // Asumiendo que tenés la columna de tipo byte[] (VARBINARY) en tu BD
                })
                .ToListAsync();

            // Instanciamos el servicio pasándole la ruta física de wwwroot
            var carnetService = new CarnetReportService(_env.WebRootPath);
            
            // Generamos el archivo PDF
            byte[] pdfBytes = carnetService.GenerarPdfCarnets(listaJugadores);

            return File(pdfBytes, "application/pdf", "Carnets_Identificacion.pdf");
        }
    

   
    }
}