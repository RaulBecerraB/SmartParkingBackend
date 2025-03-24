using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParkingBackend.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using SmartParkingBackend.Services;

namespace SmartParkingBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly ISeedService _seedService;
        private readonly ILogger<SeedController> _logger;

        public SeedController(ISeedService seedService, ILogger<SeedController> logger)
        {
            _seedService = seedService;
            _logger = logger;
        }

        // POST: api/seed/initialize
        [HttpPost("initialize")]
        public async Task<IActionResult> Initialize()
        {
            bool result = await _seedService.InitializeAsync();
            
            if (!result)
            {
                return BadRequest("La base de datos ya tiene datos. Para reiniciar, use el endpoint /reset.");
            }
            
            return Ok("Datos de prueba creados con éxito");
        }

        // POST: api/seed/reset
        [HttpPost("reset")]
        public async Task<IActionResult> Reset()
        {
            bool result = await _seedService.ResetAsync();
            
            if (!result)
            {
                return StatusCode(500, "Error al resetear la base de datos");
            }
            
            return Ok("Todos los datos han sido eliminados");
        }
    }
} 