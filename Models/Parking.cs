using System;
using System.Collections.Generic;

namespace SmartParkingBackend.Models
{
    public class Parking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        
        // Relaciones
        public ICollection<ParkingRow> ParkingRows { get; set; }
        public ICollection<OccupancyHistory> OccupancyHistories { get; set; }
    }
} 