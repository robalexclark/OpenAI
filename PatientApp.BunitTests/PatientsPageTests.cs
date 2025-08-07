using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Linq;
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
        var home = new[]
        {
            new Patient { Id = Guid.NewGuid(), Initials = "AA", Pill = Pill.None }
        };
        var handler = new FakeHttpMessageHandler(JsonSerializer.Serialize(home));
        Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
        Services.AddBlazorise().AddBootstrap5Providers().AddFontAwesomeIcons();
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<Home>();

        // Assert
        var button = cut.FindAll("tbody button").First(b => b.TextContent.Contains("Randomise"));
        Assert.False(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Disables_randomise_button_when_allocation_complete()
    {
        // Arrange
        var home = new[]
        {
            new Patient { Id = Guid.NewGuid(), Initials = "AA", Pill = Pill.Red },
            new Patient { Id = Guid.NewGuid(), Initials = "BB", Pill = Pill.Red },
            new Patient { Id = Guid.NewGuid(), Initials = "CC", Pill = Pill.Blue },
            new Patient { Id = Guid.NewGuid(), Initials = "DD", Pill = Pill.Blue },
            new Patient { Id = Guid.NewGuid(), Initials = "EE", Pill = Pill.None }
        };
        var handler = new FakeHttpMessageHandler(JsonSerializer.Serialize(home));
        Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
        Services.AddBlazorise().AddBootstrap5Providers().AddFontAwesomeIcons();
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<Home>();

        // Assert
        var button = cut.FindAll("tbody button").First(b => b.TextContent.Contains("Randomise"));
        Assert.True(button.HasAttribute("disabled"));
    }

    [Fact]
    public void Shows_message_when_all_patients_have_pills()
    {
        // Arrange
        var home = new[]
        {
            new Patient { Id = Guid.NewGuid(), Initials = "AA", Pill = Pill.Red },
            new Patient { Id = Guid.NewGuid(), Initials = "BB", Pill = Pill.Blue }
        };
        var handler = new FakeHttpMessageHandler(JsonSerializer.Serialize(home));
        Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
        Services.AddBlazorise().AddBootstrap5Providers().AddFontAwesomeIcons();
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<Home>();

        // Assert
        Assert.Contains("All patients already have pills", cut.Markup);
    }

    [Fact]
    public void Shows_message_when_no_patients_exist()
    {
        // Arrange
        var home = Array.Empty<Patient>();
        var handler = new FakeHttpMessageHandler(JsonSerializer.Serialize(home));
        Services.AddSingleton(new HttpClient(handler) { BaseAddress = new Uri("http://localhost") });
        Services.AddBlazorise().AddBootstrap5Providers().AddFontAwesomeIcons();
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Act
        var cut = RenderComponent<Home>();

        // Assert
        Assert.Contains("No patients available", cut.Markup);
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