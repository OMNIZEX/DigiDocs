using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigiDocs.dtos
{
    public class MedicationDto
    {
        [Required]
        public int MedicineId { get; set; }

        [MaxLength(200)]
        public string Dosage { get; set; }

        [MaxLength(200)]
        public string Frequency { get; set; }
    }

    public class SymptomDto
    {
        [Required]
        public int SymptomId { get; set; }
    }

    public class DoctorExaminationDto
    {
        // Required: the patient this examination belongs to
        [Required]
        public int PatientId { get; set; }

        // Optional: if client wants to update an existing examination
        public int? ExaminationId { get; set; }

        // Optional lists
        public List<SymptomDto> Symptoms { get; set; } = new List<SymptomDto>();
        public List<MedicationDto> Medications { get; set; } = new List<MedicationDto>();

        // Diagnosis fields
        [MaxLength(4000)]
        public string ClinicalDiagnosis { get; set; }

        [MaxLength(4000)]
        public string RequiredInvestigations { get; set; }

        // Next appointment for the patient (optional)
        public DateTime? NextAppointment { get; set; }

        // Audit: set by client for now but should come from authenticated user in future
        public int? UserId { get; set; }
    }
}