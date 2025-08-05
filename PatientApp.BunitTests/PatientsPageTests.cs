using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using Bunit;
using PatientApp.Components.Pages;
using PatientApp.Shared;
using Microsoft.Extensions.DependencyInjection;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

namespace PatientApp.BunitTests;

public class PatientsPageTests : TestContext
{
    [Fact]
    public void Shows_randomise_button_for_unallocated_patient()
    {
        // Arrange
        var patients = new[]
        {
            new Patient { Id = Guid.NewGuid(), Initials = "AA", Pill = Pill.None }
        };
        var handler = new FakeHttpMessageHandler(JsonSerializer.Serialize(patients));
        Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
        Services.AddBlazorise().AddBootstrap5Providers().AddFontAwesomeIcons();
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<Patients>();

        // Assert
        var button = cut.Find("button");
        Assert.False(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Disables_randomise_button_when_allocation_complete()
    {
        // Arrange
        var patients = new[]
        {
            new Patient { Id = Guid.NewGuid(), Initials = "AA", Pill = Pill.Red },
            new Patient { Id = Guid.NewGuid(), Initials = "BB", Pill = Pill.Red },
            new Patient { Id = Guid.NewGuid(), Initials = "CC", Pill = Pill.Blue },
            new Patient { Id = Guid.NewGuid(), Initials = "DD", Pill = Pill.Blue },
            new Patient { Id = Guid.NewGuid(), Initials = "EE", Pill = Pill.None }
        };
        var handler = new FakeHttpMessageHandler(JsonSerializer.Serialize(patients));
        Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
        Services.AddBlazorise().AddBootstrap5Providers().AddFontAwesomeIcons();
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<Patients>();

        // Assert
        var button = cut.Find("button");
        Assert.True(button.HasAttribute("disabled"));
    }
}

internal class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly string _response;

    public FakeHttpMessageHandler(string response)
    {
        _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var message = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_response, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(message);
    }
}
