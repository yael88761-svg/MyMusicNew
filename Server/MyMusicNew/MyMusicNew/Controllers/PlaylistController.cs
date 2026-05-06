using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;
using System.Security.Claims;
namespace MyMusicNew.Controllers
{
    


    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistController(IService<PlaylistDto> service, IPlaylist<PlaylistDto> playlistService) : ControllerBase
    {
        private readonly IService<PlaylistDto> _service = service;
        private readonly IPlaylist<PlaylistDto> _playlistService = playlistService;

        // פונקציית עזר פרטית כדי לא לשכפל קוד של חילוץ ID
        private int GetUserId()
        {
            // נסיון ראשון: לפי הטיפוס הסטנדרטי
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            // נסיון שני: אם ה-ID נשמר תחת המפתח "userId" בטוקן
            if (claim == null)
            {
                claim = User.FindFirst("userId");
            }

            return claim == null ? 0 : int.Parse(claim.Value);
        }
        [HttpGet("my-playlists")]
        public async Task<IActionResult> GetMyPlaylists()
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var myPlaylists = await _playlistService.GetAll(userId);
            return Ok(myPlaylists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var playlist = await _service.GetById(id);

            if (playlist == null) return NotFound();

            // בדיקה קריטית: האם הפלייליסט שייך למשתמש המחובר?
            if (playlist.UserId != GetUserId())
                return Forbid("You don't have permission to view this playlist.");

            return Ok(playlist);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // קודם בודקים אם הוא קיים ושייך למשתמש
                var playlist = await _service.GetById(id);
                if (playlist == null) return NotFound();

                if (playlist.UserId != GetUserId())
                    return Forbid("You cannot delete a playlist that isn't yours.");

                await _service.DeleteItem(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] PlaylistDto item)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            item.UserId = userId; // דריסת ה-ID לביטחון

            var addPlaylist = await _service.AddItem(item);
            return Ok(addPlaylist);
        }
    }
}
