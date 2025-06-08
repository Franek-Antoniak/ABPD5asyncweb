using WebApplication1.Model;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1;

public class WashingMachineHeartDbContext : DbContext
{
    public WashingMachineHeartDbContext(DbContextOptions<WashingMachineHeartDbContext> options) : base(options) { }

    public DbSet<WashingMachine> WashingMachines { get; set; }
    public DbSet<AvailableProgram> AvailablePrograms { get; set; }
    public DbSet<Model.Program> Programs { get; set; }
    public DbSet<PurchaseHistory> PurchaseHistories { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<PurchaseHistory>()
            .HasKey(ph => new { ph.CustomerId, ph.AvailableProgramId });
            
        modelBuilder.Entity<WashingMachine>()
            .HasMany(wm => wm.AvailablePrograms)
            .WithOne(ap => ap.WashingMachine)
            .HasForeignKey(ap => ap.WashingMachineId);
            
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.PurchaseHistory)
            .WithOne(ph => ph.Customer)
            .HasForeignKey(ph => ph.CustomerId);

        modelBuilder.Entity<Model.Program>()
            .HasMany(p => p.AvailablePrograms)
            .WithOne(ap => ap.Program)
            .HasForeignKey(ap => ap.ProgramId);
    
        modelBuilder.Entity<WashingMachine>().ToTable("Washing_Machine");
        modelBuilder.Entity<AvailableProgram>().ToTable("Available_Program");
        modelBuilder.Entity<Model.Program>().ToTable("Program");
        modelBuilder.Entity<PurchaseHistory>().ToTable("Purchase_History");
        modelBuilder.Entity<Customer>().ToTable("Customer");
        
        modelBuilder.Entity<Customer>()
            .HasData(
                new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe", PhoneNumber = "123456789" },
                new Customer { CustomerId = 2, FirstName = "Zofia", LastName = "Nowak", PhoneNumber = "987654321" }
            );

        modelBuilder.Entity<Model.Program>()
            .HasData(
                new Model.Program { ProgramId = 1, Name = "Quick Wash", DurationMinutes = 69, TemperatureCelsius = 30 },
                new Model.Program { ProgramId = 2, Name = "Cotton Cycle", DurationMinutes = 143, TemperatureCelsius = 60 },
                new Model.Program { ProgramId = 3, Name = "Synthetic", DurationMinutes = 90, TemperatureCelsius = 40 }
            );

        modelBuilder.Entity<WashingMachine>()
            .HasData(
                new WashingMachine { WashingMachineId = 1, MaxWeight = 9.23m, SerialNumber = "WM2012/S431/12" },
                new WashingMachine { WashingMachineId = 2, MaxWeight = 12.0m, SerialNumber = "WM2014/S491/28" }
            );
            
        modelBuilder.Entity<AvailableProgram>()
            .HasData(
                new AvailableProgram { AvailableProgramId = 1, WashingMachineId = 1, ProgramId = 1, Price = 12.99m },
                new AvailableProgram { AvailableProgramId = 2, WashingMachineId = 2, ProgramId = 2, Price = 17.29m }
            );
            
        modelBuilder.Entity<PurchaseHistory>()
            .HasData(
                new PurchaseHistory { CustomerId = 1, AvailableProgramId = 1, PurchaseDate = DateTime.Parse("2025-06-03T09:00:00"), Rating = 4 },
                new PurchaseHistory { CustomerId = 1, AvailableProgramId = 2, PurchaseDate = DateTime.Parse("2025-06-04T09:00:00"), Rating = null }
            );
    }
}