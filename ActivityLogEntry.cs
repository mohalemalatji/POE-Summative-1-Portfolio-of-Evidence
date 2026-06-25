namespace CybersecurityAwarenessBot.Models;

public class ActivityLogEntry
{
    public int Id { get; set; }
    public string ActionText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;


    public override string ToString()
    {
        return $"{CreatedAt:g} - {ActionText}";
    }
}
