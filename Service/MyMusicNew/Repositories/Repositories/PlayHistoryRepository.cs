using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    internal class PlayHistoryRepository : IRepository<PlayHistory>
    {
        private readonly IContext ctx;

        // בנאי רגיל – תואם C# 8
        public PlayHistoryRepository(IContext context)
        {
            ctx = context;
        }

        public async Task<PlayHistory> AddItem(PlayHistory item)
        {
            await ctx.PlayHistories.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            PlayHistory history = await ctx.PlayHistories.FindAsync(id);
            if (history != null)
            {
                ctx.PlayHistories.Remove(history);
                await ctx.Save();
            }
        }

        public async Task<List<PlayHistory>> GetAll()
        {
            return await ctx.PlayHistories
             .Include(h => h.User)
             .Include(h => h.Song)
              .ToListAsync();
        }

        public async Task<PlayHistory> GetById(int id)
        {
            return await ctx.PlayHistories
            .Include(h => h.User)
            .Include(h => h.Song)
            .FirstOrDefaultAsync(h => h.HistoryId == id);
        }

        public async Task<PlayHistory> UpdateItem(int id, PlayHistory item)
        {
            PlayHistory existing = await ctx.PlayHistories.FindAsync(id);
            if (existing == null)
                return null;

            existing.UserId = item.UserId;
            existing.SongId = item.SongId;
            existing.PlayedAt = item.PlayedAt;
            existing.CompletionPercentage = item.CompletionPercentage;

            await ctx.Save();
            return existing;
        }
    }
}
