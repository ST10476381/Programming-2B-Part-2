using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("ENROLMENT", Schema = "dbo")]
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        public int ParticipantId { get; set; }

        public int CategoryId { get; set; }

        [Column(TypeName = "date")]
        public DateTime EnrolmentDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}