using System;
using System.Net;
using System.Net.Http;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2WorkApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2WorkApi.Tests {
    [TestClass]
    public class UserControllerTests {
        UserController _controller;
        Mock<IUserService> _mockUserService = new Mock<IUserService>();
        Mock<ILoggingService> _mockLoggingService = new Mock<ILoggingService>();

        [TestInitialize]
        public void Initialise() {
            _controller = new UserController(_mockUserService.Object, _mockLoggingService.Object);
            _controller.Request = new HttpRequestMessage();
        }


        [TestMethod]
        public void Post_ValidUser_ReturnsOkAndUser() {
            // Arrange
            var expectedUser = new UserInfoModel { Id = 1, FirstName = "User1", LastName = "Test1", PhoneNumber = "7754321" };
            // Act
            var response = _controller.Post(expectedUser);
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected OK");
            var actualUser = JsonConvert.DeserializeObject<UserInfoModel>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(expectedUser.Id, actualUser.Id);
            Assert.AreEqual(expectedUser.FirstName, actualUser.FirstName);
            Assert.AreEqual(expectedUser.LastName, actualUser.LastName);
            Assert.AreEqual(expectedUser.PhoneNumber, actualUser.PhoneNumber);
            _mockUserService.Verify(
                m => m.RegisterUser(expectedUser), Times.Once, 
                "Expected user to be created");
        }

        [TestMethod]
        public void Post_WhenUserInvalid_LogsReturnsNotAcceptable() {
            // Arrange
            var userInfo = new UserInfoModel { };
            var exception = new ArgumentOutOfRangeException("Invalid");
            _mockUserService.Setup(m => m.RegisterUser(It.IsAny<UserInfoModel>())).Throws(exception);
            // Act
            var response = _controller.Post(userInfo);
            // Assert
            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode, "Expected NotAcceptable");
            _mockLoggingService.Verify(
                m => m.LogError(exception), Times.Once,
                "Expected error to be logged");
        }

        [TestMethod]
        public void Post_WhenUserNotFound_LogsReturnsNotFound() {
            // Arrange
            var userInfo = new UserInfoModel { };
            var exception = new ArgumentNullException("Not found");
            _mockUserService.Setup(m => m.RegisterUser(It.IsAny<UserInfoModel>())).Throws(exception);
            // Act
            var response = _controller.Post(userInfo);
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Expected NotFound");
            _mockLoggingService.Verify(
                m => m.LogError(exception), Times.Once,
                "Expected error to be logged");
        }

        [TestMethod]
        public void Post_WhenClientDeleted_LogsReturnsConflict() {
            // Arrange
            var userInfo = new UserInfoModel { };
            var exception = new ArgumentException("Deleted");
            _mockUserService.Setup(m => m.RegisterUser(It.IsAny<UserInfoModel>())).Throws(exception);
            // Act
            var response = _controller.Post(userInfo);
            // Assert
            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode, "Expected Conflict");
            _mockLoggingService.Verify(
                m => m.LogError(exception), Times.Once,
                "Expected error to be logged");
        }

        [TestMethod]
        public void Post_WhenErrorCreatingUser_LogsAndReturnsInternalServerError() {
            // Arrange
            var userInfo = new UserInfoModel { };
            var exception = new Exception("An error occurred");
            _mockUserService.Setup(m => m.RegisterUser(It.IsAny<UserInfoModel>())).Throws(exception);
            // Act
            var response = _controller.Post(userInfo);
            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Expected InternalServerError");
            _mockLoggingService.Verify(
                m => m.LogError(exception), Times.Once,
                "Expected error to be logged");
        }
    }
}
