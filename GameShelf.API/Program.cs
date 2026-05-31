using GameShelf.API.Data;
using GameShelf.API.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Logging: silencia el warning inofensivo del índice GameGenre ──
builder.Logging.AddFilter(
    "Microsoft.EntityFrameworkCore.Model",
    LogLevel.Error); // oculta dbug/info de EF Model (solo errores reales)

// ── Servicios ──────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "GameShelf API", Version = "v1" });
});

// DbContext — usa LocalDB por defecto para desarrollo local
// Cambia la connection string en appsettings.json si tienes SQL Server Express
builder.Services.AddDbContext<GameShelfDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("GameShelf")
            ?? "Server=(localdb)\\mssqllocaldb;Database=GameShelfDb;Trusted_Connection=True;"
    ));

// CLASE 3 / 4 — Registro del repositorio
// El contenedor sabe: cuando alguien pide IGameRepository → crea un GameRepository
builder.Services.AddScoped<IGameRepository, GameRepository>();

// ── Pipeline ───────────────────────────────────────────────────
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameShelf API v1");
        c.RoutePrefix = "swagger"; // Swagger en /swagger
    });

    // Redirige GET / → /swagger automáticamente (útil en demos)
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

    // Aplica esquema + seed automáticamente al arrancar en desarrollo
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<GameShelfDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Necesario para que WebApplicationFactory funcione en los tests de integración
public partial class Program { }
