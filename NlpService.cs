using CybersecurityAwarenessBot.Models;
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot.Services;

public class NlpService
{
    public NlpCommand ReadCommand(string input)
    {
        string normalized = input.Trim().ToLowerInvariant();

        if (normalized.Contains("show activity log") || normalized.Contains("what have you done"))
        {
            return new NlpCommand { Type = NlpCommandType.ShowActivityLog };
        }

        if (normalized.Contains("start quiz") || normalized.Contains("take quiz") || normalized.Contains("quiz game"))
        {
            return new NlpCommand { Type = NlpCommandType.StartQuiz };
        }

        if (normalized.Contains("show tasks") || normalized.Contains("view tasks") || normalized.Contains("list tasks"))
        {
            return new NlpCommand { Type = NlpCommandType.ViewTasks };
        }

        if (normalized.Contains("complete task") || normalized.Contains("mark task") || normalized.Contains("finish task"))
        {
            return new NlpCommand
            {
                Type = NlpCommandType.CompleteTask,
                TaskId = FindFirstNumber(normalized)
            };
        }

        if (normalized.Contains("delete task") || normalized.Contains("remove task"))
        {
            return new NlpCommand
            {
                Type = NlpCommandType.DeleteTask,
                TaskId = FindFirstNumber(normalized)
            };
        }

        if (LooksLikeTaskRequest(normalized))
        {
            return CreateTaskCommand(input, normalized);
        }

        return new NlpCommand { Type = NlpCommandType.None };
    }


    private static bool LooksLikeTaskRequest(string normalized)
    {
        return normalized.Contains("remind me to") ||
               normalized.Contains("add a task") ||
               normalized.Contains("add task") ||
               normalized.Contains("create a task") ||
               normalized.Contains("create task");
    }


    private static NlpCommand CreateTaskCommand(string originalInput, string normalized)
    {
        string title = originalInput.Trim();

        foreach (string phrase in new[] { "remind me to", "add a task to", "add task to", "add a task", "add task", "create a task to", "create task to", "create a task", "create task" })
        {
            title = RemovePhrase(title, phrase);
        }

        title = RemoveReminderWords(title).Trim(' ', '.', ',');

        (bool hasReminder, string? reminderText, DateTime? reminderDate) = FindReminder(normalized);

        if (string.IsNullOrWhiteSpace(title))
        {
            title = "Cybersecurity task";
        }

        return new NlpCommand
        {
            Type = NlpCommandType.AddTask,
            TaskTitle = title,
            TaskDescription = $"Task created from chat: {originalInput.Trim()}",
            HasReminder = hasReminder,
            ReminderText = reminderText,
            ReminderDate = reminderDate
        };
    }


    private static string RemovePhrase(string text, string phrase)
    {
        return Regex.Replace(text, Regex.Escape(phrase), string.Empty, RegexOptions.IgnoreCase);
    }


    private static string RemoveReminderWords(string text)
    {
        foreach (string phrase in new[] { "tomorrow", "today", "next week", "with a reminder", "set a reminder", "reminder" })
        {
            text = RemovePhrase(text, phrase);
        }

        return text;
    }


    private static (bool HasReminder, string? ReminderText, DateTime? ReminderDate) FindReminder(string normalized)
    {
        if (normalized.Contains("tomorrow"))
        {
            return (true, "Tomorrow", DateTime.Now.Date.AddDays(1).AddHours(9));
        }

        if (normalized.Contains("today"))
        {
            return (true, "Today", DateTime.Now.AddHours(2));
        }

        if (normalized.Contains("next week"))
        {
            return (true, "Next week", DateTime.Now.Date.AddDays(7).AddHours(9));
        }

        if (normalized.Contains("remind") || normalized.Contains("reminder"))
        {
            return (true, "Reminder requested", DateTime.Now.Date.AddDays(1).AddHours(9));
        }

        return (false, null, null);
    }


    private static int? FindFirstNumber(string normalized)
    {
        Match match = Regex.Match(normalized, @"\d+");

        if (match.Success && int.TryParse(match.Value, out int number))
        {
            return number;
        }

        return null;
    }
}
