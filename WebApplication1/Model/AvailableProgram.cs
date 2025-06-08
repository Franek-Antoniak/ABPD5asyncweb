using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Model;

public class AvailableProgram
{
    public int AvailableProgramId { get; set; }
    [Required]
    public int WashingMachineId { get; set; }
    [Required]
    public int ProgramId { get; set; }
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    public virtual WashingMachine WashingMachine { get; set; }
    public virtual Program Program { get; set; }
}