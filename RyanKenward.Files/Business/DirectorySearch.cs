using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using RyanKenward.Files.Interfaces;

namespace RyanKenward.Files.Business
{
    public class DirectorySearch : IDirectorySearch
    {
        private const string _outputFolderName = "Output";

        private IFileSearch _fileSearch { get; }
        private IFileManager _fileManager { get; }
        private string _searchDirectory { get; }
        private IFileSystem _fileSystem { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RyanKenward.Files.Business.DirectorySearch"/> class.
        /// </summary>
        /// <param name="fileSearch">File search.</param>
        /// <param name="fileManager">File manager.</param>
        /// <param name="searchDirectory">Search directory.</param>
        public DirectorySearch(
            IFileSearch fileSearch,
            IFileManager fileManager,
            string searchDirectory)
            : this(fileSearch, fileManager, searchDirectory, new FileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RyanKenward.Files.Business.DirectorySearch"/> class.
        /// </summary>
        /// <param name="fileSearch">File search.</param>
        /// <param name="fileManager">File manager.</param>
        /// <param name="searchDirectory">Search directory.</param>
        /// <param name="fileSystem">File system.</param>
        public DirectorySearch(
            IFileSearch fileSearch,
            IFileManager fileManager,
            string searchDirectory,
            IFileSystem fileSystem)
        {
            _fileSearch = fileSearch;
            _fileManager = fileManager;
            _searchDirectory = searchDirectory;
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Searches files in parallel for lines containing a search query.
        /// </summary>
        public void SearchDirectory()
        {
            if (string.IsNullOrEmpty(_searchDirectory))
                throw new ArgumentException("Directory path cannot be null or empty.");
            
            var files = _fileManager.GetFiles(_searchDirectory);

            // search each file in parallel
            Parallel.ForEach<string>(files, filePath => _fileSearch.SearchFile(filePath));
        }

        /// <summary>
        /// Writes the lines found in the search to an output file. Overwrites any existing file.
        /// </summary>
        /// <param name="destinationFileName">Destination file name.</param>
        public void BuildSearchResultsOutputFile(string destinationFileName)
        {
            if (string.IsNullOrEmpty(destinationFileName))
                throw new ArgumentException("Destination file name cannot be null or empty.");
            
            if (destinationFileName.IndexOfAny(_fileSystem.Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("Destination file contains one or more invalid file name characters.");

            var outputDirectory = _fileSystem.Path.Combine(_searchDirectory, _outputFolderName);

            if (!_fileSystem.Directory.Exists(outputDirectory))
                _fileSystem.Directory.CreateDirectory(outputDirectory);  // create directory if it does not exist

            var outputFile = _fileSystem.Path.Combine(outputDirectory, destinationFileName);

            if (_fileSystem.File.Exists(outputFile))
                _fileSystem.File.Delete(outputFile);    // delete existing file if it exists

            // write each line to the output file
            _fileSystem.File.WriteAllLines(outputFile, _fileSearch.AllLinesFound.SelectMany(l => l));
        }

        /// <summary>
        /// Writes a search result summary to the console.
        /// </summary>
        public void WriteSearchResultsToConsole()
        {
            Console.WriteLine($"Search complete for query \"{_fileSearch.SearchString}\".");
            Console.WriteLine($"Files processed: {_fileSearch.NumberOfFilesSearched}.");
            Console.WriteLine($"Number of lines containing query: {_fileSearch.AllLinesFound.Sum(c => c.Count())}.");
            Console.WriteLine($"Number of occurrences found: {_fileSearch.AllInstancesFound.Sum(c => c)}.");
            Console.WriteLine("(Press any key to exit)");
            Console.ReadKey();
        }
    }
}
