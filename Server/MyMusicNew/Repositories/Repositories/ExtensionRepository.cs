using Microsoft.Extensions.DependencyInjection;
using Repositories.Entities;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public static class ExtensionRepository
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<Song>, SongRepository>();
            services.AddScoped<IRepository<Playlist>, PlaylistRepository>();
            services.AddScoped<IRepository<PlaylistSong>, PlaylistSongRepository>();
            services.AddScoped<IRepository<PlayHistory>, PlayHistoryRepository>();
            services.AddScoped<IRepository<AudioFeatures>, AudioFeaturesRepository>();

            return services;
        }

    }
}
