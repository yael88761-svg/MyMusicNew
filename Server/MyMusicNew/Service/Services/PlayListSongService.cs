using AutoMapper;
using Repositories.Entities;
using Repositories.Interfaces;
using Service.Dto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class PlayListSongService(
        IRepository<PlaylistSong> repository,
        IRepository<Playlist> playlistRepository,
        IMapper mapper) : IService<PlaylistSongDto>, IPlaylistSong
    {
        private readonly IRepository<PlaylistSong> _repository = repository;
        private readonly IRepository<Playlist> _playlistRepository = playlistRepository;
        private readonly IMapper _mapper = mapper;


        public async Task<PlaylistSongDto> AddItem(PlaylistSongDto item, int userId)
        {
            var playlist = await _playlistRepository.GetById(item.PlaylistId);
            if (playlist == null) throw new KeyNotFoundException("Playlist not found");
            if (playlist.UserId != userId) throw new UnauthorizedAccessException("Not your playlist!");

            var playlistSongEntity = _mapper.Map<PlaylistSong>(item);
            playlistSongEntity.Song = null;
            playlistSongEntity.Playlist = null;

            var addedPlaylistSong = await _repository.AddItem(playlistSongEntity);
            return _mapper.Map<PlaylistSongDto>(addedPlaylistSong);
        }

        public async Task DeleteItem(int id, int userId)
        {
            var existing = await _repository.GetById(id);
            if (existing == null) throw new KeyNotFoundException("Item not found");

            var playlist = await _playlistRepository.GetById(existing.PlaylistId);
            if (playlist == null || playlist.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to delete this.");

            await _repository.DeleteItem(id);
        }

        public async Task<List<PlaylistSongDto>> GetAllByUserId(int userId)
        {
            var all = await _repository.GetAll();
            var filtered = all.Where(ps => ps.Playlist != null && ps.Playlist.UserId == userId).ToList();
            return _mapper.Map<List<PlaylistSongDto>>(filtered);
        }


        public async Task<PlaylistSongDto> AddItem(PlaylistSongDto item)
        {
            return await AddItem(item, 0);
        }

        public async Task DeleteItem(int id)
        {
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
            if (playlistSong == null) throw new KeyNotFoundException($"Id {id} not found");
            return _mapper.Map<PlaylistSongDto>(playlistSong);
        }

        public async Task<PlaylistSongDto> UpdateItem(int id, PlaylistSongDto item)
        {
            var entity = _mapper.Map<PlaylistSong>(item);
            var updated = await _repository.UpdateItem(id, entity);
            return _mapper.Map<PlaylistSongDto>(updated);
        }
    }
}