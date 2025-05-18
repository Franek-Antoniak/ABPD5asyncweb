using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model;

public class Patient
{
    public int PatientId { get; set; }
    [Required, MaxLength(100)]
    public string FirstName { get; set; }

    [Required, MaxLength(100)]
    public string LastName { get; set; }

    [Required]
    public DateTime Birthdate { get; set; }


    public virtual ICollection<Prescription> Prescriptions { get; set; }
}