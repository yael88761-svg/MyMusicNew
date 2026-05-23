using Microsoft.AspNetCore.Mvc;
using Service;

namespace MyMusicProject.Controllers // ודא שזה ה-Namespace המתאים לפרויקט שלך
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToolsController : ControllerBase
    {
        private readonly Tools _tools; // יצירת משתנה לכלים

        // הזרקה ב-Constructor
        public ToolsController(Tools tools)
        {
            _tools = tools;

        }

        [HttpGet("top-tracks/{userId}")]
        public async Task<IActionResult> GetTop(int userId)
        {
            var result = await _tools.GetTopTracksAsync(userId);
            return Ok(result);
        }

        // ✅ ה-Endpoint החדש שנוסף עבור ההפעלה החכמה
        [HttpGet("similar")]
        public async Task<IActionResult> GetSimilarSong([FromQuery] int currentSongId, [FromQuery] int userId)
        {
            // שליחת שני הפרמטרים לפונקציה המעודכנת
            var similarSong = await _tools.GetSimilarSongAsync(currentSongId, userId);

            if (similarSong == null)
            {
                return NotFound(new { message = "לא נמצא שיר דומה השייך למשתמש זה" });
            }

            return Ok(similarSong);
        }
    }
}