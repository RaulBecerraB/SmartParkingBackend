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
                    Address = "Av. Principal 123",
                    ParkingRows = new List<ParkingRow>(),
                    OccupancyHistories = new List<OccupancyHistory>()
                },
                new Parking
                {
                    Name = "Shopping Mall Parking",
                    Address = "Calle Comercio 456",
                    ParkingRows = new List<ParkingRow>(),
                    OccupancyHistories = new List<OccupancyHistory>()
                },
                new Parking
                {
                    Name = "Estadio Municipal",
                    Address = "Av. Deportiva 789",
                    ParkingRows = new List<ParkingRow>(),
                    OccupancyHistories = new List<OccupancyHistory>()
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
                    new ParkingRow
                    {
                        Code = "A",
                        ParkingId = parking.Id,
                        Parking = parking,
                        ParkingSpots = new List<ParkingSpot>()
                    },
                    new ParkingRow
                    {
                        Code = "B",
                        ParkingId = parking.Id,
                        Parking = parking,
                        ParkingSpots = new List<ParkingSpot>()
                    },
                    new ParkingRow
                    {
                        Code = "C",
                        ParkingId = parking.Id,
                        Parking = parking,
                        ParkingSpots = new List<ParkingSpot>()
                    }
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
                            ParkingRowId = row.Id,
                            ParkingRow = row
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

                    var totalSpots = 30; // 3 rows * 10 spots
                    histories.Add(new OccupancyHistory
                    {
                        ParkingId = parking.Id,
                        Timestamp = now.AddHours(-i),
                        OccupiedSpots = totalSpots - availableSpots,
                        TotalSpots = totalSpots,
                        Parking = parking
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

        public static void SeedData(ParkingContext context)
        {
            if (context.Parkings.Any())
            {
                return;
            }

            var parkings = new List<Parking>
            {
                new Parking
                {
                    Name = "Parking Central",
                    Address = "Calle Principal 123",
                    ParkingRows = new List<ParkingRow>(),
                    OccupancyHistories = new List<OccupancyHistory>()
                },
                new Parking
                {
                    Name = "Parking Norte",
                    Address = "Avenida Norte 456",
                    ParkingRows = new List<ParkingRow>(),
                    OccupancyHistories = new List<OccupancyHistory>()
                },
                new Parking
                {
                    Name = "Parking Sur",
                    Address = "Avenida Sur 789",
                    ParkingRows = new List<ParkingRow>(),
                    OccupancyHistories = new List<OccupancyHistory>()
                }
            };

            context.Parkings.AddRange(parkings);
            context.SaveChanges();

            var parkingRows = new List<ParkingRow>();
            foreach (var parking in parkings)
            {
                parkingRows.Add(new ParkingRow
                {
                    Code = "A",
                    ParkingId = parking.Id,
                    Parking = parking,
                    ParkingSpots = new List<ParkingSpot>()
                });
                parkingRows.Add(new ParkingRow
                {
                    Code = "B",
                    ParkingId = parking.Id,
                    Parking = parking,
                    ParkingSpots = new List<ParkingSpot>()
                });
                parkingRows.Add(new ParkingRow
                {
                    Code = "C",
                    ParkingId = parking.Id,
                    Parking = parking,
                    ParkingSpots = new List<ParkingSpot>()
                });
            }

            context.ParkingRows.AddRange(parkingRows);
            context.SaveChanges();

            var parkingSpots = new List<ParkingSpot>();
            foreach (var row in parkingRows)
            {
                for (int i = 1; i <= 10; i++)
                {
                    parkingSpots.Add(new ParkingSpot
                    {
                        Code = $"{row.Code}{i}",
                        Status = "Available",
                        ParkingRowId = row.Id,
                        ParkingRow = row
                    });
                }
            }

            context.ParkingSpots.AddRange(parkingSpots);
            context.SaveChanges();

            var occupancyHistories = new List<OccupancyHistory>();
            foreach (var parking in parkings)
            {
                occupancyHistories.Add(new OccupancyHistory
                {
                    Timestamp = DateTime.UtcNow,
                    OccupiedSpots = 0,
                    TotalSpots = 30,
                    ParkingId = parking.Id,
                    Parking = parking
                });
            }

            context.OccupancyHistories.AddRange(occupancyHistories);
            context.SaveChanges();
        }
    }
}