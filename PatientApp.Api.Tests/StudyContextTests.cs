using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientApp.Api;
using Xunit;

public class StudyContextTests
{
    [Fact]
    public void OnModelCreating_SeedsPatients()
    {
        var options = new DbContextOptionsBuilder<StudyContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new StudyContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Assert.Equal(4, context.Patients.Count());
    }
}

