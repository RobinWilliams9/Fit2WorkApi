using System;
using System.Collections.Generic;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services;
using Fit2Work.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fit2Work.Services.Tests {
    [TestClass]
    public class ClientServiceTests {
        IClientService _service;
        Mock<IFit2WorkDb> _mockDb = new Mock<IFit2WorkDb>();

        [TestInitialize]
        public void Initialise() {
            _service = new ClientService(_mockDb.Object);
        }

        [TestMethod]
        public void GetClientByCode_Success() {
            // Arrange
            var expectedClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1" };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(
                    new List<ClientModel> {
                        new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1" },
                        new ClientModel { Id = 2, Name = "Client2", MemberCode = "client2" }
                    }));
            // Act
            var actualClient = _service.GetClientByCode(expectedClient.MemberCode);
            // Assert
            Assert.IsNotNull(actualClient);
            Assert.AreEqual(expectedClient.Id, actualClient.Id, "Expected client ids to match");
            Assert.AreEqual(expectedClient.Name, actualClient.Name, "Expected client names to match");
            Assert.AreEqual(expectedClient.MemberCode, actualClient.MemberCode, "Expected client codes to match");
            _mockDb.Verify(m => m.SaveChanges(), Times.Never, "Expected no changes to the db");
        }

        [TestMethod]
        public void GetClientById_Success() {
            // Arrange
            var expectedClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1" };
            _mockDb.Setup(m => m.Clients).Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> {
                new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1" },
                new ClientModel { Id = 2, Name = "Client2", MemberCode = "client2" }
            }));
            // Act
            var actualClient = _service.GetClientById(expectedClient.Id);
            // Assert
            Assert.IsNotNull(actualClient);
            Assert.AreEqual(expectedClient.Id, actualClient.Id, "Expected client ids to match");
            Assert.AreEqual(expectedClient.Name, actualClient.Name, "Expected client names to match");
            Assert.AreEqual(expectedClient.MemberCode, actualClient.MemberCode, "Expected client codes to match");
            _mockDb.Verify(m => m.SaveChanges(), Times.Never, "Expected no changes to the db");
        }

        [TestMethod]
        public void GetClientByCode_WhenClientNotFound_ReturnsNull() {
            // Arrange
            _mockDb.Setup(m => m.Clients).Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> {
                new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1" },
                new ClientModel { Id = 2, Name = "Client2", MemberCode = "client2" }
            }));
            // Act
            var actualClient = _service.GetClientByCode("client3");
            // Assert
            Assert.IsNull(actualClient);
            _mockDb.Verify(m => m.SaveChanges(), Times.Never, "Expected no changes to the db");
        }

        [TestMethod]
        public void CreateClient_Success() {
            // Arrange
            _mockDb.Setup(m => m.Clients).Returns(
                TestHelper.GetQueryableMockDbSet(new List<ClientModel> {
                    new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1" },
                    new ClientModel { Id = 2, Name = "Client2", MemberCode = "client2" }
                }));
            var newClient = new ClientModel { Name = "Client3", MemberCode = "client3" };
            // Act
            _service.CreateClient(newClient);
            // Assert
            _mockDb.Verify(m => m.Clients.Add(It.IsAny<ClientModel>()), Times.Once, "Expected client to be added");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
            Assert.IsFalse(newClient.IsDeleted, "Expected client to be active");
            Assert.IsTrue(newClient.CreatedDate >= DateTime.MinValue, "Expected created date to be set");
        }

        [TestMethod]
        public void ActivateClient_Success() {
            // Arrange
            var existingClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1", IsDeleted = true };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { existingClient }));
            // Act
            _service.ActivateClient(existingClient.Id);
            // Assert
            Assert.IsFalse(existingClient.IsDeleted, "Expected client to be active");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
        }

        [TestMethod]
        public void ActivateClient_WhenClientNotFound_ThrowsException() {
            // Arrange
            var existingClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1", IsDeleted = true };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { existingClient }));
            try {
                // Act
                _service.ActivateClient(2);
            } catch (Exception ex) {
                // Assert
                Assert.AreEqual("Client (2) not found.", ex.Message);
                Assert.IsTrue(existingClient.IsDeleted, "Expected client to remain inactive");
                _mockDb.Verify(m => m.SaveChanges(), Times.Never, "Expected NO db changes to be saved");
            }

        }

        [TestMethod]
        public void DeactivateClient_Success() {
            // Arrange
            var existingClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1", IsDeleted = false };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { existingClient }));
            // Act
            _service.DeactivateClient(existingClient.Id);
            // Assert
            Assert.IsTrue(existingClient.IsDeleted, "Expected client to be inactive");
            _mockDb.Verify(m => m.SaveChanges(), Times.Once, "Expected db changes to be saved");
        }

        [TestMethod]
        public void DeactivateClient_WhenClientNotFound_ThrowsException() {
            // Arrange
            var existingClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1", IsDeleted = false };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { existingClient }));
            try {
                // Act
                _service.DeactivateClient(2);
            } catch (Exception ex) {
                // Assert
                Assert.AreEqual("Client (2) not found.", ex.Message);
                Assert.IsFalse(existingClient.IsDeleted, "Expected client remain active");
                _mockDb.Verify(m => m.SaveChanges(), Times.Never, "Expected NO db changes to be saved");
            }
        }

        [TestMethod]
        public void IsMemberCodeUnique_WhenNewClient_True() {
            // Arrange
            var existingClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1", IsDeleted = false };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { existingClient }));
            // Act
            var result = _service.IsMemberCodeUnique(new ClientModel { MemberCode = "client2" });
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMemberCodeUnique_WhenNewClient_False() {
            // Arrange
            var existingClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1", IsDeleted = false };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { existingClient }));
            // Act
            var result = _service.IsMemberCodeUnique(new ClientModel { MemberCode = "client1" });
            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsMemberCodeUnique_WhenExistingClient_True() {
            // Arrange
            var existingClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1", IsDeleted = false };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { existingClient }));
            // Act
            var result = _service.IsMemberCodeUnique(new ClientModel { Id = 1, MemberCode = "client1" });
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsMemberCodeUnique_WhenExistingClient_False() {
            // Arrange
            var existingClient = new ClientModel { Id = 1, Name = "Client1", MemberCode = "client1", IsDeleted = false };
            _mockDb.Setup(m => m.Clients)
                .Returns(TestHelper.GetQueryableMockDbSet(new List<ClientModel> { existingClient }));
            // Act
            var result = _service.IsMemberCodeUnique(new ClientModel { Id = 2, MemberCode = "client1" });
            // Assert
            Assert.IsFalse(result);
        }
    }
}
