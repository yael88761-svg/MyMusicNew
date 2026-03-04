using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;

namespace MyMusicNew.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AudioFeaturesController(IService<AudioFeaturesDto> service) : ControllerBase
    {
        private readonly IService<AudioFeaturesDto> _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var AudioFeatures = await _service.GetAll();
            return Ok(AudioFeatures);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var AudioFeatures = await _service.GetById(id);
            if (AudioFeatures == null)
            {
                return NotFound();
            }
            return Ok(AudioFeatures);
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
        public async Task<IActionResult> AddItem([FromBody] AudioFeaturesDto item)
        {
            try
            {
                var addAudioFeatures = await _service.AddItem(item);
                return Ok(addAudioFeatures);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

    }
}
