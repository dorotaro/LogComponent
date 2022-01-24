using Domain.Models;
using LogComponent.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogComponent
{
    public class Program
    {
        private const string prompt1 =
            @"Press S to start writing logs: 
            ==================
             [Esc] Quit
            ";

        private const string prompt2 =
            @"Press key to choose an option:
             1 Stop with Flush
             2 Stop without Flush
            ==================
             [Esc] Quit
            ";

        
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationExtensions();

            var loggerApp = configuration.ConfigureServices().GetService<LoggerApp>();
            
            var isRunning = true;
            
            do
            {
                Console.WriteLine(prompt1);
                var consoleKey = Console.ReadKey(true).Key;
                switch (consoleKey)
                {
                    case ConsoleKey.Escape: 
                        isRunning = false;
                    break;
                    case ConsoleKey.S:
                        var directoryAdress1 = GenerateDirectoryPath(Guid.NewGuid().ToString());
                        var thread1 = new Thread(async () => await loggerApp.WriteLogsAsync(GenerateLogLinesAscending(), directoryAdress1));
                        thread1.Start();

                        var directoryAdress2 = GenerateDirectoryPath(Guid.NewGuid().ToString());
                        var thread2 = new Thread(async () => await loggerApp.WriteLogsAsync(GenerateLogLinesDescending(), directoryAdress2));
                        thread2.Start();

                        loggerApp.EnableLoggerState();
                        await Task.Delay(1000);
                        SwitchBetweenFlushModes(loggerApp, thread1, thread2);
                    break;
                    default: return;
                }
            } while (isRunning);
        }

        private static List<LogLine> GenerateLogLinesAscending()
        {
            var lines = new List<LogLine>();
            
            for (int i = 0; i < 15; i++)
            {
				var line = new LogLine(i.ToString())
				{
					Timestamp = DateTime.Now
				};
				lines.Add(line);
            }
            return lines;
        }

        private static List<LogLine> GenerateLogLinesDescending()
        {
            var lines = new List<LogLine>();

            for (int i = 50; i > 0; i--)
            {
                var line = new LogLine(i.ToString())
                {
                    Timestamp = DateTime.Now
                };
                lines.Add(line);
            }
            return lines;
        }

        private static string GenerateDirectoryPath(string specialString)
		{
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + specialString + ".log";
        }

        private static void SwitchBetweenFlushModes(LoggerApp loggerApp, Thread thread1, Thread thread2)
		{
            
            var endAfterOneRun = false;
            do
            {
                Console.WriteLine(prompt2);
                var consoleKey2 = Console.ReadKey(true).Key;

                switch (consoleKey2)
                {
                    case ConsoleKey.Escape: return;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.WriteLine("Number 1 was pressed. Starting the action...");
                        loggerApp.StopLogger();
                        Console.WriteLine("Any unwritten logs have been flushed");
                        return;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        Console.WriteLine("Number 2 was pressed. Starting the action...");
                        thread1.Join();
                        thread2.Join();
                        loggerApp.StopLogger();
                        Console.WriteLine("All logs have been completely written into output files");
                        return;
                }
            } while (endAfterOneRun);
            return;
        }
    }
}


