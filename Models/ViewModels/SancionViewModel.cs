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
        public string NombreCompleto { get; set; } // Ejemplo: Apellido, Nombre
         public string Nombre { get; set; } = null!;
         public string Apellido { get; set; } = null!;
        public string? NombreEq { get; set; } = null!;
        public string Dni { get; set; }
        public string CantidadPartidos { get; set; } // Días, partidos, o tiempo de sanción
        public string MotivoEspecifico { get; set; } // El detalle registrado en SancionxJugador
    }

    // Modelo principal para la vista del Boletín (Lectura)
    public class BoletinSancionesViewModel
    {
        public int IdSanciones { get; set; }        
                public DateTime FechaBoletin { get; set; }
                public int NroFecha { get; set; }
                
                public string nombreCat { get; set; } 
                public string? Comunicado { get; set; } // Un resumen de la sanción

          [Display(Name = "Categoría")]
          [Required(ErrorMessage = "La categoría es obligatoria.")]
          public int IdCategoria { get; set; } 
    
         // ATRIBUTO PARA LA VISTA (Para mostrar el nombre en el Index)
           public string NombreCategoria { get; set; }

          // PROPIEDAD PARA EL DROPDOWN (Solo necesaria en la vista Create/Edit)
          // [NotMapped]
              public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ListaCategorias { get; set; }
       
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
        public string CantidadPartidos { get; set; }
        
        //[Required(ErrorMessage = "Campo obligatorio")]
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
        [Display(Name = "Motivo y/o Informe")]
        public string MotivoEspecifico { get; set; }

        // Datos para visualización en el frontend (no se envían al backend, son temporales)
        public string NombreCompleto { get; set; } 
        public string Nombre { get; set; } 
        public string Apellido { get; set; } 
        public string NombreEq { get; set; } 
        public string Dni { get; set; }
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
        public string nombreCat { get; set; }
        //public string NombreCategoria { get; set; }

    // PROPIEDAD PARA EL DROPDOWN (Solo necesaria en la vista Create/Edit)
   // [NotMapped]
        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ListaCategorias { get; set; }
        
        public int NroFecha { get; set; }

        [Display(Name = "Descripción General del Boletín")]
        [StringLength(1000, ErrorMessage = "Máximo 1000 caracteres")]
        public string Comunicado { get; set; } = null!;

        // Colección dinámica que será llenada en el frontend
        public List<SancionJugadorCreacionItem> JugadoresSancionados { get; set; } = new List<SancionJugadorCreacionItem>();

    }
}