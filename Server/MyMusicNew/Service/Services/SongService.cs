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
    public class SongService(IRepository<Song> repository,IMapper mapper) : IService<SongDto>
    {
        private readonly IRepository<Song> _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<SongDto> AddItem(SongDto item)
        {
            var songEntity = _mapper.Map<Song>(item);
            var addedSong = await _repository.AddItem(songEntity);
            return _mapper.Map<SongDto>(addedSong);
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
                throw new KeyNotFoundException($"Song with id {id} not found");
            }
        }

        public async Task<List<SongDto>> GetAll()
        {
            var songs = await _repository.GetAll();
            return _mapper.Map<List<SongDto>>(songs);
        }

        public async Task<SongDto> GetById(int id)
        {
            var song = await _repository.GetById(id);
            return _mapper.Map<SongDto>(song);
        }

        public async Task<SongDto> UpdateItem(int id, SongDto item)
        {
            var SongEntity = _mapper.Map<Song>(item);
            var updatedSong = await _repository.UpdateItem(id, SongEntity);
            return _mapper.Map<SongDto>(updatedSong);
        }
    }
}
