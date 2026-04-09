using DataContext;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class Tools
    {
        private readonly MusicContext _context;

        // ה-Constructor: כאן אנחנו מקבלים את החיבור ל-DB
        public Tools(MusicContext context)
        {
            _context = context;
        }


        // הפונקציה הראשונה: השירים הכי מושמעים
        public async Task<List<Song>> GetTopTracksAsync(int userId, int limit = 10)
        {
            // 1. הולכים לטבלת ההיסטוריה וסופרים כמה פעמים כל שיר הופיע
            var topStats = await _context.PlayHistories
                .Where(ph => ph.UserId == userId)
                .GroupBy(ph => ph.SongId)
                .Select(g => new
                {
                    SongId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(limit)
                .ToListAsync();

            // 2. מוציאים את ה-ID של השירים שמצאנו
            var songIds = topStats.Select(s => s.SongId).ToList();

            // 3. שולפים את פרטי השירים המלאים מטבלת Songs
            return await _context.Songs
                .Where(s => songIds.Contains(s.SongId))
                .ToListAsync();
        }


        // 1. פונקציית ה-AI: מקבלת שם ואמן ומחזירה מאפיינים פסיכולוגיים של השיר
        public async Task<AudioFeatures> GetAudioFeaturesFromAI(string title, string artist, int songId)
        {
            // יצירת ה"שליח" (RestSharp) עם הכתובת של גוגל והמפתח שלך
            var client = new RestClient("https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=AIzaSyAIeNsrjIlU6vndPY8XYZetIhdrX8NEGCc");
            var request = new RestRequest("", Method.Post);

            // הגדרת השאלה (Prompt) - אנחנו מבקשים מה-AI להחזיר רק JSON כדי שהמחשב יוכל לקרוא אותו בקלות
            var prompt = $"Analyze the song '{title}' by '{artist}'. Return ONLY a JSON object with these exact fields: " +
                         "tempo (int), energy (float 0-1), valence (float 0-1), danceability (float 0-1). " +
                         "No conversational text, just the JSON.";

            // אריזת השאלה בפורמט שגוגל מבינה
            request.AddJsonBody(new { contents = new[] { new { parts = new[] { new { text = prompt } } } } });

            // השליח יוצא לדרך ומחכה לתשובה
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && response.Content != null)
            {
                // ה-AI לפעמים עוטף את התשובה בסימנים של קוד (```json), אנחנו מנקים אותם
                var cleanJson = response.Content.Replace("```json", "").Replace("```", "").Trim();

                // הופכים את הטקסט שחזר מה-AI לאובייקט C# אמיתי
                var data = JsonSerializer.Deserialize<AudioFeaturesJsonDto>(cleanJson);

                return new AudioFeatures
                {
                    SongId = songId,
                    Tempo = data.tempo,
                    Energy = data.energy,
                    Valence = data.valence,
                    Danceability = data.danceability,
                    Key = "Unknown"
                };
            }
            return null;
        }
      // 2. פונקציית חילוץ מידע: קוראת את הנתונים ה"נסתרים" בתוך קובץ ה - MP3
        public SongInfo ExtractMetadata(string filePath)
        {
            // TagLib פותחת את הקובץ הפיזי שנמצא בנתיב שנתנו לה
            var file = TagLib.File.Create(filePath);

            return new SongInfo
            {
                Title = file.Tag.Title ?? "Unknown Title",
                Artist = file.Tag.FirstPerformer ?? "Unknown Artist",
                Duration = file.Properties.Duration
            };
        }

        // 1. פונקציית ההמלצה: מוצאת שיר עם "וייב" דומה על סמך נתוני ה-AI
        public async Task<Song> GetSimilarSongAsync(int currentSongId)
        {
            // שליפת המאפיינים של השיר שמתנגן עכשיו
            var currentFeatures = await _context.AudioFeatures
                .FirstOrDefaultAsync(f => f.SongId == currentSongId);

            if (currentFeatures == null) return null;

            // חישוב מתמטי פשוט: מי השיר שההפרש באנרגיה ובשמחה שלו הוא הכי קטן?
            var recommendedSong = await _context.AudioFeatures
                .Include(f => f.Song) // מביא גם את פרטי השיר
                .Where(f => f.SongId != currentSongId)
                .OrderBy(f => Math.Abs(f.Energy - currentFeatures.Energy) +
                              Math.Abs(f.Valence - currentFeatures.Valence))
                .Select(f => f.Song)
                .FirstOrDefaultAsync();

            return recommendedSong;
        }

        // 2. רישום היסטוריה: מעדכן את מסד הנתונים ששיר הושמע
        public async Task LogPlayHistoryAsync(int userId, int songId)
        {
            var history = new PlayHistory
            {
                UserId = userId,
                SongId = songId,
                PlayedAt = DateTime.Now
            };

            _context.PlayHistories.Add(history);
            await _context.SaveChangesAsync();
        }

    }
    public class AudioFeaturesJsonDto
    {
        public float tempo { get; set; }
        public float energy { get; set; }
        public float valence { get; set; }
        public float danceability { get; set; }
    }

    public class SongInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
