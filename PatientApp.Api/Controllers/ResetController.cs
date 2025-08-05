using Microsoft.AspNetCore.Mvc;
using PatientApp.Shared;

namespace PatientApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ResetController : ControllerBase
{
    private readonly StudyContext _db;

    public ResetController(StudyContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Reset()
    {
        foreach (var p in _db.Patients)
        {
            p.Pill = Pill.None;
            p.Initials = string.Empty;
            p.AllocatedAt = null;
        }

        await _db.SaveChangesAsync();
        return Ok();
    }
}
