using Repositories.Entities;
using Microsoft.EntityFrameworkCore;
namespace Repositories.Interfaces
{
    public interface IContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlayHistory> PlayHistories { get; set; }
        public DbSet<AudioFeatures> AudioFeatures { get; set; }
        public Task Save();
    }
}
