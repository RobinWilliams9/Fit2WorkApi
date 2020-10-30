using System;
using AnvilGroup.Library.Encryption;
using Fit2Work.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnvilGroup.Services.Fit2WorkApi.Tests {
    [TestClass]
    public class GenerateToken {
        // Method to generate a valid token for the API authentication (valid for 30 minutes)
        [TestMethod]
        public void Generate() {
            // Arrange
            var clientId = 1;                   // Anvilgroup client
            var phoneNumber = "447003002001";   // Test user
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            // Act
            var token = new Cryptography().EncryptString(TestHelper.EncryptionKey,
                $"{clientId}#{phoneNumber}#{timestamp}");
            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(token), "Expected token to be generated");
        }
    }
}
