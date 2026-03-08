using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;

namespace Repositories.Repositories
{
    // 1. שינוי חתימה: המחלקה מממשת עכשיו את הממשק הספציפי (שיורש מהגנרי)
    public class PlaylistSongRepository : IPlaylistSongRepository
    {
        private readonly IContext ctx;

        public PlaylistSongRepository(IContext context)
        {
            ctx = context;
        }

        // 2. הפונקציה המיוחדת לסינון לפי משתמש
        public async Task<List<PlaylistSong>> GetAllByUserId(int userId)
        {
            return await ctx.PlaylistSongs
                .Include(ps => ps.Playlist) // חובה בשביל הסינון
                .Include(ps => ps.Song)     // מומלץ: כדי לקבל את פרטי השיר (שם, מבצע וכו')
                .Where(ps => ps.Playlist.UserId == userId)
                .ToListAsync();
        }

        public async Task<PlaylistSong> AddItem(PlaylistSong item)
        {
            await ctx.PlaylistSongs.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            var playlistSong = await ctx.PlaylistSongs.FindAsync(id);
            if (playlistSong != null)
            {
                ctx.PlaylistSongs.Remove(playlistSong);
                await ctx.Save();
            }
        }

        // 3. עדכון GetAll הגנרי שיכלול גם את פרטי השיר
        public async Task<List<PlaylistSong>> GetAll()
        {
            return await ctx.PlaylistSongs
                .Include(ps => ps.Song)
                .Include(ps => ps.Playlist)
                .ToListAsync();
        }

        public async Task<PlaylistSong> GetById(int id)
        {
            // FindAsync לא תומך ב-Include, לכן נשתמש ב-FirstOrDefaultAsync
            return await ctx.PlaylistSongs
                .Include(ps => ps.Song)
                .Include(ps => ps.Playlist)
                .FirstOrDefaultAsync(ps => ps.PlaylistSongId == id);
        }

        public async Task<PlaylistSong> UpdateItem(int id, PlaylistSong item)
        {
            var ps = await ctx.PlaylistSongs.FindAsync(id);
            if (ps != null)
            {
                ps.PlaylistId = item.PlaylistId;
                ps.SongId = item.SongId;
                ps.Position = item.Position;
                ps.AddedAt = item.AddedAt;
                await ctx.Save();
            }
            return ps;
        }
    }
}