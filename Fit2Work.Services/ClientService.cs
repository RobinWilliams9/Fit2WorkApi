using System;
using System.Collections.Generic;
using System.Linq;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Services {
    public class ClientService : BaseService, IClientService {
        public ClientService() { }
        public ClientService(IFit2WorkDb fit2WorkDb) : base(fit2WorkDb) { }
        public ClientModel GetClientByCode(string code) {
            return Fit2WorkDb.Clients.FirstOrDefault(c =>
                c.MemberCode.ToLower().Equals(code.ToLower()));
        }
        public bool IsMemberCodeUnique(ClientModel client) {
            if(client.Id > 0) { // existing client
                return !Fit2WorkDb.Clients.Any(c =>
                c.Id != client.Id &&
                c.MemberCode.ToUpper().Equals(client.MemberCode.ToUpper()));
            } else { // new client
                return !Fit2WorkDb.Clients.Any(c =>
                c.MemberCode.ToUpper().Equals(client.MemberCode.ToUpper()));
            }            
        }

        public void CreateClient(ClientModel newClient) {            
            newClient.IsDeleted = false;
            newClient.CreatedDate = DateTime.UtcNow;
            Fit2WorkDb.Clients.Add(newClient);
            Fit2WorkDb.SaveChanges();
        }

        public void ActivateClient(int clientId) {
            var client = Fit2WorkDb.Clients.FirstOrDefault(c => c.Id == clientId);
            if(client == null) {
                throw new ApplicationException($"Client ({clientId}) not found.");
            }
            client.IsDeleted = false;
            Fit2WorkDb.SaveChanges();
        }

        public void DeactivateClient(int clientId) {
            var client = Fit2WorkDb.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client == null) {
                throw new ApplicationException($"Client ({clientId}) not found.");
            }
            client.IsDeleted = true;
            Fit2WorkDb.SaveChanges();
        }

        public ClientModel GetClientById(int id) {
            return Fit2WorkDb.Clients.FirstOrDefault(c =>
                c.Id == id);
        }

        public void UpdateClient(int id, ClientModel client) {
            var existingClient = GetClientById(id);
            if(existingClient == null) {
                throw new ApplicationException($"Unable to find client ({id})");
            }
            // Check code is unique for the client
            if (Fit2WorkDb.Clients.Any(c =>
                c.Id != id &&
                c.MemberCode.ToLower().Equals(client.MemberCode.ToLower()))) {
                throw new ApplicationException(
                    $"Client code ({client.MemberCode}) is not unique.");
            }
            existingClient.Name = client.Name;
            existingClient.MemberCode = client.MemberCode;
            existingClient.PrimaryEmailAddress = client.PrimaryEmailAddress;
            existingClient.SecondaryEmailAddress = client.SecondaryEmailAddress;
            existingClient.IsDeleted = client.IsDeleted;
            existingClient.UpdatedDate = DateTime.UtcNow;
            Fit2WorkDb.SaveChanges();
        }

        public List<ClientModel> GetClients() {
            return Fit2WorkDb.Clients
                .OrderByDescending(c => c.CreatedDate)
                .ToList();
        }
    }
}