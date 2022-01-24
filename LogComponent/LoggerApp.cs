using Domain.Models;
using Domain.Services;

namespace LogComponent
{
    public class LoggerApp
    {
        private readonly ILogService _logService;
        public LoggerApp(ILogService logService)
        {
            _logService = logService;
        }

        public async Task WriteLogsAsync(List<LogLine> logLines, string path)
        {
            await _logService.WriteLogsAsync(logLines, path);
        }

        public void StopLogger()
        {
            _logService.StopLogger();
        }

        public void EnableLoggerState()
        {
            _logService.EnableLoggerState();
        }
    }
}
