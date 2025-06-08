using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Model;

public class WashingMachine
{
    public int WashingMachineId { get; set; }
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal MaxWeight { get; set; }
    [Required]
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string SerialNumber { get; set; }
    
    public virtual ICollection<AvailableProgram> AvailablePrograms { get; set; }
}