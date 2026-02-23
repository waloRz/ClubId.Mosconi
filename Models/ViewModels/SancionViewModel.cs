using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClubId.Models.ViewModels
{
    // === Modelos para la Vista de Lectura (Boletin.cshtml) ===

    // Modelo para cada jugador dentro del boletín (Lectura)
    public class JugadorSancionadoViewModel
    {        
         public int IdJugador { get; set; }
         public int IdSanciones { get; set; }
        public string NombreCompleto { get; set; } = null!; // Ejemplo: Apellido, Nombre
         public string Nombre { get; set; } = null!;
         public string Apellido { get; set; } = null!;
        public string NombreEq { get; set; } = null!;
        public required string Dni { get; set; }
        public string CantidadPartidos { get; set; } = null!;// Días, partidos, o tiempo de sanción
        public string MotivoEspecifico { get; set; } = null!; // El detalle registrado en SancionxJugador
    }

    // Modelo principal para la vista del Boletín (Lectura)
    public class BoletinSancionesViewModel
    {
        public int IdSanciones { get; set; }        
                public DateTime FechaBoletin { get; set; }
                public int NroFecha { get; set; }
                
                public string NombreCat { get; set; } = string.Empty;
                public string? Comunicado { get; set; } // Un resumen de la sanción

          [Display(Name = "Categoría")]
          [Required(ErrorMessage = "La categoría es obligatoria.")]
          public int IdCategoria { get; set; } 
    
         // ATRIBUTO PARA LA VISTA (Para mostrar el nombre en el Index)
           public string NombreCategoria { get; set; } = string.Empty;

          // PROPIEDAD PARA EL DROPDOWN (Solo necesaria en la vista Create/Edit)
          // [NotMapped]
              public IEnumerable<SelectListItem> ListaCategorias { get; set; } = Enumerable.Empty<SelectListItem>();
       
        // La lista de todos los jugadores asociados a este boletín
        public List<JugadorSancionadoViewModel> JugadoresSancionados { get; set; } = new List<JugadorSancionadoViewModel>();
    }

    // === Modelos para la Creación (Create.cshtml) ===

    // Modelo para un jugador con su sanción específica (para el formulario de creación)
    public class SancionJugadorCreacionItem
    {
        // Datos del jugador necesarios para el POST (bindeo de modelo)
        //public int IdJugador { get; set; }
        public int IdSanciones { get; set; }
        public int IdJugador { get; set; }
        public int IdEquipo { get; set; }
        
        // Datos específicos de la sanción
      //  [Required(ErrorMessage = "Campo obligatorio")]        
       // [Display(Name = "Partidos")]
        public required string CantidadPartidos { get; set; }
        
        //[Required(ErrorMessage = "Campo obligatorio")]
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        [Display(Name = "Motivo y/o Informe")]
        public string MotivoEspecifico { get; set; }= string.Empty;

        // Datos para visualización en el frontend (no se envían al backend, son temporales)
        public string NombreCompleto { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NombreEq { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        //public string? Articulos { get; set; } = null!;
    }

    // Modelo principal para la creación del Boletín
    public class SancionCreacionViewModel
    {
        public int IdSanciones { get; set;}

        [Required(ErrorMessage = "Campo obligatorio")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del Boletín")]
        public DateTime FechaBoletin { get; set; } = DateTime.Today;

      [Display(Name = "Categoría")]
    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public int IdCategoria { get; set; } 
    
    // ATRIBUTO PARA LA VISTA (Para mostrar el nombre en el Index)
        public string NombreCat { get; set; } = string.Empty;
        //public string NombreCategoria { get; set; }

    // PROPIEDAD PARA EL DROPDOWN (Solo necesaria en la vista Create/Edit)
   // [NotMapped]
        public IEnumerable<SelectListItem> ListaCategorias { get; set; } = Enumerable.Empty<SelectListItem>();
        
        public int NroFecha { get; set; }

        [Display(Name = "Descripción General del Boletín")]
        [StringLength(1000, ErrorMessage = "Máximo 1000 caracteres")]
        public string Comunicado { get; set; } = null!;

        // Colección dinámica que será llenada en el frontend
        public List<SancionJugadorCreacionItem> JugadoresSancionados { get; set; } = new List<SancionJugadorCreacionItem>();

    }
}