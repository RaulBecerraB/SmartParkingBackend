using System;
using System.ComponentModel.DataAnnotations;

namespace SmartParkingBackend.Models
{
    public class ParkingStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TotalAvailable { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}