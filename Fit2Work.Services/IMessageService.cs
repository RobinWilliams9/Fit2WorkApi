using System;
using System.Collections.Generic;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Services {
    public interface IMessageService {
        IList<MessageResult> SendMessageToClientUsers(ClientModel client, List<UserInfoModel> users, Action<string> progress);
        MessageResult SendSms(string recipient, string messageText);
        MessageResult SendQuestionnaireToClient(UserQuestionnaireModel questionnaire);
        string PopulatePlaceHoldersSms(ClientModel client);
        string GenerateDownLinksSMSText();
    }
}