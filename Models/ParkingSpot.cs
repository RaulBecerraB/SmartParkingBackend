using System;

namespace SmartParkingBackend.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Status { get; set; } // Valores: "Available", "Occupied", "Reserved", "OutOfService"
        
        // Clave foránea
        public int ParkingRowId { get; set; }
        public ParkingRow ParkingRow { get; set; }
    }
} 