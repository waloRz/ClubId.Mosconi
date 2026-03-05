using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClubId.Data;
using ClubId.Models;
using ClubId.Services;
using ClubId.Models.ViewModels;
using System.IO; // Necesario para Path.Combine y FileStream
using Microsoft.AspNetCore.Hosting; // Necesario para manejar archivos

namespace ClubId.Controllers
{
    public class EquiposController : Controller
    {
        private readonly LigabdContext _context;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _hostEnvironment; // Para guardar fotos

        public EquiposController(IImageService imageService, IWebHostEnvironment hostEnvironment, LigabdContext context)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _imageService = imageService;

        }

        // 1. INDEX
        public async Task<IActionResult> Index()
        {
            // 1. Obtener los equipos, incluyendo la navegación a Categoría
            var equiposDb = await _context.Equipos
                .Include(e => e.IdCategoriaNavigation)
                .ToListAsync();

            // 2. Mapear el listado de Entidades (Equipo) al listado de ViewModels (EquipoViewModel)
            var equiposViewModel = equiposDb.Select(e => new EquipoViewModel
            {
                IdEquipo = e.IdEquipo,
                NombreEq = e.NombreEq,
                Delegado = e.Delegado,

                // Ejemplo de mapeo de FK a nombre
                NombreCategoria = e.IdCategoriaNavigation.NombreCat,
                Celular = e.Celular,
                Estado = e.Estado,
                FotoEq = e.FotoEq
                // Agrega todos los campos que necesite tu ViewModel
                // ...
            }).ToList();

            // 3. Pasar el listado de ViewModels a la vista
            return View(equiposViewModel);
        }

        // 2. CREATE (GET)
        public async Task<IActionResult> Create()
        {
            // Cargamos el Dropdown de Categorías           
            var categorias = await _context.Categorias.ToListAsync();
            //    var categorias =  _context.Categorias.ToList();
            if (categorias == null)
            {
                // Manejar error (ej: loguear) o usar una lista vacía para evitar la NRE
                categorias = new List<Categoria>();
            }
            // 3. Crear SelectList y enviarlo con el ViewModel
            ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat");
            return View(new EquipoViewModel() { Estado = true });
        }

        // 3. CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipoViewModel viewModel)
        {
            // A. VALIDACIÓN DE NOMBRE REPETIDO
            bool existeNombre = await _context.Equipos.AnyAsync(e => e.NombreEq == viewModel.NombreEq && e.IdCategoria == viewModel.IdCategoria);
            if (existeNombre)
            {
                ModelState.AddModelError("NombreEq", "¡Ese nombre de equipo ya existe en esta categoría!");
            }

            ModelState.Remove("IdCategoriaNavigation");

            if (!ModelState.IsValid)
            {
                var categorias = await _context.Categorias.ToListAsync();
                ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat", viewModel.IdCategoria);
                return View(viewModel);
            }

            // B. MAPEO DE ENTIDAD
            var equipo = new Equipo
            {
                NombreEq = viewModel.NombreEq,
                IdCategoria = viewModel.IdCategoria,
                Delegado = viewModel.Delegado,
                Celular = viewModel.Celular,
                Estado = viewModel.Estado,
                FotoEq = "default-escudo.webp" // Valor por defecto
            };

            // C. LÓGICA DE OPTIMIZACIÓN DE ESCUDO
            if (viewModel.FotoFile != null && viewModel.FotoFile.Length > 0)
            {
                // Definimos la ruta a la carpeta de equipos
                string folderPath = Path.Combine(_hostEnvironment.WebRootPath, "imagenes/equipos");

                // El servicio procesa, redimensiona a 400x400 (Crop) y guarda como WebP
                // Pasamos 'false' para que use ResizeMode.Pad
                equipo.FotoEq = await _imageService.SubirFotoPerfil(viewModel.FotoFile, folderPath, false);
            }

            // D. GUARDAR EN DB
            _context.Add(equipo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // 4. EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // 1. Obtener la entidad de la base de datos
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null) return NotFound();

            // 2. Mapear la Entidad (Equipo) al ViewModel (EquipoViewModel)
            var viewModel = new EquipoViewModel
            {
                IdEquipo = equipo.IdEquipo,
                NombreEq = equipo.NombreEq,
                IdCategoria = equipo.IdCategoria,
                //NombreCategoria =equipo.IdCategoriaNavigation.NombreCat,
                Delegado = equipo.Delegado,
                Celular = equipo.Celular,
                Estado = equipo.Estado,
                //d.IdjugadorNavigation != null ? d.IdjugadorNavigation.Dni.ToString() : "S/D",
                // *** IMPORTANTE ***: Solo asignamos el string de la ruta
                FotoEq = equipo.FotoEq
            };

            var categorias = await _context.Categorias.ToListAsync();
            if (categorias == null)
            {
                // Manejar error (ej: loguear) o usar una lista vacía para evitar la NRE
                categorias = new List<Categoria>();
            }
            // 3. Crear SelectList y enviarlo con el ViewModel
            ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat", equipo.IdCategoria);

            // CORRECCIÓN: Enviamos el ViewModel
            return View(viewModel);
        }

