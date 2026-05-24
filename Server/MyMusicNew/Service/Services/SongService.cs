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
            // 1. שליפת השיר מה-DB כדי לבדוק אם הוא קיים ולקבל את נתיב הקובץ
            var existing = await _repository.GetById(id);

            if (existing != null)
            {
                // 2. מחיקת הקובץ הפיזי בצורה עוקפת Environment
                if (!string.IsNullOrEmpty(existing.FilePath))
                {
                    try
                    {
                        // א. ניקוי תחיליות מהנתיב ששמור ב-DB
                        string cleanedPath = existing.FilePath.Replace("wwwroot/", "").TrimStart('/');

                        // ב. מציאת תיקיית הריצה של ה-API (למשל: YourProject/bin/Debug/net8.0)
                        string baseDir = AppContext.BaseDirectory;

                        // ג. ניווט אחורה מתיקיית ה-bin אל תיקיית הפרויקט הראשית שבה נמצאת wwwroot
                        string projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\..\"));

                        // ד. בניית הנתיב המלא לקובץ בתוך wwwroot של פרויקט ה-API שלך
                        // הקוד מחפש את התיקייה בתוך הפרויקט הראשי שמריץ את האפליקציה
                        string fullPath = Path.Combine(projectRoot, "WebApi", "wwwroot", cleanedPath);

                        // 💡 אם שם פרויקט ה-Web שלך הוא לא "WebApi" (למשל Server או UI), שנה את המילה "WebApi" למטה לשם המדויק שלו:
                        if (!File.Exists(fullPath))
                        {
                            // ליתר ביטחון, אם השרת כבר רץ במצב פרודקשן והתיקייה היא מקומית לריצה:
                            fullPath = Path.Combine(baseDir, "wwwroot", cleanedPath);
                        }

                        // ה. מחיקה פיזית של הקובץ מהדיסק
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                            Console.WriteLine($"הקובץ הפיזי נמחק בהצלחה: {fullPath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // הדפסת שגיאה ללוג למקרה שהקובץ בשימוש, כדי שהמחיקה מה-DB לא תיעצר
                        Console.WriteLine($"שגיאה במחיקת הקובץ הפיזי: {ex.Message}");
                    }
                }

                // 3. מחיקה מה-DB (בזכות ה-Cascade, מוחק אוטומטית את השיר, הפיצ'רים והפלייליסטים!)
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