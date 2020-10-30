using System;
using System.Collections.Generic;
using System.Linq;
using AnvilGroup.Library.Comms.Processor.SMS.Models;
using AnvilGroup.Library.Comms.Processor.SMS.Providers;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services.Helpers;

namespace AnvilGroup.Services.Fit2Work.Services.Providers {
    public class SmsProvider : ISmsProvider {
        private readonly ISMSProvider _smsProvider;
        private readonly IConfigurationProvider _configurationProvider;
        private List<SMSConfig> _smsConfigs;

        public SmsProvider(IConfigurationProvider configurationProvider) {
            _smsProvider = new SMSProviderService().GetSMSProvider(ProviderType.WorldText);
            _configurationProvider = configurationProvider;
            _smsConfigs = configurationProvider.SMSConfigs;
        }
        public MessageResult SendSMS(string recipient, string messageText) {
            var result = new MessageResult();                      
            try {
                // Get the sms config for the recipient
                var smsConfig = GetSMSConfigForRecipient(recipient);
                //// Set up config and routing for SMS
                var smsConfiguration = new SMSConfiguration
                {
                    SMSProviderID = 2, // World text only
                    Password = smsConfig.ApiPassword
                };
                var smsRouting = new SMSRouting
                {
                    AccountID = smsConfig.AccountId,
                    ApiKey = smsConfig.ApiKey
                };
                var smsResult = _smsProvider.SendSMS(
                    new SMSMessageInfo(smsConfiguration, smsRouting, messageText, recipient, false, false));
                result.IsOk = smsResult.IsOk;
                result.Exception = new ApplicationException(smsResult.ErrorMessage);
            } catch (Exception ex) {
                result.IsOk = false;
                result.Exception = ex;
            }
            return result;
        }

        public SMSConfig GetSMSConfigForRecipient(string recipient) {
            var recipientCountryCode = new PhoneNumberHelper().GetCountryCode(recipient);
            var defaultSmsConfig = _smsConfigs.FirstOrDefault(sms => sms.IsDefault);
            var smsConfig = _smsConfigs.FirstOrDefault(sms => sms.CountryCode == recipientCountryCode);
            if(smsConfig == null) {
                return defaultSmsConfig;
            }
            return smsConfig;            
        }
    }
}
