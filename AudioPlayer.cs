using System.Media;

namespace CybersecurityAwarenessBot.Services;

public class AudioPlayer
{
    private readonly string _audioPath = Path.Combine(AppContext.BaseDirectory, "Assets", "voice-greeting.wav");

    public bool TryPlayGreeting()
    {
        return TryPlayGreeting(out _);
    }


    public bool TryPlayGreeting(out string message)
    {
        try
        {
            if (!File.Exists(_audioPath))
            {
                message = "The voice greeting file was not found.";
                return false;
            }

            using SoundPlayer player = new(_audioPath);
            player.Load();
            player.PlaySync();
            message = "Voice greeting played.";
            return true;
        }
        catch
        {
            message = "The voice greeting could not be played.";
            return false;
        }
    }
}
