using Microsoft.EntityFrameworkCore;
using SmartParkingBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartParkingBackend.Repository
{
    public class SeedRepository : ISeedRepository
    {
        private readonly ParkingContext _context;

        public SeedRepository(ParkingContext context)
        {
            _context = context;
        }

        public async Task<bool> DatabaseHasDataAsync()
        {
            return await _context.Parkings.AnyAsync();
        }

        public async Task InitializeTestDataAsync()
        {
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
        }

        public async Task ResetDatabaseAsync()
        {
            // Eliminar todos los registros de las tablas en orden correcto
            _context.OccupancyHistories.RemoveRange(_context.OccupancyHistories);
            _context.ParkingSpots.RemoveRange(_context.ParkingSpots);
            _context.ParkingRows.RemoveRange(_context.ParkingRows);
            _context.Parkings.RemoveRange(_context.Parkings);
            
            await _context.SaveChangesAsync();
        }
    }
} 