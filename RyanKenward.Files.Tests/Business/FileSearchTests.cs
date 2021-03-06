﻿using System;
using System.IO;
using System.IO.Abstractions;
using Moq;
using NUnit.Framework;
using RyanKenward.Files.Business;

namespace RyanKenward.Files.Tests.Business
{
    [TestFixture]
    public class FileSearchTests
    {
        private FileSearch _fileSearch;
        private Mock<IFileSystem> _mockFileSystem;

        [SetUp]
        public void Setup()
        {
            _mockFileSystem = new Mock<IFileSystem>();
        }

        [Test]
        public void FileSearch_NullSearchString_ThrowsArgumentException()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new FileSearch(null));
        }

        [Test]
        public void FileSearch_EmptySearchString_ThrowsArgumentException()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => new FileSearch(string.Empty));
        }

        [Test]
        public void SearchFile_NullFilePath_ThrowsArgumentException()
        {
            // Arrange
            _fileSearch = new FileSearch("some text");

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => _fileSearch.SearchFile(null));
        }

        [Test]
        public void SearchFile_EmptyFilePath_ThrowsArgumentException()
        {
            // Arrange
            _fileSearch = new FileSearch("some text");

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => _fileSearch.SearchFile(string.Empty));
        }

        [Test]
        public void SearchFile_InvalidFile_ReturnsNoSearchResults()
        {
            // Arrange
            _mockFileSystem.Setup(f => f.File.ReadAllLines(It.IsAny<string>())).Throws<FileNotFoundException>();
            _fileSearch = new FileSearch("some text", _mockFileSystem.Object);

            // Act
            _fileSearch.SearchFile("some file");

            // Assert
            Assert.AreEqual(0, _fileSearch.NumberOfFilesSearched);
            Assert.AreEqual(0, _fileSearch.AllLinesFound.Count);
            Assert.AreEqual(0, _fileSearch.AllInstancesFound.Count);
        }

        [Test]
        public void SearchFile_ValidFileNoResults_ReturnsNoLinesOrInstancesFound()
        {
            // Arrange
            var searchString = "some text";
            string[] lines = { "a line of text" };
            _mockFileSystem.Setup(f => f.File.ReadAllLines(It.IsAny<string>())).Returns(lines);
            _fileSearch = new FileSearch(searchString, _mockFileSystem.Object);

            // Act
            _fileSearch.SearchFile("some file");

            // Assert
            Assert.AreEqual(1, _fileSearch.NumberOfFilesSearched);
            Assert.AreEqual(0, _fileSearch.AllLinesFound.Count);
            Assert.AreEqual(0, _fileSearch.AllInstancesFound.Count);
        }

        [Test]
        public void SearchFile_ValidFileWithMatch_ReturnsLineAndInstanceFound()
        {
            // Arrange
            var lineText = "a line of some text";
            var searchString = "some text";
            string[] lines = { lineText };
            _mockFileSystem.Setup(f => f.File.ReadAllLines(It.IsAny<string>())).Returns(lines);
            _fileSearch = new FileSearch(searchString, _mockFileSystem.Object);

            // Act
            _fileSearch.SearchFile("some file");

            // Assert
            Assert.AreEqual(1, _fileSearch.NumberOfFilesSearched);
            Assert.AreEqual(1, _fileSearch.AllLinesFound.Count);
            Assert.AreEqual(lineText, _fileSearch.AllLinesFound[0][0]);
            Assert.AreEqual(1, _fileSearch.AllInstancesFound.Count);
            Assert.AreEqual(1, _fileSearch.AllInstancesFound[0]);
        }

        [Test]
        public void SearchFile_ValidFileWithTwoMatches_ReturnsLinesAndInstancesFound()
        {
            // Arrange
            var lineText1 = "a line of some text";
            var lineText3 = "some text in a line";
            var searchString = "some text";
            string[] lines = { lineText1, "another line", lineText3 };
            _mockFileSystem.Setup(f => f.File.ReadAllLines(It.IsAny<string>())).Returns(lines);
            _fileSearch = new FileSearch(searchString, _mockFileSystem.Object);

            // Act
            _fileSearch.SearchFile("some file");

            // Assert
            Assert.AreEqual(1, _fileSearch.NumberOfFilesSearched);
            Assert.AreEqual(1, _fileSearch.AllLinesFound.Count);
            Assert.AreEqual(2, _fileSearch.AllLinesFound[0].Count);
            Assert.AreEqual(lineText1, _fileSearch.AllLinesFound[0][0]);
            Assert.AreEqual(lineText3, _fileSearch.AllLinesFound[0][1]);
            Assert.AreEqual(1, _fileSearch.AllInstancesFound.Count);
            Assert.AreEqual(2, _fileSearch.AllInstancesFound[0]);
        }

        [Test]
        public void SearchFile_ValidFileWithTwoMatchesInSameLine_ReturnsLineAndInstancesFound()
        {
            // Arrange
            var lineText = "a line of some text that contains some text";
            var searchString = "some text";
            string[] lines = { lineText };
            _mockFileSystem.Setup(f => f.File.ReadAllLines(It.IsAny<string>())).Returns(lines);
            _fileSearch = new FileSearch(searchString, _mockFileSystem.Object);

            // Act
            _fileSearch.SearchFile("some file");

            // Assert
            Assert.AreEqual(1, _fileSearch.NumberOfFilesSearched);
            Assert.AreEqual(1, _fileSearch.AllLinesFound.Count);
            Assert.AreEqual(lineText, _fileSearch.AllLinesFound[0][0]);
            Assert.AreEqual(1, _fileSearch.AllInstancesFound.Count);
            Assert.AreEqual(2, _fileSearch.AllInstancesFound[0]);
        }

        [Test]
        public void SearchFile_ValidFileWithMatchIgnoreCase_ReturnsLineAndInstanceFound()
        {
            // Arrange
            var lineText = "a line of SoMe TeXt";
            var searchString = "some text";
            string[] lines = { lineText };
            _mockFileSystem.Setup(f => f.File.ReadAllLines(It.IsAny<string>())).Returns(lines);
            _fileSearch = new FileSearch(searchString, _mockFileSystem.Object);

            // Act
            _fileSearch.SearchFile("some file");

            // Assert
            Assert.AreEqual(1, _fileSearch.NumberOfFilesSearched);
            Assert.AreEqual(1, _fileSearch.AllLinesFound.Count);
            Assert.AreEqual(lineText, _fileSearch.AllLinesFound[0][0]);
            Assert.AreEqual(1, _fileSearch.AllInstancesFound.Count);
            Assert.AreEqual(1, _fileSearch.AllInstancesFound[0]);
        }
    }
}
