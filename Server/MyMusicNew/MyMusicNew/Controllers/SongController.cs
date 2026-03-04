using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Repositories.Interfaces;
using Service.Dto;
using Service.Interfaces;

namespace MyMusicNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongController(IService<SongDto> service):ControllerBase
    {
        private readonly IService<SongDto> _service = service;

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
                await _service.DeleteItem(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] SongDto item)
        {
            try
            {
                var addSong = await _service.AddItem(item);
                return Ok(addSong);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }


    }
}
