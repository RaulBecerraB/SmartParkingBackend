using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmartParkingBackend.Models
{
    public class Row
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int TotalSpaces { get; set; }

        [Required]
        public string Status { get; set; } // "active", "offline", "maintenance"

        // Relación uno a muchos con SensorReadings
        public virtual ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
    }
}