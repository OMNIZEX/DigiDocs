using DigiDocs.Enums;
using System.ComponentModel.DataAnnotations;

namespace DigiDocs.Models
{
    public class PatientQueue
    {
        [Key]
        public int PatientQueueId { get; set; }
        public int PatientId { get; set; }
        public PatientDatum Patient { get; set; }
        public QueueStatus Status {get; set;}

    }
}
