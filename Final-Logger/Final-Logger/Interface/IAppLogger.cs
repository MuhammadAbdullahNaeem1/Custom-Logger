using System.Threading.Tasks;

public interface IAppLogger
{
    Task LogAsync(LogLevel level, string message, string innerException = null, string applicationName = "DefaultAppName");
}