namespace Persistence.Repositories
{
    public class FileRepository : IFileRepository
    {
        public async Task WriteLineAsync(string line, string path)
        {
            StreamWriter? _streamWriter = File.AppendText(path);

            using (_streamWriter) await _streamWriter.WriteAsync(line).ConfigureAwait(true);

            Thread.Sleep(10);
        }
    }
}
