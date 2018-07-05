using System.Collections.Generic;

namespace RyanKenward.Files.Interfaces
{
    public interface IFileSearch
    {
        string SearchString { get; }
        List<List<string>> AllLinesFound { get; }
        List<int> AllInstancesFound { get; }
        int NumberOfFilesSearched { get; }

        void SearchFile(string filePath);
    }
}
