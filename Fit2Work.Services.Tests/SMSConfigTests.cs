using AnvilGroup.Services.Fit2Work.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fit2Work.Services.Tests {
    [TestClass]
    public class SMSConfigTests {
        [TestMethod]
        public void SMSConfig_Construct() {
            // Arrange
            var configValue = "44|12345|API_KEY|API_PASSWORD|false";
            // Act
            var result = new SMSConfig(configValue);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(44, result.CountryCode);
            Assert.AreEqual("12345", result.AccountId);
            Assert.AreEqual("API_KEY", result.ApiKey);
            Assert.AreEqual("API_PASSWORD", result.ApiPassword);
            Assert.IsFalse(result.IsDefault);
        }

        [TestMethod]
        public void SMSConfig_IsDefault() {
            // Arrange
            var configValue = "44|12345|API_KEY|API_PASSWORD|true";
            // Act
            var result = new SMSConfig(configValue);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsDefault);
        }
    }
}
