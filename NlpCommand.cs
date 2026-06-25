namespace CybersecurityAwarenessBot.Models;

public class NlpCommand
{
    public NlpCommandType Type { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public string TaskDescription { get; set; } = string.Empty;
    public bool HasReminder { get; set; }
    public string? ReminderText { get; set; }
    public DateTime? ReminderDate { get; set; }
    public int? TaskId { get; set; }
}
