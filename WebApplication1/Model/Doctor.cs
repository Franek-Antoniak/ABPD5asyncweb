using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model;

public class Doctor
{
    public int DoctorId { get; set; }
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; }
}