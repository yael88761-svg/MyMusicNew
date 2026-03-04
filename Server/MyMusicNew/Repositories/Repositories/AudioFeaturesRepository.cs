

using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Interfaces;

namespace Repositories.Repositories
{
    public class AudioFeaturesRepository : IRepository<AudioFeatures>
    {
        private readonly IContext ctx;
        public AudioFeaturesRepository(IContext context)
        {
            ctx = context;
        }

        public async Task<AudioFeatures> AddItem(AudioFeatures item)
        {
            await ctx.AudioFeatures.AddAsync(item);
            await ctx.Save();
            return item;
        }

        public async Task DeleteItem(int id)
        {
            AudioFeatures feature = await ctx.AudioFeatures.FindAsync(id);
            if (feature != null)
            {
                ctx.AudioFeatures.Remove(feature);
                await ctx.Save();
            }
        }

        public async Task<List<AudioFeatures>> GetAll()
        {
            return await ctx.AudioFeatures
            .Include(a => a.Song)
             .ToListAsync();
        }

        public async Task<AudioFeatures> GetById(int id)
        {
            return await ctx.AudioFeatures
            .Include(a => a.Song)
             .FirstOrDefaultAsync(a => a.FeatureId == id);
        }

        public async Task<AudioFeatures> UpdateItem(int id, AudioFeatures item)
        {
            AudioFeatures a = await ctx.AudioFeatures.FindAsync(id);
            if (a == null)
                return null;

            a.Tempo = item.Tempo;
            a.Energy = item.Energy;
            a.Key = item.Key;
            a.Valence = item.Valence;
            a.Danceability = item.Danceability;
            a.SongId = item.SongId;

            await ctx.Save();
            return a;
        }
    }
}
