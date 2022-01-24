using Domain.Models;
using Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services
{
	public class LogServiceShould : IClassFixture<Program>
	{
		[Fact]
		public async Task Write_Logs_To_File()
		{
			//arrange
			var program = new Program();

			//system under test
			var _sut = program.ServiceProvider.GetRequiredService<ILogService>(); 

			var lines = GenerateLogLines(2);
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +"\\"+ DateTime.Now.ToString("yyyyMMdd HHmmss fff") + Guid.NewGuid().ToString() + ".log";
			
			//act
			await _sut.WriteLogsAsync(lines, path);

			//assert
			File.Exists(path).Should().BeTrue();
			File.ReadLines(path).Count().Should().Be(3);
		}

		[Fact]
		public async Task Create_New_File_At_Midnight()
		{
			//arrange
			var program = new Program();
			var _sut = program.ServiceProvider.GetRequiredService<ILogService>(); 
			var lines = GenerateLogLinesForMidnight();
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + Guid.NewGuid().ToString() + ".log";
			var newFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + lines[3].Timestamp.ToString("yyyyMMdd HHmmss fff") + lines[3].Text + ".log";
			
			//act
			await _sut.WriteLogsAsync(lines, path);

			//assert
			File.Exists(path).Should().BeTrue();
			File.ReadLines(path).Count().Should().Be(4);
			File.Exists(newFilePath).Should().BeTrue();
			File.ReadLines(path).Count().Should().Be(4);
		}

		[Fact]
		public void Stop_Writing_With_Flush()
		{
			//arrange
			var program = new Program();
			var _sut = program.ServiceProvider.GetRequiredService<ILogService>(); 
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + Guid.NewGuid().ToString() + ".log";

			//act
			var thread = new Thread(async () => await _sut.WriteLogsAsync(GenerateLogLines(40), path));
			thread.Start();
			Task.Delay(1000).Wait();
			_sut.StopLogger();
			thread.Join();

			//assert
			File.Exists(path).Should().BeTrue();
			File.ReadLines(path).Count().Should().BeGreaterThan(0);
			File.ReadLines(path).Count().Should().BeLessThan(42); // 1 line for 'timestamp & data' + 40loglines
		}

		[Fact]
		public void Stop_Writing_Without_Flush()
		{
			//arrange
			var program = new Program();
			var _sut = program.ServiceProvider.GetRequiredService<ILogService>(); 
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + Guid.NewGuid().ToString() + ".log";

			//act
			var thread = new Thread(async () => await _sut.WriteLogsAsync(GenerateLogLines(15), path));
			thread.Start();
			thread.Join();
			_sut.StopLogger();

			//assert
			File.Exists(path).Should().BeTrue();
			File.ReadLines(path).Count().Should().Be(16);
		}

		private static List<LogLine> GenerateLogLines(int items)
		{
			var lines = new List<LogLine>();

			for (int i = 1; i <= items; i++)
			{
				var line = new LogLine(i.ToString())
				{
					Timestamp = DateTime.Now
				};
				lines.Add(line);
			}
			return lines;
		}

		private static List<LogLine> GenerateLogLinesForMidnight()
		{
			var lines = new List<LogLine>();

			for (int i = 0; i < 6; i++)
			{
				var line = new LogLine(Guid.NewGuid().ToString());

				if (i > 2)
				{
					var today = DateTime.Now;
					var tomorrow = today.AddDays(1);
					line.Timestamp = tomorrow;
				}
				else
				{
					line.Timestamp = DateTime.Now;
				}
				lines.Add(line);
			}
			return lines;
		}
	}
}
