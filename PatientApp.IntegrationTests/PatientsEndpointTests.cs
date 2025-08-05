using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PatientApp.Shared;

namespace PatientApp.IntegrationTests;

public class PatientsEndpointTests
{
    [Fact]
    public async Task GetPatients_ReturnsSeededPatients()
    {
        await using var factory = new PatientApiFactory();
        using var client = factory.CreateClient();

        var patients = await client.GetFromJsonAsync<List<Patient>>("/patients");

        Assert.NotNull(patients);
        Assert.Equal(4, patients!.Count);
    }

    [Fact]
    public async Task RandomisePatient_ReturnsAllocatedPill()
    {
        await using var factory = new PatientApiFactory();
        using var client = factory.CreateClient();

        var patients = await client.GetFromJsonAsync<List<Patient>>("/patients");
        var patient = patients!.First();

        var response = await client.PostAsJsonAsync($"/patients/{patient.Id}/randomise", new { Initials = "XY" });
        response.EnsureSuccessStatusCode();

        var updated = await response.Content.ReadFromJsonAsync<Patient>();
        Assert.NotNull(updated);
        Assert.NotEqual(Pill.None, updated!.Pill);
        Assert.Equal("XY", updated.Initials);
    }
}
