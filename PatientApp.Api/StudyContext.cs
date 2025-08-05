using Microsoft.EntityFrameworkCore;
using PatientApp.Shared;

namespace PatientApp.Api;

public class StudyContext : DbContext
{
    public DbSet<Patient> Patients => Set<Patient>();

    public StudyContext(DbContextOptions<StudyContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasData(
            new Patient { Id = Guid.NewGuid(), Initials = string.Empty, DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow), AddedAt = DateTime.UtcNow, Pill = Pill.None },
            new Patient { Id = Guid.NewGuid(), Initials = string.Empty, DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow), AddedAt = DateTime.UtcNow, Pill = Pill.None },
            new Patient { Id = Guid.NewGuid(), Initials = string.Empty, DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow), AddedAt = DateTime.UtcNow, Pill = Pill.None },
            new Patient { Id = Guid.NewGuid(), Initials = string.Empty, DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow), AddedAt = DateTime.UtcNow, Pill = Pill.None }
        );
    }
}
