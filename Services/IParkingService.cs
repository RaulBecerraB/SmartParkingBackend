using SmartParkingBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartParkingBackend.Services
{
    public interface IParkingService
    {
        Task<IEnumerable<ParkingDto>> GetParkingsNearbyAsync(double? latitude, double? longitude);
        Task<ParkingDetailDto> GetParkingDetailAsync(int id);
        Task<IEnumerable<OccupancyHistoryDto>> GetParkingHistoryAsync(int id);
        Task<SpotUpdateResultDto> UpdateSpotStatusAsync(int parkingId, string rowCode, string spotCode, string status);
    }
} 