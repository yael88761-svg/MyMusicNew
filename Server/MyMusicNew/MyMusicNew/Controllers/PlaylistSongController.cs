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
    public class PlaylistSongController(IPlaylistSong playlistSongService) : ControllerBase
    {
        private readonly IPlaylistSong _playlistSongService = playlistSongService;

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return 0;
            return int.Parse(userIdClaim.Value);
        }

        [HttpGet("my-songs")] 
        public async Task<IActionResult> GetAllMySongs()
        {
            int userId = GetUserIdFromToken();
            if (userId == 0) return Unauthorized();

            var songs = await _playlistSongService.GetAllByUserId(userId);
            return Ok(songs);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] PlaylistSongDto item)
        {
            try
            {
                int userId = GetUserIdFromToken();
                if (userId == 0) return Unauthorized();

                var addPlaylistSong = await _playlistSongService.AddItem(item, userId);
                return Ok(addPlaylistSong);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int userId = GetUserIdFromToken();
                if (userId == 0) return Unauthorized();

                await _playlistSongService.DeleteItem(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}