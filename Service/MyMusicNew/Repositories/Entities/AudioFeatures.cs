using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    public class AudioFeatures
    {
        [Key]
        public int FeatureId { get; set; }

        [Required]
        public int SongId { get; set; }

        public float Tempo { get; set; } // BPM

        public float Energy { get; set; } // 0.0 to 1.0

        [MaxLength(10)]
        public string Key { get; set; } // "C", "G#", etc.

        public float Valence { get; set; } // 0.0 to 1.0 (happiness)

        public float Danceability { get; set; } // 0.0 to 1.0

        // Navigation Properties
        [ForeignKey("SongId")]
        public virtual Song Song { get; set; }
    }
}
