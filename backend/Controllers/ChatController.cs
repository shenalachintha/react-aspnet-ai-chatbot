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

        var reply = await _ai.GetChatCompletionAsync(request.Messages);
        return Ok(new ChatResponse { Reply = reply });
    }
}