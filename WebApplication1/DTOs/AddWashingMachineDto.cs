namespace WebApplication1.DTOs;

public class AddWashingMachineRequestDto
{
    public WashingMachineDataDto WashingMachine { get; set; }
    public List<AvailableProgramRequestDto> AvailablePrograms { get; set; }
}

public class WashingMachineDataDto
{
    public decimal MaxWeight { get; set; }
    public string SerialNumber { get; set; }
}

public class AvailableProgramRequestDto
{
    public string ProgramName { get; set; }
    public decimal Price { get; set; }
}
