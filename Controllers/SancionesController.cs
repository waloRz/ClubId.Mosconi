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

using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic; // Asegúrate de que este using apunte a tus modelos EF (Sancion, SancionxJugador, Jugador)
using System.Net.WebSockets;


namespace ClubId.Controllers
{
    public class SancionesController : Controller
    {
        private readonly LigabdContext _context;
        private readonly IWebHostEnvironment _env;

        public SancionesController(LigabdContext context, IWebHostEnvironment env)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
            _env = env;
        }

        //************************************************************************************************************************************ SANCIONES HISTORIAL
        // GET: Sanciones/HistorialJugador/5
        public async Task<IActionResult> HistorialJugador(int id)
        {
            // 1. Buscamos al jugador
            var jugador = await _context.Jugadores.FindAsync(id);
            if (jugador == null) return NotFound();

            // 2. Buscamos su historial (Joins con Sancion y Equipo)
            // NOTA: Ajusta los nombres de tablas (Jueqxsancions, Sanciones, etc) según tu DbContext real
            var historialDb = await _context.Jueqxsancions // Tu tabla detalle
                .Include(d => d.IdSancionesNavigation) // Join con el Boletín (SANCION)
                .Include(d => d.IdEquipoNavigation)    // Join con EQUIPO
                .Include(d => d.IdjugadorNavigation)    // Join con EQUIPO
                .Where(d => d.Idjugador == id)
                .OrderByDescending(d => d.IdSancionesNavigation.Fecha) // Ordenamos por fecha mas reciente
                .ToListAsync();

            // 3. Mapeamos al ViewModel
            var viewModel = new JugadorHistorialViewModel
            {
                IdJugador = jugador.Idjugador,
                NombreCompleto = $"{jugador.Apellido}, {jugador.Nombre}",
                Dni = jugador.Dni.ToString(),
                Carnet = jugador.Idjugador.ToString() ?? "S/D", // Asumiendo que tienes campo Carnet, en mi caso mi IDJUGADOR es el nro de carnet
                FechaNacimiento = jugador.FechaNac,


                FotoUrl = jugador.Foto,  // Asumiendo que tienes un campo para la foto

                Historial = historialDb.Select(h => new ItemHistorialSancion
                {
                    IdSancion = h.IdSanciones,
                    FechaSancion = h.IdSancionesNavigation.Fecha,
                    NombreEquipo = h.IdEquipoNavigation.NombreEq,
                    DetalleSancion = h.Sancion, // "2 FECHAS..."
                    Informe = h.Informe
                }).ToList()
            };

            return View(viewModel);
        }

        //************************************************************************************************************************************ SANCIONES HISTORIAL
        // SancionesController.cs (Método Index - Solución Recomendada)
        public async Task<IActionResult> Index()
        {
            // 1. Traemos la Entidad de la DB
            var entidades = await _context.Sanciones
                            .Include(s => s.IdCategoriasNavigation)
                                          .OrderByDescending(b => b.Fecha)
                                          .ToListAsync();

            // 2. Mapeamos de la Entidad al ViewModel (¡Aquí resolvemos el error!)
            var viewModels = entidades.Select(e => new ClubId.Models.ViewModels.BoletinSancionesViewModel
            {
                IdSanciones = e.IdSanciones,
                FechaBoletin = e.Fecha,
                NroFecha = e.NroFecha,
                IdCategoria = e.IdCategorias,
                NombreCat = e.IdCategoriasNavigation.NombreCat,
                Comunicado = e.Comunicado,
                // ... mapea todos los campos necesarios
            }).ToList();

            var categorias = await _context.Categorias.ToListAsync();
            ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat");

            // 3. Enviamos el ViewModel correcto a la vista
            return View(viewModels);
        }

