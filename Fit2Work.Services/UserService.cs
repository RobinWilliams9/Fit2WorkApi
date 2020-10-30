using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services.Helpers;
using AnvilGroup.Services.Fit2Work.Services.Providers;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2Work.Services {
    public class UserService : BaseService, IUserService {
        private IMessageService _messageService;
        private ILoggingService _loggingService;
        private IResourceProvider _resourceProvider;
        private IPhoneNumberHelper _phoneNumberHelper;
        private IFileSystemService _fileSystemService;

        public UserService() {
            _messageService = new MessageService(Fit2WorkDb);
            _loggingService = new LoggingService(Fit2WorkDb);
            _resourceProvider = new ResourceProvider(Fit2WorkDb);
            _phoneNumberHelper = new PhoneNumberHelper();
            _fileSystemService = new FileSystemService();
        }
        public UserService(IFit2WorkDb fit2WorkDb)
            : base(fit2WorkDb) {
            _messageService = new MessageService(Fit2WorkDb);
            _loggingService = new LoggingService(Fit2WorkDb);
            _resourceProvider = new ResourceProvider(Fit2WorkDb);
            _phoneNumberHelper = new PhoneNumberHelper();
            _fileSystemService = new FileSystemService();
        }
        public UserService(IFit2WorkDb fit2WorkDb,
            IMessageService messageService,
            ILoggingService loggingService,
            IResourceProvider resourceProvider,
            IPhoneNumberHelper phoneNumberHelper,
            IFileSystemService fileSystemService)
            : base(fit2WorkDb) {
            _messageService = messageService;
            _loggingService = loggingService;
            _resourceProvider = resourceProvider;
            _phoneNumberHelper = phoneNumberHelper;
            _fileSystemService = fileSystemService;
        }

        /// <summary>
        /// Createa a new user and returns a string for logging
        /// </summary>
        /// <param name="user"></param>
        /// <param name="saveChangesImmediatly"></param>
        /// <returns></returns>
        public string CreateUser(UserInfoModel user, bool saveChangesImmediatly = true) {
            if (!_phoneNumberHelper.IsValidMobileNumber(user.PhoneNumber)) {
                return $"INVALID, {user.FirstName}, {user.LastName}, {user.PhoneNumber}";
            }

            // Check for existing user (using phone number) as users may be uploaded several times
            var existingUser = Fit2WorkDb.Users
                .FirstOrDefault(u => u.ClientId == user.ClientId &&
                    u.PhoneNumber.Equals(user.PhoneNumber));

            if (existingUser != null) {
                // Found an existing user - update and save
                user.Id = existingUser.Id;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.UpdatedDate = user.UpdatedDate = DateTime.UtcNow;
            } else {
                // Create user for the first time
                user.CreatedDate = DateTime.UtcNow;
                Fit2WorkDb.Users.Add(user);
            }

            if (saveChangesImmediatly) {
                Fit2WorkDb.SaveChanges();
            }

            var createdUpdated = existingUser == null ? "CREATED" : "UPDATED";
            return $"{createdUpdated}, {user.FirstName}, {user.LastName}, {user.PhoneNumber}";
        }

        public void RegisterUser(UserInfoModel user) {
            if (!_phoneNumberHelper.IsValidMobileNumber(user.PhoneNumber)) {
                throw new ArgumentOutOfRangeException($"{user.PhoneNumber} is invalid.");
            }
            var existingUser = Fit2WorkDb.Users
                .Include("Client")
                .FirstOrDefault(u => u.ClientId == user.ClientId
                    && u.PhoneNumber.Equals(user.PhoneNumber));
            if (existingUser == null) {
                throw new ArgumentNullException(
                    $"Failed to find user for client ({user.ClientId}) and phone number ({user.PhoneNumber}).");
            }
            if (existingUser.Client.IsDeleted) {
                throw new ArgumentException($"Failed to find active client ({existingUser.ClientId}) for user.");
            }
            user.ClientId = existingUser.ClientId;
            user.Id = existingUser.Id;
            user.FirstName = existingUser.FirstName;
            user.LastName = existingUser.LastName;
            user.RegisteredDate = existingUser.RegisteredDate = DateTime.UtcNow;
            Fit2WorkDb.SaveChanges();
        }

        public int DailyQuestionnaireCountByClientId(int clientId) {
            var midnight = DateTime.Now.Date;
            return Fit2WorkDb.UserQuestionnaires
                .Count(q => q.ClientId == clientId
                && q.CreatedDate > midnight);
        }

        public int TotalQuestionnaireCountByClientId(int clientId) {
            return Fit2WorkDb.UserQuestionnaires
                .Count(q => q.ClientId == clientId);
        }

        public QuestionnaireResult SubmitUserQuestionnaire(UserQuestionnaireModel questionnaire) {
            QuestionnaireResult result = new QuestionnaireResult();
            try {
                CreateUserQuestionnaire(questionnaire);
                SendUserQuestionnaire(questionnaire);
                // Set the result for sending back to the user
                result.IsFitToWork = questionnaire.IsFit2Work;
                result.UserMessage = questionnaire.IsFit2Work
                    ? _resourceProvider.UserPrimaryMessageText
                    : _resourceProvider.UserSecondaryMessageText;
            } catch (Exception ex) {
                throw new ApplicationException($"Failed to submit questionniare ({questionnaire}).", ex);
            }
            return result;
        }


        public void SubmitReminderSettings(ReminderSettingsReceiver user)
        {
            UserInfoModel existingUser = Fit2WorkDb.Users.Find(user.UserId);

            existingUser.ReminderSettings = user.ReminderSettings;
            existingUser.ReminderSettingsUpdatedDate = DateTime.Now;

            Fit2WorkDb.SaveChanges();
        }



        public int TotalUserCountByClientId(int clientId) {
            return Fit2WorkDb.Users
               .Count(u => u.ClientId == clientId);
        }

        public int RegisteredUserCountByClientId(int clientId) {
            return Fit2WorkDb.Users
               .Count(u => u.ClientId == clientId
               && u.RegisteredDate != null);
        }

        private void CreateUserQuestionnaire(UserQuestionnaireModel questionnaire) {
            questionnaire.User = Fit2WorkDb.Users.Include("Client")
                .FirstOrDefault(u => u.Id == questionnaire.UserId);
            if (questionnaire.User == null) {
                throw new ApplicationException(
                    $"Failed to find user ({questionnaire.User.Id}) for questionnaire.");
            }
            if (questionnaire.User.Client == null) {
                throw new ApplicationException(
                    $"Failed to find client for user ({questionnaire.User.Id}) for questionnaire.");
            }
            questionnaire.Client = questionnaire.User.Client;
            if (questionnaire.User.Client.IsDeleted) {
                throw new ApplicationException(
                    $"Failed to find active client ({questionnaire.User.Client.Id}) for questionnaire.");
            }
            questionnaire.CreatedDate = DateTime.UtcNow;
            Fit2WorkDb.UserQuestionnaires.Add(questionnaire);
            Fit2WorkDb.SaveChanges();
        }

        /// <summary>
        /// Import users for given client and file stream.
        /// Expecting CSV with FirstName, LastName, PhoneNumber.
        /// </summary>
        public void ImportUsers(int clientId, string file, Stream fileStream, Action<string> progress) {
            progress.Invoke($"Uploading file :: {file}...");
            if (fileStream.CanSeek) {
                fileStream.Seek(0, SeekOrigin.Begin);
            }
            using (var reader = new StreamReader(fileStream)) {
                bool first = true;
                int rowNumber = 0;
                while (!reader.EndOfStream) {
                    var userRow = reader.ReadLine().Split(',');
                    if (!first) { // Ignore header row
                        var user = new UserInfoModel {
                            ClientId = clientId,
                            FirstName = userRow[0],
                            LastName = userRow[1],
                            PhoneNumber = userRow[2]
                        };

                        progress.Invoke($"Row [{rowNumber}] {CreateUser(user)}");

                    }
                    first = false;
                    rowNumber++;
                }
                progress.Invoke($"Upload complete!");
            }
        }

        /// <summary>
        /// Import users a given file where client is provided in the file.
        /// Expecting CSV with MemberCode, FirstName, LastName, PhoneNumber.
        /// </summary>
        public void ImportUsers(string file, Action<string> progress) {
            // NOTE:  This logs progress for the call method to handle
            // but also internally logs to Fit2WorkDb for ease of viewing by non-dev staff
            var importLog = new StringBuilder();
            importLog.AppendLine($"Importing file :: {file}...");
            var isValid = true; // assume all is good            
            try {

                ClientModel client = null;

                using (var reader = _fileSystemService.GetCSVReaderFromPath(file)) {
                    bool first = true;
                    int rowNumber = 1;
                    while (!reader.EndOfStream) {
                        var userRow = reader.ReadLine().Split(',');

                        // Ignore header row
                        if (first) {
                            first = false;
                            continue;
                        }

                        if (client == null) {
                            var fileMemberCode = userRow[0];
                            client = Fit2WorkDb.Clients
                                .FirstOrDefault(c =>
                                    c.MemberCode.ToUpper().Equals(fileMemberCode.ToUpper()));
                            if (client == null) {
                                throw new ApplicationException(
                                    $"Failed to find client for member code {fileMemberCode}.");
                            }
                        }

                        var user = new UserInfoModel {
                            ClientId = client.Id,
                            FirstName = userRow[1],
                            LastName = userRow[2],
                            PhoneNumber = userRow[3],
                            CreatedDate = DateTime.UtcNow
                        };

                        var logResult = $"Row [{rowNumber}], {CreateUser(user, false)}";

                        if (logResult.Contains("INVALID")) {
                            isValid = false;
                        }

                        importLog.AppendLine(logResult);
                        rowNumber++;
                    }
                }
                Fit2WorkDb.SaveChanges();
            } catch (Exception ex) {
                importLog.AppendLine($"IMPORT FAILED :: {ex}");
                _loggingService.LogError(
                    new ApplicationException($"Import failed for file {file}", ex));
                throw ex;
            } finally {

                if (!isValid) {
                    importLog.AppendLine("Import had issues. File contains invalid users");
                }

                importLog.AppendLine($"Import completed!");
                var importLogAsString = importLog.ToString();
                _loggingService.LogMessage($"Import completed for file '{file}'", importLog.ToString());
                progress.Invoke(importLog.ToString());
            }
        }

        private void SendUserQuestionnaire(UserQuestionnaireModel questionnaire) {
            var result = _messageService.SendQuestionnaireToClient(questionnaire);
            if (!result.IsOk) {
                throw new ApplicationException(
                    $"Failed to send questionnaire ({questionnaire.Id}) to {result.Recipient}.", result.Exception);
            }
            // Log each email sent otherwise we have no auditing
            _loggingService.LogMessage(
                    $"Questionnaire ({questionnaire.Id}) for client {questionnaire.Client.Name} sent to {result.Recipient}.",
                    result.MessageContent);
        }

        public List<UserInfoModel> GetUsersByClientId(int id) {
            var result = Fit2WorkDb.Users
                .Include("Client")
                .Where(u => u.ClientId == id)
                .ToList();
            return result;
        }


        public List<UserInfoModel> GetUsersNoAssessmentToday()
        {
            DateTime dtToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            int intTodayday = (int)DateTime.Now.DayOfWeek;

            List<UserInfoModel> calc =
            (from u in Fit2WorkDb.Users
             join q in (Fit2WorkDb.UserQuestionnaires.Where(x => x.CreatedDate > dtToday).GroupBy(p => new { p.UserId }).Select(x => new { x.Key.UserId, C = x.Count() })) on u.Id equals q.UserId into ps
             from q in ps.DefaultIfEmpty()
             where u.ReminderSettings != null && q.C == null
             select u).ToList();

            List<UserInfoModel> result = new List<UserInfoModel>();


            foreach (var u in calc)
            {
                var lstReminderSettings = JsonConvert.DeserializeObject<List<ReminderSettingsModel>>(u.ReminderSettings).Where(x => x.DayNo == intTodayday & x.IsEnabled == true).FirstOrDefault();
                if (lstReminderSettings != null)
                {
                    result.Add(u);
                }

            }

            
            return result;
        }


        public List<UserInfoModel> GetUsers() {
            return Fit2WorkDb.Users
                .ToList();
        }

        public UserInfoModel GetUserById(int id) {
            return Fit2WorkDb.Users
                .Include("Client")
                .FirstOrDefault(u => u.Id == id);
        }

        public List<UserQuestionnaireModel> GetUserQuestionnaires(int id) {
            return Fit2WorkDb.UserQuestionnaires
                .Include("User")
                .Include("Client")
                .Where(q => q.UserId == id)
                .ToList();
        }

        public void MarkSmsSent(int id) {
            var user = Fit2WorkDb.Users.SingleOrDefault(u => u.Id == id);
            user.SmsSentDate = DateTime.UtcNow;
            Fit2WorkDb.SaveChanges();
        }

        public int GetUserCount() {
            return Fit2WorkDb.Users.Count();
        }

        public List<UserInfoModel> GetUsers(int pageNumber, int maxPerPage) {            
            return Fit2WorkDb.Users.OrderByDescending(u => u.Id)
                .Skip((pageNumber - 1) * maxPerPage)
                .Take(maxPerPage)
                .ToList();
        }
    }
}