        // 5. EDIT (POST)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EquipoViewModel viewModel)
        {
            if (id != viewModel.IdEquipo) return NotFound();

            // 1. VALIDACIÓN DE NOMBRE REPETIDO
            bool existeNombre = await _context.Equipos
                .AnyAsync(e => e.NombreEq == viewModel.NombreEq && e.IdEquipo != id && e.IdCategoria == viewModel.IdCategoria);

            if (existeNombre)
            {
                ModelState.AddModelError("NombreEq", "¡Atención! Ese nombre ya lo está usando otro equipo en la categoría.");
            }

            // Limpieza de validaciones automáticas de navegación
            ModelState.Remove("IdCategoriaNavigation");
            ModelState.Remove("NombreCategoria");

            if (!ModelState.IsValid)
            {
                var categorias = await _context.Categorias.ToListAsync();
                ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat", viewModel.IdCategoria);
                return View(viewModel);
            }

            // 2. OBTENER ENTIDAD ORIGINAL
            var equipoDb = await _context.Equipos.FindAsync(id);
            if (equipoDb == null) return NotFound();

            // Mapeo de campos básicos
            equipoDb.NombreEq = viewModel.NombreEq;
            equipoDb.IdCategoria = viewModel.IdCategoria;
            equipoDb.Delegado = viewModel.Delegado;
            equipoDb.Celular = viewModel.Celular;
            equipoDb.Estado = viewModel.Estado;

            // 3. LÓGICA DE OPTIMIZACIÓN DE FOTO (ESCUDO)
            if (viewModel.FotoFile != null && viewModel.FotoFile.Length > 0)
            {
                string folderPath = Path.Combine(_hostEnvironment.WebRootPath, "imagenes", "equipos");

                // --- LIMPIEZA: Eliminar foto anterior si existe y no es la default ---
                if (!string.IsNullOrEmpty(equipoDb.FotoEq) && !equipoDb.FotoEq.Contains("default"))
                {
                    // Quitamos el '/' inicial si existe para evitar problemas de ruta
                    var oldFileName = equipoDb.FotoEq.Replace("/imagenes/equipos/", "").TrimStart('/');
                    string oldPath = Path.Combine(folderPath, oldFileName);

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // --- GUARDAR NUEVA: Procesar con el servicio (WebP + Resize) ---
                // El servicio nos devuelve solo el nombre del archivo: "guid.webp"
                // Pasamos 'false' para que use ResizeMode.Pad
                equipoDb.FotoEq = await _imageService.SubirFotoPerfil(viewModel.FotoFile, folderPath, false);
            }
            else
            {
                // Si no subió foto nueva, mantenemos la que ya tenía (proviene del campo oculto en la vista)
                equipoDb.FotoEq = viewModel.FotoEq;
            }

            // 4. GUARDAR CAMBIOS
            try
            {
                _context.Update(equipoDb);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Equipos.Any(e => e.IdEquipo == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EquipoExists(int id)
        {
            return _context.Equipos.Any(e => e.IdEquipo == id);
        }
    }
}