using System;
using System.IO;
using System.Reflection;
using AnvilGroup.Library;
using AnvilGroup.Library.LoggingManager;
using AnvilGroup.Services.Fit2Work.Services;
using Fit2Work.Tests.Common;
using Fit2WorkImportUserService.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fit2WorkImportUserService.Tests {
    [TestClass]
    public class StartArgTests {
        StartArg _startArg;

        Mock<IUserService> _mockUserService = new Mock<IUserService>();
        Mock<ILogger> _mockLogger = new Mock<ILogger>();
        Mock<IHeartBeat> _mockHeartbeat = new Mock<IHeartBeat>();
        Mock<IFileProvider> _mockFile = new Mock<IFileProvider>();
        Mock<IConfigurationProvider> _mockConfig = new Mock<IConfigurationProvider>();

        delegate void SendHeartbeat(ref BeatArg arg);

        [TestInitialize]
        public void Initialise() {
            // Required by any tests which use new BeatArg()
            SetEntryAssembly(Assembly.GetExecutingAssembly());
            // Ensure heartbeat returns valid by default
            _mockHeartbeat.Setup(m => m.Beat(ref It.Ref<BeatArg>.IsAny))
                .Callback(new SendHeartbeat((ref BeatArg arg) => arg.Status = BeatResult.Beat));

            _mockConfig.Setup(m => m.WatchPath).Returns(TestHelper.GetFullPath("Watch"));
            _mockConfig.Setup(m => m.ArchiveWatchPath).Returns(TestHelper.GetFullPath("Watch\\Archive"));
            _mockConfig.Setup(m => m.InternalBufferSize).Returns(8192);
            _mockConfig.Setup(m => m.WaitSeconds).Returns(1);

            _startArg = new StartArg(
                _mockUserService.Object,
                _mockFile.Object,
                _mockConfig.Object,
                _mockLogger.Object,
                _mockHeartbeat.Object);
        }

        [TestCleanup]
        public void Cleanup() {
            TestHelper.ClearTestFilesAndFolders();
        }

        [TestMethod]
        public void ProcessFile_FileDeleted_IsIgnored() {
            // Act
            _startArg.ProcessFile(this,
                new FileSystemEventArgs(WatcherChangeTypes.Deleted, "\\Watch", "deleted-file.csv"));
            // Assert
            _mockHeartbeat.Verify(m =>
                m.Beat(ref It.Ref<BeatArg>.IsAny), Times.Never,
                "Expected NO hearbeat");
            _mockLogger.Verify(m =>
                m.Info(It.IsAny<object>(), It.IsAny<string>()), Times.Never,
                "Expected NO messages logged");
            _mockUserService.Verify(m =>
                m.ImportUsers(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never,
                "Expected NO users imported");
        }

        [TestMethod]
        public void ProcessFile_FileRenamed_IsIgnored() {
            // Act
            _startArg.ProcessFile(this,
                new FileSystemEventArgs(WatcherChangeTypes.Renamed, "\\Watch", "deleted-file.csv"));
            // Assert
            _mockHeartbeat.Verify(m =>
                m.Beat(ref It.Ref<BeatArg>.IsAny), Times.Never,
                "Expected NO hearbeat");
            _mockLogger.Verify(m =>
                m.Info(It.IsAny<object>(), It.IsAny<string>()), Times.Never,
                "Expected NO messages logged");
            _mockUserService.Verify(m =>
                m.ImportUsers(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never,
                "Expected NO users imported");
        }

        [TestMethod]
        public void ProcessFile_FileCreated_IsProcessed() {
            // Arrange
            _startArg.StartWatcher();
            // Act
            _startArg.ProcessFile(this,
                new FileSystemEventArgs(WatcherChangeTypes.Created, "\\Watch", "created-file.csv"));
            // Assert
            _mockHeartbeat.Verify(m =>
                m.Beat(ref It.Ref<BeatArg>.IsAny), Times.Once, "Expected a hearbeat");

            _mockLogger.Verify(m =>
                m.Info(It.IsAny<object>(), It.IsAny<string>()), Times.AtLeastOnce,
                "Expected messages logged");

            _mockFile.Verify(m =>
                m.Open("\\Watch\\created-file.csv", FileMode.Open, FileAccess.Read, FileShare.Read), Times.Once,
                "Expected file to be checked");
            _mockFile.Verify(m =>
                m.Archive("\\Watch\\created-file.csv", It.IsAny<string>()), Times.Once,
                "Expected file to be archived");
            _mockFile.Verify(m =>
                m.DeleteFile(It.IsAny<string>()), Times.Never, "File was deleted after processing");

            _mockUserService.Verify(m =>
                m.ImportUsers("\\Watch\\created-file.csv", It.IsAny<Action<string>>()), Times.Once,
                "Expected users imported");
        }

        [TestMethod]
        public void ProcessFile_FileCreatedInSubFolder_IsProcessed() {
            // Arrange
            _startArg.StartWatcher();
            // Act
            _startArg.ProcessFile(this,
                new FileSystemEventArgs(WatcherChangeTypes.Created, "\\Watch\\Client1", "created-file.csv"));
            // Assert
            _mockHeartbeat.Verify(m =>
                m.Beat(ref It.Ref<BeatArg>.IsAny), Times.Once, "Expected a hearbeat");

            _mockLogger.Verify(m =>
                m.Info(It.IsAny<object>(), It.IsAny<string>()), Times.AtLeastOnce,
                "Expected messages logged");

            _mockFile.Verify(m =>
                m.Open("\\Watch\\Client1\\created-file.csv", FileMode.Open, FileAccess.Read, FileShare.Read), Times.Once,
                "Expected file to be checked");
            _mockFile.Verify(m =>
                m.Archive("\\Watch\\Client1\\created-file.csv", It.IsAny<string>()), Times.Once,
                "Expected file to be archived");
            _mockFile.Verify(m =>
                m.DeleteFile(It.IsAny<string>()), Times.Never, "File was deleted after processing");

            _mockUserService.Verify(m =>
                m.ImportUsers("\\Watch\\Client1\\created-file.csv", It.IsAny<Action<string>>()), Times.Once,
                "Expected users imported");
        }

        [TestMethod]
        public void ProcessFile_FileChanged_IsProcessed() {
            // Arrange
            _startArg.StartWatcher();
            // Act
            _startArg.ProcessFile(this,
                new FileSystemEventArgs(WatcherChangeTypes.Changed, "\\Watch", "created-file.csv"));
            // Assert
            _mockHeartbeat.Verify(m =>
                m.Beat(ref It.Ref<BeatArg>.IsAny), Times.Once, "Expected a hearbeat");

            _mockLogger.Verify(m =>
                m.Info(It.IsAny<object>(), It.IsAny<string>()), Times.AtLeastOnce,
                "Expected messages logged");

            _mockFile.Verify(m =>
                m.Open(It.IsAny<string>(), FileMode.Open, FileAccess.Read, FileShare.Read), Times.Once,
                "Expected file to be checked");
            _mockFile.Verify(m =>
                m.Archive(It.IsAny<string>(), It.IsAny<string>()), Times.Once,
                "Expected file to be archived");
            _mockFile.Verify(m =>
                m.DeleteFile(It.IsAny<string>()), Times.Never, "File was deleted after processing");

            _mockUserService.Verify(m =>
               m.ImportUsers(It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Once,
               "Expected users imported");

        }

        [TestMethod]
        public void ProcessFile_ErrorOccurs_IsLoggedAndEmailed() {
            // Arrange
            _startArg.StartWatcher();
            _mockFile.Setup(m => m.Archive(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Error"));
            // Act
            _startArg.ProcessFile(this,
                new FileSystemEventArgs(WatcherChangeTypes.Changed, "\\Watch", "created-file.csv"));
            // Assert
            _mockLogger.Verify(m => m.LogException(It.IsAny<Exception>(), true), Times.Once, 
                "Expected exception to the logged and email sent");
        }

        private static void SetEntryAssembly(Assembly assembly) {
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }
    }
}
