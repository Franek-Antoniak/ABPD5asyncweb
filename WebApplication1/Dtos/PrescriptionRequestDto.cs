using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos
{
    public class PrescriptionRequestDto
    {
        [Required]
        public PatientDto Patient { get; set; }

        [Required]
        public DoctorDto Doctor { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Recepta musi mieć przynajmniej jeden lek.")]
        [MaxLength(10, ErrorMessage = "Recepta nie może mieć więcej niż 10 leków.")]
        public List<MedicamentOnPrescriptionDto> Medicaments { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }

    public class PatientDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }
    }

    public class DoctorDto
    {
        [Required]
        public int DoctorId { get; set; }
    }

    public class MedicamentOnPrescriptionDto
    {
        [Required]
        public int MedicamentId { get; set; }

        [Required]
        public int Dose { get; set; }

        [MaxLength(100)]
        public string Details { get; set; }
    }
}