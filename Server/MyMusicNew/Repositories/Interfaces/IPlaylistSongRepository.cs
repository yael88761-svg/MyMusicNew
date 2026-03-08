using Repositories.Entities;

namespace Repositories.Interfaces
{
    public interface IPlaylistSongRepository : IRepository<PlaylistSong>
    {
        Task<List<PlaylistSong>> GetAllByUserId(int userId);
    }
}