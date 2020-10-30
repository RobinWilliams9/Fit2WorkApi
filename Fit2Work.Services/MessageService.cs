using System;
using System.Collections.Generic;
using System.Linq;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services.Providers;


namespace AnvilGroup.Services.Fit2Work.Services {
    public class MessageService : BaseService, IMessageService {
        private readonly ISmsProvider _smsProvider;
        private readonly IEmailProvider _emailProvider;
        private readonly IResourceProvider _resourceProvider;
        private readonly IConfigurationProvider _configurationProvider;

        private readonly IList<string> _placeHolders = new List<string>() {
            HeaderPlaceholder,
            FooterPlaceholder,
            ClientNamePlaceholder,
            MemberCodePlaceholder,
            FirstNamePlaceholder,
            LastNamePlaceholder,
            PhoneNumberPlaceholder,
            QuestionnairePlaceholder,
            AppleAppStoreUrlPlaceholder,
            GooglePlayStoreUrlPlaceholder
        };

        private const string HeaderPlaceholder = "{header}";
        private const string FooterPlaceholder = "{footer}";
        private const string ClientNamePlaceholder = "{clientName}";
        private const string MemberCodePlaceholder = "{memberCode}";
        private const string FirstNamePlaceholder = "{firstName}";
        private const string LastNamePlaceholder = "{lastName}";
        private const string PhoneNumberPlaceholder = "{phoneNumber}";
        private const string QuestionnairePlaceholder = "{questionnaireResults}";
        private const string AppleAppStoreUrlPlaceholder = "{AppleAppStoreUrl}";
        private const string GooglePlayStoreUrlPlaceholder = "{GooglePlayStoreUrl}";

        public MessageService() {
            _configurationProvider = new ConfigurationProvider();
            _resourceProvider = new ResourceProvider(Fit2WorkDb);
            _smsProvider = new SmsProvider(_configurationProvider);
            _emailProvider = new EmailProvider();
        }
        public MessageService(IFit2WorkDb fit2WorkDb) 
            : base(fit2WorkDb) {
            _configurationProvider = new ConfigurationProvider();
            _resourceProvider = new ResourceProvider(Fit2WorkDb);
            _smsProvider = new SmsProvider(_configurationProvider);
            _emailProvider = new EmailProvider();
        }
        public MessageService(IFit2WorkDb fit2WorkDb,
            IConfigurationProvider configurationProvider,
            IResourceProvider resourceProvider,
            ISmsProvider smsProvider,
            IEmailProvider emailProvider) : base(fit2WorkDb) {
            _configurationProvider = configurationProvider;
            _resourceProvider = resourceProvider;
            _smsProvider = smsProvider;
            _emailProvider = emailProvider;
        }
       
        public IList<MessageResult> SendMessageToClientUsers(
            ClientModel client,
            List<UserInfoModel> userList,
            Action<string> progress) {
            var results = new List<MessageResult>();
            var messageText = PopulatePlaceHoldersForClient(
                _resourceProvider.UserMessageText, client);

            var downloadMessageText = PopulatePlaceHoldersForDownloadLinks(_resourceProvider.DownloadMessageText);

            progress.Invoke($"Sending SMS :: {messageText} and {downloadMessageText}");
            foreach (var user in userList) {
                try {
                    var result = _smsProvider.SendSMS(
                        user.PhoneNumber, messageText);
                    progress.Invoke($"{(result.IsOk ? "SUCCESS" : "FAILED")} :: SMS to {user.PhoneNumber}. " +
                        $"{(result.Exception == null ? string.Empty : result.Exception.Message)}");

                    _smsProvider.SendSMS(user.PhoneNumber, downloadMessageText);

                    if (result.IsOk) {
                        var dbUser = Fit2WorkDb.Users.SingleOrDefault(u => u.Id == user.Id);
                        dbUser.SmsSentDate = DateTime.UtcNow;
                        Fit2WorkDb.SaveChanges();
                    }
                } catch (Exception ex) {
                    progress.Invoke($"Failed to send SMS to {user.PhoneNumber} : {ex.Message}");
                }
            }            
            progress.Invoke($"Sending SMS complete!");
            return results;
        }

