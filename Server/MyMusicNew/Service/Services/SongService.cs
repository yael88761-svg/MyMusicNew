using AutoMapper;
using Repositories.Entities;
using Repositories.Interfaces;
using Service.Dto;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO; // 🌟 משאירים רק את זה בשביל מחיקת הקבצים הפיזיים
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class SongService : IService<SongDto>, ISong<SongDto>
    {
        private readonly ISongRepository<Song> _songRepository;
        private readonly IRepository<Song> _repository;
        private readonly IMapper _mapper;

        // הקונסטרקטור המקורי והנקי שלך חוזר! בלי שום הזרקות של ה-Environment
        public SongService(IRepository<Song> repository, ISongRepository<Song> songRepository, IMapper mapper)
        {
            _repository = repository;
            _songRepository = songRepository;
            _mapper = mapper;
        }

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
                if (!string.IsNullOrEmpty(existing.FilePath))
                {
                    try
                    {
                        string cleanedPath = existing.FilePath.Replace("wwwroot/", "").TrimStart('/');
                        string baseDir = AppContext.BaseDirectory;

                        string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\"));
                        string fullPath = Path.Combine(projectRoot, "WebApi", "wwwroot", cleanedPath);

                        if (!File.Exists(fullPath))
                        {
                            fullPath = Path.Combine(baseDir, "wwwroot", cleanedPath);
                        }

                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                            Console.WriteLine($"הקובץ הפיזי נמחק בהצלחה: {fullPath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"שגיאה במחיקת הקובץ הפיזי: {ex.Message}");
                    }
                }

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

        public async Task<List<SongDto>> GetAll(int userId)
        {
            var songs = await _songRepository.GetAll(userId);
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