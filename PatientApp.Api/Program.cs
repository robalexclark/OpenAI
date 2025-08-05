using Microsoft.EntityFrameworkCore;
using PatientApp.Api;
using PatientApp.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<StudyContext>(opt =>
    opt.UseInMemoryDatabase("Study"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudyContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/patients", async (StudyContext db) => await db.Patients.ToListAsync())
   .WithName("GetPatients");

app.MapGet("/patients/{id:guid}", async (Guid id, StudyContext db) =>
    await db.Patients.FindAsync(id) is Patient patient
        ? Results.Ok(patient)
        : Results.NotFound())
   .WithName("GetPatientById");

app.Run();
