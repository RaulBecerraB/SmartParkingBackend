using System;
using System.Collections.Generic;

namespace SmartParkingBackend.Models
{
    public class ParkingRow
    {
        public int Id { get; set; }
        public string Code { get; set; }
        
        // Clave foránea
        public int ParkingId { get; set; }
        public Parking Parking { get; set; }
        
        // Relación
        public ICollection<ParkingSpot> ParkingSpots { get; set; }
    }
} 