using SmartParkingBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartParkingBackend.Services
{
    public interface IParkingService
    {
        Task<IEnumerable<ParkingDto>> GetParkingsNearbyAsync(double? latitude, double? longitude);
        Task<ParkingDetailDto?> GetParkingDetailAsync(int id);
        Task<IEnumerable<OccupancyHistoryDto>> GetParkingHistoryAsync(int id);
        Task<SpotUpdateResultDto> UpdateSpotStatusAsync(int parkingId, string rowCode, string spotCode, string status);
        Task<ParkingDto?> GetParkingAsync(int id);
        Task<ParkingDto?> CreateParkingAsync(ParkingDto parkingDto);
        Task<ParkingDto?> UpdateParkingAsync(int id, ParkingDto parkingDto);
        Task<bool> DeleteParkingAsync(int id);
        Task<IEnumerable<ParkingBasicDetailDto>> GetAllParkingsDetailAsync();
    }
}