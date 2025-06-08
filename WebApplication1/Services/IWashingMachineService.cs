using WebApplication1.DTOs;

namespace WebApplication1.Services;

public interface IWashingMachineService
{
    Task<CustomerPurchasesResponseDto?> GetCustomerPurchasesAsync(int customerId);
    Task<(bool Success, string ErrorMessage)> AddWashingMachineAsync(AddWashingMachineRequestDto request);
}
