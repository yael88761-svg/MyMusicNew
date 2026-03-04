using AutoMapper;
using Repositories.Entities;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ReverseMap()
                .ForPath(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));

            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));

            CreateMap<UserLoginDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ReverseMap();

            CreateMap<SongDto, Song>().ReverseMap();

            CreateMap<PlaylistSong, PlaylistSongDto>()
                //.ForMember(dest => dest.PlaylistName, opt => opt.MapFrom(src => src.Playlist.PlaylistName))
                .ForMember(dest => dest.SongTitle, opt => opt.MapFrom(src => src.Song.Title))
                .ReverseMap();

            CreateMap<Playlist, PlaylistDto>().ReverseMap();

            CreateMap<PlaylistDto, Playlist>()
                .ForMember(dest => dest.User, opt => opt.Ignore()); // מתעלמים רק מהאובייקט המקושר, לא מה-ID
            CreateMap<PlayHistory, PlayHistoryDto>()
                .ForMember(dest => dest.SongTitle, opt => opt.MapFrom(src => src.Song.Title))
                .ReverseMap()
                .ForMember(dest => dest.Song, opt => opt.Ignore());// התעלמות מאובייקט השיר ביצירה

            CreateMap<AudioFeatures, AudioFeaturesDto>().ReverseMap();
        }
    }
}
