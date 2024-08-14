
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public static class LogSetup
{
    public static async Task SetupLogFoldersAsync(string baseLogPath)
    {
        string[] logFolders = { "Error", "Information", "Warning" };
        foreach (var folder in logFolders)
        {
            string folderPath = Path.Combine(baseLogPath, folder);
            Directory.CreateDirectory(folderPath);

            string logFilePath = Path.Combine(folderPath, $"{folder.ToLower()}_log.txt");
            if (!File.Exists(logFilePath))
            {
                await File.Create(logFilePath).DisposeAsync();
            }
        }
    }

    public static async Task LogMessagesAsync(string connectionString, string baseLogPath)
    {
        var errorLogger = new Logger(connectionString, Path.Combine(baseLogPath, "Error", "error_log.txt"));
        var infoLogger = new Logger(connectionString, Path.Combine(baseLogPath, "Information", "info_log.txt"));
        var warningLogger = new Logger(connectionString, Path.Combine(baseLogPath, "Warning", "warning_log.txt"));

        await infoLogger.LogAsync(LogLevel.Information, "User attempted login");
        await warningLogger.LogAsync(LogLevel.Warning, "Multiple failed login attempts detected");
        await errorLogger.LogAsync(LogLevel.Error, "Database connection failed during login process", "SQL Exception: Connection timeout");
    }

    public static async Task VerifySqlLogsAsync(string connectionString)
    {
        Console.WriteLine("Checking SQL logs:");
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(@"SELECT TOP 3 LogType, InnerException, Message, CreatedOn 
                                           FROM LoginLogs 
                                           ORDER BY CreatedOn DESC", connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"Level: {reader["LogType"]}, " +
                                      $"Message: {reader["Message"]}, " +
                                      $"InnerException: {reader["InnerException"] ?? "N/A"}, " +
                                      $"Time: {reader["CreatedOn"]}");
                }
            }
        }
        Console.WriteLine();
    }

    public static async Task VerifyFileLogsAsync(string baseLogPath)
    {
        string[] logFolders = { "Error", "Information", "Warning" };
        foreach (var folder in logFolders)
        {
            string logFilePath = Path.Combine(baseLogPath, folder, $"{folder.ToLower()}_log.txt");
            Console.WriteLine($"Checking file logs: {logFilePath}");
            if (File.Exists(logFilePath))
            {
                var lastThreeLines = (await File.ReadAllLinesAsync(logFilePath)).Reverse().Take(3).Reverse();
                foreach (var line in lastThreeLines)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                Console.WriteLine("Log file not found.");
            }
            Console.WriteLine();
        }
    }
}