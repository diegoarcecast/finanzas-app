namespace Finanzas.API.DTOs
{
    public class UsuarioRegisterDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
    }
}