        public MessageResult SendSms(string recipient, string messageText) {
            MessageResult result;
            try {
                result = _smsProvider.SendSMS(recipient, messageText);                
            } catch (Exception ex) {
                result = new MessageResult {
                    IsOk = false,
                    Exception = ex,
                    Recipient = recipient
                };
            }
            return result;
        }

        public MessageResult SendQuestionnaireToClient(UserQuestionnaireModel questionnaire) {
            var user = questionnaire.User;
            string from = _configurationProvider.ClientEmailFromAddress;
            string to = questionnaire.IsFit2Work ?
                questionnaire.Client.PrimaryEmailAddress :
                questionnaire.Client.SecondaryEmailAddress;
            string subject = PopulatePlaceHoldersForQuestionnaire(
                questionnaire.IsFit2Work ?
                    _resourceProvider.PrimaryEmailSubjectFormat :
                    _resourceProvider.SecondaryEmailSubjectFormat,
                questionnaire);
            string body = PopulatePlaceHoldersForQuestionnaire(
                questionnaire.IsFit2Work ?
                    _resourceProvider.PrimaryEmailBodyFormat :
                    _resourceProvider.SecondaryEmailBodyFormat,
                questionnaire);
            var result = _emailProvider.SendMail(from, to, subject, body);
            result.MessageContent = body;
            return result;
        }

        public string PopulatePlaceHoldersSms(ClientModel client) {
            return PopulatePlaceHoldersForClient(_resourceProvider.UserMessageText, client);
        }

        public string GenerateDownLinksSMSText() {
            return PopulatePlaceHoldersForDownloadLinks(_resourceProvider.DownloadMessageText);
        }

        private string PopulatePlaceHoldersForQuestionnaire(string content,
            UserQuestionnaireModel questionnaire) {
            var result = content;
            foreach (var placeholder in _placeHolders) {
                if (content.Contains(placeholder)) {
                    result = result.Replace(placeholder,
                        GetPlaceHolderValueForQuestionnaire(placeholder, questionnaire));
                }
            }
            return result;
        }

        private string PopulatePlaceHoldersForClient(string content,
            ClientModel client) {
            var result = content;
            foreach (var placeholder in _placeHolders) {
                if (content.Contains(placeholder)) {
                    result = result.Replace(placeholder,
                        GetPlaceHolderValueForClient(placeholder, client));
                }
            }
            return result;
        }

        private string PopulatePlaceHoldersForDownloadLinks(string content) {
            var result = content;
            foreach (var placeholder in _placeHolders) {
                if (content.Contains(placeholder)) {
                    result = result.Replace(placeholder,
                        GetPlaceHoldersForDownloadLinks(placeholder));
                }
            }
            return result;
        }

        /// <summary>
        /// Gets placeholder values from questionnaires
        /// </summary>
        public string GetPlaceHolderValueForQuestionnaire(string placeholder, UserQuestionnaireModel questionnaire) {
            switch (placeholder) {
                case HeaderPlaceholder:
                    return _resourceProvider.EmailHeader;
                case FooterPlaceholder:
                    return _resourceProvider.EmailFooter;
                case ClientNamePlaceholder:
                    return questionnaire.Client.Name;
                case MemberCodePlaceholder:
                    return questionnaire.Client.MemberCode;
                case FirstNamePlaceholder:
                    return questionnaire.User.FirstName;
                case LastNamePlaceholder:
                    return questionnaire.User.LastName;
                case PhoneNumberPlaceholder:
                    return questionnaire.User.PhoneNumber;
                case QuestionnairePlaceholder:
                    return questionnaire.ToStringQuestionsAndAnswers();
            }
            return string.Empty;
        }

        public string GetPlaceHolderValueForClient(string placeholder, ClientModel client) {
            switch (placeholder) {
                case ClientNamePlaceholder:
                    return client.Name;
                case MemberCodePlaceholder:
                    return client.MemberCode;
            }
            return string.Empty;
        }

        public string GetPlaceHoldersForDownloadLinks(string placeholder) {
            var urlResource = _resourceProvider.ResourceUrls.Where(x => placeholder.Equals(x.Name)).FirstOrDefault();

            return urlResource != null ? urlResource.Url : string.Empty;
        }
    }
}
