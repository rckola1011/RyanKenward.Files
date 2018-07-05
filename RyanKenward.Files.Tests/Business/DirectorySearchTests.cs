using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Moq;
using NUnit.Framework;
using RyanKenward.Files.Business;
using RyanKenward.Files.Interfaces;

namespace RyanKenward.Files.Tests.Business
{
    [TestFixture]
    public class DirectorySearchTests
    {
        private DirectorySearch _directorySearch;
        private Mock<IFileSearch> _mockFileSearch;
        private Mock<IFileManager> _mockFileManager;
        private Mock<IFileSystem> _mockFileSystem;
        private string _someDirectory = "some directory";

        [SetUp]
        public void Setup()
        {
            _mockFileSearch = new Mock<IFileSearch>();
            _mockFileManager = new Mock<IFileManager>();
            _mockFileSystem = new Mock<IFileSystem>();

            _directorySearch = new DirectorySearch(_mockFileSearch.Object, _mockFileManager.Object, _someDirectory, _mockFileSystem.Object);
        }

        [Test]
        public void SearchDirectory_NullPath_ThrowsArgumentException()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new DirectorySearch(_mockFileSearch.Object, _mockFileManager.Object, null).SearchDirectory());
        }

        [Test]
        public void SearchDirectory_EmptyPath_ThrowsArgumentException()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new DirectorySearch(_mockFileSearch.Object, _mockFileManager.Object, string.Empty).SearchDirectory());
        }

        [Test]
        public void SearchDirectory_ValidDirectory_CallsGetFiles()
        {
            // Arrange
            // Act
            _directorySearch.SearchDirectory();

            // Assert
            _mockFileManager.Verify(f => f.GetFiles(_someDirectory), Times.Once);
        }

        [Test]
        public void SearchDirectory_DirectoryReturnsFile_CallsSearchFileOnce()
        {
            // Arrange
            var files = new List<string>
            {
                "file 1"
            };
            _mockFileManager.Setup(f => f.GetFiles(It.IsAny<string>())).Returns(files);

            // Act
            _directorySearch.SearchDirectory();

            // Assert
            _mockFileSearch.Verify(s => s.SearchFile(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void SearchDirectory_DirectoryReturnsTwoFiles_CallsSearchFileTwice()
        {
            // Arrange
            var files = new List<string>
            {
                "file 1",
                "file 2"
            };
            _mockFileManager.Setup(f => f.GetFiles(It.IsAny<string>())).Returns(files);

            // Act
            _directorySearch.SearchDirectory();

            // Assert
            _mockFileSearch.Verify(s => s.SearchFile(It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public void BuildSearchResultsOutputFile_NullDestinationFileName_ThrowsArgumentException()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => _directorySearch.BuildSearchResultsOutputFile(null));
        }

        [Test]
        public void BuildSearchResultsOutputFile_EmptyDestinationFileName_ThrowsArgumentException()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => _directorySearch.BuildSearchResultsOutputFile(string.Empty));
        }

        [Test]
        public void BuildSearchResultsOutputFile_InvalidCharsDestinationFileName_ThrowsArgumentException()
        {
            // Arrange
            var fileName = "some file *";
            _mockFileSystem.Setup(f => f.Path.GetInvalidFileNameChars()).Returns(new char[] { '*' });

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => _directorySearch.BuildSearchResultsOutputFile(fileName));
        }

        [Test]
        public void BuildSearchResultsOutputFile_DirectoryDoesNotExist_CreateDirectory()
        {
            // Arrange
            _mockFileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(false);
            _mockFileSystem.Setup(f => f.Path.GetInvalidFileNameChars()).Returns(new char[] { '*' });
            _mockFileSystem.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(false);
            _mockFileSearch.Setup(s => s.AllLinesFound).Returns(new List<List<string>>());

            // Act
            _directorySearch.BuildSearchResultsOutputFile("some file");

            // Assert
            _mockFileSystem.Verify(f => f.Directory.CreateDirectory(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void BuildSearchResultsOutputFile_FileExists_DeleteFile()
        {
            // Arrange
            _mockFileSystem.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(f => f.Path.GetInvalidFileNameChars()).Returns(new char[] { '*' });
            _mockFileSearch.Setup(s => s.AllLinesFound).Returns(new List<List<string>>());

            // Act
            _directorySearch.BuildSearchResultsOutputFile("some file");

            // Assert
            _mockFileSystem.Verify(f => f.File.Delete(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void BuildSearchResultsOutputFile_SearchFoundZeroLines_WriteFileWithZeroLines()
        {
            // Arrange
            _mockFileSystem.Setup(f => f.Path.GetInvalidFileNameChars()).Returns(new char[] { '*' });
            _mockFileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(false);
            _mockFileSearch.Setup(s => s.AllLinesFound).Returns(new List<List<string>>());

            // Act
            _directorySearch.BuildSearchResultsOutputFile("some file");

            // Assert
            _mockFileSystem.Verify(f => f.File.WriteAllLines(It.IsAny<string>(), new List<string>()), Times.Once);
        }

        [Test]
        public void BuildSearchResultsOutputFile_SearchFoundLines_WriteFileWithLines()
        {
            // Arrange
            var linesFound = new List<string>
            {
                "I found a line!",
                "I found another line!"
            };
            var listOfLinesFound = new List<List<string>>
            {
                linesFound
            };
            _mockFileSystem.Setup(f => f.Path.GetInvalidFileNameChars()).Returns(new char[] { '*' });
            _mockFileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(false);
            _mockFileSearch.Setup(s => s.AllLinesFound).Returns(listOfLinesFound);

            // Act
            _directorySearch.BuildSearchResultsOutputFile("some file");

            // Assert
            _mockFileSystem.Verify(f => f.File.WriteAllLines(It.IsAny<string>(), linesFound), Times.Once);
        }

        [Test]
        public void WriteSearchResultsToConsole_SearchFoundNoLines_VerifyConsoleOutput()
        {
            // Arrange
            using (var stringWriter = new StringWriter())
            {
                var originalOutput = Console.Out;
                Console.SetOut(stringWriter);

                var expectedText = "Search complete for query \"some search\".\nFiles processed: 0.\nNumber of lines containing query: 0.\nNumber of occurrences found: 0.\n(Press any key to exit)\n";

                _mockFileSearch.Setup(s => s.SearchString).Returns("some search");
                _mockFileSearch.Setup(s => s.NumberOfFilesSearched).Returns(0);
                _mockFileSearch.Setup(s => s.AllLinesFound).Returns(new List<List<string>>());
                _mockFileSearch.Setup(s => s.AllInstancesFound).Returns(new List<int>());

                // Act
                _directorySearch.WriteSearchResultsToConsole();

                // Assert
                Assert.AreEqual(expectedText, stringWriter.ToString());

                Console.SetOut(originalOutput);
            }
        }

        [Test]
        public void WriteSearchResultsToConsole_SearchFoundLines_VerifyConsoleOutput()
        {
            // Arrange
            using (var stringWriter = new StringWriter())
            {
                var originalOutput = Console.Out;
                Console.SetOut(stringWriter);

                var expectedText = "Search complete for query \"some search\".\nFiles processed: 1.\nNumber of lines containing query: 2.\nNumber of occurrences found: 2.\n(Press any key to exit)\n";

                var allLinesFound = new List<List<string>>
                {
                    new List<string>
                    {
                        "some line",
                        "some other line"
                    }
                };

                var allInstancesFound = new List<int>
                {
                    1,
                    1
                };

                _mockFileSearch.Setup(s => s.SearchString).Returns("some search");
                _mockFileSearch.Setup(s => s.NumberOfFilesSearched).Returns(1);
                _mockFileSearch.Setup(s => s.AllLinesFound).Returns(allLinesFound);
                _mockFileSearch.Setup(s => s.AllInstancesFound).Returns(allInstancesFound);

                // Act
                _directorySearch.WriteSearchResultsToConsole();

                // Assert
                Assert.AreEqual(expectedText, stringWriter.ToString());

                Console.SetOut(originalOutput);
            }
        }
    }
}
