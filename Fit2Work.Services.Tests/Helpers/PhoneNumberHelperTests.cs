using AnvilGroup.Services.Fit2Work.Services.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fit2Work.Services.Tests.Helpers {
    [TestClass]
    public class PhoneNumberHelperTests {
        IPhoneNumberHelper _helper = new PhoneNumberHelper();
        [TestMethod]
        public void IsValidPhoneNumber_WithCountryCode_True() {
            // Arrange
            var number = "447001002003";
            // Act
            var result = _helper.IsValidMobileNumber(number);
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidPhoneNumber_WithoutCountryCode_False() {
            // Arrange
            var number = "07001002003";
            // Act
            var result = _helper.IsValidMobileNumber(number);
            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidPhoneNumber_WithPlusCountryCode_False() {
            // Arrange
            var number = "+447001002003";
            // Act
            var result = _helper.IsValidMobileNumber(number);
            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidPhoneNumber_With00CountryCode_False() {
            // Arrange
            var number = "00447001002003";
            // Act
            var result = _helper.IsValidMobileNumber(number);
            // Assert
            Assert.IsFalse(result);
        }
    }
}
