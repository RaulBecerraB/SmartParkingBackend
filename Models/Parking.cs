using System;
using System.Collections.Generic;

namespace SmartParkingBackend.Models
{
    public class Parking
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }

        // Relaciones
        public required ICollection<ParkingRow> ParkingRows { get; set; }
        public required ICollection<OccupancyHistory> OccupancyHistories { get; set; }
    }
}