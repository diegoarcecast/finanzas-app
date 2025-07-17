// ------------------------------------
// 📌 Importar namespaces necesarios
// ------------------------------------
using Finanzas.API.Data; // Para acceder a AppDbContext (EF Core)
using Microsoft.EntityFrameworkCore; // Para usar UseSqlServer y EF Core
using Microsoft.AspNetCore.Authentication.JwtBearer; // Para JWT Bearer Authentication
using Microsoft.IdentityModel.Tokens; // Para TokenValidationParameters y SymmetricSecurityKey
using System.Text; // Para Encoding (convertir la llave secreta a bytes)


// ------------------------------------
// 📌 Crear el builder de la aplicación
// ------------------------------------
var builder = WebApplication.CreateBuilder(args);

// ------------------------------------
// 📌 Agregar servicios al contenedor (Dependency Injection)
// ------------------------------------
builder.Services.AddControllers(); // Habilita los controladores de API


// ------------------------------------
// 📌 Configurar Entity Framework Core con SQL Server
// ------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// 👉 Esto conecta EF Core a la base de datos usando la cadena de conexión del appsettings.json


// ------------------------------------
// 📌 Configurar Autenticación JWT
// ------------------------------------
// 🔑 Leer la clave secreta desde appsettings.json o usar un valor por defecto
var key = builder.Configuration["Jwt:Key"] ?? "EstaEsUnaLlaveSuperSecretaDePrueba123456789";
var keyBytes = Encoding.UTF8.GetBytes(key); // Convertir la clave a bytes

builder.Services.AddAuthentication(options =>
{
    // Especificamos que el esquema de autenticación por defecto será JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configuración de validación del token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Si quisieras validar el emisor, ponlo en true y configura Issuer
        ValidateAudience = false, // Si quisieras validar la audiencia, ponlo en true y configura Audience
        ValidateLifetime = true, // Verifica que el token no haya expirado
        ValidateIssuerSigningKey = true, // Verifica que la firma del token sea válida
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes), // La clave secreta usada para firmar
        ClockSkew = TimeSpan.Zero // Sin margen de tolerancia para expiración
    };
});


// ------------------------------------
// 📌 Crear la aplicación
// ------------------------------------
var app = builder.Build();


// ------------------------------------
// 📌 Configurar el pipeline HTTP
// ------------------------------------

// Redirige automáticamente a HTTPS si alguien accede por HTTP
app.UseHttpsRedirection();

// 👇 **Muy importante:** Habilitar autenticación ANTES de autorización
app.UseAuthentication();

// Habilitar la autorización en los endpoints
app.UseAuthorization();

// Mapear los controladores a rutas HTTP
app.MapControllers();

// Iniciar la aplicación
app.Run();
