using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MobileBackEnd.Data;
using MobileBackEnd.Repositories;
using MobileBackEnd.Services;

// Punto de entrada de la Web API (ASP.NET Core).
// Orden: 1) registrar servicios en el contenedor de dependencias, 2) construir la app,
// 3) preparar la base de datos, 4) configurar el pipeline HTTP y 5) arrancar.
var builder = WebApplication.CreateBuilder(args);

// SQL Server (tablas: Users, Donors, BloodRequests)
// La cadena de conexión sale de appsettings.json (ConnectionStrings:DefaultConnection).
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Cada repositorio, servicio y DbContext es "scoped": una instancia nueva por petición HTTP.
// Repositories
builder.Services.AddScoped<IBloodRequestRepository, BloodRequestRepository>();
builder.Services.AddScoped<IDonorRepository, DonorRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IBloodRequestService, BloodRequestService>();
builder.Services.AddScoped<IDonorService, DonorService>();
builder.Services.AddScoped<IUserService, UserService>();

// Authentication: el app Flutter envía el ID token de Firebase como "Bearer"
// y aquí se valida contra las llaves públicas de Google (sin Admin SDK).
// ID del proyecto de Firebase; el token solo es válido si fue emitido para este proyecto.
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"]
    ?? throw new InvalidOperationException("Missing configuration 'Firebase:ProjectId'.");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Authority permite descargar automáticamente las llaves públicas de Google para verificar la firma.
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Se comprueba quien emitio el token (issuer), para quien es (audience) y que no haya expirado.
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true
        };
    });
// Habilita el atributo [Authorize] usado en los controladores.
builder.Services.AddAuthorization();

// CORS para el frontend Flutter (desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Desde aquí se configura el pipeline: el orden de los middlewares importa.
var app = builder.Build();

// Aplica migraciones y, en desarrollo, carga datos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.InitializeAsync(context, seedSampleData: app.Environment.IsDevelopment());
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// CORS primero, para que también se respondan las peticiones previas (preflight) del navegador.
app.UseCors("AllowAll");

// Primero se identifica al usuario (lee el token) y después se decide si tiene permiso.
app.UseAuthentication();
app.UseAuthorization();

// Conecta las rutas definidas con [Route]/[Http...] en los controladores.
app.MapControllers();
// Endpoint público para comprobar que el API está viva (la app lo puede usar para probar la conexión).
app.MapGet("/api/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

app.Run();
