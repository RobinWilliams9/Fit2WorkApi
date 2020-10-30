using System;
using System.Collections.Generic;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2Work.Services.Providers;
using Fit2Work.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fit2Work.Services.Tests {
    [TestClass]
    public class LoggingServiceTests {
        ILoggingService _service;
        Mock<IFit2WorkDb> _mockDb = new Mock<IFit2WorkDb>();
        Mock<IEmailProvider> _mockEmailProvider = new Mock<IEmailProvider>();
        Mock<IConfigurationProvider> _mockConfigurationProvider = new Mock<IConfigurationProvider>();

        [TestInitialize]
        public void Initialise() {
            _mockDb.Setup(m => m.Logs).Returns(TestHelper.GetQueryableMockDbSet(new List<LogModel>()));
            _mockConfigurationProvider.Setup(m => m.SupportEmailFromAddress).Returns("SupportEmailFromAddress");
            _mockConfigurationProvider.Setup(m => m.SupportEmailToAddress).Returns("SupportEmailToAddress");
            _mockConfigurationProvider.Setup(m => m.SupportEmailSubjectPrefix).Returns("SupportEmailSubjectPrefix");
            _service = new LoggingService(_mockDb.Object, _mockEmailProvider.Object, _mockConfigurationProvider.Object);
        }

        [TestMethod]
        public void LogMessage_WithoutDetails_LogsToDb() {
            // Arrange
            var message = "This is a message";
            // Act
            _service.LogMessage(message);
            // Assert
            _mockDb.Verify(m => m.Logs.Add(It.IsAny<LogModel>()), Times.Once, "Expected message to be created");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
        }

        [TestMethod]
        public void LogMessage_WithDetails_LogsToDb() {
            // Arrange
            var message = "This is a message";
            var details = "These are the details";
            // Act
            _service.LogMessage(message, details);
            // Assert
            _mockDb.Verify(m => m.Logs.Add(It.IsAny<LogModel>()), Times.Once, "Expected message to be created");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
        }

        [TestMethod]
        public void LogMessage_WithError_SendsSupportEmail() {
            // Arrange
            var message = "This is a message";
            _mockDb.Setup(m => m.SaveChanges()).Throws(new ApplicationException());
            // Act
            _service.LogMessage(message);
            // Assert
            _mockEmailProvider.Verify(
                m => m.SendMail(
                    "SupportEmailFromAddress",
                    "SupportEmailToAddress",
                    "SupportEmailSubjectPrefix : Unable to write to db log",
                    It.IsAny<string>()),
                Times.Once,
                "Expected email to be sent to support");
        }

        [TestMethod]
        public void LogError_LogsToDb() {
            // Arrange
            var exception = new ApplicationException("This is an error");
            // Act
            _service.LogError(exception);
            // Assert
            _mockDb.Verify(m => m.Logs.Add(It.IsAny<LogModel>()), Times.Once, "Expected error to be created");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
        }

        [TestMethod]
        public void LogError_WhenError_SendsSupportEmail() {
            // Arrange
            var exception = new ApplicationException("This is an error");
            _mockDb.Setup(m => m.SaveChanges()).Throws(new ApplicationException());
            // Act
            _service.LogError(exception);
            // Assert
            _mockEmailProvider.Verify(
                m => m.SendMail(
                    "SupportEmailFromAddress",
                    "SupportEmailToAddress",
                    "SupportEmailSubjectPrefix : Unable to write to db log",
                    It.IsAny<string>()),
                Times.Once,
                "Expected email to be sent to support");
        }

        [TestMethod]
        public void LogCriticalError_LogsToDbAndSendsSupportEmail() {
            // Arrange
            var exception = new ApplicationException("This is a critical error");
            // Act
            _service.LogCriticalError(exception);
            // Assert
            _mockDb.Verify(m => m.Logs.Add(It.IsAny<LogModel>()), Times.Once, "Expected critical error to be created");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
            _mockEmailProvider.Verify(
                m => m.SendMail(
                    "SupportEmailFromAddress", 
                    "SupportEmailToAddress",
                    "SupportEmailSubjectPrefix : This is a critical error",
                    "System.ApplicationException: This is a critical error"), 
                Times.Once, 
                "Expected email to be sent to support");
        }

        [TestMethod]
        public void LogCriticalError_WhenError_SendsSupportEmail() {
            // Arrange
            var exception = new ApplicationException("This is a critical error");
            _mockDb.Setup(m => m.SaveChanges()).Throws(new ApplicationException());
            // Act
            _service.LogCriticalError(exception);
            // Assert
            _mockEmailProvider.Verify(
                m => m.SendMail(
                    "SupportEmailFromAddress",
                    "SupportEmailToAddress",
                    "SupportEmailSubjectPrefix : Unable to write to db log",
                    It.IsAny<string>()),
                Times.Once,
                "Expected email to be sent to support");
        }
    }
}
