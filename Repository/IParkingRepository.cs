using SmartParkingBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartParkingBackend.Repository
{
    public interface IParkingRepository
    {
        // Parking operations
        Task<IEnumerable<Parking>> GetAllParkingsAsync();
        Task<Parking> GetParkingByIdAsync(int id);
        Task<Parking> GetParkingWithDetailsAsync(int id);
        Task<bool> ParkingExistsAsync(int id);
        
        // OccupancyHistory operations
        Task<OccupancyHistory> GetLatestOccupancyHistoryAsync(int parkingId);
        Task<IEnumerable<OccupancyHistory>> GetOccupancyHistoryAsync(int parkingId, int limit = 100);
        Task<OccupancyHistory> AddOccupancyHistoryAsync(OccupancyHistory history);
        
        // ParkingSpot operations
        Task<ParkingSpot> UpdateParkingSpotStatusAsync(int parkingId, string rowCode, string spotCode, string status);
        Task<int> CountAvailableSpotsAsync(int parkingId);
        
        // Save changes
        Task<bool> SaveChangesAsync();
    }
} 