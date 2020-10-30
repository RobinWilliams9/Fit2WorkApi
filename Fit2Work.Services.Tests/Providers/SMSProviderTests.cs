using System.Collections.Generic;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2Work.Services.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fit2Work.Services.Tests.Providers {
    [TestClass]
    public class SMSProviderTests {
        ISmsProvider _smsProvider;
        Mock<IConfigurationProvider> _mockConfig = new Mock<IConfigurationProvider>();

        [TestInitialize]
        public void Initialise() {
            _smsProvider = new SmsProvider(_mockConfig.Object);
        }

        [TestMethod]
        [Ignore("To do.  Need to mock inputs otherwise this will fail.")]
        public void SendSMS() {
            // Arrange
            var recipient = "4412345678";
            var message = "This is a message";
            // Act
            var result = _smsProvider.SendSMS(recipient, message);
            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(recipient, result.Recipient);
            Assert.AreEqual(message, result.MessageContent);
        }

        [TestMethod]
        public void GetSMSConfigForRecipient_WithNoCountryCode_GetsDefault() {
            // Arrange
            _mockConfig.Setup(m => m.SMSConfigs).Returns(new List<SMSConfig> {
                new SMSConfig { CountryCode = 1, IsDefault = true },
                new SMSConfig { CountryCode = 2 },
            });
            // Act
            _smsProvider = new SmsProvider(_mockConfig.Object);
            var result = _smsProvider.GetSMSConfigForRecipient("4412345678");
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsDefault);
        }

        [TestMethod]
        public void GetSMSConfigForRecipient_WithUKCountryCode() {
            // Arrange
            _mockConfig.Setup(m => m.SMSConfigs).Returns(new List<SMSConfig> {
                new SMSConfig { CountryCode = 1, IsDefault = true },
                new SMSConfig { CountryCode = 44 },
            });
            // Act
            _smsProvider = new SmsProvider(_mockConfig.Object);
            var result = _smsProvider.GetSMSConfigForRecipient("447937985383");
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(44, result.CountryCode);
        }

        [TestMethod]
        public void GetSMSConfigForRecipient_WithUSCountryCode() {
            // Arrange
            _mockConfig.Setup(m => m.SMSConfigs).Returns(new List<SMSConfig> {
                new SMSConfig { CountryCode = 44, IsDefault = true },
                new SMSConfig { CountryCode = 1 },
            });
            // Act
            _smsProvider = new SmsProvider(_mockConfig.Object);
            var result = _smsProvider.GetSMSConfigForRecipient("16466613630");
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CountryCode);
        }
    }
}
