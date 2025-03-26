using System;
using System.Collections.Generic;

namespace SmartParkingBackend.Models
{
    public class ParkingRow
    {
        public int Id { get; set; }
        public required string Code { get; set; }

        // Clave foránea
        public int ParkingId { get; set; }
        public required Parking Parking { get; set; }

        // Relación
        public required ICollection<ParkingSpot> ParkingSpots { get; set; }
    }
}