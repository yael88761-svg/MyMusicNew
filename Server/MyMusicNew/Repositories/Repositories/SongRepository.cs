using Repositories.Entities;
using Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Repositories.Repositories

{
    public class SongRepository : IRepository<Song>
    {
        private readonly IContext ctx;

        public SongRepository(IContext context)
        {
            ctx = context;
        }

        public async Task<Song> AddItem(Song item)
        {
            await ctx.Songs.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            Song song = await ctx.Songs.FindAsync(id);
            if (song != null)
            {
                ctx.Songs.Remove(song);
                await ctx.Save();
            }
        }

        public async Task<List<Song>> GetAll()
        {
            return await ctx.Songs.ToListAsync();
        }

        public async Task<Song> GetById(int id)
        {
            return await ctx.Songs.FindAsync(id);
        }

        public async Task<Song> UpdateItem(int id, Song item)
        {
            Song s = await ctx.Songs.FindAsync(id);
            if (s == null)
                return null;

            s.Title = item.Title;
            s.Artist = item.Artist;
            s.FilePath = item.FilePath;
            s.Duration = item.Duration;
            s.Genre = item.Genre;
            s.Mood = item.Mood;
            s.LyricsText = item.LyricsText;
            await ctx.Save();
            return s;
        }
    }
}
