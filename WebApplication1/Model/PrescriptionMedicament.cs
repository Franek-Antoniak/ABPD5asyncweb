using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model;

public class PrescriptionMedicament
{
    [Required]
    public int MedicamentId { get; set; }

    [Required]
    public int PrescriptionId { get; set; }

    [Required]
    public int Dose { get; set; }

    [MaxLength(100)]
    public string Details { get; set; }

    public virtual Medicament Medicament { get; set; }
    public virtual Prescription Prescription { get; set; }
}