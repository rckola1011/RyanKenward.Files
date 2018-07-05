using System.Collections.Generic;

namespace RyanKenward.Files.Interfaces
{
    public interface IFileManager
    {
        IEnumerable<string> GetFiles(string directoryPath);
    }
}
