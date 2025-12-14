using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DigiDocs.dtos
{
    #region Examination DTOs

    public class DoctorExaminationCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        public int? userId { get; set; }
    }

    public class DoctorCompleteExaminationDto
    {
        [Required]
        public int ExaminationId { get; set; }

        /// <summary>
        /// List of symptom IDs to associate with this examination
        /// </summary>
        public List<int>? Symptoms { get; set; }

        /// <summary>
        /// Clinical diagnosis text
        /// </summary>
        public string? ClinicalDiagnosis { get; set; }

        /// <summary>
        /// Required investigations text (e.g., MRI, CT Scan, Blood tests)
        /// </summary>
        public string? RequiredInvestigations { get; set; }

        /// <summary>
        /// List of medications to prescribe
        /// </summary>
        public List<DoctorMedicationDto>? Medications { get; set; }

        /// <summary>
        /// Optional next appointment date
        /// </summary>
        public DateTime? NextAppointmentDate { get; set; }

        public int? userId { get; set; }
    }

    public class DoctorMedicationDto
    {
        [Required]
        public int MedicineId { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public string Frequency { get; set; }
    }

    #endregion

    #region Symptom DTOs

    public class DoctorPatientSymptomCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int SymptomId { get; set; }

        [Required]
        public int ExaminationId { get; set; }

        public int? userId { get; set; }
    }

    #endregion

    #region Diagnosis DTOs

    public class DoctorDiagnosisCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ExaminationId { get; set; }

        [Required]
        public string ClinicalDiagnosis { get; set; }

        public string? RequiredInvestigations { get; set; }

        public int? userId { get; set; }
    }

    public class DoctorDiagnosisUpdateDto
    {
        [Required]
        public string ClinicalDiagnosis { get; set; }

        public string? RequiredInvestigations { get; set; }

        public int? userId { get; set; }
    }

    #endregion

    #region Medication/Prescription DTOs

    public class DoctorPatientMedicationCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public string Frequency { get; set; }

        [Required]
        public int ExaminationId { get; set; }

        public int? userId { get; set; }
    }

    #endregion

    #region Appointment DTOs

    public class DoctorAppointmentCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public int? userId { get; set; }
    }

    #endregion
}