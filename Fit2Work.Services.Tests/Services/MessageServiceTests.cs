using System;
using System.Collections.Generic;
using System.Linq;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2Work.Services.Providers;
using Fit2Work.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fit2Work.Services.Tests {
    [TestClass]
    public class MessageServiceTests {

        const string TestErrorMessage = "Error Message";
        const string TestFromEmail = "noreply @test.ing";
        const string TestMessageText = "Test SMS from {clientName} : {memberCode}";
        const string TestDownloadLinkText = "iPhone users click here {AppleAppStoreUrl} Android users click here {GooglePlayStoreUrl}";

        TestMailMessage _mailMsg = new TestMailMessage();
        readonly UserQuestionnaireModel _mockQ = new UserQuestionnaireModel {            
            User = new UserInfoModel { FirstName = "User1", LastName = "Test1", PhoneNumber = "7654321" },
            Client = new ClientModel {
                Id = 1,
                PrimaryEmailAddress = "primary@test.ing",
                SecondaryEmailAddress = "secondary@test.ing"
            }
        };

        IMessageService _service;
        Mock<IFit2WorkDb> _mockDb = new Mock<IFit2WorkDb>();
        Mock<IUserService> _mockUserService = new Mock<IUserService>();
        Mock<ISmsProvider> _mockSms = new Mock<ISmsProvider>();
        Mock<IEmailProvider> _mockEmail = new Mock<IEmailProvider>();
        Mock<IConfigurationProvider> _mockConfig = new Mock<IConfigurationProvider>();
        Mock<IResourceProvider> _mockRes = new Mock<IResourceProvider>();

        [TestInitialize]
        public void Initialise() {
            _mockConfig.Setup(m => m.ClientEmailFromAddress).Returns(TestFromEmail);
            _mockRes.Setup(m => m.EmailHeader).Returns("<HEADER/>");
            _mockRes.Setup(m => m.EmailFooter).Returns("<FOOTER/>");
            _mockRes.Setup(m => m.PrimaryEmailSubjectFormat).Returns("Primary {firstName} {lastName}");
            _mockRes.Setup(m => m.PrimaryEmailBodyFormat).Returns(
                "<HTML>{header} | Primary {firstName} {lastName} {phoneNumber} : {questionnaireResults} | {footer}</HTML>");
            _mockRes.Setup(m => m.SecondaryEmailSubjectFormat).Returns("Secondary {firstName} {lastName}");
            _mockRes.Setup(m => m.SecondaryEmailBodyFormat).Returns(
                "<HTML>{header} | Secondary {firstName} {lastName} {phoneNumber} : {questionnaireResults} | {footer}</HTML>");
            _mockRes.Setup(m => m.UserMessageText).Returns(TestMessageText);
            _mockRes.Setup(m => m.DownloadMessageText).Returns(TestDownloadLinkText);

            var resourceUrlModels = new List<ResourceUrlModel>();
            resourceUrlModels.Add(new ResourceUrlModel() { Name = "{AppleAppStoreUrl}", Url = "https://apple.co/3hVrB1V" });
            resourceUrlModels.Add(new ResourceUrlModel() { Name = "{GooglePlayStoreUrl}", Url = "https://bit.ly/3hJAJXg" });

            _mockRes.Setup(m => m.ResourceUrls).Returns(resourceUrlModels);
            _service = new MessageService(
                _mockDb.Object, 
                _mockConfig.Object, 
                _mockRes.Object, 
                _mockSms.Object, 
                _mockEmail.Object);
        }

        [TestMethod]
        public void SendMessageToClientUsers_Success() {
            // Arrange
            List<TestSMS> smsList = new List<TestSMS>();
            List<string> progressList = new List<string>();
            Action<string> progress = delegate (string message) {
                if (!message.Contains("Sending SMS")) { // ignore start and end messages
                    progressList.Add(message);
                }
            };

            var client = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1" };
            var users = new List<UserInfoModel> {
                new UserInfoModel { Id = 1, FirstName = "User1", PhoneNumber = "4477123451", Client = client },
                new UserInfoModel { Id = 2, FirstName = "User2", PhoneNumber = "4477123452", Client = client }
            };

            _mockDb.Setup(m => m.Clients).Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { client }));
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(users));
            _mockSms.Setup(m => m.SendSMS(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((recipient, message) => {
                    smsList.Add(new TestSMS { Recipient = recipient, Message = message });
                })
                .Returns(new MessageResult());

            const string expectedMemberCodeMessage = "Test SMS from Client1 : client1";
            const string expectedDownloadLinkMessage = "iPhone users click here https://apple.co/3hVrB1V Android users click here https://bit.ly/3hJAJXg";

            // Act
            _service.SendMessageToClientUsers(client, users, progress);

            // Assert
            Assert.AreEqual(2, progressList.Count, "Expected progress for each user");
            //Assert.AreEqual(users.Count, smsList.Count, "Expected SMS for each user");

            _mockSms.Verify(m => m.SendSMS(It.IsAny<string>(), It.Is<string>(x => x.Equals(expectedMemberCodeMessage))), Times.Exactly(2));
            _mockSms.Verify(m => m.SendSMS(It.IsAny<string>(), It.Is<string>(x => x.Equals(expectedDownloadLinkMessage))), Times.Exactly(2));
            _mockDb.Verify(m => m.SaveChanges(), Times.Exactly(2), "Expected all users to be marked as sms sent");
        }        

        [TestMethod]
        public void SendMessageToClientUsers_WhenSomeMessagesFail_ReturnsResult() {
            // Arrange
            var client = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1" };
            var users = new List<UserInfoModel> {
                new UserInfoModel { Id = 1, FirstName = "User1", PhoneNumber = "4477123451", Client = client },
                new UserInfoModel { Id = 2, FirstName = "User2", PhoneNumber = "4477123452", Client = client }
            };
            List<string> progressList = new List<string>();
            Action<string> progress = delegate (string message) {
                if (!message.Contains("Sending SMS")) { // ignore start and end messages
                    progressList.Add(message);
                }
            };
            _mockDb.Setup(m => m.Clients).Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { client }));
            _mockDb.Setup(m => m.Users).Returns(TestHelper.GetQueryableMockDbSet(users));
            _mockSms.Setup(m => m.SendSMS("4477123451", It.IsAny<string>()))
                .Returns(new MessageResult());
            _mockSms.Setup(m => m.SendSMS("4477123452", It.IsAny<string>()))
                .Returns(new MessageResult { IsOk = false, Exception = new ApplicationException(TestErrorMessage) });
            // Act
            _service.SendMessageToClientUsers(client, users, progress);
            // Assert
            Assert.AreEqual(2, progressList.Count, "Expected result for each user");
            Assert.IsTrue(progressList[0].Contains("SUCCESS"), "Expected 1st result to be SENT");
            Assert.IsTrue(progressList[1].Contains("FAILED"), "Expected 2nd result to be FAILED");
            _mockDb.Verify(m => m.SaveChanges(), Times.Exactly(1),
                "Expected 1 user to be marked as sms sent");
            Assert.IsTrue(progressList[1].Contains(TestErrorMessage), "Expected 2nd result to contain TestErrorMessage");
        }

        [TestMethod]
        public void SendMessageToClientUsers_WhenAllMessagesFail_ReturnsResult() {
            // Arrange
            var client = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1" };
            var users = new List<UserInfoModel> {
                new UserInfoModel { Id = 1, FirstName = "User1", PhoneNumber = "4477123451", Client = client },
                new UserInfoModel { Id = 2, FirstName = "User2", PhoneNumber = "4477123452", Client = client }
            };
            List<string> progressList = new List<string>();
            Action<string> progress = delegate (string message) {
                if (!message.Contains("Sending SMS")) { // ignore start and end messages
                    progressList.Add(message);
                }
            };
            _mockDb.Setup(m => m.Clients).Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { client }));            
            _mockSms.Setup(m => m.SendSMS(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new MessageResult { IsOk = false, Exception = new ApplicationException(TestErrorMessage) });
            // Act
            var results = _service.SendMessageToClientUsers(client, users, progress);
            // Assert
            Assert.AreEqual(2, progressList.Count, "Expected result for each user");
            Assert.IsTrue(progressList.All(r => r.Contains("FAILED")), "Expected all results to be FAILED");
            _mockDb.Verify(m => m.SaveChanges(), Times.Never,
                "Expected no users to be marked as sms sent");
            Assert.IsTrue(progressList.All(r => r.Contains(TestErrorMessage)), "Expected all results to be TestErrorMessage");
        }

        [TestMethod]
        public void SendQuestionnaireToClient_WhenFit2WorkIsTrue_Success() {
            // Arrange
            _mockQ.QuestionsAndAnswers.Add(new UserAnswerModel { Question = "Question1", Answer = "No" });
            // Capture the email message details for asserting against as well as returning success result     
            _mockEmail.Setup(m => m.SendMail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string, string>((from, to, subject, body) => {
                    _mailMsg.From = from;
                    _mailMsg.To = to;
                    _mailMsg.Subject = subject;
                    _mailMsg.Body = body;
                })
                .Returns(new MessageResult());
            // Act
            var result = _service.SendQuestionnaireToClient(_mockQ);
            // Assert
            Assert.AreEqual(TestFromEmail, _mailMsg.From);
            Assert.AreEqual(_mockQ.Client.PrimaryEmailAddress, _mailMsg.To);
            Assert.AreEqual("Primary User1 Test1", _mailMsg.Subject);
            Assert.AreEqual("<HTML><HEADER/> | Primary User1 Test1 7654321 : Question1: No | <FOOTER/></HTML>", _mailMsg.Body);
            Assert.AreEqual(_mailMsg.Body, result.MessageContent, "Expected message body to be returned");
            _mockEmail.Verify(m => m.SendMail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once,
                "Expected email to be sent");
        }

        [TestMethod]
        public void SendQuestionnaireToClient_WhenFit2WorkIsFalse_Success() {
            // Arrange
            _mockQ.QuestionsAndAnswers.Add(new UserAnswerModel { Question = "Question1", Answer = "Yes" });
            // Capture the email message details for asserting against as well as returning success result     
            _mockEmail.Setup(m => m.SendMail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string, string>((from, to, subject, body) => {
                    _mailMsg.From = from;
                    _mailMsg.To = to;
                    _mailMsg.Subject = subject;
                    _mailMsg.Body = body;
                })
                .Returns(new MessageResult());
            // Act
            var result = _service.SendQuestionnaireToClient(_mockQ);
            // Assert
            Assert.AreEqual(TestFromEmail, _mailMsg.From);
            Assert.AreEqual(_mockQ.Client.SecondaryEmailAddress, _mailMsg.To);
            Assert.AreEqual("Secondary User1 Test1", _mailMsg.Subject);
            Assert.AreEqual("<HTML><HEADER/> | Secondary User1 Test1 7654321 : Question1: Yes | <FOOTER/></HTML>", _mailMsg.Body);
            Assert.AreEqual(_mailMsg.Body, result.MessageContent, "Expected message body to be returned");
            _mockEmail.Verify(m => m.SendMail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Once,
                "Expected email to be sent");
            
        }

        private void AssertAllMessages(string expectedMessage, List<UserInfoModel> users, List<TestSMS> smsList) {
            foreach (var user in users) {
                var sms = smsList.SingleOrDefault(s => s.Recipient.Equals(user.PhoneNumber));
                Assert.AreEqual(expectedMessage, sms.Message, $"Expected SMS message to match for recipient {user.PhoneNumber}");                
            }
        }
    }
}
