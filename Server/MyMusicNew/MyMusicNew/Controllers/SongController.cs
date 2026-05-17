using DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;
using System.Security.Claims;
using Service;
using Repositories.Entities;

namespace MyMusicNew.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SongController : ControllerBase
    {
        private readonly ISong<SongDto> _serviceSong;
        private readonly IService<SongDto> _service;
        private readonly Tools _tools;
        private readonly MusicContext _context;

        public SongController(
            IService<SongDto> service,
            ISong<SongDto> serviceSong,
            Tools tools,
            MusicContext context)
        {
            _service = service;
            _serviceSong = serviceSong;
            _tools = tools;
            _context = context;
        }

        [HttpGet("my-songs")]
        public async Task<IActionResult> GetAllSongsByUser()
        {
            try
            {
                var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdFromToken)) return Unauthorized();

                int currentUserId = int.Parse(userIdFromToken);
                var userSongs = await _serviceSong.GetAll(currentUserId);

                return Ok(userSongs ?? new List<SongDto>());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "שגיאה בשליפת השירים", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var song = await _service.GetById(id);
            return song == null ? NotFound() : Ok(song);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Unauthorized();

                var song = await _service.GetById(id);
                if (song == null) return NotFound();
                if (song.UserId != int.Parse(userIdClaim.Value)) return Forbid();

                await _service.DeleteItem(id);
                return NoContent();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("upload-music/{playlistId}")]
        public async Task<IActionResult> UploadMusic(IFormFile file, int playlistId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || file == null) return BadRequest("משתמש לא מזוהה או קובץ חסר");
                int currentUserId = int.Parse(userIdClaim.Value);

                // 1. יצירת נתיב ושמירת הקובץ בדיסק
                string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 2. חילוץ מטא-דאטה ראשוני מהקובץ
                var info = _tools.ExtractMetadata(filePath);

                // 3. הוספת השיר לטבלת Songs
                var addedSong = await _service.AddItem(new SongDto
                {
                    Title = info.Title,
                    Artist = info.Artist,
                    FilePath = "/uploads/" + fileName,
                    UserId = currentUserId
                });

                // שמירה ראשונית כדי שה-Database ייצר באופן סופי מזהה (SongId) לשיר החדש!
                await _context.SaveChangesAsync();

                // 4. הקריאה ל-Tools לניתוח מעמיק ושמירת הפיצ'רים (העברת ה-SongId שנוצר בזה הרגע)
                var features = await _tools.GetAudioFeaturesFromAI(addedSong.Title, addedSong.Artist, addedSong.SongId);
                _context.AudioFeatures.Add(features);

                // 5. יצירת הקשר והשיוך לפלייליסט הספציפי
                var playlistLink = new PlaylistSong
                {
                    PlaylistId = playlistId,
                    SongId = addedSong.SongId,
                    AddedAt = DateTime.Now
                };
                _context.PlaylistSongs.Add(playlistLink);

                // שמירה סופית של הפיצ'רים והקישור לפלייליסט במכה אחת
                await _context.SaveChangesAsync();

                // 🌟 התיקון החסין: שולפים את ה-Entity האמיתי ולא את ה-Dto, ומבצעים Reload
                var songEntity = await _context.Songs.FindAsync(addedSong.SongId);
                if (songEntity != null)
                {
                    await _context.Entry(songEntity).ReloadAsync();
                }

                // 6. שליפת השיר המעודכן והמלא דרך ה-Service
                var updatedSong = await _service.GetById(addedSong.SongId);

                return Ok(new
                {
                    status = "Success",
                    song = updatedSong,
                    features = features
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}