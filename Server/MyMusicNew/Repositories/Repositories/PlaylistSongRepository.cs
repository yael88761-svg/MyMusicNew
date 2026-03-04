using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class PlaylistSongRepository : IRepository<PlaylistSong>
    {
        private readonly IContext ctx;

        public PlaylistSongRepository(IContext context)
        {
            ctx = context;
        }


        public async Task<PlaylistSong> AddItem(PlaylistSong item)
        {
            await ctx.PlaylistSongs.AddAsync(item);
            await ctx.Save();
            return item;

        }

        public async Task DeleteItem(int id)
        {
            PlaylistSong PlaylistSong = await ctx.PlaylistSongs.FindAsync(id);
            if (PlaylistSong != null)
            {
                ctx.PlaylistSongs.Remove(PlaylistSong);
                await ctx.Save();
            }

        }

        public async Task<List<PlaylistSong>> GetAll()
        {
            return await ctx.PlaylistSongs.ToListAsync();
        }

        public async Task<PlaylistSong> GetById(int id)
        {
            return await ctx.PlaylistSongs.FindAsync(id);
        }

        public async Task<PlaylistSong> UpdateItem(int id, PlaylistSong item)
        {
            PlaylistSong ps = await ctx.PlaylistSongs.FindAsync(id);
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
