namespace WebApplication1.Model;

public class Medicament
{
    public int MedicamentId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }

    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
}