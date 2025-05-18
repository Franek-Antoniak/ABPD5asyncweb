using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApplication1.Controllers;
using WebApplication1.Dtos;
using WebApplication1.Model;

namespace WebApplication1.Tests
{
    [TestClass]
    public class PrescriptionsControllerTests
    {
        private PrescriptionDbContext _context;
        private PrescriptionsController _controller;
        private Mock<DbSet<Patient>> _mockPatients;
        private Mock<DbSet<Doctor>> _mockDoctors;
        private Mock<DbSet<Medicament>> _mockMedicaments;
        private Mock<DbSet<Prescription>> _mockPrescriptions;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PrescriptionDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;
            _context = new PrescriptionDbContext(options);

            // Seed initial data
            var doctors = new List<Doctor>
            {
                new Doctor { DoctorId = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" }
            };
            _context.Doctors.AddRange(doctors);

            var medicaments = new List<Medicament>
            {
                new Medicament { MedicamentId = 1, Name = "Med1", Description = "Desc1", Type = "Type1" },
                new Medicament { MedicamentId = 2, Name = "Med2", Description = "Desc2", Type = "Type2" }
            };
            _context.Medicaments.AddRange(medicaments);
            
            var patients = new List<Patient>
            {
                new Patient { PatientId = 1, FirstName = "Alice", LastName = "Smith", Birthdate = new DateTime(1990, 1, 1) }
            };
            _context.Patients.AddRange(patients);


            _context.SaveChanges();

            _controller = new PrescriptionsController(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted(); // Ensure the database is deleted after each test
            _context.Dispose();
        }

        // --- AddPrescription Tests ---

        [TestMethod]
        public async Task AddPrescription_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new PrescriptionRequestDto
            {
                Patient = new PatientDto { FirstName = "New", LastName = "Patient", Birthdate = new DateTime(1980, 5, 5) },
                Doctor = new DoctorDto { DoctorId = 1 },
                Medicaments = new List<MedicamentOnPrescriptionDto>
                {
                    new MedicamentOnPrescriptionDto { MedicamentId = 1, Dose = 10, Details = "Take once daily" }
                },
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(10)
            };

            // Act
            var result = await _controller.AddPrescription(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            // Check if a new patient was created
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.FirstName == "New" && p.LastName == "Patient");
            Assert.IsNotNull(patient);
            // Check if prescription was created
            var prescription = await _context.Prescriptions.Include(p => p.PrescriptionMedicaments).FirstOrDefaultAsync();
            Assert.IsNotNull(prescription);
            Assert.AreEqual(1, prescription.PrescriptionMedicaments.Count);
            Assert.AreEqual(request.Medicaments[0].MedicamentId, prescription.PrescriptionMedicaments.First().MedicamentId);
        }

