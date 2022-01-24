using Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Repositories
{
	public class FileRepositoryShould : IClassFixture<Program>
	{
		private readonly IFileRepository _sut;
		public FileRepositoryShould(Program program)
		{
			_sut = program.ServiceProvider.GetRequiredService<IFileRepository>();
		}

		[Fact]
		public async Task Write_Logs_To_File()
		{
			//arrange
			var line = new LogLine(Guid.NewGuid().ToString())
			{
				Timestamp = DateTime.Now
			};

			var lineToWrite = GenerateStringBuilderFromLog(line);
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + Guid.NewGuid().ToString() + ".log";

			//act
			await _sut.WriteLineAsync(lineToWrite.ToString(), path);

			//assert
			File.Exists(path).Should().BeTrue();
			File.ReadLines(path).Count().Should().Be(1);
		}

		private static StringBuilder GenerateStringBuilderFromLog(LogLine log)
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append(log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff") + "\t" + log.Text + "\t" + Environment.NewLine);

			return stringBuilder;
		}
	}
}
