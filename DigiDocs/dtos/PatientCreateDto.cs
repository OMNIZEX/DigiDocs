using DigiDocs.Enums;
using DigiDocs.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace DigiDocs.dtos
{
    public class PatientCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Pname { get; set; }

        public int? userId { get; set; }
        public int? Page { get; set; }

        [MaxLength(100)]
        public string Pjob { get; set; }

        [MaxLength(10)]
        public string Pgender { get; set; }

        [MaxLength(500)]
        public string Paddress { get; set; }

        public string Pcomplain { get; set; }

        public string PchronicDisease { get; set; }

        public ServiceType PserviceType { get; set; }

        public Priority Ppriority { get; set; }

        public decimal? PamountToPay { get; set; }

        [MaxLength(20)]
        public string PphoneNumber { get; set; }

        //public DateTime? PnextAppointment { get; set; }

        // public int? ExaminationId { get; set; }

    }
}