using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartParkingBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingsController : ControllerBase
    {
        private readonly ParkingContext _context;
        private readonly ILogger<ParkingsController> _logger;

        public ParkingsController(ParkingContext context, ILogger<ParkingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/parkings?latitude=...&longitude=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parking>>> GetParkingsNearby([FromQuery] double? latitude, [FromQuery] double? longitude)
        {
            // En una implementación real, usaríamos las coordenadas para filtrar los estacionamientos cercanos
            // Por ahora, simplemente devolvemos todos los estacionamientos
            _logger.LogInformation($"Buscando estacionamientos cercanos a lat:{latitude}, long:{longitude}");
            
            var parkings = await _context.Parkings.ToListAsync();
            
            // En un escenario real, podríamos ordenarlos por distancia o filtrarlos
            // Aquí simularíamos distancias si las coordenadas están presentes
            if (latitude.HasValue && longitude.HasValue)
            {
                _logger.LogInformation("Simulando cálculo de distancias para estacionamientos");
                // Ordenar aleatoriamente como simulación de ordenar por cercanía
                parkings = parkings.OrderBy(p => Guid.NewGuid()).ToList();
            }
            
            return Ok(parkings);
        }

        // GET: api/parkings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Parking>> GetParkingDetail(int id)
        {
            _logger.LogInformation($"Obteniendo detalles del estacionamiento con ID: {id}");
            
            // Consultamos el estacionamiento con sus filas y espacios
            var parking = await _context.Parkings
                .Include(p => p.ParkingRows)
                    .ThenInclude(r => r.ParkingSpots)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (parking == null)
            {
                _logger.LogWarning($"Estacionamiento con ID: {id} no encontrado");
                return NotFound($"No se encontró el estacionamiento con ID: {id}");
            }

            // Obtenemos la última actualización de ocupación
            var lastOccupancy = await _context.OccupancyHistories
                .Where(h => h.ParkingId == id)
                .OrderByDescending(h => h.Timestamp)
                .FirstOrDefaultAsync();

            // Creamos un objeto anónimo para devolver el estacionamiento con información adicional
            var result = new
            {
                parking.Id,
                parking.Name,
                parking.Address,
                LastUpdate = lastOccupancy?.Timestamp,
                AvailableSpots = parking.ParkingRows
                    .SelectMany(r => r.ParkingSpots)
                    .Count(s => s.Status == "Available"),
                TotalSpots = parking.ParkingRows
                    .SelectMany(r => r.ParkingSpots)
                    .Count(),
                Rows = parking.ParkingRows.Select(r => new 
                {
                    r.Id,
                    r.Code,
                    Spots = r.ParkingSpots.Select(s => new 
                    {
                        s.Id,
                        s.Code,
                        s.Status
                    })
                })
            };

            return Ok(result);
        }

        // GET: api/parkings/{id}/history
        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<OccupancyHistory>>> GetParkingHistory(int id)
        {
            _logger.LogInformation($"Obteniendo historial de ocupación para el estacionamiento con ID: {id}");
            
            // Verificamos si el estacionamiento existe
            var parkingExists = await _context.Parkings.AnyAsync(p => p.Id == id);
            if (!parkingExists)
            {
                _logger.LogWarning($"Estacionamiento con ID: {id} no encontrado para historial");
                return NotFound($"No se encontró el estacionamiento con ID: {id}");
            }
            
            // Obtenemos el historial de ocupación ordenado por fecha descendente
            var history = await _context.OccupancyHistories
                .Where(h => h.ParkingId == id)
                .OrderByDescending(h => h.Timestamp)
                .Take(100) // Limitamos a 100 registros para evitar respuestas muy grandes
                .ToListAsync();
            
            // Podríamos agregar más lógica aquí para dashboard, como agrupar por hora/día/semana
            
            return Ok(history);
        }

        // PUT: api/parkings/{parkingId}/rows/{rowCode}/spots/{spotCode}
        [HttpPut("{parkingId}/rows/{rowCode}/spots/{spotCode}")]
        public async Task<IActionResult> UpdateSpotStatus(
            int parkingId, 
            string rowCode, 
            string spotCode, 
            [FromBody] UpdateSpotStatusRequest request)
        {
            _logger.LogInformation($"Actualizando estado del espacio {spotCode} en fila {rowCode} del estacionamiento {parkingId}");
            
            // Validación del nuevo estado
            if (string.IsNullOrEmpty(request.Status))
            {
                return BadRequest("El estado del espacio no puede estar vacío");
            }
            
            // Validamos que el estado sea uno de los permitidos
            string[] validStatus = { "Available", "Occupied", "Reserved", "OutOfService" };
            if (!validStatus.Contains(request.Status))
            {
                return BadRequest($"Estado no válido. Valores permitidos: {string.Join(", ", validStatus)}");
            }
            
            // Obtenemos el estacionamiento
            var parking = await _context.Parkings
                .Include(p => p.ParkingRows)
                    .ThenInclude(r => r.ParkingSpots)
                .FirstOrDefaultAsync(p => p.Id == parkingId);
            
            if (parking == null)
            {
                _logger.LogWarning($"Estacionamiento con ID: {parkingId} no encontrado para actualizar espacio");
                return NotFound($"No se encontró el estacionamiento con ID: {parkingId}");
            }
            
            // Buscamos la fila
            var row = parking.ParkingRows.FirstOrDefault(r => r.Code == rowCode);
            if (row == null)
            {
                _logger.LogWarning($"Fila con código: {rowCode} no encontrada en estacionamiento {parkingId}");
                return NotFound($"No se encontró la fila con código: {rowCode}");
            }
            
            // Buscamos el espacio
            var spot = row.ParkingSpots.FirstOrDefault(s => s.Code == spotCode);
            if (spot == null)
            {
                _logger.LogWarning($"Espacio con código: {spotCode} no encontrado en fila {rowCode}");
                return NotFound($"No se encontró el espacio con código: {spotCode}");
            }
            
            // Guardamos el estado anterior para reportarlo
            string previousStatus = spot.Status;
            
            // Actualizamos el estado
            spot.Status = request.Status;
            
            // Guardamos los cambios
            await _context.SaveChangesAsync();
            
            // Actualizamos el historial de ocupación
            // Contamos cuántos espacios disponibles hay ahora
            int availableSpots = parking.ParkingRows
                .SelectMany(r => r.ParkingSpots)
                .Count(s => s.Status == "Available");
            
            // Creamos un nuevo registro en el historial
            var occupancyHistory = new OccupancyHistory
            {
                ParkingId = parkingId,
                Timestamp = DateTime.UtcNow,
                TotalAvailable = availableSpots
            };
            
            _context.OccupancyHistories.Add(occupancyHistory);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Espacio actualizado de {previousStatus} a {request.Status}. Espacios disponibles: {availableSpots}");
            
            return Ok(new 
            {
                ParkingId = parkingId,
                RowCode = rowCode,
                SpotCode = spotCode,
                PreviousStatus = previousStatus,
                CurrentStatus = request.Status,
                AvailableSpots = availableSpots,
                Timestamp = occupancyHistory.Timestamp
            });
        }
    }

    // Clase para recibir la solicitud de actualización de estado
    public class UpdateSpotStatusRequest
    {
        public string Status { get; set; }
    }
} 