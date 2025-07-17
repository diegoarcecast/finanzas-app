using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Finanzas.API.Models
{
    public class Usuario
    {
        [Key] // Marca Id como clave primaria (opcional, EF ya lo asume por convención)
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no debe superar 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un email válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La clave es obligatoria")]
        public string Clave { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navegación futura hacia transacciones o cuentas
        public ICollection<Transaccion>? Transacciones { get; set; }
        // public ICollection<Cuenta>? Cuentas { get; set; }
    }
}
