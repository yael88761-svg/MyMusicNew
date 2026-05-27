using DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Repositories.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; 

namespace Service
{
    public class Tools
    {
        private readonly MusicContext _context;
        private readonly IConfiguration _configuration; 

        public Tools(MusicContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        // Retrieves the user's most frequently played songs sorted by play count.
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
        // Calls the Gemini AI API to extract audio features and clean song metadata.
        public async Task<AudioFeatures> GetAudioFeaturesFromAI(string title, string artist, int songId)
        {
            try
            {
                var client = new RestClient("https://generativelanguage.googleapis.com");
                var request = new RestRequest("/v1/models/gemini-2.5-flash:generateContent", Method.Post);

                string ApiKey = _configuration["GeminiSettings:ApiKey"];
                Console.WriteLine($"[Test] My API Key length is: {ApiKey?.Length ?? 0}");
                request.AddQueryParameter("key", ApiKey);

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

            return new AudioFeatures { SongId = songId, Tempo = 120, Energy = 0.5f, Valence = 0.5f, Danceability = 0.5f, Key = "Unknown" };
        }

        // Extracts track title, artist, and duration from physical file ID3 tags.
        public SongInfo ExtractMetadata(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);
                return new SongInfo
                {
                    Title = file.Tag.Title ?? Path.GetFileNameWithoutExtension(filePath), 
                    Artist = file.Tag.FirstPerformer ?? "Unknown Artist",
                    Duration = file.Properties.Duration
                };
            }
            catch { return new SongInfo { Title = Path.GetFileNameWithoutExtension(filePath), Artist = "Unknown Artist", Duration = TimeSpan.Zero }; }
        }
        // Finds the closest matching song based on the absolute distance of musical features.
        public async Task<Song?> GetSimilarSongAsync(int currentSongId, int userId)
        {
            var currentFeatures = await _context.AudioFeatures.FirstOrDefaultAsync(f => f.SongId == currentSongId);
            if (currentFeatures == null) return null;

            return await _context.AudioFeatures
                .Include(f => f.Song)
                .Where(f => f.SongId != currentSongId && f.Song.UserId == userId) 
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

  //  public class RecommendationService
   // {
        // הגדרת משקלים - כמה כל תכונה חשובה (סך הכל משפיע על ה-Distance)
      //  private const float WeightBpm = 0.4f;
      //  private const float WeightEnergy = 0.3f;
      //  private const float WeightValence = 0.3f;

    //    public List<AudioFeatures> GetNextSongRecommendations(AudioFeatures currentSong, List<AudioFeatures> allSongs, List<int> recentlyPlayedIds)
    //    {
    //        var scoredSongs = new List<(AudioFeatures Song, float Score)>();

    //        foreach (var song in allSongs)
    //        {
    //            // 1. הגנה מפני כפילויות: דלג על השיר הנוכחי ושירים שנוגנו ממש עכשיו
    //            if (song.SongId == currentSong.SongId || recentlyPlayedIds.Contains(song.SongId))
    //                continue;

    //            // 2. נרמול ה-BPM (מכיוון ש-BPM הוא בין 60 ל-180, נחלק ב-100 כדי להביא אותו לסקאלה של 0-1 כמו שאר התכונות)
    //            float bpmDiff = Math.Abs(currentSong.Tempo - song.Tempo) / 100f;
    //            float energyDiff = Math.Abs(currentSong.Energy - song.Energy);
    //            float valenceDiff = Math.Abs(currentSong.Valence - song.Valence);

    //            // 3. חישוב מרחק משוקלל (Weighted Euclidean Distance)
    //            double distance = Math.Sqrt(
    //                (WeightBpm * Math.Pow(bpmDiff, 2)) +
    //                (WeightEnergy * Math.Pow(energyDiff, 2)) +
    //                (WeightValence * Math.Pow(valenceDiff, 2))
    //            );

    //            float finalScore = (float)distance;

    //            // 4. לוגיקת סולם (Harmonic Mixing Boost)
    //            // אם הסולם זהה לחלוטין - נוריד מהמרחק (כלומר נקרב את השיר)
    //            if (!string.IsNullOrEmpty(currentSong.Key) && currentSong.Key == song.Key)
    //            {
    //                finalScore -= 0.1f; // בונוס התאמה מושלמת
    //            }
    //            else if (IsHarmonicallyCompatible(currentSong.Key, song.Key))
    //            {
    //                finalScore -= 0.05f; // בונוס התאמה קרובה (מעגל הקווינטות)
    //            }

    //            scoredSongs.Add((song, finalScore));
    //        }

    //        // מיון מהמרחק הקטן ביותר (הכי מתאים) לגדול ביותר
    //        return scoredSongs.OrderBy(s => s.Score).Select(s => s.Song).ToList();
    //    }

    //    // פונקציית עזר לבדיקת תאימות סולמות בסיסית
    //    private bool IsHarmonicallyCompatible(string keyA, string keyB)
    //    {
    //        if (string.IsNullOrEmpty(keyA) || string.IsNullOrEmpty(keyB)) return false;

    //        // מיפוי בסיסי של סולמות מקבילים (Major ל-Minor יחסי) לדוגמה: C Major ו-A Minor
    //        // ניתן להרחיב את זה למילון (Dictionary) מלא של Camelot Wheel
    //        var relativeKeys = new Dictionary<string, string>
    //    {
    //        { "C Major", "A Minor" }, { "G Major", "E Minor" }, { "D Major", "B Minor" },
    //        { "A Major", "F# Minor" }, { "E Major", "C# Minor" }, { "B Major", "G# Minor" },
    //        { "F Major", "D Minor" }, { "Bb Major", "G Minor" }, { "Eb Major", "C Minor" }
    //    };

    //        if (relativeKeys.TryGetValue(keyA, out var relative) && relative == keyB) return true;
    //        if (relativeKeys.TryGetValue(keyB, out var relativeInverted) && relativeInverted == keyA) return true;

    //        return false;
    //    }
    //}

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