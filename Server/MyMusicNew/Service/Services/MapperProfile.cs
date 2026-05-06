using AutoMapper;
using Repositories.Entities;
using Service.Dto;

namespace Service.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // מיפוי משתמשים
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

            // מיפוי שירים
            CreateMap<Song, SongDto>().ReverseMap();

            // מיפוי אובייקט הקשר פלייליסט-שיר
            // הוספנו מיפוי מפורש לאובייקט השיר כדי לוודא שנתוני השיר עוברים ל-DTO
            CreateMap<PlaylistSong, PlaylistSongDto>()
                .ForMember(dest => dest.SongTitle, opt => opt.MapFrom(src => src.Song.Title))
                .ForMember(dest => dest.Song, opt => opt.MapFrom(src => src.Song))
                .ReverseMap();

            // מיפוי פלייליסט
            // הוספנו הנחיה מפורשת למיפוי האוסף PlaylistSongs
            CreateMap<Playlist, PlaylistDto>()
                .ForMember(dest => dest.PlaylistSongs, opt => opt.MapFrom(src => src.PlaylistSongs))
                .ReverseMap();

            CreateMap<PlaylistDto, Playlist>()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // מיפוי היסטוריית השמעה
            CreateMap<PlayHistory, PlayHistoryDto>()
                .ForMember(dest => dest.SongTitle, opt => opt.MapFrom(src => src.Song.Title))
                .ReverseMap()
                .ForMember(dest => dest.Song, opt => opt.Ignore());

            // מיפוי תכונות אודיו
            CreateMap<AudioFeatures, AudioFeaturesDto>().ReverseMap();
        }
    }
}