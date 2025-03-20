using Microsoft.AspNetCore.Mvc;
using SmartParkingBackend.Models;

namespace SmartParkingBackend.Controllers.Public
{
    [ApiController]
    [Route("public")]
    public class PublicParkingController : ControllerBase
    {
        private static readonly string[] RowNames = new[] { "A", "B", "C", "D" };

        [HttpGet("parking-status")]
        public ActionResult<object> GetParkingStatus()
        {
            // Para la vista pública, devolvemos un objeto simplificado
            var mockStatus = new
            {
                TotalAvailable = 45,
                LastUpdated = DateTime.UtcNow,
                Rows = new[]
                {
                    new { Name = "A", Available = 12, Total = 20 },
                    new { Name = "B", Available = 15, Total = 20 },
                    new { Name = "C", Available = 8, Total = 20 },
                    new { Name = "D", Available = 10, Total = 20 }
                }
            };

            return Ok(mockStatus);
        }

        [HttpGet("row/{rowId}")]
        public ActionResult<object> GetRow(int rowId)
        {
            if (rowId < 1 || rowId > 4)
                return NotFound();

            // Para la vista pública, devolvemos un objeto simplificado
            var mockRow = new
            {
                Name = RowNames[rowId - 1],
                AvailableSpaces = 12,
                TotalSpaces = 20,
                LastUpdated = DateTime.UtcNow
            };

            return Ok(mockRow);
        }
    }
}