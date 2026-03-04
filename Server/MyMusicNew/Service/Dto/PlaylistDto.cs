using Repositories.Entities;
using Repositories.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Service.Dto
{
    public class PlaylistDto
    {
        public int PlaylistId { get; set; }
        public int UserId { get; set; }

        [Required]
        public string PlaylistName { get; set; }
        public bool IsSmartPlaylist { get; set; }
        public DateTime AddedAt { get; set; }
        //public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; }

    }
}
