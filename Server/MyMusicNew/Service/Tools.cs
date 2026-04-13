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

        public async Task<AudioFeatures> GetAudioFeaturesFromAI(string title, string artist, int songId)
        {
            try
            {
                var client = new RestClient("https://generativelanguage.googleapis.com");
                var request = new RestRequest("/v1beta/models/gemini-1.5-flash:generateContent", Method.Post);
                request.AddQueryParameter("key", "AIzaSyAIeNsrjIlU6vndPY8XYZetIhdrX8NEGCc");

                var prompt = $"Analyze the musical characteristics of the song '{title}' by '{artist}'. " +
                             "Return ONLY a raw JSON object: {\"tempo\": int, \"energy\": float, \"valence\": float, \"danceability\": float}";

                request.AddJsonBody(new { contents = new[] { new { parts = new[] { new { text = prompt } } } } });
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
                {
                    using var doc = JsonDocument.Parse(response.Content);
                    var aiRawText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                    var cleanJson = aiRawText?.Replace("```json", "").Replace("```", "").Trim();

                    var data = JsonSerializer.Deserialize<AudioFeaturesJsonDto>(cleanJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return new AudioFeatures
                    {
                        SongId = songId,
                        Tempo = data?.tempo ?? 120,
                        Energy = data?.energy ?? 0.5f,
                        Valence = data?.valence ?? 0.5f,
                        Danceability = data?.danceability ?? 0.5f,
                        Key = "Unknown"
                    };
                }
            }
            catch { /* Fallback to defaults */ }

            return new AudioFeatures { SongId = songId, Tempo = 120, Energy = 0.5f, Valence = 0.5f, Danceability = 0.5f, Key = "Unknown" };
        }

        public SongInfo ExtractMetadata(string filePath)
        {
            try
            {
                var file = TagLib.File.Create(filePath);
                return new SongInfo
                {
                    Title = file.Tag.Title ?? "Unknown Title",
                    Artist = file.Tag.FirstPerformer ?? "Unknown Artist",
                    Duration = file.Properties.Duration
                };
            }
            catch { return new SongInfo { Title = "Unknown Title", Artist = "Unknown Artist", Duration = TimeSpan.Zero }; }
        }

        public async Task<Song?> GetSimilarSongAsync(int currentSongId)
        {
            var currentFeatures = await _context.AudioFeatures.FirstOrDefaultAsync(f => f.SongId == currentSongId);
            if (currentFeatures == null) return null;

            return await _context.AudioFeatures
                .Include(f => f.Song)
                .Where(f => f.SongId != currentSongId)
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