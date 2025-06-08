using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly IWashingMachineService _service;

    public CustomersController(IWashingMachineService service)
    {
        _service = service;
    }

    [HttpGet("{customerId}/purchases")]
    public async Task<IActionResult> GetCustomerPurchases(int customerId)
    {
        var result = await _service.GetCustomerPurchasesAsync(customerId);
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
