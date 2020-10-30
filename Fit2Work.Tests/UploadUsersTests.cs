using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AnvilGroup.Applications.Fit2Work.Controllers;
using AnvilGroup.Applications.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using Fit2Work.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fit2Work.Tests {
    [TestClass]
    public class UploadUsersTests {        
        UploadModel _model = new UploadModel();
        HomeController _homeController;

        Mock<IUserService> _mockUserService = new Mock<IUserService>();
        Mock<HttpRequestBase> _mockRequest = new Mock<HttpRequestBase>();
        Mock<HttpContextBase> _mockContext = new Mock<HttpContextBase>();
        Mock<HttpPostedFileBase> _mockFile = new Mock<HttpPostedFileBase>();
        Mock<HttpFileCollectionBase> _mockFiles = new Mock<HttpFileCollectionBase>();        

        [TestInitialize]
        public void Initialise() {            
            _mockRequest.Setup(m => m.Files).Returns(_mockFiles.Object);
            _mockContext.Setup(m => m.Request).Returns(_mockRequest.Object);
            _homeController = new HomeController(null, _mockUserService.Object);
            _homeController.ControllerContext = new ControllerContext(_mockContext.Object, new RouteData(), _homeController);
        }

        [TestMethod]
        public void Upload_WithNoFile() {
            // Arrange
            _homeController.ViewData.ModelState.Clear();
            _mockFiles.Setup(m => m.Count).Returns(0);
            // Act
            var result = _homeController.Upload(new UploadModel {
                ClientId = 1
            });
            // Arrange
            Assert.IsNotNull(result);
            Assert.AreEqual(false, TestHelper.GetReflectedProperty(result.Data, "success"),
                "Expected failed result");
            _mockUserService.Verify(m =>
                m.ImportUsers(1, It.IsAny<string>(), It.IsAny<System.IO.Stream>(), It.IsAny<Action<string>>()), Times.Never,
                "Expected users NOT to be imported");
        }

        [TestMethod]
        public void Upload_WithValidCsvFile() {
            // Arrange
            var uploadFilename = "users.csv";
            _homeController.ViewData.ModelState.Clear();
            _mockFile.Setup(m => m.FileName).Returns(uploadFilename);
            _mockFiles.Setup(m => m.Count).Returns(1);
            _mockFiles.Setup(m => m[0]).Returns(_mockFile.Object);
            // Act
            var result = _homeController.Upload(new UploadModel {
                ClientId = 1
            });
            // Arrange
            Assert.IsNotNull(result);
            Assert.AreEqual(true, TestHelper.GetReflectedProperty(result.Data, "success"),
                "Expected success result");
            _mockUserService.Verify(m =>
                m.ImportUsers(1, uploadFilename, It.IsAny<System.IO.Stream>(), It.IsAny<Action<string>>()), Times.Once,
                "Expected users to be imported");
        }

        [TestMethod]
        public void Upload_WithValidCsvFileExtension() {
            // Arrange
            var uploadFilename = "users.test.csv";
            _homeController.ViewData.ModelState.Clear();
            _mockFile.Setup(m => m.FileName).Returns(uploadFilename);
            _mockFiles.Setup(m => m.Count).Returns(1);
            _mockFiles.Setup(m => m[0]).Returns(_mockFile.Object);
            // Act
            var result = _homeController.Upload(new UploadModel {
                ClientId = 1
            });
            // Arrange
            Assert.IsNotNull(result);
            Assert.AreEqual(true, TestHelper.GetReflectedProperty(result.Data, "success"),
                "Expected success result");
            _mockUserService.Verify(m =>
                m.ImportUsers(1, uploadFilename, It.IsAny<System.IO.Stream>(), It.IsAny<Action<string>>()), Times.Once,
                "Expected users to be imported");
        }

        [TestMethod]
        public void Upload_WithInvalidCsvFile() {
            // Arrange
            var uploadFilename = "users.txt";
            _homeController.ViewData.ModelState.Clear();
            _mockFile.Setup(m => m.FileName).Returns(uploadFilename);
            _mockFiles.Setup(m => m.Count).Returns(1);
            _mockFiles.Setup(m => m[0]).Returns(_mockFile.Object);
            // Act
            var result = (JsonResult)_homeController.Upload(new UploadModel {
                ClientId = 1
            });
            // Arrange
            Assert.IsNotNull(result);
            Assert.AreEqual(false, TestHelper.GetReflectedProperty(result.Data, "success"),
                "Expected failed result");
            _mockUserService.Verify(m =>
                m.ImportUsers(1, uploadFilename, It.IsAny<System.IO.Stream>(), It.IsAny<Action<string>>()), Times.Never,
                "Expected users NOT to be imported");
        }

        [TestMethod]
        public void Upload_WithErrorOnImporting() {
            // Arrange
            var uploadFilename = "users.csv";
            _homeController.ViewData.ModelState.Clear();
            _mockFile.Setup(m => m.FileName).Returns(uploadFilename);
            _mockFiles.Setup(m => m.Count).Returns(1);
            _mockFiles.Setup(m => m[0]).Returns(_mockFile.Object);
            _mockUserService.Setup(m =>
                m.ImportUsers(1, uploadFilename, It.IsAny<System.IO.Stream>(), It.IsAny<Action<string>>()))
                .Throws(new ApplicationException("An error occurred"));
            // Act
            var result = _homeController.Upload(new UploadModel {
                ClientId = 1
            });
            // Arrange
            Assert.IsNotNull(result);
            Assert.AreEqual(false, TestHelper.GetReflectedProperty(result.Data, "success"),
                "Expected failure result");
            Assert.AreEqual("An error occurred", TestHelper.GetReflectedProperty(result.Data, "responseText"),
                "Expected error in response");
        }
    }
}
