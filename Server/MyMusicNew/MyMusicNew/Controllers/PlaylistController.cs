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

    public class PlaylistController(IService<PlaylistDto> service, IPlaylist<PlaylistDto> playlistService):ControllerBase
    {
        private readonly IService<PlaylistDto> _service = service;
        private readonly IPlaylist<PlaylistDto> _playlistService = playlistService;

        [HttpGet("my-playlists")]
        public async Task<IActionResult> GetMyPlaylists()
        {
            // 1. שליפת ה-ID מהטוקן (Claims)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("User ID not found in token");

            int userId = int.Parse(userIdClaim.Value);

            // 2. קריאה לסרוויס הספציפי עם ה-ID שחילצנו
            var myPlaylists = await _playlistService.GetAll(userId);

            return Ok(myPlaylists);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Playlists = await _service.GetAll();
            return Ok(Playlists);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Playlist = await _service.GetById(id);
            if (Playlist == null)
            {
                return NotFound();
            }
            return Ok(Playlist);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteItem(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] PlaylistDto item)
        {
            try
            {
                // חילוץ ה-ID מהטוקן כדי לוודא שהפלייליסט נרשם על שמי
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Unauthorized();

                // עדכון ה-UserId בתוך האובייקט שמגיע מהלקוח
                item.UserId = int.Parse(userIdClaim.Value);

                var addPlaylist = await _service.AddItem(item);
                return Ok(addPlaylist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
