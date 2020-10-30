using System;
using System.Collections.Generic;
using System.IO;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services.Helpers;

namespace AnvilGroup.Services.Fit2Work.Services {
    public interface IUserService {
        string CreateUser(UserInfoModel user, bool saveChangesImmediatly = true);
        void RegisterUser(UserInfoModel user);
        List<UserInfoModel> GetUsers();
        List<UserInfoModel> GetUsersByClientId(int id);
        List<UserInfoModel> GetUsersNoAssessmentToday();
        UserInfoModel GetUserById(int id);
        int GetUserCount();
        List<UserInfoModel> GetUsers(int pageNumber, int maxPerPage);
        List<UserQuestionnaireModel> GetUserQuestionnaires(int id);        
        QuestionnaireResult SubmitUserQuestionnaire(UserQuestionnaireModel questionnaire);

        void SubmitReminderSettings(ReminderSettingsReceiver settings);
        int TotalUserCountByClientId(int id);
        int RegisteredUserCountByClientId(int id);
        int DailyQuestionnaireCountByClientId(int id);
        int TotalQuestionnaireCountByClientId(int id);
        void ImportUsers(int clientId, string file, Stream fileStream, Action<string> logProgress);
        void ImportUsers(string file, Action<string> progress);
        void MarkSmsSent(int id);
    }
}