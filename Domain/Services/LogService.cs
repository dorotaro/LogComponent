using Persistence.Repositories;
using Domain.Models;
using System.Text;

namespace Domain.Services
{
    public class LogService : ILogService
    {
        private readonly IFileRepository  _fileRepository;
        private List<LogLine> _notCompletedLogs = new();
        private readonly string _fileNameOnMidnight = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "{0}" + "{1}" + ".log";
        private readonly string _timeStamp = "Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine;
        private bool _stopRunning;
        public LogService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task WriteLogsAsync(List<LogLine> logs, string path, bool midnightChange = default)
        {

            _notCompletedLogs = new List<LogLine>(logs);

            try
            {
                await _fileRepository.WriteLineAsync(_timeStamp, path);

                foreach (var log in logs)
                    {
                        if (!_stopRunning && _notCompletedLogs.Count > 0)
                        {
                            if (DateTime.Now.Day != log.Timestamp.Day && !midnightChange)
                            {
                            
                            var filename = string.Format(_fileNameOnMidnight, log.Timestamp.ToString("yyyyMMdd HHmmss fff"), log.Text);
                            
                            await WriteNotCompletedLogs_Midnight(filename);

                            }
                            else
                            {
                                var stringBuilderLog = GenerateStringBuilderFromLog(log);

                                await _fileRepository.WriteLineAsync(stringBuilderLog.ToString(), path).ConfigureAwait(true);

                                _notCompletedLogs.Remove(log);
                            }
                        }
                    }
            }
            catch (Exception)
            {
                await WriteNotCompletedLogsAsync();
                
                Console.WriteLine("An error had occurred. All previously unsaved logs have now been written to files");
            }
        }

        public void StopLogger()
        {
            _stopRunning = true;
        }

        public void EnableLoggerState()
        {
            _stopRunning = false;
        }

        private static StringBuilder GenerateStringBuilderFromLog(LogLine log)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff")+ "\t"+ log.Text+ "\t"+ Environment.NewLine);

            return stringBuilder;
        }

        private static string GenerateDirectoryPath(string specialString)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + specialString + ".log";
        }

        private async Task WriteNotCompletedLogsAsync()
        {
            await WriteLogsAsync(_notCompletedLogs, GenerateDirectoryPath(Guid.NewGuid().ToString()));
        }

        private async Task WriteNotCompletedLogs_Midnight(string fileName)
        {
            await WriteLogsAsync(_notCompletedLogs, fileName, true);
        }
    }
}
