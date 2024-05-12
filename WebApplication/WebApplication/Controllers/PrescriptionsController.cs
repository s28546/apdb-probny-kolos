using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/prescriptions")]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;

    public PrescriptionsController(IPrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }
    
    //CreatePrescriptionRequest - w modelu zamodelowane dane co przychodza na wejsciu
    [HttpPost]
    public async Task<ActionResult<Prescription>> AddPrescription(CreatePrescriptionRequest request)
    {
        if (request.DueDate < request.Date)
            return BadRequest("DueDate earlier than Date.");

        if (request.IdPatient < 1)
            return BadRequest("IdPatient invalid.");
        
        if (request.IdDoctor < 1)
            return BadRequest("IdDoctor invalid.");

        try
        {
            var res = await _prescriptionService.AddPrescription(request);
            return Ok(res);
        }
        catch (Exception e)
        {
            return Conflict("Adding prescription failed.");
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions(string? doctorName)
    {
        var res = await _prescriptionService.GetPrescriptions(doctorName);
        return Ok(res);
    }
}

//walidacja danych, actionResult
//zwracanie statusow czy sie udalo