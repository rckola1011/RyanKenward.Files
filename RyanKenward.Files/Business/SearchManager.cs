namespace RyanKenward.Files.Business
{
    public class SearchManager
    {
        /// <summary>
        /// Search the specified searchDirectory, searchPhrase and outputFileName.
        /// </summary>
        /// <param name="searchDirectory">Search directory.</param>
        /// <param name="searchPhrase">Search phrase.</param>
        /// <param name="outputFileName">Output file name.</param>
        public void Search(string searchDirectory, string searchPhrase, string outputFileName)
        {
            var fileSearch = new FileSearch(searchPhrase);
            var fileManager = new FileManager();

            var dirSearch = new DirectorySearch(fileSearch, fileManager, searchDirectory);

            dirSearch.SearchDirectory();
            dirSearch.BuildSearchResultsOutputFile(outputFileName);
            dirSearch.WriteSearchResultsToConsole();
        }
    }
}
