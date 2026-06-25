using CybersecurityAwarenessBot.Models;
using MySqlConnector;

namespace CybersecurityAwarenessBot.Services;

public class ActivityLogService
{
    private readonly DatabaseService _databaseService;
    private readonly List<ActivityLogEntry> _localLog = new();


    public ActivityLogService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }


    public async Task AddAsync(string actionText)
    {
        ActivityLogEntry entry = new()
        {
            ActionText = actionText,
            CreatedAt = DateTime.Now
        };

        _localLog.Insert(0, entry);

        if (!_databaseService.IsReady)
        {
            return;
        }

        try
        {
            await using MySqlConnection connection = _databaseService.CreateConnection();
            await connection.OpenAsync();

            await using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO activity_log (action_text, created_at) VALUES (@action_text, @created_at);";
            command.Parameters.AddWithValue("@action_text", actionText);
            command.Parameters.AddWithValue("@created_at", entry.CreatedAt);
            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            // If MySQL is off, the app can still show what happened in this session.
        }
    }


    public async Task<List<ActivityLogEntry>> GetRecentAsync(int count)
    {
        if (!_databaseService.IsReady)
        {
            return _localLog.Take(count).ToList();
        }

        try
        {
            List<ActivityLogEntry> entries = new();

            await using MySqlConnection connection = _databaseService.CreateConnection();
            await connection.OpenAsync();

            await using MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT id, action_text, created_at FROM activity_log ORDER BY created_at DESC LIMIT @count;";
            command.Parameters.AddWithValue("@count", count);

            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                entries.Add(new ActivityLogEntry
                {
                    Id = reader.GetInt32("id"),
                    ActionText = reader.GetString("action_text"),
                    CreatedAt = reader.GetDateTime("created_at")
                });
            }

            return entries;
        }
        catch
        {
            return _localLog.Take(count).ToList();
        }
    }
}
