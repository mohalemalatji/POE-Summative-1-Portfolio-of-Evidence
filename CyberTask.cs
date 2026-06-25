namespace CybersecurityAwarenessBot.Models;

public class CyberTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool HasReminder { get; set; }
    public string? ReminderText { get; set; }
    public DateTime? ReminderDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;


    public override string ToString()
    {
        string status = IsCompleted ? "Done" : "Open";
        string reminder = HasReminder && ReminderDate.HasValue
            ? $" | Reminder: {ReminderDate.Value:g}"
            : string.Empty;

        return $"#{Id} [{status}] {Title}{reminder}";
    }
}
