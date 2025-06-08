namespace WebApplication1.DTOs;

public class CustomerPurchasesResponseDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public List<PurchaseDto> Purchases { get; set; }
}

public class PurchaseDto
{
    public DateTime Date { get; set; }
    public int? Rating { get; set; }
    public decimal Price { get; set; }
    public WashingMachineDetailsDto WashingMachine { get; set; }
    public ProgramDetailsDto Program { get; set; }
}

public class WashingMachineDetailsDto
{
    public string Serial { get; set; }
    public decimal MaxWeight { get; set; }
}

public class ProgramDetailsDto
{
    public string Name { get; set; }
    public int Duration { get; set; }
}
