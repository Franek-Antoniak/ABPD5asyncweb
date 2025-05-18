using System;
using System.Collections.Generic;

namespace WebApplication1.Dtos
{
    public class PatientResponseDto
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public List<PrescriptionForPatientDto> Prescriptions { get; set; }
    }

    public class PrescriptionForPatientDto
    {
        public int PrescriptionId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public List<MedicamentForPrescriptionDto> Medicaments { get; set; }
        public DoctorForPrescriptionDto Doctor { get; set; }
    }

    public class MedicamentForPrescriptionDto
    {
        public int MedicamentId { get; set; }
        public string Name { get; set; }
        public int? Dose { get; set; } 
        public string Description { get; set; }
    }

    public class DoctorForPrescriptionDto
    {
        public int DoctorId { get; set; }
        public string FirstName { get; set; }
    }
}

