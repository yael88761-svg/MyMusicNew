using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class SongDto
    {
        public int SongId { get; set; }

        public int UserId { get; set; } // מי העלה את השיר
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; }

        [Required]
        [MaxLength(255)]
        public string Artist { get; set; }

        public int Duration { get; set; } // in seconds

        [MaxLength(100)]
        public string Genre { get; set; }

        [MaxLength(100)]
        public string Mood { get; set; }



    }
}
