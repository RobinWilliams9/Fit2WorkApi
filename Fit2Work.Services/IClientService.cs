using System.Collections.Generic;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Services {
    public interface IClientService {
        ClientModel GetClientByCode(string code);
        void ActivateClient(int clientId);
        void CreateClient(ClientModel newClient);
        void DeactivateClient(int clientId);
        ClientModel GetClientById(int id);
        void UpdateClient(int id, ClientModel client);
        List<ClientModel> GetClients();
        bool IsMemberCodeUnique(ClientModel client);
    }
}