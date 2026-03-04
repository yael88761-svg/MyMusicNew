using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

namespace MyMusicNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlaylistController(IService<PlaylistDto> service):ControllerBase
    {
        private readonly IService<PlaylistDto> _service = service;

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
                var addPlaylist = await _service.AddItem(item);
                return Ok(addPlaylist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }


    }
}
