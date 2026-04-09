using Microsoft.AspNetCore.Mvc;
using Service;

[ApiController]
[Route("api/[controller]")]
public class ToolsController : ControllerBase
{
    private readonly Tools _tools; // יצירת משתנה לכלים

    // הזרקה ב-Constructor
    public ToolsController(Tools tools)
    {
        _tools = tools;
    }

    [HttpGet("top-tracks/{userId}")]
    public async Task<IActionResult> GetTop(int userId)
    {
        var result = await _tools.GetTopTracksAsync(userId);
        return Ok(result);
    }
}