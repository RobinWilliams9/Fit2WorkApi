using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Services.Providers {
    public interface ISmsProvider {
        MessageResult SendSMS(string recipient, string messageText);
        SMSConfig GetSMSConfigForRecipient(string recipient);
    }
}