using System;

namespace SmartParkingBackend.Models
{
    public class OccupancyHistory
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int TotalAvailable { get; set; }
        
        // Clave foránea
        public int ParkingId { get; set; }
        public Parking Parking { get; set; }
    }
} 