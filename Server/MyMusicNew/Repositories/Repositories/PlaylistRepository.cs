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
                    .Include(p => p.PlaylistSongs)      // טעינת רשימת הקשר (פלייליסט-שיר)
                        .ThenInclude(ps => ps.Song)     // טעינת נתוני השיר הספציפי
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
            existing.UserId = item.UserId;

            await ctx.Save();
            return existing;
        }
        public async Task<List<Song>> GetSongsByDate(DateTime fromDate)
        {
                 return await ctx.Songs
                .Where(s => s.UploadedAt >= fromDate)
                .OrderByDescending(s => s.UploadedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<dynamic>> GetRecentSongsAsync(int userId, DateTime startDate)
        {
            // שליפה ישירות מטבלת השירים (Songs)
            return await ctx.Songs
                .Where(s => s.UserId == userId && s.UploadedAt >= startDate)
                .OrderByDescending(s => s.UploadedAt) // שהחדשים ביותר יהיו למעלה
                .Select(s => new {
                    SongId = s.SongId,
                    PlaylistId = 0, // מסמנים כ-0 כי זה לא שייך לפלייליסט ספציפי
                    Song = s // האובייקט המלא של השיר
                })
                .ToListAsync();
        }
    }
}
