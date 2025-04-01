using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using SmartParkingBackend.Models;
using SmartParkingBackend.Repository;
using SmartParkingBackend.Services;

// Cargar variables de entorno desde el archivo .env
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde el archivo .env o la configuración
string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Server=localhost;Port=3306;Database=SmartParking;User=root;Password=admin1;";

// Add services to the container.
builder.Services.AddDbContext<ParkingContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registrar repositorios
builder.Services.AddScoped<IParkingRepository, ParkingRepository>();
builder.Services.AddScoped<ISeedRepository, SeedRepository>();

// Registrar servicios
builder.Services.AddScoped<IParkingService, ParkingService>();
builder.Services.AddScoped<ISeedService, SeedService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParkingContext>();
    var maxRetries = 5;
    var retryDelay = 10; // segundos
    var retryCount = 0;

    while (retryCount < maxRetries)
    {
        try
        {
            // Intentar aplicar migraciones
            db.Database.Migrate();
            break;
        }
        catch (Exception)
        {
            retryCount++;
            if (retryCount == maxRetries)
            {
                // Si hay un error, probablemente las tablas ya existen pero no hay historial de migraciones
                // En este caso, eliminamos la base de datos y la recreamos
                db.Database.EnsureDeleted();
                db.Database.Migrate();
                break;
            }
            Console.WriteLine($"Retrying database connection in {retryDelay} seconds... (Attempt {retryCount}/{maxRetries})");
            Thread.Sleep(retryDelay * 1000);
        }
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Parking API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
