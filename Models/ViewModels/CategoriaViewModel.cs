// Models/ViewModels/CategoriaViewModel.cs

using System.ComponentModel.DataAnnotations;


// namespace ClubId.Models.ViewModels
// {
        public class CategoriaViewModel
        {
            // IdCategoria solo es necesario para EDITAR (Modificación)
            public int IdCategoria { get; set; }

            [Required(ErrorMessage = "El nombre es obligatorio.")]
            [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
            [Display(Name = "Nombre de la Categoría")]
            public  string NombreCat { get; set; } = string.Empty;

            [Display(Name = "Estado (Activa)")]
            public bool EstadoCat { get; set; } = true; // Valor por defecto para el Alta
        }
//}