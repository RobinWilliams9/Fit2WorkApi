using System.IO;
using Fit2Work.Tests.Common;
using Fit2WorkImportUserService.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fit2WorkImportUserService.Tests.Providers {
    [TestClass]
    public class FileProviderTests {
        readonly string TestFile = $"Files\\{TestHelper.TestFileName}";
        readonly string ImportFile = $"Watch\\{TestHelper.TestFileName}";
        readonly string ArchiveFolder = "Watch\\Archive";

        IFileProvider _provider = new FileProvider();

        [TestCleanup]
        public void Cleanup() {
            TestHelper.ClearTestFilesAndFolders();
        }

        [TestMethod]
        public void Archive_CreatedArchiveFolder() {
            // Arrange
            string testFile = TestHelper.GetFullPath(TestFile);
            string importFile = TestHelper.GetFullPath(ImportFile);
            string archiveFolder = TestHelper.GetFullPath(ArchiveFolder);

            File.WriteAllText(importFile, Properties.Resources.file);

            // Act
            _provider.Archive(importFile, archiveFolder);
            // Assert
            Assert.IsTrue(Directory.Exists(archiveFolder), "Expected archive folder to be created");
        }

        [TestMethod]
        public void Archive_CopiestoArchiveFolder() {
            // Arrange
            string testFile = TestHelper.GetFullPath(TestFile);
            string importFile = TestHelper.GetFullPath(ImportFile);
            string archiveFolder = TestHelper.GetFullPath(ArchiveFolder);

            File.WriteAllText(importFile, Properties.Resources.file);

            // Act
            _provider.Archive(importFile, archiveFolder);
            // Assert
            Assert.IsTrue(File.Exists(importFile),
                "Expected import file to remain");
            Assert.IsTrue(Directory.GetFiles(archiveFolder, "*.*").Length == 1,
                "Expected import file to be copied to archive");
        }

        [TestMethod]
        public void Archive_RetainsFileExtension() {
            // Arrange
            string testFile = TestHelper.GetFullPath(TestFile);
            string importFile = TestHelper.GetFullPath(ImportFile);
            string archiveFolder = TestHelper.GetFullPath(ArchiveFolder);

            File.WriteAllText(importFile, Properties.Resources.file);

            // Act
            _provider.Archive(importFile, archiveFolder);
            var archiveFile = Directory.GetFiles(archiveFolder, "*.*")[0];
            // Assert            
            Assert.AreEqual(
                Path.GetExtension(importFile),
                Path.GetExtension(archiveFile),
                "Expected files to have same extension");
        }

        [TestMethod]
        public void Archive_WhenFileDoesntExist_NoArchiveFolderIsCreated() {
            // Arrange
            string importFile = TestHelper.GetFullPath(ImportFile);
            string archiveFolder = TestHelper.GetFullPath(ArchiveFolder);
            // Act
            _provider.Archive(importFile, archiveFolder);
            // Assert
            Assert.IsFalse(Directory.Exists(archiveFolder), "Expected NO archive folder");
        }

        [TestMethod]
        public void Archive_WhenFileDoesntExist_NothingIsArchive() {
            // Arrange
            string importFile = TestHelper.GetFullPath(ImportFile);
            string archiveFolder = TestHelper.GetFullPath(ArchiveFolder);
            Directory.CreateDirectory(archiveFolder); // archive folder already exists
            // Act
            _provider.Archive(importFile, archiveFolder);
            // Assert
            Assert.IsTrue(Directory.Exists(archiveFolder), "Expected archive folder");
            Assert.IsTrue(Directory.GetFiles(archiveFolder, "*.*").Length == 0,
                "Expected NO files to be copied to archive");
        }

        [TestMethod]
        public void Provider_Can_Delete_File() {
            // Arrange
            string testFile = TestHelper.GetFullPath(TestFile);
            string importFile = TestHelper.GetFullPath(ImportFile);

            File.WriteAllText(importFile, Properties.Resources.file);

            // Act
            _provider.DeleteFile(importFile);

            // Assert
            Assert.IsTrue(!File.Exists(importFile));
        }
    }
}
