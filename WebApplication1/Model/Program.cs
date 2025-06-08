using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model;

public class Program
{
    public int ProgramId { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }
    [Required]
    public int DurationMinutes { get; set; }
    [Required]
    public int TemperatureCelsius { get; set; }

    public virtual ICollection<AvailableProgram> AvailablePrograms { get; set; }
}