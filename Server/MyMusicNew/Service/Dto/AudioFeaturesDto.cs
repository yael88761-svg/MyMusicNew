using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dto
{
    public class AudioFeaturesDto
    {
        public int FeatureId { get; set; }
        public int SongId { get; set; }
        public float Tempo { get; set; }
        public float Energy { get; set; }
        public string Key { get; set; }
        public float Valence { get; set; }
        public float Danceability { get; set; }
    }
}
