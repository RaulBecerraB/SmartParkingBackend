using System;
using System.Collections.Generic;

namespace SmartParkingBackend.DTOs
{
    // DTOs para respuestas
    public class ParkingDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class ParkingDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int AvailableSpots { get; set; }
        public int TotalSpots { get; set; }
        public IEnumerable<ParkingRowDto> Rows { get; set; }
    }

    public class ParkingRowDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public IEnumerable<ParkingSpotDto> Spots { get; set; }
    }

    public class ParkingSpotDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
    }

    public class OccupancyHistoryDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int TotalAvailable { get; set; }
        public int ParkingId { get; set; }
    }

    // DTOs para solicitudes
    public class UpdateSpotStatusRequestDto
    {
        public string Status { get; set; }
    }

    public class SpotUpdateResultDto
    {
        public int ParkingId { get; set; }
        public string RowCode { get; set; }
        public string SpotCode { get; set; }
        public string PreviousStatus { get; set; }
        public string CurrentStatus { get; set; }
        public int AvailableSpots { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 