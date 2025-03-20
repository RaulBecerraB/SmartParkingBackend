using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartParkingBackend.Models
{
    public class SensorReading
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RowId { get; set; }

        [Required]
        public int AvailableSpaces { get; set; }

        [Required]
        public int TotalSpaces { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public string Status { get; set; } // "active", "offline", "maintenance"

        [ForeignKey("RowId")]
        public virtual Row Row { get; set; }
    }
}