        //************************************************************************************************************************************ MODIFICAR BOLETIN
        // ---------------------------------------------------------
        // 4. EDIT: Modificar Existente (GET y POST)
        // ---------------------------------------------------------
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Buscamos el boletín y sus detalles
            var boletin = await _context.Sanciones
                                     .Include(b => b.IdCategoriasNavigation)
                                     .Include(b => b.Jueqxsancions)
                                            .ThenInclude(j => j.IdjugadorNavigation)
                                    .Include(b => b.Jueqxsancions)
                                            .ThenInclude(e => e.IdEquipoNavigation)
                                        .FirstOrDefaultAsync(b => b.IdSanciones == id);

            if (boletin == null)
            {
                return NotFound();
            }
            var categorias = await _context.Categorias
            .Where(b => b.EstadoCat)
            .ToListAsync();
            ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat", boletin.IdCategorias);
           
            // Mapeamos de Entidad -> ViewModel para mostrarlo en el formulario
            var viewModel = new SancionCreacionViewModel
            {
                IdSanciones = boletin.IdSanciones, // Asegurate de tener este campo en el VM si vas a editar
                FechaBoletin = boletin.Fecha,
                NroFecha = boletin.NroFecha,
                IdCategoria = boletin.IdCategorias,
                NombreCat = boletin.IdCategoriasNavigation.NombreCat,
                Comunicado = boletin.Comunicado,
                JugadoresSancionados = boletin.Jueqxsancions.Select(d => new SancionJugadorCreacionItem
                {
                    IdSanciones = d.IdSanciones,
                    IdJugador = d.Idjugador,
                    Dni = d.IdjugadorNavigation.Dni,
                    Nombre = d.IdjugadorNavigation.Nombre,
                    Apellido = d.IdjugadorNavigation.Apellido,
                    NombreCompleto = d.IdjugadorNavigation.Apellido + ", " + d.IdjugadorNavigation.Nombre, // NombreJugadorSnapshot,
                    NombreEq = d.IdEquipoNavigation.NombreEq,  //NombreEquipoSnapshot,
                    IdEquipo = d.IdEquipoNavigation.IdEquipo,
                    CantidadPartidos = d.Sancion, // .CantidadPartidos,                       
                    MotivoEspecifico = d.Informe,   // Motivo,                    
                    // Agrega DNI u otros datos si los guardaste en el snapshot
                }).ToList()
            };
            return View("Create", viewModel); // Reutilizamos la vista Create para Editar
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SancionCreacionViewModel viewModel)
        {
            if (id != viewModel.IdSanciones) return NotFound(); // Validación de seguridad básica

            ModelState.Remove("ListaCategorias");
            ModelState.Remove("nombreCat");
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Buscamos el boletín original en la BD incluyendo hijos
                    var boletinDb = await _context.Sanciones
                                        .Include(b => b.IdCategoriasNavigation)
                                        .Include(b => b.Jueqxsancions)
                                            .ThenInclude(j => j.IdjugadorNavigation)
                                        .Include(b => b.Jueqxsancions)
                                            .ThenInclude(e => e.IdEquipoNavigation)
                                        .FirstOrDefaultAsync(b => b.IdSanciones == id);

                    if (boletinDb == null) return NotFound();

                    // 2. Actualizamos datos cabecera
                    boletinDb.Fecha = viewModel.FechaBoletin;
                    boletinDb.NroFecha = viewModel.NroFecha;
                    boletinDb.IdCategorias = viewModel.IdCategoria;
                    boletinDb.Comunicado = viewModel.Comunicado;

                    // 3. Actualización inteligente de Detalles (Hijos):
                    // Opción simple: Borrar los viejos y poner los nuevos.
                    _context.Jueqxsancions.RemoveRange(boletinDb.Jueqxsancions);

                    // Agregamos los que vienen del formulario
                    if (viewModel.JugadoresSancionados != null)
                    {
                        foreach (var item in viewModel.JugadoresSancionados)
                        {
                            boletinDb.Jueqxsancions.Add(new Jueqxsancion
                            {
                                Idjugador = item.IdJugador,
                                // NombreJugadorSnapshot = item.NombreCompleto,
                                IdEquipo = item.IdEquipo,
                                // NombreEquipoSnapshot = item.NombreEq,
                                Sancion = item.CantidadPartidos,
                                Informe = item.MotivoEspecifico,
                            });
                        }
                    }

                    _context.Update(boletinDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoletinExists(viewModel.IdSanciones)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            var categorias = await _context.Categorias.ToListAsync();
            ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat", viewModel.IdCategoria);
            return View("Create", viewModel);
        }

        private bool BoletinExists(int id)
        {
            return _context.Sanciones.Any(e => e.IdSanciones == id);
        }

        //************************************************************************************************************************************ generar PDF
        public async Task<IActionResult> GenerarBoletin(int id)
        {

            // --- 1. BLOQUE NUEVO: TRAER DATOS ---
            var boletinEntidad = await _context.Sanciones
            .Include(b => b.IdCategoriasNavigation)
                .Include(b => b.Jueqxsancions)
                    // Asumiendo que tu entidad DetalleSancion tiene una propiedad de navegación 'Jugador'
                    // Si no la tiene, borra la linea .ThenInclude       
                    .ThenInclude(j => j.IdjugadorNavigation)
                .Include(b => b.Jueqxsancions)
                        .ThenInclude(e => e.IdEquipoNavigation)
                .FirstOrDefaultAsync(m => m.IdSanciones == id);

            if (boletinEntidad == null) return NotFound();

            BoletinSancionesViewModel viewModel = new BoletinSancionesViewModel
            {
                IdSanciones = boletinEntidad.IdSanciones,
                FechaBoletin = boletinEntidad.Fecha,
                NroFecha = boletinEntidad.NroFecha,
                IdCategoria = boletinEntidad.IdCategorias,
                NombreCat = boletinEntidad.IdCategoriasNavigation.NombreCat,
                Comunicado = boletinEntidad.Comunicado,
                JugadoresSancionados = boletinEntidad.Jueqxsancions.Select(d => new JugadorSancionadoViewModel
                {
                    IdJugador = d.Idjugador,
                    // Priorizamos el nombre guardado en el detalle (Snapshot), si no, el del maestro Jugador
                    NombreCompleto = !string.IsNullOrEmpty(d.IdjugadorNavigation.Nombre) ? d.IdjugadorNavigation.Apellido + ", " + d.IdjugadorNavigation.Nombre : "Sin Nombre",
                    NombreEq = d.IdEquipoNavigation.NombreEq ?? "Sin Equipo",

                    // OJO: Asegurate de cómo obtienes el DNI. 
                    // Si tu entidad DetalleSancion tiene el campo Dni, usa d.Dni. 
                    // Si tienes que ir a la tabla Jugador, usa d.Jugador?.Dni.ToString()
                    Dni = d.IdjugadorNavigation != null ? d.IdjugadorNavigation.Dni.ToString() : "S/D",

                    CantidadPartidos = d.Sancion.ToString(), //+ (d.Sancion == "1" ? " FECHA" : " FECHAS "), // Agregamos sufijo estético
                    MotivoEspecifico = d.Informe, // + (string.IsNullOrEmpty(d.Articulos) ? "" : $" ({d.Articulos})") // Concatenamos artículos si existen
                }).ToList()
            };
            // -----------------------------------

            // --- 2. MAPEO AL MODELO DEL PDF (LO QUE VIMOS ANTES) ---
            var datosPdf = new BoletinSancionesModel
            {
                FechaTexto = viewModel.FechaBoletin.ToString("dddd d 'de' MMMM 'del' yyyy", new System.Globalization.CultureInfo("es-ES")),
                Categoria = viewModel.NombreCat,
                NroFecha = $"FECHA {viewModel.NroFecha}",
                Sanciones = viewModel.JugadoresSancionados.Select(j => new SancionItem
                {
                    Club = j.NombreEq,
                    Jugador = j.NombreCompleto,
                    Carnet = j.IdJugador.ToString(),
                    // Aquí unimos la cantidad y el motivo para que se vea bien en la grilla
                    Sancion = $"{j.CantidadPartidos}" //- {j.MotivoEspecifico}
                }).ToList()
            };

            // --- 3. GENERAR PDF ---   
            var documento = new ReporteBoletin(datosPdf);
            byte[] pdfBytes = documento.GeneratePdf();

            //return File(pdfBytes, "application/pdf", $"Boletin_{viewModel.NroFecha}_{viewModel.nombreCat}.pdf");
            // 4. CAMBIO CLAVE: Configurar el header para visualización "Inline"
            // Esto le dice al navegador: "Muéstralo, no lo guardes todavía"
            // También definimos un nombre de archivo por si el usuario decide guardarlo después
            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = $"Boletin_{viewModel.NroFecha}_{viewModel.NombreCat}.pdf", // O el nombre que quieras
                Inline = true  // <--- ESTO ES LO IMPORTANTE (true = ver, false = descargar)
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            // 5. Retornar el archivo (sin poner el nombre de archivo aquí, ya lo pusimos en el header)
            return File(pdfBytes, "application/pdf");
        }

        //************************************************************************************************************************************

        // GET: Sanciones/Create (Muestra el formulario vacío para crear)
        public async Task<IActionResult> Create()

        {
            var categorias = await _context.Categorias
            .Where(b => b.EstadoCat)
            .ToListAsync();
            ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat");

            var viewModel = new SancionCreacionViewModel();
            return View(viewModel);
        }

        // POST: Sanciones/Create (Recibe los datos del formulario y los guarda en la DB)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SancionCreacionViewModel model)
        {
            ModelState.Remove("ListaCategorias");
            ModelState.Remove("nombreCat");

            // 1. Validar el modelo principal y asegurarse de que haya jugadores
            if (!ModelState.IsValid || !model.JugadoresSancionados.Any())
            {
                if (!model.JugadoresSancionados.Any())
                {
                    ModelState.AddModelError("", "Debe agregar al menos un jugador sancionado.");
                }
                var categorias = await _context.Categorias.ToListAsync();
                ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat");
                // Retorna la vista con los datos ingresados para que el usuario corrija.
                return View(model);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 2. Crear la entidad principal (Sancion)
                    var nuevaSancion = new Sancione
                    {
                        Fecha = model.FechaBoletin,    //FECHA DEL BOLETIN
                        IdCategorias = model.IdCategoria,
                        NroFecha = model.NroFecha,
                        Comunicado = model.Comunicado,
                        // Propiedades adicionales de Sancion
                    };

                    _context.Sanciones.Add(nuevaSancion);
                    await _context.SaveChangesAsync(); // Importante para obtener IdSancion

                    // 3. Crear las entidades relacionadas (SancionxJugador) es el DETALLE que se graba en la BD
                    var sancionesJugador = model.JugadoresSancionados.Select(item => new Jueqxsancion
                    {
                        IdSanciones = nuevaSancion.IdSanciones,
                        Idjugador = item.IdJugador,
                        IdEquipo = item.IdEquipo,
                        Sancion = item.CantidadPartidos, //d.Sancion.ToString() + (d.Sancion == "1" ? " FECHA" : " FECHAS")
                        Informe = item.MotivoEspecifico,
                        // Propiedades adicionales de SancionxJugador

                    }).ToList();

                    _context.Jueqxsancions.AddRange(sancionesJugador);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // Redirigir al detalle del boletín creado
                    TempData["SuccessMessage"] = $"Boletín Nro {nuevaSancion.IdSanciones} creado con éxito.";
                    return RedirectToAction(nameof(Index), new { id = nuevaSancion.IdSanciones });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    // Opcional: Registrar el error (ex)
                    ModelState.AddModelError("", "Ocurrió un error al intentar guardar el boletín de sanciones. Por favor, intente de nuevo.");
                    var categorias = await _context.Categorias
                    .Where(b => b.EstadoCat)
                    .ToListAsync();
                    ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat");
                    return View(model);
                }
            }
        }

        // AJAX Endpoint para buscar jugadores (Utilizado por Select2 en la vista)
        [HttpGet]
      public async Task<IActionResult> BuscarJugadores(string term)
{
    if (string.IsNullOrWhiteSpace(term))
    {
        return Json(new { results = new List<object>() });
    }

    // 1. Limpiamos y dividimos el término por espacios (ej: "Lopez Pablo" -> ["Lopez", "Pablo"])
    var palabras = term.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

    IQueryable<Jugadore> query = _context.Jugadores;

    // 2. Por cada palabra, filtramos la consulta (esto hace un AND lógico entre palabras)
    foreach (var palabra in palabras)
    {
        query = query.Where(j => j.Nombre.ToLower().Contains(palabra) || 
                                 j.Apellido.ToLower().Contains(palabra) || 
                                 j.Dni.Contains(palabra));
    }

    var results = await query
        .OrderBy(j => j.Apellido)
        .ThenBy(j => j.Nombre)
        .Take(15) // Subimos un poco el límite para dar más opciones
        .Select(j => new
        {
            id = j.Idjugador,
            text = $"{j.Apellido.ToUpper()}, {j.Nombre} (DNI: {j.Dni})",
            dni = j.Dni,
            nombre = j.Nombre,
            apellido = j.Apellido,
            nombreCompleto = $"{j.Apellido}, {j.Nombre}"
        })
        .ToListAsync();

    return Json(new { results = results });
}

        [HttpGet]
        public async Task<IActionResult> BuscarEquipos(string term) //int idcat
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new { results = new List<object>() });
            }

            var results = await _context.Equipos
            .Include(b => b.IdCategoriaNavigation)
                // Busca por nombre
                .Where(j => j.NombreEq.Contains(term)) //&& j.IdCategoria == idcat
                .Take(10) // Limita resultados para mejor rendimiento
                .Select(j => new
                {
                    id = j.IdEquipo, // Necesario para el valor del select
                    text = $"{j.NombreEq} (Cat: {j.IdCategoriaNavigation.NombreCat}) ",    // Texto a mostrar
                                                                                           //text = $"{j.Apellido}, {j.Nombre} (DNI: {j.Dni})", // Texto a mostrar
                    ncat = j.IdCategoriaNavigation.NombreCat,
                    // Campos extra para tu lógica de JS:
                    nombreEquipo = j.NombreEq
                })
                .ToListAsync();

            return Json(new { results = results });
        }
    
   //***************************************************** DELETE
// GET: Sanciones/Delete/5
public async Task<IActionResult> Delete(int? id)
{
    if (id == null) return NotFound();

    // Traemos la entidad con sus relaciones
    var sancion = await _context.Sanciones
        .Include(s => s.IdCategoriasNavigation)
        .FirstOrDefaultAsync(m => m.IdSanciones == id);

    if (sancion == null) return NotFound();

    // 🛠️ MAPEAMOS AL VIEWMODEL (Esto resuelve el error)
    var viewModel = new ClubId.Models.ViewModels.BoletinSancionesViewModel
    {
        IdSanciones = sancion.IdSanciones,
        FechaBoletin = sancion.Fecha, // fecha Date
        NombreCategoria = sancion.IdCategoriasNavigation?.NombreCat ?? "Sin Categoría",
        Comunicado = sancion.Comunicado,
        NroFecha = sancion.NroFecha
    };

    return View(viewModel); // Ahora le pasamos el tipo correcto
}

// POST: Sanciones/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var sancion = await _context.Sanciones.FindAsync(id);
    
    if (sancion != null)
    {
        _context.Sanciones.Remove(sancion);
        await _context.SaveChangesAsync();
    }
    
    return RedirectToAction(nameof(Index));
}
    }
    
}
