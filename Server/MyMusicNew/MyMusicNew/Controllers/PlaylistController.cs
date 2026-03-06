using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;
using System.Security.Claims;
namespace MyMusicNew.Controllers
{
    //[Authorize]
    //[ApiController]
    //[Route("api/[controller]")]

    //public class PlaylistController(IService<PlaylistDto> service, IPlaylist<PlaylistDto> playlistService):ControllerBase
    //{
    //    private readonly IService<PlaylistDto> _service = service;
    //    private readonly IPlaylist<PlaylistDto> _playlistService = playlistService;

    //    [HttpGet("my-playlists")]
    //    public async Task<IActionResult> GetMyPlaylists()
    //    {
    //        // 1. שליפת ה-ID מהטוקן (Claims)
    //        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
    //        if (userIdClaim == null) return Unauthorized("User ID not found in token");

    //        int userId = int.Parse(userIdClaim.Value);

    //        // 2. קריאה לסרוויס הספציפי עם ה-ID שחילצנו
    //        var myPlaylists = await _playlistService.GetAll(userId);

    //        return Ok(myPlaylists);
    //    }


    //    [HttpGet]
    //    public async Task<IActionResult> GetAll()
    //    {
    //        var Playlists = await _service.GetAll();
    //        return Ok(Playlists);
    //    }
    //    [HttpGet("{id}")]
    //    public async Task<IActionResult> GetById(int id)
    //    {
    //        var Playlist = await _service.GetById(id);
    //        if (Playlist == null)
    //        {
    //            return NotFound();
    //        }
    //        return Ok(Playlist);
    //    }

    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        try
    //        {
    //            // קודם בודקים אם הוא קיים ושייך למשתמש
    //            var playlist = await _service.GetById(id);
    //            if (playlist == null) return NotFound();

    //            if (playlist.UserId != GetUserId())
    //                return Forbid("You cannot delete a playlist that isn't yours.");

    //            await _service.DeleteItem(id);
    //            return NoContent();
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //    [HttpPost]
    //    public async Task<IActionResult> AddItem([FromBody] PlaylistDto item)
    //    {
    //        try
    //        {
    //            // חילוץ ה-ID מהטוקן כדי לוודא שהפלייליסט נרשם על שמי
    //            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
    //            if (userIdClaim == null) return Unauthorized();

    //            // עדכון ה-UserId בתוך האובייקט שמגיע מהלקוח
    //            item.UserId = int.Parse(userIdClaim.Value);

    //            var addPlaylist = await _service.AddItem(item);
    //            return Ok(addPlaylist);
    //        }
    //        catch (Exception ex)
    //        {
    //            return BadRequest(ex.Message);
    //        }
    //    }

    //}


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
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
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
