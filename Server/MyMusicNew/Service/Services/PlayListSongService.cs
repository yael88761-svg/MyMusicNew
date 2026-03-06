using AutoMapper;
using Repositories.Entities;
using Repositories.Interfaces;
using Service.Dto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class PlayListSongService(IRepository<PlaylistSong> repository,IRepository<Playlist> playlistRepository, IMapper mapper) : IService<PlaylistSongDto>
    {
        private readonly IRepository<PlaylistSong> _repository = repository;
        private readonly IRepository<Playlist> _playlistRepository = playlistRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<PlaylistSongDto> AddItem(PlaylistSongDto item)
        {
            var playlistSongEntity = _mapper.Map<PlaylistSong>(item);

            // 2. התיקון הקריטי: מוודאים שהניווט (Navigation Property) הוא null
            // זה גורם ל-EF להסתכל רק על ה-SongId וה-PlaylistId הקיימים
            playlistSongEntity.Song = null;
            playlistSongEntity.Playlist = null;

            // 3. עכשיו השמירה תעבור כי EF רק יקשר בין ה-IDs הקיימים
            var addedPlaylistSong = await _repository.AddItem(playlistSongEntity);

            return _mapper.Map<PlaylistSongDto>(addedPlaylistSong);
        }

        public async Task DeleteItem(int id)
        {
            var existing = await _repository.GetById(id);  
            if (existing == null)
                throw new KeyNotFoundException($"PlaylistSong with id {id} not found");

            await _repository.DeleteItem(id);  
        }

        public async Task<List<PlaylistSongDto>> GetAll()
        {
            var playlistSongs = await _repository.GetAll();  
            return _mapper.Map<List<PlaylistSongDto>>(playlistSongs); 
        }

        public async Task<PlaylistSongDto> GetById(int id)
        {
            var playlistSong = await _repository.GetById(id); 
            if (playlistSong == null)
                throw new KeyNotFoundException($"PlaylistSong with id {id} not found");

            return _mapper.Map<PlaylistSongDto>(playlistSong); 
        }

        public async Task<PlaylistSongDto> UpdateItem(int id, PlaylistSongDto item)
        {
            var PlaylistSongEntity = _mapper.Map<PlaylistSong>(item);
            var updatedPlaylistSong = await _repository.UpdateItem(id, PlaylistSongEntity);
            return _mapper.Map<PlaylistSongDto>(updatedPlaylistSong);
        }
    }
}
