using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model;

public class Prescription
{
    public int PrescriptionId { get; set; }
    [Required]
    public DateTime Date { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    public virtual Patient Patient { get; set; }
    public virtual Doctor Doctor { get; set; }
    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
}