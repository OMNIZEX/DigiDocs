using DigiDocs.Enums;
using DigiDocs.Models;
using System.ComponentModel.DataAnnotations;

namespace DigiDocs.dtos
{
    public class PatientQueueResponseDto
    {
        public int PatientQueueId { get; set; }
        public int PatientId { get; set; }
        public PatientDataResponseDto Patient { get; set; }
        public QueueStatus Status { get; set; }
    }
}
