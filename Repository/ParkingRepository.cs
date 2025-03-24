using Microsoft.EntityFrameworkCore;
using SmartParkingBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartParkingBackend.Repository
{
    public class ParkingRepository : IParkingRepository
    {
        private readonly ParkingContext _context;

        public ParkingRepository(ParkingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Parking>> GetAllParkingsAsync()
        {
            return await _context.Parkings.ToListAsync();
        }

        public async Task<Parking?> GetParkingByIdAsync(int id)
        {
            return await _context.Parkings.FindAsync(id);
        }

        public async Task<Parking?> GetParkingWithDetailsAsync(int id)
        {
            return await _context.Parkings
                .Include(p => p.ParkingRows)
                    .ThenInclude(r => r.ParkingSpots)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ParkingExistsAsync(int id)
        {
            return await _context.Parkings.AnyAsync(p => p.Id == id);
        }

        public async Task<OccupancyHistory?> GetLatestOccupancyHistoryAsync(int parkingId)
        {
            return await _context.OccupancyHistories
                .Where(h => h.ParkingId == parkingId)
                .OrderByDescending(h => h.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OccupancyHistory>> GetOccupancyHistoryAsync(int parkingId, int limit = 100)
        {
            return await _context.OccupancyHistories
                .Where(h => h.ParkingId == parkingId)
                .OrderByDescending(h => h.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<OccupancyHistory> AddOccupancyHistoryAsync(OccupancyHistory history)
        {
            await _context.OccupancyHistories.AddAsync(history);
            return history;
        }

        public async Task<ParkingSpot?> UpdateParkingSpotStatusAsync(int parkingId, string rowCode, string spotCode, string status)
        {
            var parking = await GetParkingWithDetailsAsync(parkingId);
            if (parking == null)
                return null;

            var row = parking.ParkingRows.FirstOrDefault(r => r.Code == rowCode);
            if (row == null)
                return null;

            var spot = row.ParkingSpots.FirstOrDefault(s => s.Code == spotCode);
            if (spot == null)
                return null;

            spot.Status = status;
            _context.ParkingSpots.Update(spot);
            
            return spot;
        }

        public async Task<int> CountAvailableSpotsAsync(int parkingId)
        {
            return await _context.ParkingSpots
                .Include(s => s.ParkingRow)
                .Where(s => s.ParkingRow.ParkingId == parkingId && s.Status == "Available")
                .CountAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
} 