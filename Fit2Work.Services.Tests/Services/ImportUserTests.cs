using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2Work.Services.Helpers;
using AnvilGroup.Services.Fit2Work.Services.Providers;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Fit2Work.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fit2Work.Services.Tests.Services {
    [TestClass]
    public class ImportUserTests {
        const string VALID_CSV = "Files\\Users-Valid-WithValidMemberCode.csv";
        const string INVALID_MEMBERCODE_CSV = "Files\\Users-WithInvalidMemberCode.csv";
        const string INVALID_PHONENUMBER_CSV = "Files\\Users-Invalid-WithValidMemberCode.csv";
        const string SOME_INVALID_PHONENUMBER_CSV = "Files\\Users-Mixed-WithValidMemberCode.csv";
        const string INVALID_CSV = "Files\\Users-NoCsv.txt";
        const string DUPLICATES = "Files\\Users-Duplicates.csv";
        const string UPDATE_USER = "Files\\Users-UpdateUser.csv";

        IUserService _service;
        List<string> _log = new List<string>();
        Mock<IFit2WorkDb> _mockDb = new Mock<IFit2WorkDb>();
        Mock<IMessageService> _mockMessageService = new Mock<IMessageService>();
        Mock<ILoggingService> _mockLoggingService = new Mock<ILoggingService>();
        Mock<IResourceProvider> _mockResources = new Mock<IResourceProvider>();
        Mock<IFileSystemService> _mockFileSystemService = new Mock<IFileSystemService>();

        [TestInitialize]
        public void Initialise() {

            _log = new List<string>();
            _service = new UserService(
                _mockDb.Object,
                _mockMessageService.Object,
                _mockLoggingService.Object,
                _mockResources.Object,
                new PhoneNumberHelper(),
                _mockFileSystemService.Object);

            var client = new ClientModel { Id = 1, MemberCode = "client1" };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(
                    new List<ClientModel> { client }));
            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(
                    new List<UserInfoModel>()));
        }

        [TestMethod]
        public void ImportUsers_WithNoFileFound() {
            // Act
            var service = new UserService(
                _mockDb.Object,
                _mockMessageService.Object,
                _mockLoggingService.Object,
                _mockResources.Object,
                new PhoneNumberHelper(),
                new FileSystemService());
            // Act
            try {
                service.ImportUsers("doesnotexist.csv", LogImport);
            } catch {
                // ignore
            }
            // Assert
            _mockLoggingService.Verify(m => m.LogError(It.IsAny<Exception>()), Times.Once, "Expected error to be logged.");
            Assert.AreEqual(3, _log.Count, "Expected 3 log entries");
            Assert.IsTrue(_log[0].Contains("doesnotexist.csv"), "Expected filename in first log entry");
            Assert.IsTrue(_log[1].Contains("does not exist or is not a CSV"), "Expected error message in log entry");
            Assert.IsTrue(_log[2].Contains("complete"), "Expected last entry to contain 'complete'");
        }

        [TestMethod]
        public void ImportUsers_WithNoCsv() {
            // Arrange
            var service = new UserService(
                _mockDb.Object,
                _mockMessageService.Object,
                _mockLoggingService.Object,
                _mockResources.Object,
                new PhoneNumberHelper(),
                new FileSystemService());
            // Act
            try {
                service.ImportUsers(INVALID_CSV, LogImport);
            }
            catch { 
                // ignore 
            }
            // Assert
            _mockLoggingService.Verify(m => m.LogError(It.IsAny<Exception>()), Times.Once, "Expected error to be logged.");
            Assert.AreEqual(3, _log.Count, "Expected 3 log entries");
            Assert.IsTrue(_log[0].Contains(INVALID_CSV), "Expected filename in first log entry");
            Assert.IsTrue(_log[1].Contains("does not exist or is not a CSV"), "Expected error message in log entry");
            Assert.IsTrue(_log[2].Contains("complete"), "Expected last entry to contain 'complete'");
        }

        [TestMethod]
        public void ImportUsers_WithValidCsv() {
            // Arrange
            SetupMockFileSystem(Properties.Resources.Users_Valid_WithValidMemberCode);
            // Act
            var importFile = VALID_CSV;
            _service.ImportUsers(importFile, LogImport);
            // Assert
            _mockDb.Verify(m => m.Users.Add(It.IsAny<UserInfoModel>()), Times.Exactly(10), "Expected 10 users to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
            Assert.AreEqual(12, _log.Count, "Expected 12 log entries");
            Assert.IsTrue(_log[0].Contains(VALID_CSV), "Expected filename in first log entry");
            Assert.IsTrue(_log[_log.Count - 1].Contains("complete"), "Expected last entry to contain 'complete'");
            AssertAllValid(_log);
            _mockLoggingService.Verify(m => m.LogMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once,
                "Expected import information to be logged");
        }

        [TestMethod]
        public void ImportUsers_NoDuplicates() {
            // Arrange
            SetupMockFileSystem(Properties.Resources.Users_Duplicates);

            // Act
            var importFile = TestHelper.GetFullPath(DUPLICATES);
            _service.ImportUsers(importFile, LogImport);
            // Assert
            _mockDb.Verify(m => m.Users.Add(It.IsAny<UserInfoModel>()), Times.Exactly(1), "Expected 1 user to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
            Assert.AreEqual(4, _log.Count, "Expected 4 log entries");
            Assert.IsTrue(_log[0].Contains(DUPLICATES), "Expected filename in first log entry");
            Assert.IsTrue(_log[_log.Count - 1].Contains("complete"), "Expected last entry to contain 'complete'");
            Assert.IsTrue(_mockDb.Object.Users.Count() == 1, "Only 1 user expected");
            _mockLoggingService.Verify(m => m.LogMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once,
                "Expected import information to be logged");
        }

        [TestMethod]
        public void ImportUsers_UpdatesExistingUser() {
            // Arrange
            SetupMockFileSystem(Properties.Resources.Users_UpdateUser);

            // Act
            var importFile = TestHelper.GetFullPath(UPDATE_USER);
            _service.ImportUsers(importFile, LogImport);
            // Assert
            _mockDb.Verify(m => m.Users.Add(It.IsAny<UserInfoModel>()), Times.Exactly(1), "Expected 1 user to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");

            Assert.AreEqual(4, _log.Count, "Expected 3 log entries");
            Assert.IsTrue(_log[0].Contains(UPDATE_USER), "Expected filename in first log entry");
            Assert.IsTrue(_log[_log.Count - 1].Contains("complete"), "Expected last entry to contain 'complete'");
            Assert.IsTrue(_mockDb.Object.Users.Any(x => x.LastName == "NewLastName"), "User's name has not beeen updated");
            //AssertAllValid(_log);
            _mockLoggingService.Verify(m => m.LogMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once,
                "Expected import information to be logged");
        }

        [TestMethod]
        public void ImportUsers_WithValidCsv_InvalidMemberCode() {
            // Arrange
            var client = new ClientModel { Id = 2, MemberCode = "client2" };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(
                    new List<ClientModel> { client }));
            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(
                    new List<UserInfoModel>()));

            SetupMockFileSystem(Properties.Resources.Users_WithInvalidMemberCode);

            // Act
            try { 
                var importFile = TestHelper.GetFullPath(INVALID_MEMBERCODE_CSV);
                _service.ImportUsers(importFile, LogImport);
            } catch {
                // ignore
            }
            // Assert
            _mockDb.Verify(m => m.Users.Add(It.IsAny<UserInfoModel>()), Times.Never, "Expected NO users to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Never, "Expected NO db changes to be saved");
            _mockLoggingService.Verify(m => m.LogError(It.IsAny<Exception>()), Times.Once, "Expected error to be logged.");
            Assert.AreEqual(3, _log.Count, "Expected 3 log entries");
            Assert.IsTrue(_log[0].Contains(INVALID_MEMBERCODE_CSV), "Expected filename in first log entry");
            Assert.IsTrue(_log[1].Contains("Failed to find client"), "Expected error message in log entry");
            Assert.IsTrue(_log[_log.Count - 1].Contains("complete"), "Expected last entry to contain 'complete'");
            _mockLoggingService.Verify(m => m.LogMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once,
                "Expected import information to be logged");
        }

        [TestMethod]
        public void ImportUsers_WithValidCsv_InvalidPhoneNumbers() {
            // Arrange

            SetupMockFileSystem(Properties.Resources.Users_Invalid_WithValidMemberCode);

            // Act
            var importFile = TestHelper.GetFullPath(INVALID_PHONENUMBER_CSV);
            _service.ImportUsers(importFile, LogImport);
            // Assert

            _mockDb.Verify(m => m.Users.Add(It.IsAny<UserInfoModel>()), Times.Never, "Expected never to add users");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected save to be called once");

            Assert.IsTrue(_mockDb.Object.Users.Count() == 0, "No users should exist in the db");
            Assert.AreEqual(13, _log.Count, "Expected 13 log entries");
            Assert.IsTrue(_log[0].Contains(INVALID_PHONENUMBER_CSV), "Expected filename in first log entry");
            Assert.IsTrue(_log[_log.Count - 2].Contains("File contains invalid users"), "Expected error message in log entry");
            Assert.IsTrue(_log[_log.Count - 1].Contains("complete"), "Expected last entry to contain 'complete'");
            AssertAllInvalid(_log);
            _mockLoggingService.Verify(m => m.LogMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once,
                "Expected import information to be logged");
        }

        [TestMethod]
        public void ImportUsers_WithValidCsv_SomeInvalidPhoneNumbers() {
            // Arrange

            SetupMockFileSystem(Properties.Resources.Users_Mixed_WithValidMemberCode);

            // Act
            var importFile = TestHelper.GetFullPath(SOME_INVALID_PHONENUMBER_CSV);
            _service.ImportUsers(importFile, LogImport);
            // Assert
            _mockDb.Verify(m => m.Users.Add(It.IsAny<UserInfoModel>()), Times.Exactly(4), "Expected never to add users");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected 1 change to be saved");
            Assert.AreEqual(13, _log.Count, "Expected 13 log entries");
            Assert.IsTrue(_log[0].Contains(SOME_INVALID_PHONENUMBER_CSV), "Expected filename in first log entry");
            Assert.IsTrue(_log[_log.Count - 2].Contains("File contains invalid users"), "Expected error message in log entry");
            Assert.IsTrue(_log[_log.Count - 1].Contains("complete"), "Expected last entry to contain 'complete'");
            AssertSomeInvalid(_log);
            _mockLoggingService.Verify(m => m.LogMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once,
                "Expected import information to be logged");
        }

        private void AssertAllValid(List<string> log) {
            // Ignore the first and last entries
            for (int i = 1; i < log.Count - 1; i++) {
                Assert.IsTrue(
                   _log[i].Contains("CREATED") || _log[i].Contains("UPDATED"), "Expected all entries to contain 'CREATED' or 'UPDATED'");
                Assert.IsTrue(
                    _log[i].Contains($"Row [{i}]"), "Expected entries to contain row number");
            }
        }

        private void AssertAllInvalid(List<string> log) {
            // Ignore the first and last entries
            for (int i = 1; i < log.Count - 2; i++) {
                Assert.IsTrue(
                    _log[i].Contains("INVALID"), "Expected all entries to contain 'INVALID'");
                Assert.IsTrue(
                    _log[i].Contains($"Row [{i}]"), "Expected entries to contain row number");
            }
        }

        private void AssertSomeInvalid(List<string> log) {
            var foundInvalidEntry = false;
            // Ignore the first and last entries
            for (int i = 1; i < log.Count - 2; i++) {
                if(_log[i].Contains("INVALID")) {
                    foundInvalidEntry = true;
                }
                Assert.IsTrue(
                    _log[i].Contains($"Row [{i}]"), "Expected entries to contain row number");
            }
            Assert.IsTrue(foundInvalidEntry, "Expected all entries to contain 'INVALID'");
        }

        /// <summary>
        /// Log is provided as all entries in a single string.
        /// So split out here so we can assert on individual log entries
        /// </summary>
        private void LogImport(string logEntry) {
            var entries = logEntry.Split(("\r\n").ToCharArray()).ToList();
            _log.AddRange(entries
                .Where(e =>
                    !string.IsNullOrEmpty(e)
                    && (e.ToUpper().StartsWith("ROW")
                    || e.ToUpper().StartsWith("IMPORT"))));
        }

        /// <summary>
        /// Helper method to turn a resourced text file into a stream
        /// </summary>
        /// <param name="streamAsString"></param>
        private void SetupMockFileSystem(string streamAsString) {
            byte[] byteArray = Encoding.ASCII.GetBytes(streamAsString);
            MemoryStream stream = new MemoryStream(byteArray);

            // convert stream to string
            StreamReader reader = new StreamReader(stream);

            _mockFileSystemService.Setup(m => m.GetCSVReaderFromPath(It.IsAny<string>()))
                .Returns(reader);
        }
    }
}
