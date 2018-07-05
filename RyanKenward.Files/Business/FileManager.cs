using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using RyanKenward.Files.Interfaces;

namespace RyanKenward.Files.Business
{
    public class FileManager : IFileManager
    {
        private IFileSystem _fileSystem { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RyanKenward.Files.Business.FileManager"/> class.
        /// </summary>
        public FileManager()
            : this(new FileSystem())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RyanKenward.Files.Business.FileManager"/> class.
        /// </summary>
        /// <param name="fileSystem">File system.</param>
        public FileManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Gets all the files in the given directory. Does not return files in subdirectories.
        /// </summary>
        /// <param name="directoryPath">Path of the directory.</param>
        /// <returns>Files in the directory.</returns>
        public IEnumerable<string> GetFiles(string directoryPath)
        {
            if (!_fileSystem.Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException("Directory path does not exist.");

            // gets all files in the immediate directory
            return _fileSystem.Directory.EnumerateFiles(directoryPath);
        }
    }
}
