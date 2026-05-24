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

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("userId");
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

            if (playlist.UserId != GetUserId())
                return Forbid("You don't have permission to view this playlist.");

            return Ok(playlist);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var playlist = await _service.GetById(id);
                if (playlist == null) return NotFound();

                if (playlist.UserId != GetUserId())
                    return Forbid("You cannot delete a playlist that isn't yours.");

                // הגנה בשרת: בדיקה שהפלייליסט אכן ריק לפני מחיקה
                // הערה: יש לוודא שב-PlaylistDto קיים מערך Songs או שדה דומה המייצג את השירים
                if (playlist.PlaylistSongs != null && playlist.PlaylistSongs.Count > 0)
                {
                    return BadRequest("Cannot delete a playlist that contains songs.");
                }

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

            item.UserId = userId;

            var addPlaylist = await _service.AddItem(item);
            return Ok(addPlaylist);
        }

        [HttpGet("recent-playlist")]
        public async Task<IActionResult> GetRecentPlaylist()
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized();

            try
            {
                var userRecentSongs = await _playlistService.GetRecentSongs(userId);
                return Ok(new
                {
                    PlaylistName = "נוספו לאחרונה",
                    PlaylistSongs = userRecentSongs
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}