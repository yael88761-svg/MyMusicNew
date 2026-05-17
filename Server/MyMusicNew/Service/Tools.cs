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

        public Tools(MusicContext context)
        {
            _context = context;
        }

        public async Task<List<Song>> GetTopTracksAsync(int userId, int limit = 10)
        {
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

            var songIds = topStats.Select(s => s.SongId).ToList();

            return await _context.Songs
                .Where(s => songIds.Contains(s.SongId))
                .ToListAsync();
        }

        //public async Task<AudioFeatures> GetAudioFeaturesFromAI(string title, string artist, int songId)
        //{
        //    try
        //    {
        //        // יצירת Client חדש לכל קריאה כדי למנוע Cache של נתונים ישנים
        //        var client = new RestClient("https://generativelanguage.googleapis.com");
        //        var request = new RestRequest("/v1beta/models/gemini-1.5-flash:generateContent", Method.Post);
        //        request.AddQueryParameter("key", "AIzaSyAIeNsrjIlU6vndPY8XYZetIhdrX8NEGCc");

        //        // פרומפט הרבה יותר נוקשה שמכריח הפרדה
        //        var prompt = $@"
        //    STRICT INSTRUCTION: Analyze only the string: '{title}'. 
        //    Task: Separate it into two distinct parts: 'cleaned_artist' and 'cleaned_title'.
        //    Rules:
        //    1. Remove any underscores (__), file extensions (.mp3), or junk characters.
        //    2. If the string contains a dash '-' or ':', split it. 
        //    3. Return the result ONLY as a JSON object.

        //    Format Example:
        //    {{
        //        ""cleaned_title"": ""שם השיר בלבד"",
        //        ""cleaned_artist"": ""שם האמן בלבד"",
        //        ""key"": ""C Major"",
        //        ""tempo"": 128,
        //        ""energy"": 0.7,
        //        ""valence"": 0.5,
        //        ""danceability"": 0.6
        //    }}";

        //        request.AddJsonBody(new
        //        {
        //            contents = new[] { new { parts = new[] { new { text = prompt } } } },
        //            generationConfig = new
        //            {
        //                temperature = 0.0, // שינוי ל-0.0 - הכי פחות יצירתי שיש
        //                topP = 1,
        //                responseMimeType = "application/json"
        //            }
        //        });

        //        var response = await client.ExecuteAsync(request);

        //        if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
        //        {
        //            using var doc = JsonDocument.Parse(response.Content);
        //            var aiRawText = doc.RootElement.GetProperty("candidates")[0]
        //                                           .GetProperty("content")
        //                                           .GetProperty("parts")[0]
        //                                           .GetProperty("text").GetString();

        //            var cleanJson = aiRawText?.Replace("```json", "").Replace("```", "").Trim();
        //            var data = JsonSerializer.Deserialize<AudioFeaturesJsonDto>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //            var song = await _context.Songs.FindAsync(songId);
        //            if (song != null && data != null)
        //            {
        //                // כאן אנחנו מוודאים שאנחנו לא דורסים עם נתונים ריקים
        //                if (!string.IsNullOrEmpty(data.cleaned_title)) song.Title = data.cleaned_title;
        //                if (!string.IsNullOrEmpty(data.cleaned_artist)) song.Artist = data.cleaned_artist;

        //                await _context.SaveChangesAsync();
        //            }

        //            return new AudioFeatures
        //            {
        //                SongId = songId,
        //                Tempo = data?.tempo ?? 120,
        //                Energy = data?.energy ?? 0.5f,
        //                Valence = data?.valence ?? 0.5f,
        //                Danceability = data?.danceability ?? 0.5f,
        //                Key = data?.key ?? "Unknown"
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("AI Error: " + ex.Message);
        //    }

        //    return new AudioFeatures { SongId = songId, Tempo = 120, Energy = 0.5f, Valence = 0.5f, Danceability = 0.5f, Key = "Unknown" };
        //}
        //public SongInfo ExtractMetadata(string filePath)
        //{
        //    try
        //    {
        //        var file = TagLib.File.Create(filePath);
        //        return new SongInfo
        //        {
        //            Title = file.Tag.Title ?? "Unknown Title",
        //            Artist = file.Tag.FirstPerformer ?? "Unknown Artist",
        //            Duration = file.Properties.Duration
        //        };
        //    }
        //    catch { return new SongInfo { Title = "Unknown Title", Artist = "Unknown Artist", Duration = TimeSpan.Zero }; }
        //}
        public async Task<AudioFeatures> GetAudioFeaturesFromAI(string title, string artist, int songId)
        {
            try
            {
                var client = new RestClient("https://generativelanguage.googleapis.com");
                // שינוי לגרסה v1 ולמודל gemini-2.5-flash החדש והמהיר
                var request = new RestRequest("/v1/models/gemini-2.5-flash:generateContent", Method.Post);
                string myApiKey = "AIzaSyCKfufcoyl_raKnbIsWdk7I68QJy_BnAsE";
                request.AddQueryParameter("key", myApiKey);

                var prompt = $@"
    STRICT INSTRUCTION: Analyze the song title: '{title}' and artist: '{artist}'. 
    Tasks:
    1. Separate and clean the title string into two distinct parts: 'cleaned_artist' and 'cleaned_title' (remove underscores, .mp3, junk text).
    2. Based on your global knowledge of this song, evaluate its musical features: musical key, estimated tempo (BPM as integer), energy level (0.0 to 1.0), valence (happiness 0.0 to 1.0), and danceability (0.0 to 1.0).
    3. Return the result ONLY as a JSON object matching the format below. Do not include markdown code blocks.
    
    Format Example:
    {{
        ""cleaned_title"": ""שם השיר"",
        ""cleaned_artist"": ""שם האמן"",
        ""key"": ""C Major"",
        ""tempo"": 120,
        ""energy"": 0.5,
        ""valence"": 0.5,
        ""danceability"": 0.5
    }}";

                request.AddJsonBody(new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } },
                    generationConfig = new
                    {
                        temperature = 0.0,
                        topP = 1,
                    }
                });
                var response = await client.ExecuteAsync(request);

                // הדפסת לוג בשרת כדי לעקוב אחרי התשובה הגולמית של גוגל
                if (!response.IsSuccessful)
                {
                    Console.WriteLine($"[AI Error] Request failed with status: {response.StatusCode}. Content: {response.Content}");
                }

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    using var doc = JsonDocument.Parse(response.Content);
                    var aiRawText = doc.RootElement.GetProperty("candidates")[0]
                                                   .GetProperty("content")
                                                   .GetProperty("parts")[0]
                                                   .GetProperty("text").GetString();

                    var cleanJson = aiRawText?.Replace("```json", "").Replace("```", "").Trim();

                    // 🧠 שימוש ב-JsonNode לפענוח חסין של השדות ללא תלות ב-Case Sensitivity של ה-DTO
                    var jsonNode = System.Text.Json.Nodes.JsonNode.Parse(cleanJson);
                    if (jsonNode != null)
                    {
                        string cleanedTitle = jsonNode["cleaned_title"]?.ToString() ?? jsonNode["Cleaned_Title"]?.ToString();
                        string cleanedArtist = jsonNode["cleaned_artist"]?.ToString() ?? jsonNode["Cleaned_Artist"]?.ToString();

                        float tempo = float.TryParse(jsonNode["tempo"]?.ToString() ?? jsonNode["Tempo"]?.ToString(), out var t) ? t : 120f;
                        float energy = float.TryParse(jsonNode["energy"]?.ToString() ?? jsonNode["Energy"]?.ToString(), out var e) ? e : 0.5f;
                        float valence = float.TryParse(jsonNode["valence"]?.ToString() ?? jsonNode["Valence"]?.ToString(), out var v) ? v : 0.5f;
                        float danceability = float.TryParse(jsonNode["danceability"]?.ToString() ?? jsonNode["Danceability"]?.ToString(), out var d) ? d : 0.5f;
                        string key = jsonNode["key"]?.ToString() ?? jsonNode["Key"]?.ToString() ?? "Unknown";

                        // עדכון ה-Song ב-Database (השמירה הסופית תתבצע בקונטרולר)
                        var song = await _context.Songs.FindAsync(songId);
                        if (song != null)
                        {
                            if (!string.IsNullOrEmpty(cleanedTitle)) song.Title = cleanedTitle;
                            if (!string.IsNullOrEmpty(cleanedArtist)) song.Artist = cleanedArtist;
                        }

                        return new AudioFeatures
                        {
                            SongId = songId,
                            Tempo = tempo,
                            Energy = energy,
                            Valence = valence,
                            Danceability = danceability,
                            Key = key
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AI Critical Exception]: " + ex.Message);
            }

            // ברירת מחדל בטוחה למניעת קריסה
            return new AudioFeatures { SongId = songId, Tempo = 120, Energy = 0.5f, Valence = 0.5f, Danceability = 0.5f, Key = "Unknown" };
        }
        public SongInfo ExtractMetadata(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);
                return new SongInfo
                {
                    Title = file.Tag.Title ?? Path.GetFileNameWithoutExtension(filePath), // אם אין כותרת במטא-דאטה, ניקח את שם הקובץ לפחות
                    Artist = file.Tag.FirstPerformer ?? "Unknown Artist",
                    Duration = file.Properties.Duration
                };
            }
            catch { return new SongInfo { Title = Path.GetFileNameWithoutExtension(filePath), Artist = "Unknown Artist", Duration = TimeSpan.Zero }; }
        }
        public async Task<Song?> GetSimilarSongAsync(int currentSongId, int userId)
        {
            var currentFeatures = await _context.AudioFeatures.FirstOrDefaultAsync(f => f.SongId == currentSongId);
            if (currentFeatures == null) return null;

            return await _context.AudioFeatures
                .Include(f => f.Song)
                .Where(f => f.SongId != currentSongId && f.Song.UserId == userId) // ✅ סינון: רק שירים ששייכים למשתמש הזה
                .OrderBy(f => Math.Abs((double)f.Energy - (double)(currentFeatures.Energy ?? 0.5f)) +
                                 Math.Abs((double)f.Valence - (double)(currentFeatures.Valence ?? 0.5f)))
                .Select(f => f.Song)
                .FirstOrDefaultAsync();
        }
        public async Task LogPlayHistoryAsync(int userId, int songId)
        {
            var history = new PlayHistory { UserId = userId, SongId = songId, PlayedAt = DateTime.Now };
            _context.PlayHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }

    public class AudioFeaturesJsonDto
    {
        public string? key { get; set; }
        public string? cleaned_title { get; set; }
        public string? cleaned_artist { get; set; }
        public float tempo { get; set; }
        public float? energy { get; set; }
        public float? valence { get; set; }
        public float? danceability { get; set; }
    }

    public class SongInfo
    {
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
    }
}