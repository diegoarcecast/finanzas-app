using Microsoft.AspNetCore.Mvc;
using Finanzas.API.Data;
using Finanzas.API.Models;
using Microsoft.AspNetCore.Authorization;


namespace Finanzas.API.Controllers
{
    // 📌 Esta clase expone endpoints HTTP para manejar Transacciones
    // Ruta base: api/transaccion
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionController : ControllerBase
    {
        // 🔧 Inyección de dependencias: usamos el AppDbContext para acceder a la BD
        private readonly AppDbContext _context;

        public TransaccionController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/transaccion
        // 🔹 Objetivo: devolver la lista completa de transacciones
        // 🔹 Respuesta: 200 OK con la lista en formato JSON
        [HttpGet]
        public IActionResult GetTransacciones()
        {
            // Obtenemos todas las transacciones de la base de datos
            var transacciones = _context.Transacciones.ToList();

            // Retornamos la lista con código 200
            return Ok(transacciones);
        }

        // ✅ POST: api/transaccion
        // 🔹 Objetivo: crear una nueva transacción asociada a un usuario
        // 🔹 Respuesta:
        //    - 400 BadRequest si el usuario no existe o el modelo es inválido
        //    - 201 Created si se guarda exitosamente
        [HttpPost]
        [Authorize]
        public IActionResult CrearTransaccion([FromBody] Transaccion nueva)
        {
            // 1️⃣ Validar el modelo recibido (usa las DataAnnotations del modelo Transaccion)
            if (!ModelState.IsValid)
            {
                // Si algún campo requerido falta o no cumple validaciones
                return BadRequest(ModelState);
            }

            // 2️⃣ Validar si el UsuarioId asociado existe en la tabla Usuarios
            var usuarioExiste = _context.Usuarios.Any(u => u.Id == nueva.UsuarioId);
            if (!usuarioExiste)
            {
                // Si no existe, devolvemos un error 400 con un mensaje claro
                return BadRequest($"El usuario con Id {nueva.UsuarioId} no existe.");
            }

            // 3️⃣ Guardar la transacción en la base de datos
            _context.Transacciones.Add(nueva);
            _context.SaveChanges();

            // 4️⃣ Responder con código 201 Created y devolver el objeto creado
            return CreatedAtAction(
                nameof(GetTransacciones), // Acción relacionada (GET)
                new { id = nueva.Id },    // Parámetros de la ruta
                nueva                     // Objeto creado
            );
        }
    }
}
