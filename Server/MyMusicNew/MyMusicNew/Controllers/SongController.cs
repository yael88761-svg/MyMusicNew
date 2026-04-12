using DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Repositories.Interfaces;
using Service;
using Service.Dto;
using Service.Interfaces;
using System.Security.Claims;

namespace MyMusicNew.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SongController(
        IService<SongDto> service,
        ISong<SongDto> serviceSong,
        Tools tools,
        IWebHostEnvironment env,
        MusicContext context) : ControllerBase
    {
        private readonly ISong<SongDto> _serviceSong = serviceSong;
        private readonly IService<SongDto> _service = service;
        private readonly Tools _tools = tools;
        private readonly IWebHostEnvironment _env = env;
        private readonly MusicContext _context = context;

        [HttpGet("my-songs")]
        public async Task<IActionResult> GetAllSongsByUser()
        {
            try
            {
                var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdFromToken)) return Unauthorized("לא נמצא מזהה משתמש בטוקן");

                int currentUserId = int.Parse(userIdFromToken);
                var userSongs = await _serviceSong.GetAll(currentUserId);
                return Ok(userSongs);
            }
            catch (Exception ex)
            {
                return BadRequest("שגיאה בשליפת השירים: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var songs = await _service.GetAll();
            return Ok(songs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var song = await _service.GetById(id);
            if (song == null) return NotFound();
            return Ok(song);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int currentUserId = int.Parse(userIdClaim.Value);

                var song = await _service.GetById(id);
                if (song == null) return NotFound();

                if (song.UserId != currentUserId) return Forbid();

                await _service.DeleteItem(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("upload-music")]
        public async Task<IActionResult> UploadMusic(IFormFile file)
        {
            try
            {
                // 1. זיהוי משתמש ובדיקת קובץ
                var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdFromToken)) return Unauthorized();
                int currentUserId = int.Parse(userIdFromToken);

                if (file == null || file.Length == 0) return BadRequest("לא נבחר קובץ");

                // 2. שמירת הקובץ בשרת
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // 3. חילוץ נתונים ושמירת השיר ב-DB
                var info = _tools.ExtractMetadata(filePath);
                var newSongDto = new SongDto
                {
                    Title = info.Title,
                    Artist = info.Artist,
                    FilePath = "/uploads/" + uniqueFileName,
                    UserId = currentUserId,
                    Genre = "General"
                };

                var addedSong = await _service.AddItem(newSongDto);

                // 4. ניתוח AI ושמירת מאפיינים (AudioFeatures)
                try
                {
                    var features = await _tools.GetAudioFeaturesFromAI(addedSong.Title, addedSong.Artist, addedSong.SongId);

                    if (features != null)
                    {
                        features.SongId = addedSong.SongId;
                        _context.AudioFeatures.Add(features);
                        await _context.SaveChangesAsync();

                        return Ok(new
                        {
                            status = "Success",
                            message = "השיר הועלה ונותח בהצלחה",
                            song = addedSong,
                            ai_features = features
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            status = "Partial Success",
                            message = "השיר הועלה, אך ה-AI לא החזיר נתונים",
                            song = addedSong
                        });
                    }
                }
                catch (Exception aiEx)
                {
                    return Ok(new
                    {
                        status = "Partial Success",
                        message = "השיר הועלה, אך ניתוח ה-AI נכשל",
                        error = aiEx.Message,
                        song = addedSong
                    });
                }
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "אין פירוט נוסף";
                return BadRequest(new { error = "שגיאה כללית", details = ex.Message, dbError = innerMessage });
            }
        }
    }
}