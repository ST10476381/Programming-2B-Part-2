
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDayAPI.Models
{
    [Table("CATEGORY", Schema = "dbo")]
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal Distance { get; set; }

        public int MaxParticipants { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal EntryFee { get; set; }

        [Range(0, 120)]
        public int MinAge { get; set; }

        [Range(0, 120)]
        public int MaxAge { get; set; }
    }
}
