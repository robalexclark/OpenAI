using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientApp.Api;
using PatientApp.Api.Controllers;
using PatientApp.Shared;
using Xunit;

public class ResetControllerTests
{
    private static StudyContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<StudyContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new StudyContext(options);
    }

    [Fact]
    public async Task Reset_ClearsPatients()
    {
        using var context = CreateContext();
        context.Patients.AddRange(
            new Patient { Id = Guid.NewGuid(), Initials = "AA", Pill = Pill.Red, DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow, AllocatedAt = DateTime.UtcNow },
            new Patient { Id = Guid.NewGuid(), Initials = "BB", Pill = Pill.Blue, DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow, AllocatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var controller = new ResetController(context);

        var result = await controller.Reset();

        Assert.IsType<OkResult>(result);
        var patients = context.Patients.ToList();
        Assert.All(patients, p =>
        {
            Assert.Equal(Pill.None, p.Pill);
            Assert.Equal(string.Empty, p.Initials);
            Assert.Null(p.AllocatedAt);
        });
    }
}

