namespace VoiceMessagePlayer.Services;

using Microsoft.AspNetCore.SignalR.Client;

public class SignalRService
{
    private readonly HubConnection _hubConnection;

    public SignalRService()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7013/hub", options =>
            {
                options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            })
            .Build();
       
        _hubConnection.Closed += async (exception) =>
        {
            Console.WriteLine($"Connection closed: {exception?.Message}");
            await Task.Delay(1000);  // Reconnect attempt
            await StartAsync();      // Optionally restart the connection
        };
    }

    public async Task StartAsync()
    {
        try
        {
            await _hubConnection.StartAsync();
            Console.WriteLine("Connected to SignalR hub");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to SignalR: {ex.Message}");
        }
    }

    public async Task StopAsync()
    {
        await _hubConnection.StopAsync();
    }

    // Receive voice message
    public event Action<byte[]> OnVoiceMessageReceived;

    public void Initialize()
    {
        _hubConnection.On<byte[]>("ReceiveVoiceMessage", (messageData) =>
        {
            OnVoiceMessageReceived?.Invoke(messageData);
        });
    }
}
