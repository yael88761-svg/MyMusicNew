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
    public class PlayHistoryService(IRepository<PlayHistory> repository, IMapper mapper) : IService<PlayHistoryDto>
    {
        private readonly IRepository<PlayHistory> _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<PlayHistoryDto> AddItem(PlayHistoryDto item)
        {
            var PlayHistoryEntity = _mapper.Map<PlayHistory>(item);
            PlayHistoryEntity.SongId = 3;
            var addedPlayHistory = await _repository.AddItem(PlayHistoryEntity);
            return _mapper.Map<PlayHistoryDto>(addedPlayHistory);
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
                throw new KeyNotFoundException($"PlayHistory with id {id} not found");
            }
        }

        public async Task<List<PlayHistoryDto>> GetAll()
        {
            var PlayHistory = await _repository.GetAll();
            return _mapper.Map<List<PlayHistoryDto>>(PlayHistory);
        }

        public async Task<PlayHistoryDto> GetById(int id)
        {
            var PlayHistory = await _repository.GetById(id);
            return _mapper.Map<PlayHistoryDto>(PlayHistory);
        }

        public async Task<PlayHistoryDto> UpdateItem(int id, PlayHistoryDto item)
        {
            var PlayHistoryEntity = _mapper.Map<PlayHistory>(item);
            var updatedPlayHistory = await _repository.UpdateItem(id, PlayHistoryEntity);
            return _mapper.Map<PlayHistoryDto>(PlayHistoryEntity);
        }
    }
}
