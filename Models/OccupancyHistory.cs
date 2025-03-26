using System;

namespace SmartParkingBackend.Models
{
    public class OccupancyHistory
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int OccupiedSpots { get; set; }
        public int TotalSpots { get; set; }

        // Clave foránea
        public int ParkingId { get; set; }
        public required Parking Parking { get; set; }
    }
}