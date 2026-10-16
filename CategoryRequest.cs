
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models
{
    public class CategoryRequest
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Range(0.01, 999.99)]
        public decimal Distance { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxParticipants { get; set; }

        [Range(0, 999999.99)]
        public decimal EntryFee { get; set; }

        [Range(0, 120)]
        public int MinAge { get; set; }

        [Range(0, 120)]
        public int MaxAge { get; set; }
    }
}
