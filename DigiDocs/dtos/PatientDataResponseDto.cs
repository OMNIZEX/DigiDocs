using DigiDocs.Enums;
using DigiDocs.Models;

namespace DigiDocs.dtos
{
    public class PatientDataResponseDto
    {
        public int PatientId { get; set; }

        public string Pname { get; set; }

        public int? Page { get; set; }

        public string Pjob { get; set; }

        public string Pgender { get; set; }

        public string Paddress { get; set; }

        public string Pcomplain { get; set; }

        public string PchronicDisease { get; set; }

        public ServiceType PserviceType { get; set; }

        public Priority Ppriority { get; set; }

        public decimal? PamountToPay { get; set; }

        public string PphoneNumber { get; set; }

    }
}
