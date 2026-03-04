using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Service.Dto;
using Service.Interfaces;

namespace MyMusicNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlaylistSongController(IService<PlaylistSongDto> service): ControllerBase
    {
        private readonly IService<PlaylistSongDto> _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var PlaylistSongs = await _service.GetAll();
            return Ok(PlaylistSongs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var PlaylistSong = await _service.GetById(id);
            if (PlaylistSong == null)
            {
                return NotFound();
            }
            return Ok(PlaylistSong);
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
        public async Task<IActionResult> AddItem([FromBody] PlaylistSongDto item)
        {
            try
            {
                var addPlaylistSong = await _service.AddItem(item);
                return Ok(addPlaylistSong);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }

    }
}
