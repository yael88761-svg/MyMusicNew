using DataContext;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Entities;
using Repositories.Repositories;
using Service.Dto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Service.Services
{
    public static class ExtensionService
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services, string connectionString)
        {
            services.AddAutoMapper(typeof(MapperProfile));
            services.AddDataLayer(connectionString);
            services.AddRepository();
            services.AddScoped<IToken<User>, TokenService>();
            services.AddScoped<IRegister<UserRegisterDto>, UserRegisterService>();
            services.AddScoped<ILogin<UserLoginDto>, UserLoginService>();
            services.AddScoped<IService<UserDto>, UserService>();
            services.AddScoped<IService<SongDto>, SongService>();
            services.AddScoped<ISong<SongDto>, SongService>();
            services.AddScoped<IService<PlaylistSongDto>, PlayListSongService>();
            services.AddScoped<IService<PlaylistDto>,PlaylistService>();
            services.AddScoped<IPlaylist<PlaylistDto>, PlaylistService>();
            services.AddScoped<IService<PlayHistoryDto>, PlayHistoryService>();
            services.AddScoped<IService<AudioFeaturesDto>, AudioFeaturesService1>();
            services.AddScoped<IPlaylistSong, PlayListSongService>();

            return services;
        }
    }
}
