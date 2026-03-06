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
    public class PlaylistService(IRepository<Playlist> repository,IPlaylistRepository<Playlist>  playlistRepository,IMapper mapper) : IService<PlaylistDto>, IPlaylist<PlaylistDto>
    {
        private readonly IPlaylistRepository<Playlist> _playlistRepository = playlistRepository;
        private readonly IRepository<Playlist> _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PlaylistDto> AddItem(PlaylistDto item)
        {
            var PlaylistEntity = _mapper.Map<Playlist>(item);
            var addedPlaylist = await _repository.AddItem(PlaylistEntity);
            return _mapper.Map<PlaylistDto>(addedPlaylist);
        }

        public async Task DeleteItem(int id)
        {
            var existing = await _repository.GetById(id);
            if (existing != null)
            {
                await _repository.DeleteItem(id);
            }
            else
            {
                throw new KeyNotFoundException($"Playlist with id {id} not found");
            }
        }

        public async Task<List<PlaylistDto>> GetAll()
        {
            var Playlist = await _repository.GetAll();
            return _mapper.Map<List<PlaylistDto>>(Playlist);
        }

        public async Task<List<PlaylistDto>> GetAll(int userId)
        {
            // 1. קריאה לרפוזיטורי הספציפי שסידרנו קודם (זה ששולף לפי UserId)
            var playlists = await _playlistRepository.GetAll(userId);

            // 2. המרה של רשימת הישויות (Entities) לרשימה של DTOs בעזרת המאפר
            return _mapper.Map<List<PlaylistDto>>(playlists);
        }

        public async Task<PlaylistDto> GetById(int id)
        {
            var playlist = await _repository.GetById(id);
            return _mapper.Map<PlaylistDto>(playlist);
        }

        public async Task<PlaylistDto> UpdateItem(int id, PlaylistDto item)
        {
            var PlaylistEntity = _mapper.Map<Playlist>(item);
            var updatedPlaylist = await _repository.UpdateItem(id, PlaylistEntity);
            return _mapper.Map<PlaylistDto>(PlaylistEntity);
        }
    }
}
