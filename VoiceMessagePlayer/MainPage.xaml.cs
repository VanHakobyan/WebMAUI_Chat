using Plugin.Maui.Audio;
using VoiceMessagePlayer.Models;
using VoiceMessagePlayer.Services;

namespace VoiceMessagePlayer;

public partial class MainPage : ContentPage
{
    private readonly Plugin.Maui.Audio.IAudioManager _audioManager;
    private readonly SignalRService _signalRService;
    private List<Message> _messages = new List<Message>();
    private AsyncAudioPlayer _currentAudioPlayer;

    public MainPage(Plugin.Maui.Audio.IAudioManager audioManager)
    {
        _audioManager = audioManager;
        _signalRService = new SignalRService();

        InitializeComponent();
        InitializeSignalRAsync();
    }
    private async void InitializeSignalRAsync()
    {
        try
        {
            // Initialize SignalR
            _signalRService.Initialize();
            _signalRService.OnVoiceMessageReceived += OnVoiceMessageReceived;

            // Start SignalR connection asynchronously
            await _signalRService.StartAsync();
        }
        catch (Exception ex)
        {
            // Handle any errors during initialization
            Console.WriteLine($"Error during SignalR initialization: {ex.Message}");
        }
    }
    // Handle received voice messages from SignalR
    private void OnVoiceMessageReceived(byte[] audioData)
    {
        // Create a new message with the received audio data
        var newMessage = new Message
        {
            DisplayName = $"Message {DateTime.Now:HH:mm:ss}",
            AudioData = audioData
        };

        // Add the message to the list
        _messages.Add(newMessage);

        // Rebind the updated list to the UI on the main thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MessagesListView.ItemsSource = null; // Reset to update binding
            MessagesListView.ItemsSource = _messages; // Rebind the updated list
        });
    }


    // When a message is selected from the list
    private void OnMessageSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // You can handle selection if needed
    }

    //Play the selected message
    //private async void OnPlayButtonClicked(object sender, EventArgs e)
    //{
    //    var button = (Button)sender;
    //    var selectedMessage = (Message)button.BindingContext;

    //    // Use the AudioPlayer to play the selected message's audio
    //    using var stream = new MemoryStream(selectedMessage.AudioData);
    //    var audioPlayer = _audioManager.CreateAsyncPlayer(stream);
    //    await audioPlayer.PlayAsync(CancellationToken.None);
    //}

    private async void OnPlayButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var selectedMessage = (Message)button.BindingContext;

        // If there's already an audio playing, stop it
        if (_currentAudioPlayer != null)
        {
            _currentAudioPlayer.Stop();
            _currentAudioPlayer = null;  // Reset the player
        }

        // Use the AudioPlayer to play the selected message's audio
        using var stream = new MemoryStream(selectedMessage.AudioData);
        _currentAudioPlayer = _audioManager.CreateAsyncPlayer(stream);  // Create the async audio player

        // Play the audio asynchronously
        await _currentAudioPlayer.PlayAsync(CancellationToken.None);
    }

    private async void OnStopButtonClicked(object sender, EventArgs e)
    {
        // If there's an audio playing, stop it
        if (_currentAudioPlayer != null)
        {
            _currentAudioPlayer.Stop();  // Stop the audio playback
            _currentAudioPlayer = null;  // Reset the player
        }
    }



}
