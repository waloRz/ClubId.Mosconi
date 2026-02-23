using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClubId.Data;
using ClubId.Models;
using ClubId.Models.ViewModels;
using System.IO; // Necesario para Path.Combine y FileStream
using Microsoft.AspNetCore.Hosting; // Necesario para manejar archivos

namespace ClubId.Controllers
{
    public class EquiposController : Controller
    {
        private readonly LigabdContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // Para guardar fotos

        public EquiposController(LigabdContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
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
        public async Task <IActionResult> Create()
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
                ModelState.AddModelError("NombreEq", "¡Ese nombre de equipo ya existe en esta categoria!");            
            }      
    
    //remuevo el IdCategoriaNavigation porque viene null y da un error en el modelo
    ModelState.Remove("IdCategoriaNavigation");
          // 2. Si el ModelState NO es válido después de la validación
    if (!ModelState.IsValid)
    {            
       var categorias = await _context.Categorias.ToListAsync();                    
        ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat",viewModel.IdCategoria);         
         return View(viewModel);     
    }

        // 3. MAPEO DE VIEWMODEL A ENTIDAD DB
        var equipo = new Equipo
        {
            NombreEq = viewModel.NombreEq,
            IdCategoria = viewModel.IdCategoria,
            Delegado = viewModel.Delegado,
            Celular = viewModel.Celular,
            Estado = viewModel.Estado,
            // FotoEq será poblada en el siguiente paso
        };
        
        // 4. LÓGICA DE GUARDADO DE FOTO (Solo si se adjuntó un archivo)
        // NOTA: Asumo que el input type="file" en la vista apunta a FotoFile en el ViewModel.
        // Si tu vista usa asp-for="FotoEq" para el type="file", debes cambiarlo por asp-for="FotoFile"
        if (viewModel.FotoFile != null)
        {
            // a) Definir rutas
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(viewModel.FotoFile.FileName);
            string extension = Path.GetExtension(viewModel.FotoFile.FileName);
            
            // b) Crear nombre único y ruta
            fileName = fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath, "imagenes/equipos/", fileName);
            
            // c) Guardar archivo en disco
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await viewModel.FotoFile.CopyToAsync(fileStream);
            }

            // d) Guardar la ruta en la entidad
            equipo.FotoEq = "/imagenes/equipos/" + fileName;
        }

        // 5. GUARDAR EN LA BASE DE DATOS
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
                // Buscamos si existe otro equipo con el mismo nombre y categoria (excluyendo el que estamos editando)
            
                bool existeNombre = await _context.Equipos
                    .AnyAsync(e => e.NombreEq == viewModel.NombreEq && e.IdEquipo != id && e.IdCategoria == viewModel.IdCategoria);
                if (existeNombre)
                {
                    ModelState.AddModelError("NombreEq", "¡Atención! Ese nombre ya lo está usando otro equipo en la categoria.");
                }
                var categorias = await _context.Categorias.ToListAsync();
                if (categorias == null) 
                {
                    // Manejar error (ej: loguear) o usar una lista vacía para evitar la NRE
                    categorias = new List<Categoria>();
                }
             
             ModelState.Remove("IdCategoriaNavigation");
             ModelState.Remove("NombreCategoria");
                // 2. VERIFICACIÓN DE MODELO
                // Si la validación falla (incluyendo el error de nombre repetido)
                if (!ModelState.IsValid)
                {
                    // Si el ModelState es inválido, recargamos el SelectList y devolvemos la vista con el ViewModel
                    //ViewData["IdCategoria"] = new SelectList(_context.Categorias, "IdCategoria", "NombreCat", viewModel.IdCategoria);
                    // 3. Crear SelectList y enviarlo con el ViewModel
                ViewData["IdCategoria"] = new SelectList(categorias, "IdCategorias", "NombreCat", viewModel.IdCategoria); 
                    return View(viewModel);
                }

                // 3. OBTENER LA ENTIDAD ORIGINAL Y MAIPEAREMOS LOS CAMBIOS
                var equipoDb = await _context.Equipos.FindAsync(id);
                
                if (equipoDb == null) return NotFound();

                // Mapeo de ViewModel a Entidad DB
                equipoDb.NombreEq = viewModel.NombreEq;
                equipoDb.IdCategoria = viewModel.IdCategoria;
                equipoDb.Delegado = viewModel.Delegado;
                equipoDb.Celular = viewModel.Celular;
                equipoDb.Estado = viewModel.Estado;
                // La ruta de la foto actual (FotoEq) la mantenemos en el objeto equipoDb por ahora.

                // 4. LÓGICA DE MANEJO DE FOTO
                
                // a) Si el usuario SUBIÓ UN NUEVO ARCHIVO (asumo que se llama FotoFile en tu ViewModel)
                if (viewModel.FotoFile != null) 
                {
                    // 4.1. Eliminar la foto antigua del disco (Opcional, pero recomendado para limpieza)
                    if (!string.IsNullOrEmpty(equipoDb.FotoEq))
                    {
                        string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, equipoDb.FotoEq.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // 4.2. Guardar la nueva foto
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(viewModel.FotoFile.FileName);
                    string extension = Path.GetExtension(viewModel.FotoFile.FileName);
                    
                    fileName = fileName + "_" + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath, "imagenes/equipos/", fileName);
                    
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await viewModel.FotoFile.CopyToAsync(fileStream);
                    }

                    // 4.3. Actualizar la ruta en la Entidad DB
                    equipoDb.FotoEq = "/imagenes/equipos/" + fileName;
                } 
                else 
                {
                    // b) Si no subió un nuevo archivo:
                    // El campo oculto <input asp-for="FotoEq" /> de la vista ya nos trajo la ruta vieja
                    // en el ViewModel. Lo usamos si el objeto DB tiene un valor.
                    
                    // Si la vista pasó la ruta vieja en el campo oculto FotoEq del ViewModel, la usamos.
                    // Pero dado que el ViewModel está diseñado para el POST, asumiré que la ruta antigua
                    // fue cargada correctamente en el campo oculto y mapeada de vuelta a viewModel.FotoEq.
                    // Como no se subió FotoFile, mantenemos la ruta antigua de la base de datos (equipoDb.FotoEq).
                    // Si el campo oculto <input asp-for="FotoEq" /> está en el HTML, el valor ya se mantiene.
                    // Si usaste el campo oculto como en la vista:
                    equipoDb.FotoEq = viewModel.FotoEq; 
                }

                // 5. GUARDAR CAMBIOS
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