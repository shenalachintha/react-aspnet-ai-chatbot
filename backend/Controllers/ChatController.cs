using Microsoft.AspNetCore.Mvc;
using AIChatbot.Models;
using AIChatbot.Services;

namespace AIChatbot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly AIService _ai;

    public ChatController(AIService ai)
    {
        _ai = ai;
    }

    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Post([FromBody] ChatRequest request)
    {
        if (request.Messages is null || request.Messages.Count == 0)
            return BadRequest(new { message = "Messages are required." });

        try
        {
            var reply = await _ai.GetChatCompletionAsync(request.Messages);
            return Ok(new ChatResponse { Reply = reply });
        }
        catch (InvalidOperationException ex)
        {
            // Client configuration or parsing error -> 400 so UI can show corrective message
            return BadRequest(new { message = ex.Message });
        }
        catch (HttpRequestException ex)
        {
            // Upstream HTTP issues: include upstream body/message so client can inspect
            return StatusCode(502, new { message = "Upstream API request failed.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            // Generic handler
            return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
        }
    }
}