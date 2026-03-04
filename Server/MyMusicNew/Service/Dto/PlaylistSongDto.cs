using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class PlaylistSongDto
    {
            public int PlaylistId { get; set; }
            public int SongId { get; set; }
            public int Position { get; set; } 

            //public string PlaylistName { get; set; }
            public string SongTitle { get; set; }

    }
}
