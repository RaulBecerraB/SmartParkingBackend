using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingBackend.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartParkingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly ParkingContext _context;
        private readonly ILogger<SeedController> _logger;

        public SeedController(ParkingContext context, ILogger<SeedController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/seed/initialize
        [HttpPost("initialize")]
        public async Task<IActionResult> Initialize()
        {
            _logger.LogInformation("Inicializando la base de datos con datos de prueba");

            try
            {
                // Verificar si ya hay datos
                if (await _context.Parkings.AnyAsync())
                {
                    _logger.LogWarning("La base de datos ya contiene estacionamientos. No se inicializarán datos de prueba.");
                    return BadRequest("La base de datos ya tiene datos. Para reiniciar, use el endpoint /reset.");
                }

                // Crear estacionamientos de prueba
                var parkings = new[]
                {
                    new Parking 
                    { 
                        Name = "Plaza Central", 
                        Address = "Av. Principal 123"
                    },
                    new Parking 
                    { 
                        Name = "Shopping Mall Parking", 
                        Address = "Calle Comercio 456"
                    },
                    new Parking 
                    { 
                        Name = "Estadio Municipal", 
                        Address = "Av. Deportiva 789"
                    }
                };

                await _context.Parkings.AddRangeAsync(parkings);
                await _context.SaveChangesAsync();

                // Crear filas para cada estacionamiento
                foreach (var parking in parkings)
                {
                    // Cada estacionamiento tiene 3 filas: A, B, C
                    var rows = new[]
                    {
                        new ParkingRow { Code = "A", ParkingId = parking.Id },
                        new ParkingRow { Code = "B", ParkingId = parking.Id },
                        new ParkingRow { Code = "C", ParkingId = parking.Id }
                    };

                    await _context.ParkingRows.AddRangeAsync(rows);
                    await _context.SaveChangesAsync();

                    // Crear espacios para cada fila
                    foreach (var row in rows)
                    {
                        // Cada fila tiene 10 espacios (1-10)
                        var spots = new List<ParkingSpot>();
                        for (int i = 1; i <= 10; i++)
                        {
                            // Asignar estados aleatoriamente para simular un estacionamiento real
                            string[] statuses = { "Available", "Occupied", "Reserved", "OutOfService" };
                            Random random = new Random();
                            string status = statuses[random.Next(statuses.Length)];
                            
                            // Pero intentamos hacer que la mayoría estén disponibles para pruebas
                            if (random.NextDouble() > 0.7) // 70% de probabilidad de estar disponible
                            {
                                status = "Available";
                            }

                            spots.Add(new ParkingSpot
                            {
                                Code = i.ToString("D2"), // 01, 02, etc.
                                Status = status,
                                ParkingRowId = row.Id
                            });
                        }

                        await _context.ParkingSpots.AddRangeAsync(spots);
                        await _context.SaveChangesAsync();
                    }

                    // Crear registros de historial para cada estacionamiento
                    // Simulamos datos de las últimas 24 horas con registros cada hora
                    var now = DateTime.UtcNow;
                    var histories = new List<OccupancyHistory>();
                    
                    for (int i = 24; i >= 0; i--)
                    {
                        // Contamos cuántos espacios disponibles hay ahora en este estacionamiento
                        var availableSpots = await _context.ParkingSpots
                            .Include(s => s.ParkingRow)
                            .Where(s => s.ParkingRow.ParkingId == parking.Id && s.Status == "Available")
                            .CountAsync();
                        
                        // Agregamos algo de aleatoridad para simular cambios en ocupación
                        Random random = new Random();
                        availableSpots = Math.Max(0, availableSpots + random.Next(-5, 6));
                        
                        histories.Add(new OccupancyHistory
                        {
                            ParkingId = parking.Id,
                            Timestamp = now.AddHours(-i),
                            TotalAvailable = availableSpots
                        });
                    }

                    await _context.OccupancyHistories.AddRangeAsync(histories);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("Base de datos inicializada con éxito");
                return Ok("Datos de prueba creados con éxito");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al inicializar la base de datos");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // POST: api/seed/reset
        [HttpPost("reset")]
        public async Task<IActionResult> Reset()
        {
            _logger.LogWarning("Reseteando todos los datos de la base de datos");

            try
            {
                // Eliminar todos los registros de las tablas en orden correcto
                _context.OccupancyHistories.RemoveRange(_context.OccupancyHistories);
                _context.ParkingSpots.RemoveRange(_context.ParkingSpots);
                _context.ParkingRows.RemoveRange(_context.ParkingRows);
                _context.Parkings.RemoveRange(_context.Parkings);
                
                await _context.SaveChangesAsync();

                _logger.LogInformation("Datos eliminados con éxito");
                return Ok("Todos los datos han sido eliminados");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear la base de datos");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
} 