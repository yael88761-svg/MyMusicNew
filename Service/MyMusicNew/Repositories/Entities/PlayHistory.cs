using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    public class PlayHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SongId { get; set; }

        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

        public int CompletionPercentage { get; set; } // 0-100

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("SongId")]
        public virtual Song Song { get; set; }
    }
}
