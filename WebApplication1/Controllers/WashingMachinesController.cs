using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[Route("api/washing-machines")]
[ApiController]
public class WashingMachinesController(IWashingMachineService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddWashingMachine(AddWashingMachineRequestDto request)
    {
        var (success, errorMessage) = await service.AddWashingMachineAsync(request);

        if (!success)
        {
            return BadRequest(new { message = errorMessage });
        }

        return StatusCode(201);
    }
}
