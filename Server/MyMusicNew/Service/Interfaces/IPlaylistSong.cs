using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPlaylistSong
    {
        Task<PlaylistSongDto> AddItem(PlaylistSongDto item, int userId);

        Task<List<PlaylistSongDto>> GetAllByUserId(int userId);

        Task DeleteItem(int id, int userId);
    }
}
