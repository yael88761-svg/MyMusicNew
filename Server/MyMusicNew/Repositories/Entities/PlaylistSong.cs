using Repositories.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    public class PlaylistSong
    {
       //פה להוסיף לו מפתח לעצמו

        [Key]
         public int PlaylistSongId { get; set; }
        [Required]
        public int PlaylistId { get; set; }

        [Required]
        public int SongId { get; set; }

        public int Position { get; set; } // Order in playlist

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("PlaylistId")]
        public virtual Playlist Playlist { get; set; }

        [ForeignKey("SongId")]
        public virtual Song Song { get; set; }
    }
}
