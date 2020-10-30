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
    public class ClientControllerTests {
        ClientController _controller;
        Mock<IClientService> _mockClientService = new Mock<IClientService>();
        Mock<ILoggingService> _mockLoggingService = new Mock<ILoggingService>();

        [TestInitialize]
        public void Initialise() {
            _controller = new ClientController(
                _mockClientService.Object,
                _mockLoggingService.Object);
            _controller.Request = new HttpRequestMessage();
        }

        [TestMethod]
        public void Get_ReturnsOkAndClient() {
            // Arrange
            var expectedClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "clientcode1" };
            _mockClientService.Setup(m => m.GetClientByCode(It.IsAny<string>())).Returns(expectedClient);
            // Act
            var response = _controller.Get(expectedClient.MemberCode);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected OK");

            var actualClient = JsonConvert.DeserializeObject<ClientModel>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(expectedClient.Id, actualClient.Id);
            Assert.AreEqual(expectedClient.IsDeleted, actualClient.IsDeleted);
            // Do not want to return any more client details
            Assert.IsNull(actualClient.Name, "Did not expect Name top be returned");
            Assert.IsNull(actualClient.PrimaryEmailAddress, "Did not expect PrimaryEmailAddress top be returned");
            Assert.IsNull(actualClient.SecondaryEmailAddress, "Did not expect SecondaryEmailAddress top be returned");
        }

        [TestMethod]
        public void Get_WhenNoClient_ReturnsNotFound() {
            // Arrange
            ClientModel expectedClient = null;
            _mockClientService.Setup(m => m.GetClientByCode(It.IsAny<string>())).Returns(expectedClient);
            // Act
            var response = _controller.Get("anycode");
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Expected NotFound");           
        }

        [TestMethod]
        public void Get_WhenError_LogsAndReturnsError() {
            // Arrange
            var exception = new ApplicationException("An error occurred");
            _mockClientService.Setup(m => m.GetClientByCode(It.IsAny<string>())).Throws(exception);
            // Act
            var response = _controller.Get("anycode");
            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Expected InternalServerError");
            _mockLoggingService.Verify(m => m.LogError(exception), Times.Once, "Expected error to be logged.");
        }
    }
}
