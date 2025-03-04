using System.ComponentModel.DataAnnotations;

namespace Tarea3_Core.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        public int RolId { get; set; } // 🔹 Asegúrate de que esta propiedad esté en el modelo
    }
}
