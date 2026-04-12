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


        public async Task<AudioFeatures> GetAudioFeaturesFromAI(string title, string artist, int songId)
        {
            try
            {
                var client = new RestClient("https://generativelanguage.googleapis.com");
                var request = new RestRequest("/v1beta/models/gemini-1.5-flash:generateContent", Method.Post);
                request.AddQueryParameter("key", "AIzaSyAIeNsrjIlU6vndPY8XYZetIhdrX8NEGCc");

                // פרומפט ממוקד יותר
                var prompt = $"Analyze the musical characteristics of the song '{title}' by '{artist}'. " +
                             "If you don't know the song, provide estimated values based on the genre. " +
                             "Return ONLY a raw JSON object: {\"tempo\": int, \"energy\": float, \"valence\": float, \"danceability\": float}";

                request.AddJsonBody(new { contents = new[] { new { parts = new[] { new { text = prompt } } } } });

                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    using var doc = JsonDocument.Parse(response.Content);
                    var aiRawText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                    var cleanJson = aiRawText.Replace("```json", "").Replace("```", "").Trim();
                    var data = JsonSerializer.Deserialize<AudioFeaturesJsonDto>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return new AudioFeatures
                    {
                        SongId = songId,
                        Tempo = (int)data.tempo,
                        Energy = data.energy,
                        Valence = data.valence,
                        Danceability = data.danceability,
                        Key = "Unknown"
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AI Error: " + ex.Message);
            }

            // אם הכל נכשל - מחזירים ערכי ברירת מחדל כדי שהטבלה לא תישאר ריקה
            return new AudioFeatures
            {
                SongId = songId,
                Tempo = 120,
                Energy = 0.5f,
                Valence = 0.5f,
                Danceability = 0.5f,
                Key = "Unknown"
            };
        }

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
            // חישוב מתמטי פשוט: מי השיר שההפרש באנרגיה ובשמחה שלו הוא הכי קטן
            var recommendedSong = await _context.AudioFeatures
                .Include(f => f.Song) // מביא גם את פרטי השיר
                .Where(f => f.SongId != currentSongId)
                .OrderBy(f => Math.Abs((double)(f.Energy ?? 0.5f) - (double)(currentFeatures.Energy ?? 0.5f)) +
                             Math.Abs((double)(f.Valence ?? 0.5f) - (double)(currentFeatures.Valence ?? 0.5f)))
                .Select(f => f.Song) // שליפת השיר עצמו ולא את טבלת המאפיינים
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
        public float ?energy { get; set; }
        public float ?valence { get; set; }
        public float danceability { get; set; }
    }

    public class SongInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
