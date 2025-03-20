using Microsoft.EntityFrameworkCore;

namespace SmartParkingBackend.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Row> Rows { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<ParkingStatus> ParkingStatuses { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación entre Row y SensorReading
            modelBuilder.Entity<Row>()
                .HasMany(r => r.SensorReadings)
                .WithOne(sr => sr.Row)
                .HasForeignKey(sr => sr.RowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de índices
            modelBuilder.Entity<SensorReading>()
                .HasIndex(sr => sr.Timestamp);

            modelBuilder.Entity<ParkingStatus>()
                .HasIndex(ps => ps.Timestamp);

            modelBuilder.Entity<Alert>()
                .HasIndex(a => a.Timestamp);
        }
    }
}