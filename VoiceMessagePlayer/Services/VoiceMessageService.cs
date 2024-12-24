namespace VoiceMessagePlayer.Services;

using Microsoft.AspNetCore.SignalR.Client;

public class VoiceMessageService
{
    private HubConnection _hubConnection;

    public event Action<byte[]>? OnVoiceMessageReceived;

    public VoiceMessageService(string serverUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{serverUrl}/voicehub")
            .Build();

        _hubConnection.On<byte[]>("ReceiveVoiceMessage", data =>
        {
            OnVoiceMessageReceived?.Invoke(data);
        });
    }

    public async Task StartAsync() => await _hubConnection.StartAsync();
    public async Task StopAsync() => await _hubConnection.StopAsync();
}
