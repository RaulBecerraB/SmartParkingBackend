using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SmartParkingBackend.Models;
using SmartParkingBackend.Repository;
using SmartParkingBackend.Services;

// Cargar variables de entorno desde el archivo .env
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde el archivo .env
string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

// Add services to the container.
builder.Services.AddDbContext<ParkingContext>(options =>
    options.UseSqlServer(connectionString));

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

    // Asegurarnos de crear la base de datos si no existe
    db.Database.EnsureCreated();

    // Aplicar cualquier migración pendiente
    if (db.Database.GetPendingMigrations().Any())
    {
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // En producción también queremos Swagger disponible
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Parking API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
