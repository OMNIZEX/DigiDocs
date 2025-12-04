using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigiDocs.Models;
using DigiDocs.dtos;
using DigiDocs.Enums;

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

        // POST: api/doctor/SaveExamination
        // Creates or updates an Examination and related records: Diagnosis, PatientSymptoms, PatientMedications.
        [HttpPost("SaveExamination")]
        public async Task<IActionResult> SaveExamination([FromBody] DoctorExaminationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.UserId is null)
                return Unauthorized(new { message = "userId is required" });

            // Start transaction for atomicity
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Examination exam;
                var now = DateTime.UtcNow;

                if (dto.ExaminationId.HasValue)
                {
                    exam = await _context.Examinations.FindAsync(dto.ExaminationId.Value);
                    if (exam == null)
                        return NotFound(new { message = "Examination not found" });

                    exam.LastModifiedById = dto.UserId;
                    exam.LastModifiedDateTime = now;
                }
                else
                {
                    exam = new Examination
                    {
                        CreatedById = dto.UserId,
                        LastModifiedById = dto.UserId,
                        CreatedDateTime = now,
                        LastModifiedDateTime = now,
                        StartAt = now
                    };
                    await _context.Examinations.AddAsync(exam);
                    await _context.SaveChangesAsync(); // ensure ExaminationId assigned
                }

                // Set EndAt to now (marking save/completion time). Keep it nullable if you prefer otherwise.
                exam.EndAt = now;

                // Create Diagnosis if provided
                if (!string.IsNullOrWhiteSpace(dto.ClinicalDiagnosis) || !string.IsNullOrWhiteSpace(dto.RequiredInvestigations))
                {
                    var diagnosis = new Diagnosis
                    {
                        PatientId = dto.PatientId,
                        ExaminationId = exam.ExaminationId,
                        PatientDiagnosis = dto.ClinicalDiagnosis,
                        PatientRegInvestigation = dto.RequiredInvestigations,
                        CreatedById = dto.UserId,
                        LastModifiedById = dto.UserId,
                        CreatedDateTime = now,
                        LastModifiedDateTime = now
                    };

                    await _context.Diagnoses.AddAsync(diagnosis);
                }

                // Add symptoms
                if (dto.Symptoms?.Any() == true)
                {
                    foreach (var s in dto.Symptoms)
                    {
                        var patientSymptom = new PatientSymptom
                        {
                            PatientId = dto.PatientId,
                            SymptomId = s.SymptomId,
                            ExaminationId = exam.ExaminationId,
                            CreatedById = dto.UserId,
                            LastModifiedById = dto.UserId,
                            CreatedDateTime = now,
                            LastModifiedDateTime = now
                        };
                        await _context.PatientSymptoms.AddAsync(patientSymptom);
                    }
                }

                // Add medications
                if (dto.Medications?.Any() == true)
                {
                    foreach (var m in dto.Medications)
                    {
                        var med = new PatientMedication
                        {
                            PatientId = dto.PatientId,
                            MedicineId = m.MedicineId,
                            Dosage = m.Dosage,
                            Frequency = m.Frequency,
                            ExaminationId = exam.ExaminationId,
                            CreatedById = dto.UserId,
                            LastModifiedById = dto.UserId,
                            CreatedDateTime = now,
                            LastModifiedDateTime = now
                        };
                        await _context.PatientMedications.AddAsync(med);
                    }
                }

                // Update patient's next appointment if provided
                if (dto.NextAppointment.HasValue)
                {
                    var patient = await _context.PatientData.FindAsync(dto.PatientId);
                    if (patient != null)
                    {
                        patient.PnextAppointment = dto.NextAppointment;
                        patient.LastModifiedById = dto.UserId;
                        patient.LastModifiedDateTime = now;
                        _context.PatientData.Update(patient);
                    }
                }

                // Persist all changes and commit
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Examination saved successfully", examinationId = exam.ExaminationId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // For production, consider logging the exception rather than returning full details
                return StatusCode(500, new { message = "An error occurred saving the examination", detail = ex.Message });
            }
        }

        // GET: api/doctor/Examination/5
        [HttpGet("Examination/{id:int}")]
        public async Task<IActionResult> GetExamination(int id)
        {
            var exam = await _context.Examinations
                .Include(e => e.Diagnoses)
                .Include(e => e.PatientMedications)
                    .ThenInclude(pm => pm.Medicine)
                .Include(e => e.PatientSymptoms)
                    .ThenInclude(ps => ps.Symptom)
                .Include(e => e.PatientData)
                .FirstOrDefaultAsync(e => e.ExaminationId == id);

            if (exam == null) return NotFound();

            // Minimal projection to return relevant data (you can adapt to DTOs if desired)
            return Ok(new
            {
                exam.ExaminationId,
                exam.StartAt,
                exam.EndAt,
                Diagnoses = exam.Diagnoses.Select(d => new { d.DiagnosisId, d.PatientDiagnosis, d.PatientRegInvestigation }),
                Medications = exam.PatientMedications.Select(pm => new { pm.PatientMedId, pm.MedicineId, pm.Dosage, pm.Frequency, MedicineName = pm.Medicine?.MedicineName }),
                Symptoms = exam.PatientSymptoms.Select(ps => new { ps.PatientSymId, ps.SymptomId, SymptomName = ps.Symptom?.SymptomName })
            });
        }
    }
}