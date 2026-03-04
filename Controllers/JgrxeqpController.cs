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
    public class JgrxeqpController : Controller
    {
        private readonly LigabdContext _context;   //CONTEXTO 
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _env;

        public JgrxeqpController(IImageService imageService, LigabdContext context, IWebHostEnvironment env)
        {
            _context = context;
            _imageService = imageService;
            QuestPDF.Settings.License = LicenseType.Community;
            _env = env;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = (
            from je in _context.Jgrxequipos
            join ult in (
                from jxe in _context.Jgrxequipos
                group jxe by jxe.Idjugador into g
                select new { Idjugador = g.Key, FechaReciboMasReciente = g.Max(je => je.FechaRecibo) }
            ) on new { je.Idjugador, je.FechaRecibo } equals new { ult.Idjugador, FechaRecibo = ult.FechaReciboMasReciente }
            join Jugadore in _context.Jugadores on je.Idjugador equals Jugadore.Idjugador
            join Equipo in _context.Equipos on je.IdEquipo equals Equipo.IdEquipo
            join Categoria in _context.Categorias on je.IdCategorias equals Categoria.IdCategorias

            select new JugadorPorEquipoViewModel
            {
                Idjugador = je.IdjugadorNavigation.Idjugador,
                Dni = je.IdjugadorNavigation.Dni,
                Nombre = je.IdjugadorNavigation.Nombre,
                Apellido = je.IdjugadorNavigation.Apellido,
                FechaRecibo = je.FechaRecibo,
                Activo = je.IdjugadorNavigation.Activo,
                Foto = je.IdjugadorNavigation.Foto,
                NombreCat = je.IdCategoriasNavigation.NombreCat,
                NombreEq = je.IdEquipoNavigation.NombreEq,
                idjugadorxEquipo = je.IdJxE
            }
          );

            if (!string.IsNullOrEmpty(q))
            {
                query = _context.Jgrxequipos
                .Include(je => je.IdjugadorNavigation)
                .Include(je => je.IdEquipoNavigation)
                .Select(je => new JugadorPorEquipoViewModel
                {
                    Idjugador = je.IdjugadorNavigation.Idjugador,
                    idjugadorxEquipo = je.IdJxE,
                    Dni = je.IdjugadorNavigation.Dni,
                    Nombre = je.IdjugadorNavigation.Nombre,
                    Apellido = je.IdjugadorNavigation.Apellido,
                    FechaRecibo = je.FechaRecibo,
                    Activo = je.IdjugadorNavigation.Activo,
                    Foto = je.IdjugadorNavigation.Foto,
                    NombreCat = je.IdCategoriasNavigation.NombreCat,
                    NombreEq = je.IdEquipoNavigation.NombreEq
                });
                query = query.Where(vm => vm.Nombre.Contains(q) || vm.Apellido.Contains(q) || vm.Dni.Contains(q) || vm.Idjugador.ToString() == q);
            }
            query = query
                .OrderByDescending(vm => vm.FechaRecibo)
                .Take(12);

            var ultimoRecibosPorJugador = await query.ToListAsync();
            return View(ultimoRecibosPorJugador);
        }

        public async Task<IActionResult> Details(int id)
        {
            var jugador = await _context.Jugadores
                                    .Include(j => j.Jgrxequipos)
                                        .ThenInclude(je => je.IdEquipoNavigation)
                                    .Include(j => j.Jgrxequipos)
                                        .ThenInclude(je => je.IdCategoriasNavigation)
                                    .FirstOrDefaultAsync(m => m.Idjugador == id);

            if (jugador == null)
            {
                return NotFound();
            }
            // Mapear los datos al ViewModel
            var viewModel = new JugadorDetallesViewModel
            {
                JugadorId = jugador.Idjugador,
                Nombre = jugador.Nombre,
                Apellido = jugador.Apellido,
                idJxEdit = jugador.Jgrxequipos.Last().IdJxE,
                Dni = jugador.Dni,
                FechaNac = jugador.FechaNac,
                Activo = jugador.Activo,
                Foto = jugador.Foto,
                HistorialEquipos = jugador.Jgrxequipos
                    .Select(je => new EquipoDetallesViewModel

                    {//viewModel - TABLA DEL dataSet
                        EquipoId = je.IdEquipoNavigation.IdEquipo,
                        NombreEquipo = je.IdEquipoNavigation.NombreEq,
                        //      FechaInscripcion = je.FechaInscripcion,
                        idJxE = je.IdJxE,
                        NombreCat = je.IdCategoriasNavigation.NombreCat,
                        FechaRecibo = je.FechaRecibo
                    })
                    .OrderByDescending(e => e.FechaInscripcion) // Opcional: ordenar por fecha más reciente
                    .ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new CrearJugadorViewModel
            {
                FechaNac = DateOnly.FromDateTime(DateTime.Today),
                //     FechaInscripcion = DateTime.Today,
                FechaRecibo = DateTime.Today,

                ListaCategoria = await _context.Categorias
                 .Where(e => e.EstadoCat == true)
                .Select(e => new SelectListItem
                {
                    Value = e.IdCategorias.ToString(),
                    Text = e.NombreCat
                }).ToListAsync(),

                ListaEquipos = await _context.Equipos.Select(e => new SelectListItem
                {
                    Value = e.IdEquipo.ToString(),
                    Text = e.NombreEq
                }).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearJugadorViewModel model, IFormFile? photo)
        {
            // 1. Preparar el ViewModel de vuelta por si algo falla (recargar listas)
            var viewModelError = new CrearJugadorViewModel
            {
                FechaNac = model.FechaNac,
                FechaRecibo = model.FechaRecibo,
                Dni = model.Dni,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                IdCategoria = model.IdCategoria,
                IdEquipo = model.IdEquipo,
                ListaCategoria = await _context.Categorias.Select(e => new SelectListItem { Value = e.IdCategorias.ToString(), Text = e.NombreCat }).ToListAsync(),
                ListaEquipos = await _context.Equipos.Select(e => new SelectListItem { Value = e.IdEquipo.ToString(), Text = e.NombreEq }).ToListAsync()
            };

            if (!ModelState.IsValid) return View(viewModelError);

            // --- VERIFICACIÓN DE UNICIDAD DEL DNI ---
            var jugadorExistente = await _context.Jugadores.FirstOrDefaultAsync(j => j.Dni == model.Dni);
            if (jugadorExistente != null)
            {
                ModelState.AddModelError("Dni", "Ya existe un jugador con este DNI.");
                return View(viewModelError);
            }

            // --- PROCESAMIENTO DE LA FOTO CON EL NUEVO SERVICIO ---
            string nombreFoto = "default-user.webp"; // Imagen por defecto si no suben nada
            if (photo != null && photo.Length > 0)
            {
                // Usamos la carpeta fotosPerfiles que definimos
                string folderPath = Path.Combine(_env.WebRootPath, "fotosPerfiles");

                // El servicio se encarga de todo: Resize, Crop, WebP y Guardado
                nombreFoto = await _imageService.SubirFotoPerfil(photo, folderPath);
            }

            // --- CREACIÓN DEL JUGADOR ---
            var nuevoJugador = new Jugadore
            {
                Nombre = model.Nombre.ToUpper(),
                Apellido = model.Apellido.ToUpper(),
                Dni = model.Dni,
                FechaNac = model.FechaNac,
                Activo = model.Activo,
                Foto = nombreFoto // Guardamos solo el nombre del archivo (ej: "guid.webp")
            };

            _context.Jugadores.Add(nuevoJugador);
            await _context.SaveChangesAsync();

            // --- REGISTRO INTERMEDIO (RELACIÓN EQUIPO/CATEGORÍA) ---
            var registroIntermedio = new Jgrxequipo
            {
                Idjugador = nuevoJugador.Idjugador,
                IdEquipo = model.IdEquipo,
                IdCategorias = model.IdCategoria,
                FechaRecibo = DateTime.Now
            };

            _context.Jgrxequipos.Add(registroIntermedio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GenerarCarnet(int idjugador, int idpjxe)  //es el idJxE       
        {//genera carnet desde el INDEX  y tiene en cuenta la fecha del recibo, es decir toma el ultimo           
            var jugador = await _context.Jgrxequipos
             .Include(x => x.IdEquipoNavigation)
                .Include(j => j.IdjugadorNavigation)
                    .Include(j => j.IdCategoriasNavigation)
                .OrderByDescending(x => x.FechaRecibo).FirstOrDefaultAsync(m => m.IdJxE == idpjxe || m.Idjugador == idjugador);
            //.FirstOrDefaultAsync(m => m.IdJxE == idpjxe || m.Idjugador == idjugador);

            if (jugador == null)
            {
                return NotFound();
            }

            var viewModel = new JugadorCarnetViewModel
            {
                JugadorId = jugador.Idjugador,
                Nombre = jugador.IdjugadorNavigation.Nombre,
                Apellido = jugador.IdjugadorNavigation.Apellido,
                Foto = jugador.IdjugadorNavigation.Foto,
                Dni = jugador.IdjugadorNavigation.Dni,
                NombreEquipo = jugador.IdEquipoNavigation.NombreEq,
                NombreCat = jugador.IdCategoriasNavigation.NombreCat,
                FechaNac = jugador.IdjugadorNavigation.FechaNac,
                FechaRecibo = jugador.FechaRecibo
            };

          //      var document = new CarnetDocument(viewModel);
            var document = new CarnetDocument(viewModel, _env.WebRootPath);
            byte[] pdfBytes = document.GeneratePdf();

            // 4. CAMBIO CLAVE: Configurar el header para visualización "Inline"
            // Esto le dice al navegador: "Muéstralo, no lo guardes todavía"
            // También definimos un nombre de archivo por si el usuario decide guardarlo después
            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = $"Carnet_{jugador.IdjugadorNavigation.Idjugador}_{jugador.IdjugadorNavigation.Apellido}_{jugador.IdjugadorNavigation.Nombre.Replace(" ", "_")}.pdf", // O el nombre que quieras
                Inline = true  // <--- ESTO ES LO IMPORTANTE (true = ver, false = descargar)
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            // 5. Retornar el archivo (sin poner el nombre de archivo aquí, ya lo pusimos en el header)
            return File(pdfBytes, "application/pdf");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var player = await _context.Jgrxequipos
         .Include(j => j.IdjugadorNavigation)
             .Include(je => je.IdEquipoNavigation)
             .Include(c => c.IdCategoriasNavigation)
         .FirstOrDefaultAsync(m => m.IdJxE == id);

            //var player = await _context.Jgrxequipos.FindAsync(id);
            //JugadorPorEquipoViewModel
            if (player == null) return NotFound();

            var viewModel = new JugadorPorEquipoViewModel
            {
                idjugadorxEquipo = id,
                NroCarnetOld = player.IdjugadorNavigation.NroCarnetOld,
                Nombre = player.IdjugadorNavigation.Nombre,
                Apellido = player.IdjugadorNavigation.Apellido,
                Dni = player.IdjugadorNavigation.Dni,
                FechaNac = player.IdjugadorNavigation.FechaNac,
                FechaRecibo = player.FechaRecibo,
                IdEquipo = player.IdEquipoNavigation.IdEquipo,
                NombreEq = player.IdEquipoNavigation.NombreEq,
                IdCategoria = player.IdCategoriasNavigation.IdCategorias,
                NombreCat = player.IdCategoriasNavigation.NombreCat,
                Activo = player.IdjugadorNavigation.Activo,
                Foto = player.IdjugadorNavigation.Foto,

                ListaCategoria = await _context.Categorias.Select(e => new SelectListItem
                {
                    Value = e.IdCategorias.ToString(),
                    Text = e.NombreCat
                }).ToListAsync(),

                ListaEquipos = await _context.Equipos.Select(e => new SelectListItem
                {
                    Value = e.IdEquipo.ToString(),
                    Text = e.NombreEq
                }).ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JugadorPorEquipoViewModel input, IFormFile? photo)
        {
            if (id != input.idjugadorxEquipo) return NotFound();
            if (!ModelState.IsValid) return View(input);

            // Encuentro la tupla incluyendo la navegación del jugador
            var player = await _context.Jgrxequipos
                .Include(j => j.IdjugadorNavigation)
                .FirstOrDefaultAsync(m => m.IdJxE == id);

            if (player == null) return NotFound();

            // 1. CARGA DE DATOS BÁSICOS
            player.IdjugadorNavigation.Nombre = input.Nombre.ToUpper();
            player.IdjugadorNavigation.Apellido = input.Apellido.ToUpper();
            player.IdjugadorNavigation.Dni = input.Dni;
            player.IdjugadorNavigation.FechaNac = input.FechaNac;
            player.IdjugadorNavigation.Activo = input.Activo;

            // 2. GESTIÓN DE LA NUEVA FOTO
            if (photo != null && photo.Length > 0)
            {
                string folderPath = Path.Combine(_env.WebRootPath, "fotosPerfiles");

                // --- OPCIONAL: Borrar la foto anterior para no acumular basura ---
                if (!string.IsNullOrEmpty(player.IdjugadorNavigation.Foto) &&
                    player.IdjugadorNavigation.Foto != "default-user.webp")
                {
                    var oldPath = Path.Combine(folderPath, player.IdjugadorNavigation.Foto);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                // --- Procesar y guardar la nueva foto ---
                player.IdjugadorNavigation.Foto = await _imageService.SubirFotoPerfil(photo, folderPath);
            }

            // 3. ACTUALIZACIÓN DE LA RELACIÓN
            player.IdEquipo = input.IdEquipo;
            player.IdCategorias = input.IdCategoria;
            player.FechaRecibo = input.FechaRecibo;

            try
            {
                _context.Update(player); // Usamos Update directamente sobre el objeto trackeado
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Jgrxequipos.Any(e => e.IdJxE == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // pase o transferencia de equipo/categoria
        public async Task<IActionResult> Pase(int id)
        {
            var player = await _context.Jgrxequipos
                .Include(j => j.IdjugadorNavigation)
                .Include(je => je.IdEquipoNavigation)
                .Include(c => c.IdCategoriasNavigation)
                .FirstOrDefaultAsync(m => m.IdJxE == id);

            if (player == null) return NotFound();

            var viewModel = new JugadorPorEquipoViewModel
            {
                idjugadorxEquipo = id,
                Idjugador = player.Idjugador,
                Nombre = player.IdjugadorNavigation.Nombre,
                Apellido = player.IdjugadorNavigation.Apellido,
                Dni = player.IdjugadorNavigation.Dni,
                FechaNac = player.IdjugadorNavigation.FechaNac,
                FechaRecibo = DateTime.Now,
                IdEquipo = player.IdEquipoNavigation.IdEquipo,
                NombreEq = player.IdEquipoNavigation.NombreEq,
                IdCategoria = player.IdCategoriasNavigation.IdCategorias,
                NombreCat = player.IdCategoriasNavigation.NombreCat,
                Activo = player.IdjugadorNavigation.Activo,
                Foto = player.IdjugadorNavigation.Foto,

                ListaCategoria = await _context.Categorias
                    .Where(e => e.EstadoCat == true)
                    .Select(e => new SelectListItem
                    {
                        Value = e.IdCategorias.ToString(),
                        Text = e.NombreCat
                    }).ToListAsync(),

                ListaEquipos = await _context.Equipos.Select(e => new SelectListItem
                {
                    Value = e.IdEquipo.ToString(),
                    Text = e.NombreEq
                }).ToListAsync()
            };

            // --- AQUÍ VA LA NUEVA LÓGICA DE SANCIÓN ---
            // Buscamos si el jugador tiene alguna sanción que contenga "SUSPENDIDO" o "EXPULSADO"
            var sancionActiva = await _context.Jueqxsancions
                .Include(s => s.IdSancionesNavigation)
                .Where(s => s.Idjugador == viewModel.Idjugador &&
                           (s.Sancion.Contains("SUSPENDIDO") || s.Sancion.Contains("EXPULSADO")))
                .OrderByDescending(s => s.IdSancionesNavigation.Fecha)
                .FirstOrDefaultAsync();

            if (sancionActiva != null)
            {
                // Guardamos el texto de la sanción en el ViewModel para que la Vista lo muestre
                viewModel.MensajeSancion = sancionActiva.Sancion;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pase(int id, JugadorPorEquipoViewModel input, IFormFile? photo)
        {
            if (id != input.idjugadorxEquipo) return NotFound();
            if (!ModelState.IsValid) return View(input);

            // encuentro la tupla
            var player = await _context.Jgrxequipos
             .Include(j => j.IdjugadorNavigation)
                 .Include(je => je.IdEquipoNavigation)
                 .Include(c => c.IdCategoriasNavigation)
             .FirstOrDefaultAsync(m => m.IdJxE == id);

            if (player == null) return NotFound();

            // VALIDACIÓN DE DUPLICADOS:
            // Si la categoría, el equipo y la fecha son idénticos a lo que ya tiene, es un error.
            // (Ajusta la lógica si permites re-fichajes en el mismo equipo en diferente fecha, pero 
            //  generalmente una transferencia implica cambio de equipo o categoría).

            bool esMismoEquipo = input.IdEquipo == player.IdEquipo;
            bool esMismaCategoria = input.IdCategoria == player.IdCategorias;
            // Compara fecha solo si es relevante para tu lógica de negocio evitar duplicados por fecha
            // bool esMismaFecha = input.FechaRecibo == player.FechaRecibo; 

            if (esMismoEquipo && esMismaCategoria)
            {
                ModelState.AddModelError("", "No se puede realizar el pase: El jugador ya pertenece a este Equipo y Categoría.");
                // Necesitas recargar las listas para volver a mostrar la vista
                // input.ListaCategoria = ... 
                // input.ListaEquipos = ...
                return View(input);
            }

            if (photo != null && photo.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
                var relPath = Path.Combine("uploads", fileName);
                var absPath = Path.Combine(_env.WebRootPath, relPath);
                Directory.CreateDirectory(Path.GetDirectoryName(absPath)!);
                using var stream = new FileStream(absPath, FileMode.Create);
                await photo.CopyToAsync(stream);
                player.IdjugadorNavigation.Foto = "/" + relPath.Replace('\\', '/');     //JUGADOR
            }

            var registroIntermedio = new Jgrxequipo
            {
                Idjugador = player.IdjugadorNavigation.Idjugador,
                IdEquipo = input.IdEquipo,
                IdCategorias = input.IdCategoria,
                FechaRecibo = input.FechaRecibo
            };

            _context.Jgrxequipos.Add(registroIntermedio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<JsonResult> GetEquiposPorCategoria(int idCategoria)
        {
            // Asegurarse de que el IdCategoria sea válido
            if (idCategoria == 0)
            {
                return Json(new List<object>()); // Devuelve una lista vacía si no hay categoría
            }

            // 1. Obtener los equipos filtrados por el IdCategoria
            // Asumo que tu entidad Equipo tiene la FK IdCategoria
            var equiposFiltrados = await _context.Equipos
                .Where(e => e.IdCategoria == idCategoria && e.Estado == true)
                .Select(e => new
                {
                    value = e.IdEquipo,
                    text = e.NombreEq
                })
                .OrderBy(e => e.text)
                .ToListAsync();

            // 2. Devolver la lista como JSON.
            // El objeto anónimo { value, text } es lo que jQuery espera para llenar un select.
            return Json(equiposFiltrados);
        }
    }
}
