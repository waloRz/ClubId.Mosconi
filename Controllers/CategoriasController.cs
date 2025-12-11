// Controllers/CategoriasController.cs

// Asegúrate de inyectar el DbContext en el constructor
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClubId.Data;
using ClubId.Models;
using ClubId.Models.ViewModels;

namespace ClubId.Controllers
{
        public class CategoriasController : Controller
    {
        private readonly LigabdContext _context;

        public CategoriasController(LigabdContext context)
        {
            _context = context;
        }

// GET: Categorias
    public async Task<IActionResult> Index()
    {
        // Obtener todas las categorías de la base de datos
        var categorias = await _context.Categorias.ToListAsync();

        // Mapear la lista de entidades a la lista de ViewModels
        var viewModelList = categorias.Select(c => new CategoriaViewModel
        {
            IdCategoria = c.IdCategorias,
            NombreCat = c.NombreCat,
            EstadoCat = c.EstadoCat
        }).ToList();

        // Devolver la lista de ViewModels a la vista
        return View(viewModelList);
    }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            // Devuelve un ViewModel vacío para llenar el formulario de alta
            return View(new CategoriaViewModel());
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaViewModel viewModel)
        {
            // Validación de unicidad (opcional pero recomendado)
            bool existeNombre = await _context.Categorias.AnyAsync(c => c.NombreCat == viewModel.NombreCat);
            if (existeNombre)
            {
                ModelState.AddModelError("NombreCat", "¡Esa categoría ya existe!");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Mapeo de ViewModel a Entidad de Base de Datos
            var nuevaCategoria = new Categoria 
            {
                NombreCat = viewModel.NombreCat,
                EstadoCat = viewModel.EstadoCat
            };

            _context.Add(nuevaCategoria);
            await _context.SaveChangesAsync();
            
            // Redirigir a la lista de categorías (Index)
            return RedirectToAction(nameof(Index));
        }

    // Controllers/CategoriasController.cs (Continuación)

    // GET: Categorias/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
        {
            return NotFound();
        }

        // Mapeo de Entidad a ViewModel para mostrar en el formulario
        var viewModel = new CategoriaViewModel
        {
            IdCategoria = categoria.IdCategorias,
            NombreCat = categoria.NombreCat,
            EstadoCat = categoria.EstadoCat
        };

        return View(viewModel);
    }

    // POST: Categorias/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoriaViewModel viewModel)
    {
        if (id != viewModel.IdCategoria)
        {
            return NotFound();
        }
        
        // Validación de unicidad (asegura que el nombre nuevo no exista en OTRA categoría)
        bool existeNombre = await _context.Categorias
            .AnyAsync(c => c.NombreCat == viewModel.NombreCat && c.IdCategorias != id);
        
        if (existeNombre)
        {
            ModelState.AddModelError("NombreCat", "¡Esa categoría ya existe!");
        }

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        try
        {
            // Buscar la entidad existente para actualizarla
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            // Aplicar los cambios del ViewModel a la Entidad
            categoria.NombreCat = viewModel.NombreCat;
            categoria.EstadoCat = viewModel.EstadoCat;

            _context.Update(categoria);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Lógica de manejo de errores de concurrencia
            if (!await _context.Categorias.AnyAsync(e => e.IdCategorias == id))
            {
                return NotFound();
            }
            throw;
        }
        
        return RedirectToAction(nameof(Index));
    }


    }
}
