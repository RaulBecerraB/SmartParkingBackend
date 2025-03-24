using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingBackend.DTOs;
using SmartParkingBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartParkingBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingsController : ControllerBase
    {
        private readonly IParkingService _parkingService;
        private readonly ILogger<ParkingsController> _logger;

        public ParkingsController(IParkingService parkingService, ILogger<ParkingsController> logger)
        {
            _parkingService = parkingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingDto>>> GetParkingsNearby([FromQuery] double? latitude, [FromQuery] double? longitude)
        {
            var parkings = await _parkingService.GetParkingsNearbyAsync(latitude, longitude);
            
            if (parkings == null || !parkings.Any())
            {
                return NoContent();
            }
            
            return Ok(parkings);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingDetailDto>> GetParkingDetail(int id)
        {
            var parkingDetail = await _parkingService.GetParkingDetailAsync(id);
            
            if (parkingDetail == null)
            {
                return NotFound($"No se encontró el estacionamiento con ID: {id}");
            }

            return Ok(parkingDetail);
        }

        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<OccupancyHistoryDto>>> GetParkingHistory(int id)
        {
            var history = await _parkingService.GetParkingHistoryAsync(id);
            
            if (history == null)
            {
                return NotFound($"No se encontró el estacionamiento con ID: {id}");
            }
            
            return Ok(history);
        }

        [HttpPut("{parkingId}/rows/{rowCode}/spots/{spotCode}")]
        public async Task<IActionResult> UpdateSpotStatus(
            int parkingId, 
            string rowCode, 
            string spotCode, 
            [FromBody] UpdateSpotStatusRequestDto request)
        {
            // Validación del nuevo estado
            if (string.IsNullOrEmpty(request.Status))
            {
                return BadRequest("El estado del espacio no puede estar vacío");
            }
            
            // Validamos que el estado sea uno de los permitidos
            string[] validStatus = { "Available", "Occupied", "Reserved", "OutOfService" };
            if (!System.Linq.Enumerable.Contains(validStatus, request.Status))
            {
                return BadRequest($"Estado no válido. Valores permitidos: {string.Join(", ", validStatus)}");
            }
            
            var result = await _parkingService.UpdateSpotStatusAsync(parkingId, rowCode, spotCode, request.Status);
            
            if (result == null)
            {
                return NotFound($"No se encontró el espacio especificado");
            }
            
            return Ok(result);
        }
    }
} 