namespace SignalRVoiceMessageApp;

using Microsoft.AspNetCore.SignalR;

public class VoiceMessageHub : Hub
{
    // This method will send the voice message (byte array) to all connected clients
    public async Task SendVoiceMessage(byte[] audioData)
    {
        // Broadcast the audio message to all clients connected to the hub
        await Clients.All.SendAsync("ReceiveVoiceMessage", audioData);
    }
}
