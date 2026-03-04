using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Repositories.Interfaces;
using Service.Dto;
using Service.Interfaces;

namespace MyMusicNew.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SongController(IService<SongDto> service,ISong<SongDto> serviceSong) :ControllerBase
    {
        private readonly ISong<SongDto> _serviceSong = serviceSong;
        private readonly IService<SongDto> _service = service;

        [HttpGet("my-songs")]
        public async Task<IActionResult> GetAllSongsByUser()
        {
            try
            {
                // 1. חילוץ ה-ID מהטוקן
                var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdFromToken))
                {
                    return Unauthorized("לא נמצא מזהה משתמש בטוקן");
                }

                int currentUserId = int.Parse(userIdFromToken);

                // 2. קריאה לסרוויס המיוחד של השירים
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
            if (song == null)
            {
                return NotFound();
            }
            return Ok(song);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // 1. מי מנסה למחוק?
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                int currentUserId = int.Parse(userIdClaim.Value);

                // 2. נביא את השיר כדי לבדוק למי הוא שייך
                var song = await _service.GetById(id);
                if (song == null) return NotFound();

                // 3. הבדיקה הקריטית: האם ה-UserId של השיר שווה ל-ID של מי שמחובר?
                if (song.UserId != currentUserId)
                {
                    return Forbid(); // מחזיר 403 - אסור לך לגעת בשירים של אחרים
                }

                await _service.DeleteItem(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] SongDto item)
        {
            try
            {
                // 1. חילוץ ה-ID של המשתמש מהטוקן המאובטח
                // אנחנו מחפשים את ה-Claim שקראנו לו NameIdentifier ב-TokenService
                var userIdFromToken = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdFromToken))
                {
                    return Unauthorized("לא נמצא מזהה משתמש בטוקן");
                }

                // 2. עדכון ה-DTO עם ה-ID האמיתי של המשתמש
                // כך אנחנו מבטיחים שהשיר יירשם על שמו של מי שמחובר כרגע
                item.UserId = int.Parse(userIdFromToken);

                // 3. שליחה ל-Service לשמירה בדאטה-בייס
                var addedSong = await _service.AddItem(item);

                return Ok(addedSong);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }


    }
}
