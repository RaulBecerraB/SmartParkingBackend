using System;
using System.Collections.Generic;

namespace SmartParkingBackend.DTOs
{
    // DTOs para respuestas
    public class ParkingDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public int TotalSpots { get; set; }
        public int AvailableSpots { get; set; }
        public required ICollection<ParkingRowDto> Rows { get; set; }
    }

    public class ParkingDetailDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int AvailableSpots { get; set; }
        public int TotalSpots { get; set; }
        public required ICollection<ParkingRowDto> Rows { get; set; }
        public required string Status { get; set; }
    }

    public class ParkingRowDto
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required ICollection<ParkingSpotDto> Spots { get; set; }
    }

    public class ParkingSpotDto
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Status { get; set; }
    }

    public class OccupancyHistoryDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int OccupiedSpots { get; set; }
        public int TotalSpots { get; set; }
        public int ParkingId { get; set; }
    }

    // DTOs para solicitudes
    public class UpdateSpotStatusRequestDto
    {
        public required string Status { get; set; }
    }

    public class SpotUpdateResultDto
    {
        public int ParkingId { get; set; }
        public required string RowCode { get; set; }
        public required string SpotCode { get; set; }
        public required string PreviousStatus { get; set; }
        public required string CurrentStatus { get; set; }
        public int AvailableSpots { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ParkingBasicDetailDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int AvailableSpots { get; set; }
        public int TotalSpots { get; set; }
    }
}