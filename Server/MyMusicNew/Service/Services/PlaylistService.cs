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
    public class PlaylistService(IRepository<Playlist> repository,IMapper mapper) : IService<PlaylistDto>
    {
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
