using System;
using System.ComponentModel.DataAnnotations;

namespace SmartParkingBackend.Models
{
    public class Alert
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}