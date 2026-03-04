using AutoMapper;
using Repositories.Entities;
using Repositories.Interfaces;
using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public class AudioFeaturesService(IRepository<AudioFeatures> repository, IMapper mapper) : IService<AudioFeaturesDto>
    {
        private readonly IRepository<AudioFeatures> _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<AudioFeaturesDto> AddItem(AudioFeaturesDto item)
        {
            var AudioFeaturesEntity = _mapper.Map<AudioFeatures>(item);
            var addedAudioFeatures = await _repository.AddItem(AudioFeaturesEntity);
            return _mapper.Map<AudioFeaturesDto>(addedAudioFeatures);
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
                throw new KeyNotFoundException($"AudioFeatures with id {id} not found");
            }
        }

        public async Task<List<AudioFeaturesDto>> GetAll()
        {
            var AudioFeatures = await _repository.GetAll();
            return _mapper.Map<List<AudioFeaturesDto>>(AudioFeatures);
        }

        public async Task<AudioFeaturesDto> GetById(int id)
        {
            var AudioFeatures = await _repository.GetById(id);
            return _mapper.Map<AudioFeaturesDto>(AudioFeatures);
        }

        public async Task<AudioFeaturesDto> UpdateItem(int id, AudioFeaturesDto item)
        {
            var AudioFeaturesEntity = _mapper.Map<AudioFeatures>(item);
            var updatedAudioFeatures = await _repository.UpdateItem(id, AudioFeaturesEntity);
            return _mapper.Map<AudioFeaturesDto>(AudioFeaturesEntity);
        }
    }
}
