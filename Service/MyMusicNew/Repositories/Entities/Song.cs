using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    public enum EHolidayTags { Tishrei, Cheshvan, Keslev, Tevet, Shevat, Adar, Nissan, Iyar, Sivan, Tammuz, Av, Elul, Wedding, Quiet, Stormy, Classic, Beautiful, All, Kinds, Mine, Girls, Shabbat, Home, Melodies, Vocal }

    public class Song
    {

        [Key]
        public int SongId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(255)]
        public string Artist { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; }

        public int Duration { get; set; } // in seconds

        [MaxLength(100)]
        public string Genre { get; set; }

        [MaxLength(100)]
        public string Mood { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string LyricsText { get; set; }

        public EHolidayTags EHoliday { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public virtual AudioFeatures AudioFeatures { get; set; }
        public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; }
        public virtual ICollection<PlayHistory> PlayHistories { get; set; }
    }
}
