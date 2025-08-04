var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var patients = new List<Patient>
{
    new Patient(1, "John Doe", new DateOnly(1990, 1, 1)),
    new Patient(2, "Jane Smith", new DateOnly(1985, 5, 23))
};

app.MapGet("/patients", () => patients)
   .WithName("GetPatients");

app.MapGet("/patients/{id:int}", (int id) =>
{
    var patient = patients.FirstOrDefault(p => p.Id == id);
    return patient is not null ? Results.Ok(patient) : Results.NotFound();
})
.WithName("GetPatientById");

app.Run();

record Patient(int Id, string Name, DateOnly DateOfBirth);
