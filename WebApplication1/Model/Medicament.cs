using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model;

public class Medicament
{
    public int MedicamentId { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required, MaxLength(100)]
    public string Description { get; set; }

    [Required, MaxLength(100)]
    public string Type { get; set; }

    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
}