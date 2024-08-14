
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

public class Logger : IAppLogger
{
    private readonly string _connectionString;
    private readonly string _logFilePath;

    public Logger(string connectionString, string logFilePath)
    {
        _connectionString = connectionString;
        _logFilePath = logFilePath;
    }

    public async Task LogAsync(LogLevel level, string message, string innerException = null, string applicationName = "DefaultAppName")
    {
        await LogToSqlAsync(level, message, innerException, applicationName);
        await LogToFileAsync(level, message, innerException);
    }

    private async Task LogToSqlAsync(LogLevel level, string message, string innerException, string applicationName)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    @"INSERT INTO LoginLogs (LogType, ApplicationName, InnerException, Message, CreatedOn, CreatedBy, Is_Deleted) 
                      VALUES (@LogType, @ApplicationName, @InnerException, @Message, @CreatedOn, @CreatedBy, @Is_Deleted)",
                    connection);

                command.Parameters.AddWithValue("@LogType", level.ToString());
                command.Parameters.AddWithValue("@ApplicationName", applicationName);
                command.Parameters.AddWithValue("@InnerException", (object)innerException ?? DBNull.Value);
                command.Parameters.AddWithValue("@Message", message);
                command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                command.Parameters.AddWithValue("@CreatedBy", 1);
                command.Parameters.AddWithValue("@Is_Deleted", "N");

                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log to SQL: {ex.Message}");
        }
    }

    private async Task LogToFileAsync(LogLevel level, string message, string innerException)
    {
        try
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            if (!string.IsNullOrEmpty(innerException))
            {
                logEntry += $" | InnerException: {innerException}";
            }
            await File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log to file: {ex.Message}");
        }
    }
}