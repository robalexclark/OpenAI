using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientApp.Shared;

namespace PatientApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController : ControllerBase
{
    private readonly StudyContext _db;

    public PatientsController(StudyContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetPatients([FromQuery] string? sort, [FromQuery] bool desc = false)
    {
        IQueryable<Patient> query = _db.Patients;

        if (!string.IsNullOrWhiteSpace(sort))
        {
            var property = typeof(Patient).GetProperty(sort,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property is not null)
            {
                query = desc
                    ? query.OrderByDescending(p => EF.Property<object>(p, property.Name))
                    : query.OrderBy(p => EF.Property<object>(p, property.Name));
            }
        }

        return await query.ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Patient>> GetPatient(Guid id)
    {
        var patient = await _db.Patients.FindAsync(id);
        return patient is not null ? Ok(patient) : NotFound();
    }

    [HttpPost("{id:guid}/randomise")]
    public async Task<ActionResult<Patient>> Randomise(Guid id, [FromBody] RandomiseRequest request)
    {
        var patient = await _db.Patients.FindAsync(id);
        if (patient is null)
        {
            return NotFound();
        }

        if (patient.Pill != Pill.None)
        {
            return BadRequest("Patient already randomised");
        }

        patient.Initials = request.Initials;
        patient.AllocatedAt = DateTime.UtcNow;

        var redCount = await _db.Patients.CountAsync(p => p.Pill == Pill.Red);
        var blueCount = await _db.Patients.CountAsync(p => p.Pill == Pill.Blue);

        patient.Pill = redCount >= 2
            ? Pill.Blue
            : blueCount >= 2
                ? Pill.Red
                : (Random.Shared.Next(2) == 0 ? Pill.Red : Pill.Blue);

        await _db.SaveChangesAsync();
        return Ok(patient);
    }

    public record RandomiseRequest(string Initials);
}
