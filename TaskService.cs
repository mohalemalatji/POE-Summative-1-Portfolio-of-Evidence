using CybersecurityAwarenessBot.Models;
using MySqlConnector;

namespace CybersecurityAwarenessBot.Services;

public class TaskService
{
    private readonly DatabaseService _databaseService;
    private readonly ActivityLogService _activityLogService;
    private readonly List<CyberTask> _localTasks = new();
    private int _localTaskId = 1;


    public TaskService(DatabaseService databaseService, ActivityLogService activityLogService)
    {
        _databaseService = databaseService;
        _activityLogService = activityLogService;
    }


    public async Task<(bool Success, string Message)> AddTaskAsync(CyberTask task)
    {
        if (!await _databaseService.TryConnectAsync())
        {
            task.Id = _localTaskId++;
            _localTasks.Add(task);
            await _activityLogService.AddAsync($"Task added in session: {task.Title}");
            return (true, $"Task added for this session: {task.Title}. Start local MySQL/XAMPP to save it permanently.");
        }

        await using MySqlConnection connection = _databaseService.CreateConnection();
        await connection.OpenAsync();

        await using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO cyber_tasks
                (title, description, has_reminder, reminder_text, reminder_date, is_completed, created_at)
            VALUES
                (@title, @description, @has_reminder, @reminder_text, @reminder_date, @is_completed, @created_at);
            """;

        command.Parameters.AddWithValue("@title", task.Title);
        command.Parameters.AddWithValue("@description", task.Description);
        command.Parameters.AddWithValue("@has_reminder", task.HasReminder);
        command.Parameters.AddWithValue("@reminder_text", (object?)task.ReminderText ?? DBNull.Value);
        command.Parameters.AddWithValue("@reminder_date", (object?)task.ReminderDate ?? DBNull.Value);
        command.Parameters.AddWithValue("@is_completed", task.IsCompleted);
        command.Parameters.AddWithValue("@created_at", task.CreatedAt);
        await command.ExecuteNonQueryAsync();

        await _activityLogService.AddAsync($"Task added: {task.Title}");
        return (true, $"Task added and saved in MySQL: {task.Title}");
    }


    public async Task<List<CyberTask>> GetTasksAsync()
    {
        if (!await _databaseService.TryConnectAsync())
        {
            return _localTasks.ToList();
        }

        List<CyberTask> tasks = new();

        await using MySqlConnection connection = _databaseService.CreateConnection();
        await connection.OpenAsync();

        await using MySqlCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, title, description, has_reminder, reminder_text, reminder_date, is_completed, created_at
            FROM cyber_tasks
            ORDER BY is_completed ASC, created_at DESC;
            """;

        await using MySqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(ReadTask(reader));
        }

        return tasks;
    }


    public async Task<(bool Success, string Message)> CompleteTaskAsync(int taskId)
    {
        if (!await _databaseService.TryConnectAsync())
        {
            CyberTask? localTask = _localTasks.FirstOrDefault(task => task.Id == taskId);
            if (localTask is null)
            {
                return (false, "I could not find that task.");
            }

            localTask.IsCompleted = true;
            await _activityLogService.AddAsync($"Task completed in session: {localTask.Title}");
            return (true, $"Marked task as completed: {localTask.Title}");
        }

        await using MySqlConnection connection = _databaseService.CreateConnection();
        await connection.OpenAsync();

        await using MySqlCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE cyber_tasks SET is_completed = 1 WHERE id = @id;";
        command.Parameters.AddWithValue("@id", taskId);
        int affectedRows = await command.ExecuteNonQueryAsync();

        if (affectedRows == 0)
        {
            return (false, "I could not find that task.");
        }

        await _activityLogService.AddAsync($"Task completed: #{taskId}");
        return (true, $"Marked task #{taskId} as completed.");
    }


    public async Task<(bool Success, string Message)> DeleteTaskAsync(int taskId)
    {
        if (!await _databaseService.TryConnectAsync())
        {
            CyberTask? localTask = _localTasks.FirstOrDefault(task => task.Id == taskId);
            if (localTask is null)
            {
                return (false, "I could not find that task.");
            }

            _localTasks.Remove(localTask);
            await _activityLogService.AddAsync($"Task deleted in session: {localTask.Title}");
            return (true, $"Deleted task: {localTask.Title}");
        }

        await using MySqlConnection connection = _databaseService.CreateConnection();
        await connection.OpenAsync();

        await using MySqlCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM cyber_tasks WHERE id = @id;";
        command.Parameters.AddWithValue("@id", taskId);
        int affectedRows = await command.ExecuteNonQueryAsync();

        if (affectedRows == 0)
        {
            return (false, "I could not find that task.");
        }

        await _activityLogService.AddAsync($"Task deleted: #{taskId}");
        return (true, $"Deleted task #{taskId}.");
    }


    private static CyberTask ReadTask(MySqlDataReader reader)
    {
        return new CyberTask
        {
            Id = reader.GetInt32("id"),
            Title = reader.GetString("title"),
            Description = reader.GetString("description"),
            HasReminder = reader.GetBoolean("has_reminder"),
            ReminderText = reader.IsDBNull(reader.GetOrdinal("reminder_text")) ? null : reader.GetString("reminder_text"),
            ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date")) ? null : reader.GetDateTime("reminder_date"),
            IsCompleted = reader.GetBoolean("is_completed"),
            CreatedAt = reader.GetDateTime("created_at")
        };
    }
}
