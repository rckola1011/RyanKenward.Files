using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using RyanKenward.Files.Interfaces;

namespace RyanKenward.Files.Business
{
    public class FileSearch : IFileSearch
    {
        public string SearchString { get; }

        public List<List<string>> AllLinesFound
        {
            get 
            {
                return _allLinesFound;
            }
        }

        public List<int> AllInstancesFound
        {
            get
            {
                return _allInstancesFound;
            }
        }

        public int NumberOfFilesSearched
        {
            get
            {
                return _numberOfFilesSearched;
            }
        }

        private IFileSystem _fileSystem { get; }
        private List<List<string>> _allLinesFound { get; set; } = new List<List<string>>();
        private List<int> _allInstancesFound { get; set; } = new List<int>();
        private int _numberOfFilesSearched { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RyanKenward.Files.Business.FileSearch"/> class.
        /// </summary>
        /// <param name="searchString">Search string.</param>
        public FileSearch(string searchString)
            : this(searchString, new FileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RyanKenward.Files.Business.FileSearch"/> class.
        /// </summary>
        /// <param name="searchString">Search string.</param>
        /// <param name="fileSystem">File system.</param>
        public FileSearch(string searchString, IFileSystem fileSystem)
        {
            if (string.IsNullOrEmpty(searchString))
                throw new ArgumentException("Search string cannot be null or empty.");
            
            SearchString = searchString;
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Searches a given file for lines containing the search string. Case insensitive.
        /// </summary>
        /// <param name="filePath">Full path of the file to be searched.</param>
        public void SearchFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.");
            
            var linesFound = new List<string>();
            var instancesFound = 0;

            try
            {
                // search file line by line
                foreach (var line in _fileSystem.File.ReadAllLines(filePath))
                {
                    // check if line contains search string
                    if (line.IndexOf(SearchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        linesFound.Add(line);
                        instancesFound += Regex.Matches(line, SearchString, RegexOptions.IgnoreCase).Count;
                    }
                }

                _numberOfFilesSearched++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching file {filePath}: {ex.Message}");
            }

            // add search results to collective search results
            if (linesFound.Any())
                _allLinesFound.Add(linesFound);
            if (instancesFound > 0)
                _allInstancesFound.Add(instancesFound);
        }
    }
}
