using Microsoft.AspNetCore.Mvc;
using Finanzas.API.Data;
using Finanzas.API.Models;
using Finanzas.API.DTOs; // Para usar UsuarioLoginDto
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Finanzas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration; // 👈 Para leer la clave del appsettings.json

        public UsuarioController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ GET: api/usuario
        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            return Ok(usuarios);
        }

        // ✅ POST: api/usuario
        [HttpPost]
        public IActionResult CrearUsuario([FromBody] Usuario nuevo)
        {
            // Validar que el modelo cumpla los [Required], [EmailAddress], etc.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Usuarios.Add(nuevo);
            _context.SaveChanges();

            // Devuelve el usuario creado con código 201 Created
            return CreatedAtAction(nameof(GetUsuarios), new { id = nuevo.Id }, nuevo);
        }

        // ✅ POST: api/usuario/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] UsuarioLoginDto loginDto)
        {
            // 1. Buscar usuario en la base de datos
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == loginDto.Email && u.Clave == loginDto.Clave);

            if (usuario == null)
            {
                return Unauthorized("Credenciales inválidas");
            }

            // 2. Crear los Claims (datos que viajan dentro del token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
                new Claim("id", usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 3. Obtener la clave secreta desde appsettings.json
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Generar el token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])
                ),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 5. Devolver el token y datos básicos del usuario
            return Ok(new
            {
                token = tokenString,
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Email
                }
            });
        }
    }
}
