using Domain.Models;

namespace Domain.Services
{
    public interface ILogService
    {
        void StopLogger();

        Task WriteLogsAsync(List<LogLine> logs, string path, bool midnightChange = default);

        void EnableLoggerState();
    }
}
