using Microsoft.AspNetCore.Mvc;
using SmartParkingBackend.Models;

namespace SmartParkingBackend.Controllers.Admin
{
    [ApiController]
    [Route("admin")]
    public class AdminAlertController : ControllerBase
    {
        [HttpPost("alert")]
        public ActionResult<Alert> CreateAlert([FromBody] Alert alert)
        {
            if (string.IsNullOrEmpty(alert.Message))
                return BadRequest("Message is required");

            // Simular creación de alerta
            var newAlert = new Alert
            {
                Id = Random.Shared.Next(1, 1000),
                Message = alert.Message,
                Timestamp = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(CreateAlert), new { id = newAlert.Id }, newAlert);
        }
    }
}