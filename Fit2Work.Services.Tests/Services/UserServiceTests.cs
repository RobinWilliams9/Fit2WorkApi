using System;
using System.Collections.Generic;
using System.Linq;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2Work.Services.Helpers;
using AnvilGroup.Services.Fit2Work.Services.Providers;
using Fit2Work.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fit2Work.Services.Tests {
    [TestClass]
    public class UserServiceTests {
        IUserService _service;
        Mock<IFit2WorkDb> _mockDb = new Mock<IFit2WorkDb>();
        Mock<IMessageService> _mockMessageService = new Mock<IMessageService>();
        Mock<ILoggingService> _mockLoggingService = new Mock<ILoggingService>();
        Mock<IResourceProvider> _mockResources = new Mock<IResourceProvider>();
        Mock<IPhoneNumberHelper> _mockPhoneNumberHelper = new Mock<IPhoneNumberHelper>();

        [TestInitialize]
        public void Initialise() {
            _mockResources.Setup(m => m.UserPrimaryMessageText).Returns("UserPrimaryMessageText");
            _mockResources.Setup(m => m.UserSecondaryMessageText).Returns("UserSecondaryMessageText");
            _mockDb.Setup(m => m.UserQuestionnaires)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<UserQuestionnaireModel>()));
            _service = new UserService(_mockDb.Object,
                _mockMessageService.Object,
                _mockLoggingService.Object,
                _mockResources.Object,
                _mockPhoneNumberHelper.Object,
                null);
        }

        [TestMethod]
        public void CreateUser_WithValidPhoneNumber() {
            // Arrange
            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> {
                    new UserInfoModel { Id = 1, FirstName = "User1", PhoneNumber = "4477123451" },
                    new UserInfoModel { Id = 2, FirstName = "User2", PhoneNumber = "4477123452" }
            }));
            _mockPhoneNumberHelper.Setup(m => m.IsValidMobileNumber(It.IsAny<string>())).Returns(true);
            var newUser = new UserInfoModel {
                FirstName = "User3", LastName = "Test3", PhoneNumber = "4477123453"
            };            
            // Act
            _service.CreateUser(newUser);
            // Assert
            _mockDb.Verify(m => m.Users.Add(It.IsAny<UserInfoModel>()), Times.Once,
                "Expected user to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once,
                "Expected db changes to be saved");
            Assert.IsTrue(newUser.CreatedDate > DateTime.MinValue,
                "Expected CreatedDate to be set");
            Assert.IsFalse(newUser.IsDeleted,
                "Expected IsDeleted to be false");
        }

        [TestMethod]
        public void CreateUser_WithInvalidPhoneNumber() {
            // Arrange
            Exception expectedException = new Exception();
            _mockPhoneNumberHelper.Setup(m => m.IsValidMobileNumber(It.IsAny<string>())).Returns(false);

            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(
                    new List<UserInfoModel>()));

            var newUser = new UserInfoModel {
                FirstName = "User3", LastName = "Test3", PhoneNumber = "4477123453"
            };

            // Act
            _service.CreateUser(newUser);

            // Assert
            _mockDb.Verify(m => m.Users.Add(It.IsAny<UserInfoModel>()), Times.Never,
                "Expected user NOT to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Never,
                "Expected NO db changes to be saved");
            Assert.IsTrue(_mockDb.Object.Users.Count() == 0, "Expected - The db should be empty");
        }

        [TestMethod]
        public void CreateUser_WithExistingPhoneNumber_RetainsExistingUserId() {
            // Arrange
            var newUser = new UserInfoModel {
                FirstName = "User3", LastName = "Test3", PhoneNumber = "4477123451"
            };
            var existingUser = new UserInfoModel {
                FirstName = "User1", LastName = "Test1", PhoneNumber = "4477123451"
            };            
            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> { existingUser }));
            _mockPhoneNumberHelper.Setup(m => m.IsValidMobileNumber(It.IsAny<string>())).Returns(true);
            // Act
            _service.CreateUser(newUser);
            // Assert
            Assert.AreEqual(existingUser.Id, newUser.Id,
                "Expected user id to be set to existing user");
            Assert.IsTrue(existingUser.UpdatedDate > DateTime.MinValue,
                "Expected existing user UpdatedDate to be set");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once,
                "Expected db changes to be saved");
        }

        [TestMethod]
        public void CreateUser_WithExistingPhoneNumber_DifferentClient_CreatesUser() {
            // Arrange
            var newUser = new UserInfoModel {
                ClientId = 2, FirstName = "User3", LastName = "Test3", PhoneNumber = "4477123451"
            };
            var existingUser = new UserInfoModel {
                ClientId = 1, FirstName = "User1", LastName = "Test1", PhoneNumber = "4477123451"
            };
            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> { existingUser }));
            _mockPhoneNumberHelper.Setup(m => m.IsValidMobileNumber(It.IsAny<string>())).Returns(true);
            // Act
            _service.CreateUser(newUser);
            // Assert
            _mockDb.Verify(m => m.Users.Add(It.IsAny<UserInfoModel>()), Times.Once,
                "Expected user to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once,
                "Expected db changes to be saved");
            Assert.IsTrue(newUser.CreatedDate > DateTime.MinValue,
                "Expected CreatedDate to be set");
        }

        [TestMethod]
        public void CreateUser_WithExistingPhoneNumber_UpdatesExistingUser() {
            // Arrange
            var newUser = new UserInfoModel {
                FirstName = "User3", LastName = "Test3", PhoneNumber = "4477123451"
            };
            var existingUser = new UserInfoModel {
                FirstName = "User1", LastName = "Test1", PhoneNumber = "4477123451"
            };
            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> { existingUser }));
            _mockPhoneNumberHelper.Setup(m => m.IsValidMobileNumber(It.IsAny<string>())).Returns(true);
            // Act
            _service.CreateUser(newUser);
            // Assert
            Assert.AreEqual(newUser.FirstName, existingUser.FirstName,
                "Expected user FirstName to be set to existing user");
            Assert.AreEqual(newUser.LastName, existingUser.LastName,
                "Expected user LastName to be set to existing user");
            Assert.IsTrue(existingUser.UpdatedDate > DateTime.MinValue,
                "Expected existing user UpdatedDate to be set");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once,
                "Expected db changes to be saved");
        }

        [TestMethod]
        public void SubmitUserQuestionnaire_FitToWork_Success() {
            // Arrange
            var client = new ClientModel { Id = 1, IsDeleted = false };
            var user = new UserInfoModel { Id = 1, Client = client };
            var questionnaire = new UserQuestionnaireModel { UserId = user.Id, QuestionsAndAnswers = { new UserAnswerModel { Answer = "No" } } };
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> { user }));
            _mockMessageService.Setup(m => m.SendQuestionnaireToClient(It.IsAny<UserQuestionnaireModel>())).Returns(new MessageResult());
            // Act
            var result = _service.SubmitUserQuestionnaire(questionnaire);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsFitToWork, "Expected result.IsFitToWork");
            Assert.AreEqual("UserPrimaryMessageText", result.UserMessage, "Expected UserPrimaryMessageText");
            Assert.IsTrue(questionnaire.CreatedDate > DateTime.MinValue, "Expected questionnaire created date to be set");
            _mockDb.Verify(m => m.UserQuestionnaires.Add(It.IsAny<UserQuestionnaireModel>()), Times.Once,
                "Expected questionnare to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
            _mockMessageService.Verify(m =>
                m.SendQuestionnaireToClient(It.IsAny<UserQuestionnaireModel>()), Times.Once,
                "Expected message sent to client");
            _mockLoggingService.Verify(m => m.LogMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once,
                "Expeced message to be logged");
        }

        [TestMethod]
        public void SubmitUserQuestionnaire_NotFitToWork_Success() {
            // Arrange
            var client = new ClientModel { Id = 1, IsDeleted = false };
            var user = new UserInfoModel { Id = 1, Client = client };
            var questionnaire = new UserQuestionnaireModel { UserId = user.Id, QuestionsAndAnswers = { new UserAnswerModel { Answer = "Yes" } } };
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> { user }));
            _mockMessageService.Setup(m => m.SendQuestionnaireToClient(It.IsAny<UserQuestionnaireModel>())).Returns(new MessageResult());
            // Act
            var result = _service.SubmitUserQuestionnaire(questionnaire);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsFitToWork, "Expected NOT result.IsFitToWork");
            Assert.AreEqual("UserSecondaryMessageText", result.UserMessage, "Expected UserSecondaryMessageText");
            Assert.IsTrue(questionnaire.CreatedDate > DateTime.MinValue, "Expected questionnaire created date to be set");
            _mockDb.Verify(m => m.UserQuestionnaires.Add(It.IsAny<UserQuestionnaireModel>()), Times.Once,
                "Expected questionnare to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
            _mockMessageService.Verify(m =>
                m.SendQuestionnaireToClient(It.IsAny<UserQuestionnaireModel>()), Times.Once,
                "Expected message sent to client");
            _mockLoggingService.Verify(m => m.LogMessage(It.IsAny<string>(), It.IsAny<string>()), Times.Once,
                "Expeced message to be logged");
        }

        [TestMethod]
        public void SubmitUserQuestionnaire_WithoutUser_ThrowsException() {
            try {
                // Arrange 
                var questionnaire = new UserQuestionnaireModel();
                // Act
                _service.SubmitUserQuestionnaire(questionnaire);
            } catch {
                // Assert
                _mockDb.Verify(m => m.UserQuestionnaires.Add(It.IsAny<UserQuestionnaireModel>()), Times.Never,
                    "Expected NO questionnare to be added");
                _mockDb.Verify(m => m.SaveChanges(), Times.Never,
                    "Expected NO db changes to be saved");
                _mockMessageService.Verify(m =>
                    m.SendQuestionnaireToClient(It.IsAny<UserQuestionnaireModel>()), Times.Never,
                    "Expected NO message sent to client");
            }
        }

        [TestMethod]
        public void SubmitUserQuestionnaire_WithoutClient_ThrowsException() {
            try {
                // Arrange
                var user = new UserInfoModel { Id = 1, Client = null };
                var questionnaire = new UserQuestionnaireModel { User = user };
                // Act
                _service.SubmitUserQuestionnaire(questionnaire);
            } catch {
                // Assert
                _mockDb.Verify(m => m.UserQuestionnaires.Add(It.IsAny<UserQuestionnaireModel>()), Times.Never,
                    "Expected NO questionnare to be added");
                _mockDb.Verify(m => m.SaveChanges(), Times.Never,
                    "Expected NO db changes to be saved");
                _mockMessageService.Verify(m =>
                    m.SendQuestionnaireToClient(It.IsAny<UserQuestionnaireModel>()), Times.Never,
                    "Expected NO message sent to client");
            }
        }

        [TestMethod]
        public void SubmitUserQuestionnaire_WhenClientIsDeleted_ThrowsException() {
            try {
                // Arrange
                var client = new ClientModel { Id = 1, IsDeleted = true };
                var user = new UserInfoModel { Id = 1, Client = client };
                var questionnaire = new UserQuestionnaireModel { UserId = user.Id };
                _mockDb.Setup(m => m.Users)
                    .Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> { user }));
                // Act
                _service.SubmitUserQuestionnaire(questionnaire);
            } catch {
                // Assert
                _mockDb.Verify(m => m.UserQuestionnaires.Add(It.IsAny<UserQuestionnaireModel>()), Times.Never,
                    "Expected NO questionnare to be added");
                _mockDb.Verify(m => m.SaveChanges(), Times.Never, 
                    "Expected NO db changes to be saved");
                _mockMessageService.Verify(m =>
                    m.SendQuestionnaireToClient(It.IsAny<UserQuestionnaireModel>()), Times.Never,
                    "Expected NO message sent to client");
            }
        }

        [TestMethod]
        public void RegisterUser_WithInvalidPhoneNumber() {
            // Arrange
            Exception expectedException = null;
            _mockPhoneNumberHelper.Setup(m => m.IsValidMobileNumber(It.IsAny<string>())).Returns(false);
            // Act
            try {
                _service.RegisterUser(new UserInfoModel { ClientId = 1, PhoneNumber = "+447654321" });
            } catch(Exception ex) {
                expectedException = ex;
            }
            // Assert
            Assert.IsNotNull(expectedException, "Expected exception to be thrown");
            Assert.AreEqual(typeof(ArgumentOutOfRangeException), expectedException.GetType());
            _mockDb.Verify(m => m.SaveChanges(), Times.Never,
                "Expected NO db changes to be saved");
        }

        [TestMethod]
        public void RegisterUser_WithValidPhoneNumber_WhenNoUserExists() {
            // Arrange
            Exception expectedException = null;
            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel>())); // No users
            _mockPhoneNumberHelper.Setup(m => m.IsValidMobileNumber(It.IsAny<string>())).Returns(true);
            // Act
            try {
                _service.RegisterUser(new UserInfoModel { ClientId = 1, PhoneNumber = "447654321" });
            } catch (Exception ex) {
                expectedException = ex;
            }
            // Assert
            Assert.IsNotNull(expectedException, "Expected exception to be thrown");
            Assert.AreEqual(typeof(ArgumentNullException), expectedException.GetType());
            _mockDb.Verify(m => m.SaveChanges(), Times.Never,
                "Expected NO db changes to be saved");
        }

        [TestMethod]
        public void RegisterUser_WhenClientIsDeleted() {
            // Arrange
            Exception expectedException = null;
            var userToRegister = new UserInfoModel { ClientId = 1, PhoneNumber = "447654321" };
            var existingUser = new UserInfoModel {
                Id = 1, ClientId = 1, Client = new ClientModel {  Id = 1, IsDeleted = true },
                FirstName = "Fname1", LastName = "Lname1", PhoneNumber = "447654321"
            };
            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> { existingUser }));
            _mockPhoneNumberHelper.Setup(m => m.IsValidMobileNumber(It.IsAny<string>())).Returns(true);
            // Act
            try {
                _service.RegisterUser(new UserInfoModel { ClientId = 1, PhoneNumber = "447654321" });
            } catch (Exception ex) {
                expectedException = ex;
            }
            // Assert
            Assert.IsNotNull(expectedException, "Expected exception to be thrown");
            Assert.AreEqual(typeof(ArgumentException), expectedException.GetType());
            _mockDb.Verify(m => m.SaveChanges(), Times.Never,
                "Expected NO db changes to be saved");
        }

        [TestMethod]
        public void RegisterUser_WithValidPhoneNumber_WhenUserExists() {
            // Arrange
            var userToRegister = new UserInfoModel { ClientId = 1, PhoneNumber = "447654321" };
            var existingUser = new UserInfoModel {
                Id = 1, ClientId = 1, Client = new ClientModel { Id = 1, IsDeleted = false },
                FirstName = "Fname1", LastName = "Lname1", PhoneNumber = "447654321" };
            _mockDb.Setup(m => m.Users)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<UserInfoModel> { existingUser }));
            _mockPhoneNumberHelper.Setup(m => m.IsValidMobileNumber(It.IsAny<string>())).Returns(true);
            // Act
            _service.RegisterUser(userToRegister);
            // Assert            
            Assert.IsTrue(existingUser.RegisteredDate > DateTime.MinValue, "Expected RegisteredDate to be set");
            Assert.AreEqual(existingUser.Id, userToRegister.Id, "Expected Id to match");
            Assert.AreEqual(existingUser.ClientId, userToRegister.ClientId, "Expected ClientId to match");
            Assert.AreEqual(existingUser.FirstName, userToRegister.FirstName, "Expected FirstName to match");
            Assert.AreEqual(existingUser.LastName, userToRegister.LastName, "Expected LastName to match");
            Assert.AreEqual(existingUser.PhoneNumber, userToRegister.PhoneNumber, "Expected PhoneNumber to match");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once,
                "Expected db changes to be saved");
        }
    }
}
