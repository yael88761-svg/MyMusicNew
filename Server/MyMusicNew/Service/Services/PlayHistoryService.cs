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
    public class PlayHistoryService(IPlayHistoryRepository repository, IMapper mapper) : IService<PlayHistoryDto>, IPlayHistory
    {
        private readonly IPlayHistoryRepository _repository = repository;
        private readonly IMapper _mapper = mapper;


        public async Task<List<PlayHistoryDto>> GetUserHistory(int userId)
        {
            var history = await _repository.GetByUserId(userId);
            return _mapper.Map<List<PlayHistoryDto>>(history);
        }

        public async Task<PlayHistoryDto> AddToHistory(PlayHistoryDto dto, int userId)
        {
            // בדיקה האם השיר כבר קיים בהיסטוריה של המשתמש כדי למנוע כפילויות
            var userHistory = await _repository.GetByUserId(userId);
            var existingEntry = userHistory.FirstOrDefault(h => h.SongId == dto.SongId);

            if (existingEntry != null)
            {
                // אם קיים - עדכון זמן השמעה ל"עכשיו" (קופץ לראש הרשימה)
                existingEntry.PlayedAt = DateTime.UtcNow;
                existingEntry.CompletionPercentage = dto.CompletionPercentage;
                var updated = await _repository.UpdateItem(existingEntry.HistoryId, existingEntry);
                return _mapper.Map<PlayHistoryDto>(updated);
            }

            // אם לא קיים - יצירת רשומה חדשה
            var entity = _mapper.Map<PlayHistory>(dto);
            entity.UserId = userId;
            entity.PlayedAt = DateTime.UtcNow;

            var added = await _repository.AddItem(entity);
            return _mapper.Map<PlayHistoryDto>(added);
        }

        // --- מימוש IService (מתודות גנריות) ---

        public async Task<List<PlayHistoryDto>> GetAll()
        {
            var history = await _repository.GetAll();
            return _mapper.Map<List<PlayHistoryDto>>(history);
        }

        public async Task<PlayHistoryDto> GetById(int id)
        {
            var history = await _repository.GetById(id);
            return _mapper.Map<PlayHistoryDto>(history);
        }

        public async Task<PlayHistoryDto> AddItem(PlayHistoryDto item)
        {
            var entity = _mapper.Map<PlayHistory>(item);
            var added = await _repository.AddItem(entity);
            return _mapper.Map<PlayHistoryDto>(added);
        }

        public async Task<PlayHistoryDto> UpdateItem(int id, PlayHistoryDto item)
        {
            var entity = _mapper.Map<PlayHistory>(item);
            var updated = await _repository.UpdateItem(id, entity);
            return _mapper.Map<PlayHistoryDto>(updated);
        }

        public async Task DeleteItem(int id)
        {
            var existing = await _repository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException($"PlayHistory with id {id} not found");

            await _repository.DeleteItem(id);
        }
    }
}