using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    internal class PlaylistRepository : IRepository<Playlist>, IPlaylistRepository<Playlist>
    {
        private readonly IContext ctx;

        public PlaylistRepository(IContext context)
        {
            ctx = context;
        }

        public async Task<Playlist> AddItem(Playlist item)
        {
            await ctx.Playlists.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            Playlist playlist = await ctx.Playlists.FindAsync(id);
            if (playlist != null)
            {
                ctx.Playlists.Remove(playlist);
                await ctx.Save();
            }
        }

        public async Task<List<Playlist>> GetAll()
        {
            return await ctx.Playlists
            .Include(p => p.User)
            .Include(p => p.PlaylistSongs)
            .ToListAsync();
        }

        public async Task<List<Playlist>> GetAll(int userId)
        {
            return await ctx.Playlists
                    .Where(p => p.UserId == userId)
                    .ToListAsync();
        }

        public async Task<Playlist> GetById(int id)
        {
            return await ctx.Playlists
           .Include(p => p.User)
           .Include(p => p.PlaylistSongs)
           .FirstOrDefaultAsync(p => p.PlaylistId == id);
        }

        public async Task<Playlist> UpdateItem(int id, Playlist item)
        {
            Playlist existing = await ctx.Playlists.FindAsync(id);
            if (existing == null)
                return null;

            existing.PlaylistName = item.PlaylistName;
            existing.IsSmartPlaylist = item.IsSmartPlaylist;
            existing.UserId = item.UserId;

            await ctx.Save();
            return existing;
        }
    }
}
