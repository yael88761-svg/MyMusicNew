using DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;
using System.Security.Claims;
using Service;

namespace MyMusicNew.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SongController(
        IService<SongDto> service,
        ISong<SongDto> serviceSong,
        Tools tools,
        MusicContext context) : ControllerBase
    {
        private readonly ISong<SongDto> _serviceSong = serviceSong;
        private readonly IService<SongDto> _service = service;
        private readonly Tools _tools = tools;
        private readonly MusicContext _context = context;

        [HttpGet("my-songs")]
        public async Task<IActionResult> GetAllSongsByUser()
        {
            try
            {
                var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdFromToken)) return Unauthorized();

                int currentUserId = int.Parse(userIdFromToken);
                var userSongs = await _serviceSong.GetAll(currentUserId);

                // תיקון: החזרת רשימה ריקה למניעת שגיאת Data is Null (נפתר עבור image_769f43.png)
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

        [HttpPost("upload-music")]
        public async Task<IActionResult> UploadMusic(IFormFile file)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || file == null) return BadRequest("משתמש לא מזוהה או קובץ חסר");
                int currentUserId = int.Parse(userIdClaim.Value);

                string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(stream); }

                var info = _tools.ExtractMetadata(filePath);
                var addedSong = await _service.AddItem(new SongDto
                {
                    Title = info.Title,
                    Artist = info.Artist,
                    FilePath = "/uploads/" + fileName,
                    UserId = currentUserId
                });

                // הפעלת ה-AI ושמירה
                var features = await _tools.GetAudioFeaturesFromAI(addedSong.Title, addedSong.Artist, addedSong.SongId);
                _context.AudioFeatures.Add(features);
                await _context.SaveChangesAsync();

                return Ok(new { status = "Success", song = addedSong, features });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}