        [TestMethod]
        public async Task AddPrescription_ExistingPatient_ReturnsOk()
        {
            // Arrange
            var request = new PrescriptionRequestDto
            {
                Patient = new PatientDto { FirstName = "Alice", LastName = "Smith", Birthdate = new DateTime(1990, 1, 1) }, // Existing patient
                Doctor = new DoctorDto { DoctorId = 1 },
                Medicaments = new List<MedicamentOnPrescriptionDto>
                {
                    new MedicamentOnPrescriptionDto { MedicamentId = 1, Dose = 10, Details = "Take once daily" }
                },
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(10)
            };
            var initialPatientCount = await _context.Patients.CountAsync();

            // Act
            var result = await _controller.AddPrescription(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var patientCountAfter = await _context.Patients.CountAsync();
            Assert.AreEqual(initialPatientCount, patientCountAfter); // No new patient should be added
        }

        [TestMethod]
        public async Task AddPrescription_TooManyMedicaments_ReturnsBadRequest()
        {
            // Arrange
            var request = new PrescriptionRequestDto
            {
                Patient = new PatientDto { FirstName = "Test", LastName = "User", Birthdate = new DateTime(2000, 1, 1) },
                Doctor = new DoctorDto { DoctorId = 1 },
                Medicaments = new List<MedicamentOnPrescriptionDto>(Enumerable.Range(1, 11).Select(i => new MedicamentOnPrescriptionDto { MedicamentId = 1, Dose = i, Details = "Detail" + i })),
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(10)
            };

            // Act
            var result = await _controller.AddPrescription(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Recepta nie może mieć więcej niż 10 leków.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddPrescription_DueDateBeforeDate_ReturnsBadRequest()
        {
            // Arrange
            var request = new PrescriptionRequestDto
            {
                Patient = new PatientDto { FirstName = "Test", LastName = "User", Birthdate = new DateTime(2000, 1, 1) },
                Doctor = new DoctorDto { DoctorId = 1 },
                Medicaments = new List<MedicamentOnPrescriptionDto> { new MedicamentOnPrescriptionDto { MedicamentId = 1, Dose = 1, Details = "Detail" } },
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(-1) // Invalid DueDate
            };

            // Act
            var result = await _controller.AddPrescription(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("DueDate nie może być wcześniejszy niż Date.", badRequestResult.Value);
        }

        [TestMethod]
        public async Task AddPrescription_MedicamentNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new PrescriptionRequestDto
            {
                Patient = new PatientDto { FirstName = "Test", LastName = "User", Birthdate = new DateTime(2000, 1, 1) },
                Doctor = new DoctorDto { DoctorId = 1 },
                Medicaments = new List<MedicamentOnPrescriptionDto> { new MedicamentOnPrescriptionDto { MedicamentId = 999, Dose = 1, Details = "Detail" } }, // Non-existent medicament
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(10)
            };

            // Act
            var result = await _controller.AddPrescription(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Brakuje leków o Id: 999", badRequestResult.Value);
        }
        
        [TestMethod]
        public async Task AddPrescription_DoctorNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new PrescriptionRequestDto
            {
                Patient = new PatientDto { FirstName = "Test", LastName = "User", Birthdate = new DateTime(2000, 1, 1) },
                Doctor = new DoctorDto { DoctorId = 999 }, // Non-existent doctor
                Medicaments = new List<MedicamentOnPrescriptionDto> { new MedicamentOnPrescriptionDto { MedicamentId = 1, Dose = 1, Details = "Detail" } },
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(10)
            };

            // Act
            var result = await _controller.AddPrescription(request);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("Lekarz nie istnieje.", badRequestResult.Value);
        }

        // --- GetPatientData Tests ---
        [TestMethod]
        public async Task GetPatientData_PatientExists_ReturnsOkWithPatientData()
        {
            // Arrange
            var patientId = 1; // Existing patient seeded in Setup
             _context.Prescriptions.Add(new Prescription
            {
                PrescriptionId = 1,
                Date = new DateTime(2023, 1, 1),
                DueDate = new DateTime(2023, 1, 10),
                PatientId = patientId,
                DoctorId = 1,
                PrescriptionMedicaments = new List<PrescriptionMedicament>
                {
                    new PrescriptionMedicament { MedicamentId = 1, Dose = 10, Details = "M1 Details" }
                }
            });
            _context.Prescriptions.Add(new Prescription
            {
                PrescriptionId = 2,
                Date = new DateTime(2023, 1, 5),
                DueDate = new DateTime(2023, 1, 15), // Later DueDate
                PatientId = patientId,
                DoctorId = 1,
                 PrescriptionMedicaments = new List<PrescriptionMedicament>
                {
                    new PrescriptionMedicament { MedicamentId = 2, Dose = 5, Details = "M2 Details" }
                }
            });
            await _context.SaveChangesAsync();


            // Act
            var result = await _controller.GetPatientData(patientId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var patientData = okResult.Value as PatientResponseDto;
            Assert.IsNotNull(patientData);
            Assert.AreEqual(patientId, patientData.PatientId);
            Assert.AreEqual("Alice", patientData.FirstName);
            Assert.AreEqual(2, patientData.Prescriptions.Count);
            // Check sorting by DueDate (ascending)
            Assert.AreEqual(new DateTime(2023, 1, 10), patientData.Prescriptions[0].DueDate);
            Assert.AreEqual(new DateTime(2023, 1, 15), patientData.Prescriptions[1].DueDate);
            Assert.AreEqual(1, patientData.Prescriptions[0].Medicaments.Count);
            Assert.AreEqual("Med1", patientData.Prescriptions[0].Medicaments[0].Name);
            Assert.AreEqual(10, patientData.Prescriptions[0].Medicaments[0].Dose);
            Assert.AreEqual(1, patientData.Prescriptions[0].Doctor.DoctorId);
            Assert.AreEqual("John", patientData.Prescriptions[0].Doctor.FirstName);
        }

        [TestMethod]
        public async Task GetPatientData_PatientNotExists_ReturnsNotFound()
        {
            // Arrange
            var patientId = 999; // Non-existent patient

            // Act
            var result = await _controller.GetPatientData(patientId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual($"Pacjent o Id {patientId} nie został znaleziony.", notFoundResult.Value);
        }
        
        [TestMethod]
        public async Task GetPatientData_PatientExists_NoPrescriptions_ReturnsOkWithEmptyPrescriptions()
        {
            // Arrange
            var newPatient = new Patient { PatientId = 2, FirstName = "Bob", LastName = "Brown", Birthdate = new DateTime(1995, 5, 5) };
            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();
            var patientId = newPatient.PatientId;
        
            // Act
            var result = await _controller.GetPatientData(patientId);
        
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            var patientData = okResult.Value as PatientResponseDto;
            Assert.IsNotNull(patientData);
            Assert.AreEqual(patientId, patientData.PatientId);
            Assert.AreEqual("Bob", patientData.FirstName);
            Assert.AreEqual(0, patientData.Prescriptions.Count); // Should have no prescriptions
        }
    }
}

