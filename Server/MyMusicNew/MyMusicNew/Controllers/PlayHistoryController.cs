using Microsoft.AspNetCore.Mvc;
using Service.Dto;
using Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyMusicNew.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("api/[controller]")]
    public class PlayHistoryController(IPlayHistory service) : ControllerBase
    {
        private readonly IPlayHistory _service = service;

        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim == null ? 0 : int.Parse(claim.Value);
        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory()
        {
            int userId = GetUserIdFromToken();
            var history = await _service.GetUserHistory(userId);
            return Ok(history);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var playHistory = await ((IService<PlayHistoryDto>)_service).GetById(id);
            if (playHistory == null)
            {
                return NotFound();
            }
            return Ok(playHistory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await ((IService<PlayHistoryDto>)_service).DeleteItem(id);
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
                int userId = GetUserIdFromToken();
                var addPlayHistory = await _service.AddToHistory(item, userId);
                return Ok(addPlayHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}