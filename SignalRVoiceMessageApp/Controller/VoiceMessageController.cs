using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalRVoiceMessageApp.Controller;

[Route("api/[controller]")]
[ApiController]
public class VoiceMessageController : ControllerBase
{
    private readonly IHubContext<VoiceMessageHub> _hubContext;

    public VoiceMessageController(IHubContext<VoiceMessageHub> hubContext)
    {
        _hubContext = hubContext;
    }
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(IFormFile audioData)
    {
        if (audioData == null || audioData.Length == 0)
        {
            return BadRequest("No audio file uploaded.");
        }

        // Read the audio file content into a byte array
        using (var memoryStream = new MemoryStream())
        {
            await audioData.CopyToAsync(memoryStream);
            byte[] audioBytes = memoryStream.ToArray();

            // Send the audio data to all connected clients
            await _hubContext.Clients.All.SendAsync("ReceiveVoiceMessage", audioBytes);
        }

        return Ok(new { message = "Audio message sent to clients." });
    }

}