using System.Data.Entity;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Data {
    public interface IFit2WorkDb {
        DbSet<ClientModel> Clients { get; set; }
        DbSet<UserQuestionnaireModel> UserQuestionnaires { get; set; }
        DbSet<UserInfoModel> Users { get; set; }
        DbSet<ResourceModel> Resources { get; set; }
        DbSet<ResourceUrlModel> ResourceUrls { get; set; }
        DbSet<LogModel> Logs { get; set; }
        int SaveChanges();
    }
}