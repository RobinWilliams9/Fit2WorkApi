using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2WorkApi.Controllers;
using AnvilGroup.Services.Fit2Work.Services.Providers;
using Moq;
using System.Net.Http;
using AnvilGroup.Services.Fit2Work.Models;
using System.Net;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2WorkApi.Tests {
    /// <summary>
    /// Summary description for ResourcesControllerTests
    /// </summary>
    [TestClass]
    public class ResourcesControllerTests {
        ResourcesController _controller;
        Mock<IResourceProvider> _mockResources = new Mock<IResourceProvider>();
        Mock<ILoggingService> _mockLoggingService = new Mock<ILoggingService>();

        [TestInitialize]
        public void Initialise() {
            _controller = new ResourcesController(
                _mockResources.Object,
                _mockLoggingService.Object);
            _controller.Request = new HttpRequestMessage();
        }

        [TestMethod]
        public void Get_ReturnsOkAndResourceUrls() {
            // Arrange
            var expectedResources = new List<ResourceUrlModel> {
                new ResourceUrlModel { Name = "Url1", Url = "http://test1.url" },
                new ResourceUrlModel { Name = "Url2", Url = "http://test2.url" },
                new ResourceUrlModel { Name = "Url3", Url = "http://test3.url" }
            };
            _mockResources.Setup(m => m.ResourceUrls).Returns(expectedResources);
            // Act
            var response = _controller.Get();
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected OK");
            var actualResources = JsonConvert.DeserializeObject<List<ResourceUrlModel>>(response.Content.ReadAsStringAsync().Result);
            for(int i=0;i<expectedResources.Count;i++) {
                Assert.AreEqual(expectedResources[i].Name, actualResources[i].Name, $"Expected index {i} Name to match");
                Assert.AreEqual(expectedResources[i].Url, actualResources[i].Url, $"Expected index {i} Url to match");
            }
        }

        [TestMethod]
        public void Get_WhenError_LogsAndReturnsError() {
            // Arrange
            var exception = new ApplicationException("An error occurred");
            _mockResources.Setup(m => m.ResourceUrls).Throws(exception);
            // Act
            var response = _controller.Get();
            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Expected InternalServerError");
            _mockLoggingService.Verify(m => m.LogError(exception), Times.Once, "Expected error to be logged.");
        }
    }
}
