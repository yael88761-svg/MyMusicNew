using System.ComponentModel.DataAnnotations;

namespace Repositories.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Song> Songs { get; set; }
        public virtual ICollection<Playlist> Playlists { get; set; }
        public virtual ICollection<PlayHistory> PlayHistories { get; set; }
    }
}
