using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class PlayHistoryDto
    {
        public int HistoryId { get; set; }
        public int SongId { get; set; }
        public string SongTitle { get; set; } 
        public string ArtistName { get; set; } 
        public DateTime PlayedAt { get; set; }
        public int CompletionPercentage { get; set; }
    }
}
