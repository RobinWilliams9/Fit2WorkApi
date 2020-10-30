using System;
using System.Collections.Generic;
using AnvilGroup.Library.Encryption;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2WorkApi.Helpers;
using Fit2Work.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AnvilGroup.Services.Fit2WorkApi.Tests.Helpers {
    [TestClass]
    public class RequestHelperTests {
        IRequestHelper _helper;

        Mock<IFit2WorkDb> _mockDb = new Mock<IFit2WorkDb>();
        Mock<ILoggingService> _mockLoggingService = new Mock<ILoggingService>();

        [TestInitialize]
        public void Initialise() {
            _helper = new RequestHelper(_mockDb.Object, _mockLoggingService.Object);
        }

        [TestMethod]
        public void ValidateToken_Success() {
            // Arrange
            var token = GenerateToken(1, "447003002001", DateTime.UtcNow);
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> {
                new UserInfoModel { ClientId = 1, PhoneNumber = "447003002001" }
            }));
            // Act
            bool result = _helper.ValidateToken(token, TestHelper.EncryptionKey);
            // Assert
            Assert.IsTrue(result, "Expected token to be VALID");           
            _mockLoggingService.Verify(m => m.LogCriticalError(It.IsAny<Exception>()), Times.Never,
                "Expected no errors to be logged");
        }

        [TestMethod]
        public void ValidateToken_InvalidWhenTimeStampIsOld() {
            // Arrange
            var token = GenerateToken(1, "447003002001", DateTime.UtcNow.AddMinutes(-31));
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> {
                new UserInfoModel { ClientId = 1, PhoneNumber = "447003002001" }
            }));
            // Act
            bool result = _helper.ValidateToken(token, TestHelper.EncryptionKey);
            // Assert
            Assert.IsFalse(result, "Expected token to be INVALID");
            _mockLoggingService.Verify(m => m.LogCriticalError(It.IsAny<Exception>()), Times.Never,
                "Expected no errors to be logged");
        }        

        [TestMethod]
        public void ValidateToken_InvalidWhenClientIdInvalid() {
            // Arrange
            var token = GenerateToken(2, "447003002001", DateTime.UtcNow.AddMinutes(-31));
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> {
                new UserInfoModel { ClientId = 1, PhoneNumber = "447003002001" }
            }));
            // Act
            bool result = _helper.ValidateToken(token, TestHelper.EncryptionKey);
            // Assert
            Assert.IsFalse(result, "Expected token to be INVALID");
            _mockLoggingService.Verify(m => m.LogCriticalError(It.IsAny<Exception>()), Times.Never,
                "Expected no errors to be logged");
        }

        [TestMethod]
        public void ValidateToken_InvalidWhenPhoneNumberInvalid() {
            // Arrange
            var token = GenerateToken(1, "447003002002", DateTime.UtcNow.AddMinutes(-31));
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> {
                new UserInfoModel { ClientId = 1, PhoneNumber = "447003002001" }
            }));
            // Act
            bool result = _helper.ValidateToken(token, TestHelper.EncryptionKey);
            // Assert
            Assert.IsFalse(result, "Expected token to be INVALID");
            _mockLoggingService.Verify(m => m.LogCriticalError(It.IsAny<Exception>()), Times.Never,
                "Expected no errors to be logged");
        }

        [TestMethod]
        public void ValidateToken_InvalidWhenTokenHasNoClientId() {
            // Arrange
            var token = GenerateToken(null, "447003002002", DateTime.UtcNow);
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> {
                new UserInfoModel { ClientId = 1, PhoneNumber = "447003002001" }
            }));
            // Act
            bool result = _helper.ValidateToken(token, TestHelper.EncryptionKey);
            // Assert
            Assert.IsFalse(result, "Expected token to be INVALID");
            _mockLoggingService.Verify(m => m.LogCriticalError(It.IsAny<Exception>()), Times.Once,
                "Expected error to be logged");
        }

        [TestMethod]
        public void ValidateToken_InvalidWhenTokenHasNoPhoneNumber() {
            // Arrange
            var token = GenerateToken(1, null, DateTime.UtcNow);
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> {
                new UserInfoModel { ClientId = 1, PhoneNumber = "447003002001" }
            }));
            // Act
            bool result = _helper.ValidateToken(token, TestHelper.EncryptionKey);
            // Assert
            Assert.IsFalse(result, "Expected token to be INVALID");
            _mockLoggingService.Verify(m => m.LogCriticalError(It.IsAny<Exception>()), Times.Once,
                "Expected error to be logged");
        }

        [TestMethod]
        public void ValidateToken_InvalidWhenTokenHasNoTimestamp() {
            // Arrange
            var token = GenerateToken(1, "447003002002", null);
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> {
                new UserInfoModel { ClientId = 1, PhoneNumber = "447003002001" }
            }));
            // Act
            bool result = _helper.ValidateToken(token, TestHelper.EncryptionKey);
            // Assert
            Assert.IsFalse(result, "Expected token to be INVALID");
            _mockLoggingService.Verify(m => m.LogCriticalError(It.IsAny<Exception>()), Times.Once,
                "Expected error to be logged");
        }

        [TestMethod]
        public void ValidateToken_InvalidWhenTokenRubbish() {
            // Arrange
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> {
                new UserInfoModel { ClientId = 1, PhoneNumber = "447003002001" }
            }));
            // Act
            bool result = _helper.ValidateToken("RUBB1SHT0K3N", TestHelper.EncryptionKey);
            // Assert
            Assert.IsFalse(result, "Expected token to be INVALID");
            _mockLoggingService.Verify(m => m.LogCriticalError(It.IsAny<Exception>()), Times.Once,
                "Expected error to be logged");
        }

        private string GenerateToken(int? clientId, string phoneNumber, DateTime? timestamp) {            
            var message = $"{(clientId.HasValue ? clientId.Value.ToString() : string.Empty)}" +
                $"#{phoneNumber}" +
                $"#{(timestamp.HasValue ? timestamp.Value.ToString("yyyy-MM-ddTHH:mm:ss") : string.Empty)}";
            return new Cryptography().EncryptString(TestHelper.EncryptionKey, message);
        }
    }
}
