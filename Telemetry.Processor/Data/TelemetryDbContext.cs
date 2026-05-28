using Microsoft.EntityFrameworkCore;

namespace Telemetry.Processor.Data;

public sealed class TelemetryDbContext(
    DbContextOptions<TelemetryDbContext> options
) : DbContext(options)
{
    public DbSet<TelemetryReading> TelemetryReadings => Set<TelemetryReading>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelemetryReading>(e =>
        {
            e.HasKey(x => x.Id);
        });
    }
}