using System;
using System.Collections.Generic;
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
        private string _someDirectory = "some directory";

        [SetUp]
        public void Setup()
        {
            _mockFileSearch = new Mock<IFileSearch>();
            _mockFileManager = new Mock<IFileManager>();

            _directorySearch = new DirectorySearch(_mockFileSearch.Object, _mockFileManager.Object, _someDirectory);
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

        // TODO: BuildSearchResultsOutputFile_NullDestinationFileName_ThrowsArgumentException

        // TODO: BuildSearchResultsOutputFile_EmptyDestinationFileName_ThrowsArgumentException

        // TODO: BuildSearchResultsOutputFile_InvalidCharsDestinationFileName_ThrowsArgumentException

        // TODO: BuildSearchResultsOutputFile_DirectoryDoesNotExist_CreateDirectory

        // TODO: BuildSearchResultsOutputFile_FileExists_DeleteFile

        // TODO: BuildSearchResultsOutputFile_SearchFoundZeroLines_WriteFileWithZeroLines

        // TODO: BuildSearchResultsOutputFile_SearchFoundLines_WriteFileWithLines

        // TODO: WriteSearchResultsToConsole_SearchFoundNoLines_VerifyConsoleOutput

        // TODO: WriteSearchResultsToConsole_SearchFoundLines_VerifyConsoleOutput
    }
}
