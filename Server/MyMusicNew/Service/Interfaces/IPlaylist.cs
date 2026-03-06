using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPlaylist<PlaylistDto>
    {
        Task<List<PlaylistDto>> GetAll(int userId);
    }
}
