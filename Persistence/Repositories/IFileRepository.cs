namespace Persistence.Repositories
{
    public interface IFileRepository
    {
        Task WriteLineAsync(string line, string path);
    }
}
