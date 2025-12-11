using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Necesario para IFormFile


namespace ClubId.Models.ViewModels

{
    //[Table("Equipo")] // Asegura que mapee a tu tabla
    public partial class EquipoViewModel
    {
        [Key]
        public int IdEquipo { get; set; }

        // --- RELACIÓN CON CATEGORIA ---
        [Display(Name = "Categoría")]
        public int IdCategoria { get; set; } // FK

        [ForeignKey("IdCategoria")]
        public virtual Categoria IdCategoriaNavigation { get; set; } = null!;// Propiedad de navegación

        [Display(Name = "Nombre del Categoria")]
        public string? NombreCategoria  { get; set; } 
        // --- DATOS DEL EQUIPO ---
       // [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre del Equipo")]
        public string NombreEq { get; set; } = null!;

        [Display(Name = "Nombre del Delegado")]
        public string Delegado { get; set; }  = null!;

        [Display(Name = "Celular / Contacto")]
        public string? Celular { get; set; }

        [Display(Name = "Estado (Activo)")]
        public bool Estado { get; set; } // true = Activo, false = Inactivo

        // --- MANEJO DE FOTO ---
        [Display(Name = "Ruta Foto")]
        public string? FotoEq { get; set; } // Guarda la ruta en la BD (ej: "/img/equipos/foto1.jpg")

        [NotMapped] // No va a la base de datos, solo sirve para recibir el archivo en el formulario
        [Display(Name = "Subir Escudo/Foto")]
        public IFormFile? FotoFile { get; set; }

        public virtual ICollection<Equipoxpartido> EquipoxpartidoIdEquipo2Navigations { get; set; } = new List<Equipoxpartido>();

    public virtual ICollection<Equipoxpartido> EquipoxpartidoIdEquipoNavigations { get; set; } = new List<Equipoxpartido>();

    public virtual ICollection<Equipoxtorneo> Equipoxtorneos { get; set; } = new List<Equipoxtorneo>();

    public virtual ICollection<Jgrxequipo> Jgrxequipos { get; set; } = new List<Jgrxequipo>();

    public virtual ICollection<Jueqxsancion> Jueqxsancions { get; set; } = new List<Jueqxsancion>();

     }
}