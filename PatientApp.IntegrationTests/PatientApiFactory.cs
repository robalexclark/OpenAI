using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PatientApp.Api;
using PatientApp.Shared;
using Microsoft.EntityFrameworkCore.Storage;

namespace PatientApp.IntegrationTests;

public class PatientApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<StudyContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            var root = new InMemoryDatabaseRoot();
            services.AddDbContext<StudyContext>(options =>
                options.UseInMemoryDatabase("IntegrationTests", root));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StudyContext>();
            db.Database.EnsureCreated();
        });
    }
}
