using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tarea3_Core.Models
{
    [Table("Usuarios")]
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo Correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo es inválido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        [ForeignKey("Rol")]
        public int RolId { get; set; }

        // 🔹 Agregar la relación con la tabla Roles
        public virtual Roles Rol { get; set; }

        // 🔹 Nueva propiedad para la foto de perfil
        public string Image { get; set; } = "/uploads/default.png"; // Imagen por defecto
    }

}
