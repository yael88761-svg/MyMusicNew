using AutoMapper;
using Repositories.Entities;
using Repositories.Interfaces;
using Service.Dto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services
{
    // 1. הזרקת הממשק הספציפי IPlayHistoryRepository במקום IRepository הכללי
    // 2. מימוש הממשק IPlayHistory בנוסף ל-IService
    public class PlayHistoryService(IPlayHistory repository, IMapper mapper) : IService<PlayHistoryDto>, IPlayHistory
    {
        private readonly IPlayHistory _repository = repository;
        private readonly IMapper _mapper = mapper;

        // --- מימוש המתודות של IPlayHistory (עבור הטוקן) ---

        public async Task<List<PlayHistoryDto>> GetUserHistory(int userId)
        {
            // שימוש במתודה החדשה שיצרנו ברפוזיטורי
            var history = await _repository.GetByUserId(userId);
            return _mapper.Map<List<PlayHistoryDto>>(history);
        }

        public async Task<PlayHistoryDto> AddToHistory(PlayHistoryDto dto, int userId)
        {
            var entity = _mapper.Map<PlayHistory>(dto);

            // אבטחה: שותלים את ה-ID מהטוקן ומבטיחים תאריך נכון
            entity.UserId = userId;
            entity.PlayedAt = DateTime.UtcNow;

            var added = await _repository.AddItem(entity);
            return _mapper.Map<PlayHistoryDto>(added);
        }

        // --- מתודות קיימות עם תיקונים קלים ---

        public async Task<PlayHistoryDto> AddItem(PlayHistoryDto item)
        {
            // שימי לב: מחקתי את השורה שקובעת SongId = 3, זה בטח היה לבדיקה :)
            var playHistoryEntity = _mapper.Map<PlayHistory>(item);
            var addedPlayHistory = await _repository.AddItem(playHistoryEntity);
            return _mapper.Map<PlayHistoryDto>(addedPlayHistory);
        }

        public async Task DeleteItem(int id)
        {
            var existing = await _repository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException($"PlayHistory with id {id} not found");

            await _repository.DeleteItem(id);
        }

        public async Task<List<PlayHistoryDto>> GetAll()
        {
            var playHistory = await _repository.GetAll();
            return _mapper.Map<List<PlayHistoryDto>>(playHistory);
        }

        public async Task<PlayHistoryDto> GetById(int id)
        {
            var playHistory = await _repository.GetById(id);
            return _mapper.Map<PlayHistoryDto>(playHistory);
        }

        public async Task<PlayHistoryDto> UpdateItem(int id, PlayHistoryDto item)
        {
            var playHistoryEntity = _mapper.Map<PlayHistory>(item);
            var updatedPlayHistory = await _repository.UpdateItem(id, playHistoryEntity);
            return _mapper.Map<PlayHistoryDto>(updatedPlayHistory);
        }
    }
}