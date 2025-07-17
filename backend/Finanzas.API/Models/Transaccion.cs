using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finanzas.API.Models
{
    public class Transaccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] // ✅ Especifica el tipo exacto para SQL Server
        public decimal Monto { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Tipo { get; set; } = string.Empty;
        // Ejemplo: "Ingreso" o "Gasto"

        // Relación con Usuario
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
