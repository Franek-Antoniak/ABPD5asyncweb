namespace WebApplication1.Model;

public class Prescription
{
    public int PrescriptionId { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }

    public virtual Patient Patient { get; set; }
    public virtual Doctor Doctor { get; set; }
    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
}