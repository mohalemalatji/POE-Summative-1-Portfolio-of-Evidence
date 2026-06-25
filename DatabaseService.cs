using MySqlConnector;
using System.Diagnostics;
using System.Net.Sockets;

namespace CybersecurityAwarenessBot.Services;

public class DatabaseService
{
    private const string DefaultConnectionString = "Server=localhost;Port=3306;Database=cyber_awareness_bot;User ID=root;Password=;";

    private static readonly string[] LocalServiceNames =
    {
        "MySQL80",
        "MySQL",
        "MySQL57",
        "MariaDB",
        "MariaDB10"
    };

    private static readonly string[] XamppStartFiles =
    {
        @"C:\xampp\mysql_start.bat"
    };

    private readonly string _baseConnectionString;
    private string _databaseConnectionString = DefaultConnectionString;

    public bool IsReady { get; private set; }
    public string StatusMessage { get; private set; } = "MySQL has not been checked yet.";


    public DatabaseService()
    {
        _baseConnectionString = Environment.GetEnvironmentVariable("CYBERBOT_MYSQL") ?? DefaultConnectionString;
        _databaseConnectionString = _baseConnectionString;
    }


    public async Task InitializeAsync()
    {
        try
        {
            MySqlConnectionStringBuilder builder = new(_baseConnectionString);
            int port = (int)builder.Port;
            string databaseName = string.IsNullOrWhiteSpace(builder.Database)
                ? "cyber_awareness_bot"
                : builder.Database;

            if (IsLocalServer(builder.Server) && !await CanReachLocalMySqlAsync(port))
            {
                await TryStartLocalMySqlAsync(port);
            }

            builder.Database = string.Empty;

            await using MySqlConnection serverConnection = new(builder.ConnectionString);
            await serverConnection.OpenAsync();

            await using MySqlCommand createDatabaseCommand = serverConnection.CreateCommand();
            createDatabaseCommand.CommandText = $"CREATE DATABASE IF NOT EXISTS `{databaseName.Replace("`", "``")}`;";
            await createDatabaseCommand.ExecuteNonQueryAsync();

            builder.Database = databaseName;
            _databaseConnectionString = builder.ConnectionString;

            await using MySqlConnection databaseConnection = new(_databaseConnectionString);
            await databaseConnection.OpenAsync();
            await CreateTablesAsync(databaseConnection);
            await CheckTablesAsync(databaseConnection);

            IsReady = true;
            StatusMessage = "Offline local MySQL connected. Database and tables are ready.";
        }
        catch
        {
            IsReady = false;
            StatusMessage = "Offline local MySQL is not running yet. Start MySQL in XAMPP or MySQL Server, then click Reconnect SQL.";
        }
    }


    public MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_databaseConnectionString);
    }


    public async Task<bool> TryConnectAsync()
    {
        if (IsReady && await CanUseDatabaseAsync())
        {
            return true;
        }

        await InitializeAsync();
        return IsReady;
    }


    public void MarkOffline(string details)
    {
        IsReady = false;
        StatusMessage = "Offline local MySQL connection was lost. Restart MySQL, then click Reconnect SQL.";
    }


    private static async Task CreateTablesAsync(MySqlConnection connection)
    {
        string createTasksTable = """
            CREATE TABLE IF NOT EXISTS cyber_tasks (
                id INT AUTO_INCREMENT PRIMARY KEY,
                title VARCHAR(150) NOT NULL,
                description TEXT NOT NULL,
                has_reminder BOOLEAN NOT NULL DEFAULT 0,
                reminder_text VARCHAR(100) NULL,
                reminder_date DATETIME NULL,
                is_completed BOOLEAN NOT NULL DEFAULT 0,
                created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
            """;

        string createActivityTable = """
            CREATE TABLE IF NOT EXISTS activity_log (
                id INT AUTO_INCREMENT PRIMARY KEY,
                action_text TEXT NOT NULL,
                created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
            """;

        await using MySqlCommand taskCommand = new(createTasksTable, connection);
        await taskCommand.ExecuteNonQueryAsync();

        await using MySqlCommand activityCommand = new(createActivityTable, connection);
        await activityCommand.ExecuteNonQueryAsync();
    }


    private async Task<bool> CanUseDatabaseAsync()
    {
        try
        {
            await using MySqlConnection connection = CreateConnection();
            await connection.OpenAsync();
            await CheckTablesAsync(connection);
            return true;
        }
        catch (Exception exception)
        {
            MarkOffline(exception.Message);
            return false;
        }
    }


    private static async Task CheckTablesAsync(MySqlConnection connection)
    {
        await using MySqlCommand taskCommand = new("SELECT COUNT(*) FROM cyber_tasks;", connection);
        await taskCommand.ExecuteScalarAsync();

        await using MySqlCommand activityCommand = new("SELECT COUNT(*) FROM activity_log;", connection);
        await activityCommand.ExecuteScalarAsync();
    }


    private static bool IsLocalServer(string server)
    {
        return server.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
               server.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase);
    }


    private static async Task<bool> CanReachLocalMySqlAsync(int port)
    {
        try
        {
            using CancellationTokenSource timeout = new(TimeSpan.FromMilliseconds(800));
            using TcpClient client = new();
            await client.ConnectAsync("127.0.0.1", port, timeout.Token);
            return true;
        }
        catch
        {
            return false;
        }
    }


    private static async Task TryStartLocalMySqlAsync(int port)
    {
        foreach (string serviceName in LocalServiceNames)
        {
            await TryRunCommandAsync("sc.exe", $"start {serviceName}");

            if (await WaitForLocalMySqlAsync(port))
            {
                return;
            }
        }

        foreach (string startFile in XamppStartFiles)
        {
            if (!File.Exists(startFile))
            {
                continue;
            }

            await TryRunCommandAsync("cmd.exe", $"/c \"\"{startFile}\"\"");

            if (await WaitForLocalMySqlAsync(port))
            {
                return;
            }
        }
    }


    private static async Task<bool> WaitForLocalMySqlAsync(int port)
    {
        for (int attempt = 0; attempt < 8; attempt++)
        {
            if (await CanReachLocalMySqlAsync(port))
            {
                return true;
            }

            await Task.Delay(700);
        }

        return false;
    }


    private static async Task TryRunCommandAsync(string fileName, string arguments)
    {
        try
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using Process? process = Process.Start(startInfo);

            if (process is not null)
            {
                await Task.WhenAny(process.WaitForExitAsync(), Task.Delay(3000));
            }
        }
        catch
        {
            // If the service is not installed, the app will explain that MySQL is not connected.
        }
    }
}
