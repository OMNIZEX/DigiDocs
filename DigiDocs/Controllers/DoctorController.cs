using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigiDocs.Models;
using DigiDocs.dtos;
using DigiDocs.Enums;
using Mapster;

namespace DigiDocs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly DigidocsContext _context;

        public DoctorController(DigidocsContext context)
        {
            _context = context;
        }

        #region Examination Management

        // GET: api/doctor/examination/patient/{patientId}
        [HttpGet("examination/patient/{patientId:int}")]
        public async Task<IActionResult> GetPatientExamination(int patientId)
        {
            var patient = await _context.PatientData
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null)
                return NotFound(new { message = "Patient not found" });

            // Get the latest examination for this patient
            var latestExamination = await _context.Examinations
                .Include(e => e.Diagnoses)
                .Include(e => e.PatientSymptoms)
                    .ThenInclude(ps => ps.Symptom)
                        .ThenInclude(s => s.SymptomsCategory)
                .Include(e => e.PatientMedications)
                    .ThenInclude(pm => pm.Medicine)
                .Where(e => e.PatientData.Any(pd => pd.PatientId == patientId))
                .OrderByDescending(e => e.CreatedDateTime)
                .FirstOrDefaultAsync();

            // Prepare symptoms list
            List<object> symptoms;
            if (latestExamination != null)
            {
                symptoms = latestExamination.PatientSymptoms
                    .Select(ps => (object)new
                    {
                        ps.PatientSymId,
                        ps.SymptomId,
                        SymptomName = ps.Symptom.SymptomName,
                        CategoryId = ps.Symptom.SymptomsCategoryId,
                        CategoryName = ps.Symptom.SymptomsCategory?.SymptomsCategoryName,
                        ps.ExaminationId
                    })
                    .ToList();
            }
            else
            {
                symptoms = new List<object>();
            }

            // Prepare medications list
            List<object> medications;
            if (latestExamination != null)
            {
                medications = latestExamination.PatientMedications
                    .Select(pm => (object)new
                    {
                        pm.PatientMedId,
                        pm.MedicineId,
                        MedicineName = pm.Medicine.MedicineName,
                        pm.Dosage,
                        pm.Frequency
                    })
                    .ToList();
            }
            else
            {
                medications = new List<object>();
            }

            var response = new
            {
                patient = new
                {
                    patient.PatientId,
                    patient.Pname,
                    patient.Page,
                    patient.Pgender,
                    patient.Pcomplain,
                    patient.PchronicDisease,
                    patient.PphoneNumber,
                    patient.Paddress
                },
                examination = latestExamination != null ? new
                {
                    latestExamination.ExaminationId,
                    latestExamination.StartAt,
                    latestExamination.EndAt
                } : null,
                symptoms = symptoms,
                diagnosis = latestExamination?.Diagnoses.FirstOrDefault(),
                medications = medications
            };

            return Ok(response);
        }

        // POST: api/doctor/examination/start
        [HttpPost("examination/start")]
        public async Task<IActionResult> StartExamination([FromBody] DoctorExaminationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.userId == null)
                return Unauthorized(new { message = "userId is required" });

            var patient = await _context.PatientData.FindAsync(dto.PatientId);
            if (patient == null)
                return NotFound(new { message = "Patient not found" });

            // Check if there's already an active examination for this patient
            var activeExamination = await _context.Examinations
                .Where(e => e.PatientData.Any(pd => pd.PatientId == dto.PatientId) && e.EndAt == null)
                .FirstOrDefaultAsync();

            if (activeExamination != null)
            {
                return Ok(new
                {
                    message = "Examination already in progress",
                    examinationId = activeExamination.ExaminationId
                });
            }

            var examination = new Examination
            {
                StartAt = DateTime.UtcNow,
                CreatedById = dto.userId.Value,
                LastModifiedById = dto.userId.Value,
                CreatedDateTime = DateTime.UtcNow,
                LastModifiedDateTime = DateTime.UtcNow
            };

            // Add patient to examination relationship
            examination.PatientData.Add(patient);

            _context.Examinations.Add(examination);
            await _context.SaveChangesAsync();

            // Update queue status to InProgress
            var queueItem = await _context.PatientQueues
                .FirstOrDefaultAsync(pq => pq.PatientId == dto.PatientId && pq.Status == QueueStatus.Waiting);

            if (queueItem != null)
            {
                queueItem.Status = QueueStatus.InProgress;
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                message = "Examination started successfully",
                examinationId = examination.ExaminationId,
                patientId = dto.PatientId
            });
        }

        #endregion

        #region Symptoms Management

        // GET: api/doctor/symptoms/categories
        [HttpGet("symptoms/categories")]
        public async Task<IActionResult> GetSymptomCategories()
        {
            var categories = await _context.SymptomsCategories
                .Select(sc => new
                {
                    sc.SymptomsCategoryId,
                    sc.SymptomsCategoryName
                })
                .OrderBy(sc => sc.SymptomsCategoryName)
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/doctor/symptoms/category/{categoryId}
        [HttpGet("symptoms/category/{categoryId:int}")]
        public async Task<IActionResult> GetSymptomsByCategory(int categoryId)
        {
            var symptoms = await _context.Symptoms
                .Where(s => s.SymptomsCategoryId == categoryId)
                .Select(s => new
                {
                    s.SymptomId,
                    s.SymptomName,
                    s.SymptomsCategoryId
                })
                .OrderBy(s => s.SymptomName)
                .ToListAsync();

            return Ok(symptoms);
        }

        //// POST: api/doctor/symptoms/add
        [HttpPost("symptoms/add")]
        public async Task<IActionResult> AddSymptom([FromBody] DoctorPatientSymptomCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.userId == null)
                return Unauthorized(new { message = "userId is required" });

            // Check if symptom already exists for this examination
            var existingSymptom = await _context.PatientSymptoms
                .FirstOrDefaultAsync(ps => ps.PatientId == dto.PatientId
                    && ps.SymptomId == dto.SymptomId
                    && ps.ExaminationId == dto.ExaminationId);

            if (existingSymptom != null)
                return BadRequest(new { message = "Symptom already added" });

            var patientSymptom = new PatientSymptom
            {
                PatientId = dto.PatientId,
                SymptomId = dto.SymptomId,
                ExaminationId = dto.ExaminationId,
                CreatedById = dto.userId.Value,
                LastModifiedById = dto.userId.Value,
                CreatedDateTime = DateTime.UtcNow,
                LastModifiedDateTime = DateTime.UtcNow
            };

            _context.PatientSymptoms.Add(patientSymptom);
            await _context.SaveChangesAsync();

            // Return the symptom with full details
            var symptom = await _context.Symptoms
                .Include(s => s.SymptomsCategory)
                .FirstOrDefaultAsync(s => s.SymptomId == dto.SymptomId);

            return Ok(new
            {
                message = "Symptom added successfully",
                patientSymId = patientSymptom.PatientSymId,
                symptom = new
                {
                    patientSymptom.PatientSymId,
                    symptom.SymptomId,
                    symptom.SymptomName,
                    CategoryName = symptom.SymptomsCategory?.SymptomsCategoryName
                }
            });
        }

        // DELETE: api/doctor/symptoms/{id}
        [HttpDelete("symptoms/{id:int}")]
        public async Task<IActionResult> RemoveSymptom(int id)
        {
            var symptom = await _context.PatientSymptoms.FindAsync(id);
            if (symptom == null)
                return NotFound(new { message = "Symptom not found" });

            _context.PatientSymptoms.Remove(symptom);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Symptom removed successfully" });
        }

        #endregion

        #region Diagnosis Management

        // POST: api/doctor/diagnosis/add
        [HttpPost("diagnosis/add")]
        public async Task<IActionResult> AddDiagnosis([FromBody] DoctorDiagnosisCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.userId == null)
                return Unauthorized(new { message = "userId is required" });

            // Check if diagnosis already exists for this examination
            var existingDiagnosis = await _context.Diagnoses
                .FirstOrDefaultAsync(d => d.ExaminationId == dto.ExaminationId);

            if (existingDiagnosis != null)
            {
                // Update existing diagnosis
                existingDiagnosis.PatientDiagnosis = dto.ClinicalDiagnosis;
                existingDiagnosis.PatientRegInvestigation = dto.RequiredInvestigations;
                existingDiagnosis.LastModifiedById = dto.userId.Value;
                existingDiagnosis.LastModifiedDateTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Diagnosis updated successfully",
                    diagnosisId = existingDiagnosis.DiagnosisId
                });
            }

            var diagnosis = new Diagnosis
            {
                ExaminationId = dto.ExaminationId,
                PatientId = dto.PatientId,
                PatientDiagnosis = dto.ClinicalDiagnosis,
                PatientRegInvestigation = dto.RequiredInvestigations,
                CreatedById = dto.userId.Value,
                LastModifiedById = dto.userId.Value,
                CreatedDateTime = DateTime.UtcNow,
                LastModifiedDateTime = DateTime.UtcNow
            };

            _context.Diagnoses.Add(diagnosis);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Diagnosis added successfully",
                diagnosisId = diagnosis.DiagnosisId
            });
        }

        // PUT: api/doctor/diagnosis/update/{id}
        [HttpPut("diagnosis/update/{id:int}")]
        public async Task<IActionResult> UpdateDiagnosis(int id, [FromBody] DoctorDiagnosisUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.userId == null)
                return Unauthorized(new { message = "userId is required" });

            var diagnosis = await _context.Diagnoses.FindAsync(id);
            if (diagnosis == null)
                return NotFound(new { message = "Diagnosis not found" });

            diagnosis.PatientDiagnosis = dto.ClinicalDiagnosis;
            diagnosis.PatientRegInvestigation = dto.RequiredInvestigations;
            diagnosis.LastModifiedById = dto.userId.Value;
            diagnosis.LastModifiedDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Diagnosis updated successfully" });
        }

        #endregion

        #region Prescription Management

        //// GET: api/doctor/medicines
        [HttpGet("medicines")]
        public async Task<IActionResult> GetMedicines([FromQuery] string? search = null)
        {
            var query = _context.Medicines.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(m => m.MedicineName.Contains(search));
            }

            var medicines = await query
                .Select(m => new
                {
                    m.MedicineId,
                    m.MedicineName
                })
                .OrderBy(m => m.MedicineName)
                .Take(50)
                .ToListAsync();

            return Ok(medicines);
        }

        // POST: api/doctor/prescription/add
        [HttpPost("prescription/add")]
        public async Task<IActionResult> AddMedication([FromBody] DoctorPatientMedicationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.userId == null)
                return Unauthorized(new { message = "userId is required" });

            // Check if medication already exists
            var existingMedication = await _context.PatientMedications
                .FirstOrDefaultAsync(pm => pm.PatientId == dto.PatientId
                    && pm.MedicineId == dto.MedicineId
                    && pm.ExaminationId == dto.ExaminationId);

            if (existingMedication != null)
                return BadRequest(new { message = "Medication already added to prescription" });

            var medication = new PatientMedication
            {
                PatientId = dto.PatientId,
                MedicineId = dto.MedicineId,
                Dosage = dto.Dosage,
                Frequency = dto.Frequency,
                ExaminationId = dto.ExaminationId,
                CreatedById = dto.userId.Value,
                LastModifiedById = dto.userId.Value,
                CreatedDateTime = DateTime.UtcNow,
                LastModifiedDateTime = DateTime.UtcNow
            };

            _context.PatientMedications.Add(medication);
            await _context.SaveChangesAsync();

            // Return medication with medicine name
            var medicine = await _context.Medicines.FindAsync(dto.MedicineId);

            return Ok(new
            {
                message = "Medication added successfully",
                patientMedId = medication.PatientMedId,
                medication = new
                {
                    medication.PatientMedId,
                    medication.MedicineId,
                    MedicineName = medicine?.MedicineName,
                    medication.Dosage,
                    medication.Frequency
                }
            });
        }

        // DELETE: api/doctor/prescription/{id}
        [HttpDelete("prescription/{id:int}")]
        public async Task<IActionResult> RemoveMedication(int id)
        {
            var medication = await _context.PatientMedications.FindAsync(id);
            if (medication == null)
                return NotFound(new { message = "Medication not found" });

            _context.PatientMedications.Remove(medication);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Medication removed successfully" });
        }

        #endregion

        #region Appointment Management

        // POST: api/doctor/appointment/schedule
        [HttpPost("appointment/schedule")]
        public async Task<IActionResult> ScheduleNextAppointment([FromBody] DoctorAppointmentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.userId == null)
                return Unauthorized(new { message = "userId is required" });

            var patient = await _context.PatientData.FindAsync(dto.PatientId);
            if (patient == null)
                return NotFound(new { message = "Patient not found" });

            // Update patient with next appointment date
            patient.PnextAppointment = dto.AppointmentDate;
            patient.LastModifiedById = dto.userId.Value;
            patient.LastModifiedDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Next appointment scheduled successfully" });
        }

        #endregion

        #region Save Complete Examination (All-in-One)

        // POST: api/doctor/examination/save
        [HttpPost("examination/save")]
        public async Task<IActionResult> SaveCompleteExamination([FromBody] DoctorCompleteExaminationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.userId == null)
                return Unauthorized(new { message = "userId is required" });

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Validate Examination exists
                var examination = await _context.Examinations
                    .Include(e => e.PatientSymptoms)
                    .Include(e => e.Diagnoses)
                    .Include(e => e.PatientMedications)
                    .Include(e => e.PatientData)
                    .FirstOrDefaultAsync(e => e.ExaminationId == dto.ExaminationId);

                if (examination == null)
                    return NotFound(new { message = "Examination not found" });

                // Get patient from the examination relationship
                var patient = examination.PatientData.FirstOrDefault();
                if (patient == null)
                    return NotFound(new { message = "Patient not found for this examination" });

                int patientId = patient.PatientId;

                // 2. Handle Symptoms - Remove old ones and add new ones
                if (dto.Symptoms != null && dto.Symptoms.Any())
                {
                    // Remove existing symptoms for this examination
                    var existingSymptoms = examination.PatientSymptoms.ToList();
                    _context.PatientSymptoms.RemoveRange(existingSymptoms);

                    // Add new symptoms
                    foreach (var symptomId in dto.Symptoms)
                    {
                        var patientSymptom = new PatientSymptom
                        {
                            PatientId = patientId,
                            SymptomId = symptomId,
                            ExaminationId = dto.ExaminationId,
                            CreatedById = dto.userId.Value,
                            LastModifiedById = dto.userId.Value,
                            CreatedDateTime = DateTime.UtcNow,
                            LastModifiedDateTime = DateTime.UtcNow
                        };
                        _context.PatientSymptoms.Add(patientSymptom);
                    }
                }

                // 3. Handle Diagnosis
                if (!string.IsNullOrWhiteSpace(dto.ClinicalDiagnosis))
                {
                    var existingDiagnosis = examination.Diagnoses.FirstOrDefault();

                    if (existingDiagnosis != null)
                    {
                        // Update existing
                        existingDiagnosis.PatientDiagnosis = dto.ClinicalDiagnosis;
                        existingDiagnosis.PatientRegInvestigation = dto.RequiredInvestigations;
                        existingDiagnosis.LastModifiedById = dto.userId.Value;
                        existingDiagnosis.LastModifiedDateTime = DateTime.UtcNow;
                    }
                    else
                    {
                        // Create new
                        var diagnosis = new Diagnosis
                        {
                            ExaminationId = dto.ExaminationId,
                            PatientId = patientId,
                            PatientDiagnosis = dto.ClinicalDiagnosis,
                            PatientRegInvestigation = dto.RequiredInvestigations,
                            CreatedById = dto.userId.Value,
                            LastModifiedById = dto.userId.Value,
                            CreatedDateTime = DateTime.UtcNow,
                            LastModifiedDateTime = DateTime.UtcNow
                        };
                        _context.Diagnoses.Add(diagnosis);
                    }
                }

                // 4. Handle Medications - Remove old ones and add new ones
                if (dto.Medications != null && dto.Medications.Any())
                {
                    // Remove existing medications for this examination
                    var existingMedications = examination.PatientMedications.ToList();
                    _context.PatientMedications.RemoveRange(existingMedications);

                    // Add new medications
                    foreach (var medDto in dto.Medications)
                    {
                        var medication = new PatientMedication
                        {
                            PatientId = patientId,
                            MedicineId = medDto.MedicineId,
                            Dosage = medDto.Dosage,
                            Frequency = medDto.Frequency,
                            ExaminationId = dto.ExaminationId,
                            CreatedById = dto.userId.Value,
                            LastModifiedById = dto.userId.Value,
                            CreatedDateTime = DateTime.UtcNow,
                            LastModifiedDateTime = DateTime.UtcNow
                        };
                        _context.PatientMedications.Add(medication);
                    }
                }

                // 5. Handle Next Appointment
                if (dto.NextAppointmentDate.HasValue)
                {
                    patient.PnextAppointment = dto.NextAppointmentDate.Value;
                    patient.LastModifiedById = dto.userId.Value;
                    patient.LastModifiedDateTime = DateTime.UtcNow;
                }

                // 6. Complete Examination
                examination.EndAt = DateTime.UtcNow;
                examination.LastModifiedById = dto.userId.Value;
                examination.LastModifiedDateTime = DateTime.UtcNow;

                // 7. Update Queue Status to Completed
                var queueItem = await _context.PatientQueues
                    .FirstOrDefaultAsync(pq => pq.PatientId == patientId
                        && pq.Status == QueueStatus.InProgress);

                if (queueItem != null)
                {
                    queueItem.Status = QueueStatus.Completed;
                }

                // Save all changes
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Examination saved and completed successfully",
                    examinationId = dto.ExaminationId,
                    patientId = patientId
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    message = "An error occurred while saving the examination",
                    error = ex.Message
                });
            }
        }

        #endregion
    }
}