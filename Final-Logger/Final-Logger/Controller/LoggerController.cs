
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class LoggerController : ControllerBase
{
    private readonly string _connectionString;
    private readonly string _baseLogPath;

    public LoggerController(IConfiguration configuration)
    {
        _connectionString = configuration["ConnectionStrings:SqlServerDb"] ?? "";
        _baseLogPath = configuration["ConnectionStrings:BaseLogPath"] ?? "";
    }

    [HttpPost("setup")]
    public async Task<IActionResult> SetupLogs()
    {
        await LogSetup.SetupLogFoldersAsync(_baseLogPath);
        return Ok("Log folders and files set up successfully.");
    }

    [HttpPost("log")]
    public async Task<IActionResult> LogMessages()
    {
        await LogSetup.LogMessagesAsync(_connectionString, _baseLogPath);
        return Ok("Log messages written successfully.");
    }

    [HttpGet("verify/sql")]
    public async Task<IActionResult> VerifySqlLogs()
    {
        await LogSetup.VerifySqlLogsAsync(_connectionString);
        return Ok("SQL logs verified. Check console for details.");
    }

    [HttpGet("verify/file")]
    public async Task<IActionResult> VerifyFileLogs()
    {
        await LogSetup.VerifyFileLogsAsync(_baseLogPath);
        return Ok("File logs verified. Check console for details.");
    }
}
