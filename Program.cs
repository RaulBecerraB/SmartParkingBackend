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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
