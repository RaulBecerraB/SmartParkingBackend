using System;

namespace SmartParkingBackend.Models
{
    public class ParkingSpot
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Status { get; set; } // Valores: "Available", "Occupied", "Reserved", "OutOfService"

        // Clave foránea
        public int ParkingRowId { get; set; }
        public required ParkingRow ParkingRow { get; set; }
    }
}