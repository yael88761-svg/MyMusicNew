using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface ISong<SongDto>
    {
        Task<List<SongDto>> GetAll(int id);

    }
}
