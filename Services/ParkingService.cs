using Microsoft.Extensions.Logging;
using SmartParkingBackend.DTOs;
using SmartParkingBackend.Models;
using SmartParkingBackend.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartParkingBackend.Services
{
    public class ParkingService : IParkingService
    {
        private readonly IParkingRepository _repository;
        private readonly ILogger<ParkingService> _logger;
        private readonly ParkingContext _context;

        public ParkingService(IParkingRepository repository, ILogger<ParkingService> logger, ParkingContext context)
        {
            _repository = repository;
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<ParkingDto>> GetParkingsNearbyAsync(double? latitude, double? longitude)
        {
            _logger.LogInformation($"Buscando estacionamientos cercanos a lat:{latitude}, long:{longitude}");

            var parkings = await _repository.GetAllParkingsAsync();

            // En un escenario real, podríamos ordenarlos por distancia o filtrarlos
            // Aquí simularíamos distancias si las coordenadas están presentes
            if (latitude.HasValue && longitude.HasValue)
            {
                _logger.LogInformation("Simulando cálculo de distancias para estacionamientos");
                // Ordenar aleatoriamente como simulación de ordenar por cercanía
                parkings = parkings.OrderBy(p => Guid.NewGuid()).ToList();
            }

            // Mapear a DTOs
            return parkings.Select(p => MapToParkingDto(p));
        }

        public async Task<ParkingDetailDto?> GetParkingDetailAsync(int id)
        {
            _logger.LogInformation($"Obteniendo detalles del estacionamiento con ID: {id}");

            var parking = await _context.Parkings
                .Include(p => p.ParkingRows)
                    .ThenInclude(r => r.ParkingSpots)
                .Include(p => p.OccupancyHistories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parking == null)
            {
                _logger.LogWarning($"Estacionamiento con ID: {id} no encontrado");
                return null;
            }

            var lastOccupancy = parking.OccupancyHistories
                .OrderByDescending(h => h.Timestamp)
                .FirstOrDefault();

            // Mapear a DTO
            return MapToParkingDetailDto(parking, lastOccupancy);
        }

        public async Task<IEnumerable<OccupancyHistoryDto>> GetParkingHistoryAsync(int id)
        {
            _logger.LogInformation($"Obteniendo historial de ocupación para el estacionamiento con ID: {id}");

            var exists = await _repository.ParkingExistsAsync(id);
            if (!exists)
            {
                _logger.LogWarning($"Estacionamiento con ID: {id} no encontrado para historial");
                return null;
            }

            var history = await _repository.GetOccupancyHistoryAsync(id);

            // Mapear a DTOs
            return history.Select(h => MapToOccupancyHistoryDto(h));
        }

        public async Task<SpotUpdateResultDto> UpdateSpotStatusAsync(int parkingId, string rowCode, string spotCode, string status)
        {
            _logger.LogInformation($"Actualizando estado del espacio {spotCode} en fila {rowCode} del estacionamiento {parkingId}");

            // Actualizar el estado del espacio
            var spot = await _repository.UpdateParkingSpotStatusAsync(parkingId, rowCode, spotCode, status);

            if (spot == null)
            {
                _logger.LogWarning($"No se pudo encontrar el espacio para actualizar");
                return null;
            }

            string previousStatus = spot.Status;

            // Guardar los cambios
            if (!await _repository.SaveChangesAsync())
            {
                _logger.LogError("Error al guardar los cambios del espacio");
                return null;
            }

            // Contar espacios disponibles
            int availableSpots = await _repository.CountAvailableSpotsAsync(parkingId);
            int totalSpots = await _repository.CountTotalSpotsAsync(parkingId);
            int occupiedSpots = totalSpots - availableSpots;

            // Crear un nuevo registro en el historial
            var occupancyHistory = new OccupancyHistory
            {
                ParkingId = parkingId,
                Timestamp = DateTime.UtcNow,
                OccupiedSpots = occupiedSpots,
                TotalSpots = totalSpots,
                Parking = await _context.Parkings.FindAsync(parkingId)
            };

            await _repository.AddOccupancyHistoryAsync(occupancyHistory);

            // Guardar los cambios del historial
            if (!await _repository.SaveChangesAsync())
            {
                _logger.LogError("Error al guardar el historial de ocupación");
            }

            _logger.LogInformation($"Espacio actualizado de {previousStatus} a {status}. Espacios disponibles: {availableSpots}");

            // Retornar el resultado
            return new SpotUpdateResultDto
            {
                ParkingId = parkingId,
                RowCode = rowCode,
                SpotCode = spotCode,
                PreviousStatus = previousStatus,
                CurrentStatus = status,
                AvailableSpots = availableSpots,
                Timestamp = occupancyHistory.Timestamp
            };
        }

        public async Task<ParkingDto?> GetParkingAsync(int id)
        {
            var parking = await _context.Parkings
                .Include(p => p.ParkingRows)
                    .ThenInclude(r => r.ParkingSpots)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (parking == null)
                return null;

            return MapToParkingDto(parking);
        }

        public async Task<ParkingDto?> CreateParkingAsync(ParkingDto parkingDto)
        {
            var parking = new Parking
            {
                Name = parkingDto.Name,
                Address = parkingDto.Address,
                ParkingRows = new List<ParkingRow>(),
                OccupancyHistories = new List<OccupancyHistory>()
            };

            _context.Parkings.Add(parking);
            await _context.SaveChangesAsync();

            return MapToParkingDto(parking);
        }

        public async Task<ParkingDto?> UpdateParkingAsync(int id, ParkingDto parkingDto)
        {
            var parking = await _context.Parkings.FindAsync(id);
            if (parking == null)
                return null;

            parking.Name = parkingDto.Name;
            parking.Address = parkingDto.Address;

            await _context.SaveChangesAsync();
            return MapToParkingDto(parking);
        }

        public async Task<bool> DeleteParkingAsync(int id)
        {
            var parking = await _context.Parkings.FindAsync(id);
            if (parking == null)
                return false;

            _context.Parkings.Remove(parking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ParkingBasicDetailDto>> GetAllParkingsDetailAsync()
        {
            _logger.LogInformation("Obteniendo detalles de todos los estacionamientos");

            var parkings = await _repository.GetAllParkingsWithDetailsAsync();

            var result = new List<ParkingBasicDetailDto>();
            foreach (var parking in parkings)
            {
                var lastOccupancy = parking.OccupancyHistories
                    .OrderByDescending(h => h.Timestamp)
                    .FirstOrDefault();

                result.Add(new ParkingBasicDetailDto
                {
                    Id = parking.Id,
                    Name = parking.Name,
                    Address = parking.Address,
                    LastUpdate = lastOccupancy?.Timestamp,
                    AvailableSpots = parking.ParkingRows.Sum(r => r.ParkingSpots.Count(s => s.Status == "Available")),
                    TotalSpots = parking.ParkingRows.Sum(r => r.ParkingSpots.Count)
                });
            }

            return result;
        }

        #region Helper Methods

        private ParkingDto MapToParkingDto(Parking parking)
        {
            return new ParkingDto
            {
                Id = parking.Id,
                Name = parking.Name,
                Address = parking.Address,
                TotalSpots = parking.ParkingRows.Sum(r => r.ParkingSpots.Count),
                AvailableSpots = parking.ParkingRows.Sum(r => r.ParkingSpots.Count(s => s.Status == "Available")),
                Rows = parking.ParkingRows.Select(r => new ParkingRowDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Spots = r.ParkingSpots.Select(s => new ParkingSpotDto
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Status = s.Status
                    }).ToList()
                }).ToList()
            };
        }

        private ParkingDetailDto MapToParkingDetailDto(Parking parking, OccupancyHistory? lastOccupancy)
        {
            return new ParkingDetailDto
            {
                Id = parking.Id,
                Name = parking.Name,
                Address = parking.Address,
                LastUpdate = lastOccupancy?.Timestamp,
                AvailableSpots = parking.ParkingRows.Sum(r => r.ParkingSpots.Count(s => s.Status == "Available")),
                TotalSpots = parking.ParkingRows.Sum(r => r.ParkingSpots.Count),
                Rows = parking.ParkingRows.Select(r => new ParkingRowDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Spots = r.ParkingSpots.Select(s => new ParkingSpotDto
                    {
                        Id = s.Id,
                        Code = s.Code,
                        Status = s.Status
                    }).ToList()
                }).ToList(),
                Status = lastOccupancy != null ?
                    (lastOccupancy.OccupiedSpots == 0 ? "Empty" :
                     lastOccupancy.OccupiedSpots == lastOccupancy.TotalSpots ? "Full" : "PartiallyOccupied")
                    : "Unknown"
            };
        }

        private OccupancyHistoryDto MapToOccupancyHistoryDto(OccupancyHistory history)
        {
            return new OccupancyHistoryDto
            {
                Id = history.Id,
                ParkingId = history.ParkingId,
                Timestamp = history.Timestamp,
                OccupiedSpots = history.OccupiedSpots,
                TotalSpots = history.TotalSpots
            };
        }

        #endregion
    }
}