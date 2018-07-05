using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using NUnit.Framework;
using RyanKenward.Files.Business;

namespace RyanKenward.Files.Tests.Business
{
    [TestFixture]
    public class FileManagerTests
    {
        private FileManager _fileManager;
        private Mock<IFileSystem> _mockFileSystem;

        [SetUp]
        public void Setup()
        {
            _mockFileSystem = new Mock<IFileSystem>();

            _fileManager = new FileManager(_mockFileSystem.Object);
        }

        [Test]
        public void GetFiles_InvalidDirectory_ThrowsDirectoryNotFoundException()
        {
            // Arrange
            var fileDirectory = "some/path";
            _mockFileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(false);

            // Act
            // Assert
            Assert.Throws<DirectoryNotFoundException>(() => _fileManager.GetFiles(fileDirectory));
        }

        [Test]
        public void GetFiles_ValidDirectory_ReturnsFile()
        {
            // Arrange
            var files = new List<string>
            {
                "file 1"
            };

            var fileDirectory = "some/path";
            _mockFileSystem.Setup(f => f.Directory.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(f => f.Directory.EnumerateFiles(It.IsAny<string>())).Returns(files);

            // Act
            var result = _fileManager.GetFiles(fileDirectory);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(1, result.ToList().Count);
            Assert.AreEqual("file 1", result.ToList<string>()[0]);
        }
    }
}
