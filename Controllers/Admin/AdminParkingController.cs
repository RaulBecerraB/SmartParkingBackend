using Microsoft.AspNetCore.Mvc;
using SmartParkingBackend.Models;

namespace SmartParkingBackend.Controllers.Admin
{
    [ApiController]
    [Route("admin")]
    public class AdminParkingController : ControllerBase
    {
        private static readonly string[] RowNames = new[] { "A", "B", "C", "D", "ASD" };
        private static readonly string[] Statuses = new[] { "active", "offline", "maintenance" };

        [HttpGet("parking-status")]
        public ActionResult<ParkingStatus> GetParkingStatus()
        {
            var mockStatus = new ParkingStatus
            {
                Id = 1,
                TotalAvailable = 45,
                Timestamp = DateTime.UtcNow
            };

            return Ok(mockStatus);
        }

        [HttpGet("row/{rowId}")]
        public ActionResult<Row> GetRow(int rowId)
        {
            if (rowId < 1 || rowId > 4)
                return NotFound();

            var mockRow = new Row
            {
                Id = rowId,
                Name = RowNames[rowId - 1],
                TotalSpaces = 20,
                Status = "active",
                SensorReadings = new List<SensorReading>
                {
                    new SensorReading
                    {
                        Id = 1,
                        RowId = rowId,
                        AvailableSpaces = 12,
                        TotalSpaces = 20,
                        Status = "active",
                        Timestamp = DateTime.UtcNow
                    }
                }
            };

            return Ok(mockRow);
        }

        [HttpGet("history")]
        public ActionResult<IEnumerable<SensorReading>> GetHistory()
        {
            var mockReadings = Enumerable.Range(1, 10).Select(i =>
                new SensorReading
                {
                    Id = i,
                    RowId = (i % 4) + 1,
                    AvailableSpaces = Random.Shared.Next(0, 20),
                    TotalSpaces = 20,
                    Status = "active",
                    Timestamp = DateTime.UtcNow.AddMinutes(-i * 5)
                }).ToList();

            return Ok(mockReadings);
        }

        [HttpPost("row/{rowId}/status")]
        public ActionResult UpdateRowStatus(int rowId, [FromBody] string status)
        {
            if (rowId < 1 || rowId > 4)
                return NotFound();

            if (!Statuses.Contains(status))
                return BadRequest("Invalid status. Must be 'active', 'offline', or 'maintenance'");

            // Simular actualización exitosa
            return Ok(new { message = $"Status updated for row {rowId}" });
        }
    }
}