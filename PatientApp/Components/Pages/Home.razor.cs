using Blazorise;
using Microsoft.AspNetCore.Components;
using PatientApp.Shared;

namespace PatientApp.Components.Pages;

public partial class Home
{
    private List<Patient> patients = new();

    private Modal? randomiseModal;
    private Patient? selectedPatient;
    private string modalInitials = string.Empty;

    private bool IsRandomisationComplete => patients.Count(p => p.Pill == Pill.Red) >= 2 && patients.Count(p => p.Pill == Pill.Blue) >= 2;
    private bool IsInitialsValid => !string.IsNullOrWhiteSpace(modalInitials) && modalInitials.Length <= 10 && modalInitials.All(char.IsLetter);

    [Inject]
    private HttpClient Http { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var result = await Http.GetFromJsonAsync<List<Patient>>("patients?sort=AddedAt&desc=true");
        if (result is not null)
        {
            patients = result;
        }
    }

    private async Task ResetData()
    {
        var response = await Http.PostAsync("reset", null);
        if (response.IsSuccessStatusCode)
        {
            var result = await Http.GetFromJsonAsync<List<Patient>>("patients?sort=AddedAt&desc=true");
            patients = result ?? new();
        }
    }

    private Task OpenRandomiseModal(Patient patient)
    {
        selectedPatient = patient;
        modalInitials = string.Empty;
        return randomiseModal?.Show() ?? Task.CompletedTask;
    }

    private async Task ConfirmRandomise()
    {
        if (selectedPatient is null)
        {
            return;
        }

        var response = await Http.PostAsJsonAsync($"patients/{selectedPatient.Id}/randomise", new { Initials = modalInitials });
        if (response.IsSuccessStatusCode)
        {
            var updated = await response.Content.ReadFromJsonAsync<Patient>();
            if (updated is not null)
            {
                var index = patients.FindIndex(p => p.Id == updated.Id);
                if (index >= 0)
                {
                    patients[index] = updated;
                }
            }
        }

        await randomiseModal!.Hide();
        selectedPatient = null;
    }

    private Task CancelRandomise()
    {
        selectedPatient = null;
        return randomiseModal!.Hide();
    }
}
