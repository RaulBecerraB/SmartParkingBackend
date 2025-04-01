using System;
using Microsoft.EntityFrameworkCore;
using SmartParkingBackend.Models;

namespace SmartParkingBackend.Models
{
    public class ParkingContext : DbContext
    {
        public ParkingContext(DbContextOptions<ParkingContext> options)
            : base(options)
        {
        }

        public DbSet<Parking> Parkings { get; set; }
        public DbSet<ParkingRow> ParkingRows { get; set; }
        public DbSet<ParkingSpot> ParkingSpots { get; set; }
        public DbSet<OccupancyHistory> OccupancyHistories { get; set; }

        // Este método se utiliza solo para herramientas de EF Core (migraciones, etc.)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Obtiene la cadena de conexión desde las variables de entorno
                string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de relaciones con Fluent API si es necesario.
            modelBuilder.Entity<Parking>()
                .HasMany(p => p.ParkingRows)
                .WithOne(r => r.Parking)
                .HasForeignKey(r => r.ParkingId);

            modelBuilder.Entity<ParkingRow>()
                .HasMany(r => r.ParkingSpots)
                .WithOne(s => s.ParkingRow)
                .HasForeignKey(s => s.ParkingRowId);

            modelBuilder.Entity<Parking>()
                .HasMany(p => p.OccupancyHistories)
                .WithOne(h => h.Parking)
                .HasForeignKey(h => h.ParkingId);
        }
    }
}