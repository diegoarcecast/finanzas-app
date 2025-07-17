using Finanzas.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Finanzas.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Transaccion> Transacciones { get; set; }

    }
}
