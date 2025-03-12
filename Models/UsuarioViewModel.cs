namespace Tarea3_Core.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }

        // 🔹 Nombre del Rol en lugar del ID
        public string RolNombre { get; set; }

        // 🔹 Imagen de perfil para mostrarla correctamente
        public string Image { get; set; }
    }
}
