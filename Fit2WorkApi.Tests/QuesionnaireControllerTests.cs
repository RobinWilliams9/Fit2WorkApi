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
    public class QuestionnaireControllerTests {
        QuestionnaireController _controller;
        Mock<IUserService> _mockUserService = new Mock<IUserService>();
        Mock<ILoggingService> _mockLoggingService = new Mock<ILoggingService>();

        [TestInitialize]
        public void Initialise() {
            _controller = new QuestionnaireController(
                _mockUserService.Object, 
                _mockLoggingService.Object);
            _controller.Request = new HttpRequestMessage();
        }

        [TestMethod]
        public void Post_FitToWork_ReturnsOk() {
            // Arrange
            var questionnaire = new UserQuestionnaireModel { };
            var questionnaireResult = new QuestionnaireResult { IsFitToWork = true, UserMessage = "FitToWork" };
            _mockUserService.Setup(m => m.SubmitUserQuestionnaire(questionnaire))
                .Returns(questionnaireResult);
            // Act
            var response = _controller.Post(questionnaire);
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected OK");
            var result = JsonConvert.DeserializeObject<QuestionnaireResult>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(questionnaireResult.IsFitToWork, result.IsFitToWork);
            Assert.AreEqual(questionnaireResult.UserMessage, result.UserMessage);
            _mockUserService.Verify(m => m.SubmitUserQuestionnaire(questionnaire), Times.Once, "Expected questionnaire to be submitted");
        }

        [TestMethod]
        public void Post_NotFitToWork_ReturnsOk() {
            // Arrange
            var questionnaire = new UserQuestionnaireModel { };
            var questionnaireResult = new QuestionnaireResult { IsFitToWork = false, UserMessage = "NotFitToWork" };
            _mockUserService.Setup(m => m.SubmitUserQuestionnaire(questionnaire))
                .Returns(questionnaireResult);
            // Act
            var response = _controller.Post(questionnaire);
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected OK");
            var result = JsonConvert.DeserializeObject<QuestionnaireResult>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(questionnaireResult.IsFitToWork, result.IsFitToWork);
            Assert.AreEqual(questionnaireResult.UserMessage, result.UserMessage);
            _mockUserService.Verify(m => m.SubmitUserQuestionnaire(questionnaire), Times.Once, "Expected questionnaire to be submitted");
        }

        [TestMethod]
        public void Post_WhenError_LogsAndReturnsError() {
            // Arrange
            var questionnaire = new UserQuestionnaireModel { };
            var exception = new ApplicationException("An error has occurred.");
            _mockUserService.Setup(m => m.SubmitUserQuestionnaire(It.IsAny<UserQuestionnaireModel>())).Throws(exception);
            // Act
            var response = _controller.Post(questionnaire);
            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode, "Expected InternalServerError");
            _mockUserService.Verify(m => m.SubmitUserQuestionnaire(questionnaire), Times.Once, "Expected questionnaire to be submitted");
            _mockLoggingService.Verify(m => m.LogCriticalError(exception), Times.Once, "Expected critical error to be logged.");
        }
    }
}
