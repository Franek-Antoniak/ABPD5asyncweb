namespace WebApplication1.Model;

public class Doctor
{
    public int DoctorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; }
}