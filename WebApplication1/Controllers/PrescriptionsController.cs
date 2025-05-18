using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Dtos;
using WebApplication1.Model;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly PrescriptionDbContext _context;

    public PrescriptionsController(PrescriptionDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionRequestDto dto)
    {
        if (dto.Medicaments.Count > 10)
            return BadRequest("Recepta nie może mieć więcej niż 10 leków.");

        if (dto.DueDate < dto.Date)
            return BadRequest("DueDate nie może być wcześniejszy niż Date.");

        var medicamentIds = dto.Medicaments.Select(m => m.MedicamentId).ToList();
        var existingMedicaments = await _context.Medicaments
            .Where(m => medicamentIds.Contains(m.MedicamentId))
            .Select(m => m.MedicamentId)
            .ToListAsync();

        var missingMedicaments = medicamentIds.Except(existingMedicaments).ToList();
        if (missingMedicaments.Any())
            return BadRequest($"Brakuje leków o Id: {string.Join(", ", missingMedicaments)}");

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p =>
                p.FirstName == dto.Patient.FirstName &&
                p.LastName == dto.Patient.LastName &&
                p.Birthdate == dto.Patient.Birthdate);

        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = dto.Patient.FirstName,
                LastName = dto.Patient.LastName,
                Birthdate = dto.Patient.Birthdate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        var doctor = await _context.Doctors.FindAsync(dto.Doctor.DoctorId);
        if (doctor == null)
            return BadRequest("Lekarz nie istnieje.");

        var prescription = new Prescription
        {
            Date = dto.Date,
            DueDate = dto.DueDate,
            PatientId = patient.PatientId,
            DoctorId = doctor.DoctorId,
            PrescriptionMedicaments = dto.Medicaments.Select(m => new PrescriptionMedicament
            {
                MedicamentId = m.MedicamentId,
                Dose = m.Dose,
                Details = m.Details
            }).ToList()
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Recepta została wystawiona", IdPrescription = prescription.PrescriptionId });
    }

    [HttpGet("patients/{id}")]
    public async Task<IActionResult> GetPatientData(int id)
    {
        var dto = await _context.Patients
            .Where(p => p.PatientId == id)
            .AsNoTracking()
            .Select(p => new PatientResponseDto
            {
                PatientId  = p.PatientId,
                FirstName  = p.FirstName,
                LastName   = p.LastName,
                Birthdate  = p.Birthdate,
                Prescriptions = p.Prescriptions
                    .OrderBy(pr => pr.DueDate)
                    .Select(pr => new PrescriptionForPatientDto
                    {
                        PrescriptionId = pr.PrescriptionId,
                        Date           = pr.Date,
                        DueDate        = pr.DueDate,
                        Doctor = new DoctorForPrescriptionDto
                        {
                            DoctorId   = pr.Doctor.DoctorId,
                            FirstName  = pr.Doctor.FirstName
                        },
                        Medicaments = pr.PrescriptionMedicaments
                            .Select(pm => new MedicamentForPrescriptionDto
                            {
                                MedicamentId = pm.MedicamentId,
                                Name         = pm.Medicament.Name,
                                Dose         = pm.Dose,
                                Description  = pm.Medicament.Description
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return dto is null
            ? NotFound($"Pacjent o Id {id} nie został znaleziony.")
            : Ok(dto);
    }
}
