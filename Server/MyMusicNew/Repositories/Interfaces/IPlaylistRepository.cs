using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IPlaylistRepository<Playlist>
    {
        Task<List<Playlist>> GetAll(int userId);
        Task<List<Song>> GetSongsByDate(DateTime fromDate);
        Task<IEnumerable<dynamic>> GetRecentSongsAsync(int userId, DateTime startDate);
    }
}
