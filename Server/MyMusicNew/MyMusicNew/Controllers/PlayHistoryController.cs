using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

namespace MyMusicNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class PlayHistoryController(IService<PlayHistoryDto> service) : ControllerBase
    {
        private readonly IService<PlayHistoryDto> _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var PlayHistory = await _service.GetAll();
            return Ok(PlayHistory);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var PlayHistory = await _service.GetById(id);
            if (PlayHistory == null)
            {
                return NotFound();
            }
            return Ok(PlayHistory);
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
        public async Task<IActionResult> AddItem([FromBody] PlayHistoryDto item)
        {
            try
            {
                var addPlayHistory = await _service.AddItem(item);
                return Ok(addPlayHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        }
}
