using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PatientApp.Shared;

namespace PatientApp.Services;

public class PatientService
{
    private readonly ApiClient _client;

    public PatientService(ApiClient client)
    {
        _client = client;
    }

    public Task<IEnumerable<Patient>?> GetAsync() => _client.GetAsync<IEnumerable<Patient>>("patients");

    public Task<Patient?> RandomiseAsync(Guid id, string initials)
        => _client.PostAsync<Patient>($"patients/{id}/randomise", new { initials });

    public Task ResetAsync() => _client.PostAsync("reset");
}
