using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientApp.Api.Controllers;
using PatientApp.Shared;

namespace PatientApp.Api.Tests;

public class PatientsControllerTests
{
    private static StudyContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<StudyContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new StudyContext(options);
    }

    [Fact]
    public async Task GetPatients_ReturnsPatientsSortedByProperty()
    {
        using var context = CreateContext();
        context.Patients.AddRange(
            new Patient { Id = Guid.NewGuid(), Initials = "BB", DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow },
            new Patient { Id = Guid.NewGuid(), Initials = "AA", DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow },
            new Patient { Id = Guid.NewGuid(), Initials = "CC", DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var controller = new PatientsController(context);

        var result = await controller.GetPatients("Initials");

        Assert.Equal(new[] { "AA", "BB", "CC" }, result.Value!.Select(p => p.Initials));
    }

    [Fact]
    public async Task GetPatients_ReturnsDescendingWhenRequested()
    {
        using var context = CreateContext();
        context.Patients.AddRange(
            new Patient { Id = Guid.NewGuid(), Initials = "AA", DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow },
            new Patient { Id = Guid.NewGuid(), Initials = "BB", DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();
        var controller = new PatientsController(context);

        var result = await controller.GetPatients("Initials", desc: true);

        Assert.Equal(new[] { "BB", "AA" }, result.Value!.Select(p => p.Initials));
    }

    [Fact]
    public async Task GetPatient_ReturnsNotFound_WhenPatientMissing()
    {
        using var context = CreateContext();
        var controller = new PatientsController(context);

        var result = await controller.GetPatient(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetPatient_ReturnsPatient_WhenExists()
    {
        using var context = CreateContext();
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            Initials = "AA",
            DateOfBirth = new DateOnly(1980,1,1),
            AddedAt = DateTime.UtcNow
        };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        var controller = new PatientsController(context);

        var result = await controller.GetPatient(patient.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<Patient>(okResult.Value);
        Assert.Equal(patient.Id, returned.Id);
    }

    [Fact]
    public async Task Randomise_AssignsBlueWhenTwoRedsExist()
    {
        using var context = CreateContext();
        context.Patients.AddRange(
            new Patient { Id = Guid.NewGuid(), Initials = "RR1", Pill = Pill.Red, DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow },
            new Patient { Id = Guid.NewGuid(), Initials = "RR2", Pill = Pill.Red, DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow }
        );
        var target = new Patient { Id = Guid.NewGuid(), DateOfBirth = new DateOnly(1980,1,1), AddedAt = DateTime.UtcNow };
        context.Patients.Add(target);
        await context.SaveChangesAsync();
        var controller = new PatientsController(context);

        var result = await controller.Randomise(target.Id, new PatientsController.RandomiseRequest("XY"));

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var updated = Assert.IsType<Patient>(okResult.Value);
        Assert.Equal(Pill.Blue, updated.Pill);
        Assert.Equal("XY", updated.Initials);
        Assert.NotNull(updated.AllocatedAt);
    }

    [Fact]
    public async Task Randomise_ReturnsBadRequest_WhenAlreadyRandomised()
    {
        using var context = CreateContext();
        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            Initials = "AA",
            Pill = Pill.Red,
            DateOfBirth = new DateOnly(1980,1,1),
            AddedAt = DateTime.UtcNow,
            AllocatedAt = DateTime.UtcNow
        };
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        var controller = new PatientsController(context);

        var result = await controller.Randomise(patient.Id, new PatientsController.RandomiseRequest("BB"));

        var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Patient already randomised", badResult.Value);
    }
}

