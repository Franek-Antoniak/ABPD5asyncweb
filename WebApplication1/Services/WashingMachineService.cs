using Microsoft.EntityFrameworkCore;
using WebApplication1.DTOs;
using WebApplication1.Model;

namespace WebApplication1.Services;

public class WashingMachineService : IWashingMachineService
{
    private readonly WashingMachineHeartDbContext _context;

    public WashingMachineService(WashingMachineHeartDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerPurchasesResponseDto?> GetCustomerPurchasesAsync(int customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.PurchaseHistory)
            .ThenInclude(ph => ph.AvailableProgram)
            .ThenInclude(ap => ap.WashingMachine)
            .Include(c => c.PurchaseHistory)
                .ThenInclude(ph => ph.AvailableProgram)
                    .ThenInclude(ap => ap.Program)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (customer == null)
        {
            return null;
        }

        return new CustomerPurchasesResponseDto
        {
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            Purchases = customer.PurchaseHistory.Select(ph => new PurchaseDto
            {
                Date = ph.PurchaseDate,
                Rating = ph.Rating,
                Price = ph.AvailableProgram.Price,
                WashingMachine = new WashingMachineDetailsDto
                {
                    Serial = ph.AvailableProgram.WashingMachine.SerialNumber,
                    MaxWeight = ph.AvailableProgram.WashingMachine.MaxWeight
                },
                Program = new ProgramDetailsDto
                {
                    Name = ph.AvailableProgram.Program.Name,
                    Duration = ph.AvailableProgram.Program.DurationMinutes
                }
            }).ToList()
        };
    }
    
    public async Task<(bool Success, string ErrorMessage)> AddWashingMachineAsync(AddWashingMachineRequestDto request)
    {
        if (request.WashingMachine.MaxWeight < 8)
        {
            return (false, "Maksymalna dopuszczalna waga nie może być mniejsza niż 8.");
        }
        if (request.AvailablePrograms.Any(p => p.Price > 25))
        {
            return (false, "Cena danego programu nie może przekraczać 25.");
        }
        if (await _context.WashingMachines.AnyAsync(wm => wm.SerialNumber == request.WashingMachine.SerialNumber))
        {
            return (false, "Istnieje taka pralka o podanym numerze seryjnym.");
        }

        var programNames = request.AvailablePrograms.Select(p => p.ProgramName).ToList();
        var existingPrograms = await _context.Programs
            .Where(p => programNames.Contains(p.Name))
            .ToListAsync();

        if (existingPrograms.Count != programNames.Distinct().Count())
        {
             return (false, "Nie istnieje program o podanej nazwie.");
        }
        
        var programDict = existingPrograms.ToDictionary(p => p.Name);
        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        var newWashingMachine = new WashingMachine
        {
            MaxWeight = request.WashingMachine.MaxWeight,
            SerialNumber = request.WashingMachine.SerialNumber
        };
        _context.WashingMachines.Add(newWashingMachine);
        await _context.SaveChangesAsync();

        foreach (var prog in request.AvailablePrograms)
        {
            var availableProgram = new AvailableProgram
            {
                WashingMachineId = newWashingMachine.WashingMachineId,
                ProgramId = programDict[prog.ProgramName].ProgramId,
                Price = prog.Price
            };
            _context.AvailablePrograms.Add(availableProgram);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return (true, string.Empty);
    }
}