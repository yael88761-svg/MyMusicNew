using Repositories.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        //enum 
        public MyPlaylistsNames PlaylistName { get; set; }

        public bool IsSmartPlaylist { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        //יכול להיות רשימה של שירים רגיל
        public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; }
    }
}
