namespace RyanKenward.Files.Interfaces
{
    public interface IDirectorySearch
    {
        void SearchDirectory();
        void BuildSearchResultsOutputFile(string destinationFileName);
        void WriteSearchResultsToConsole();
    }
}
