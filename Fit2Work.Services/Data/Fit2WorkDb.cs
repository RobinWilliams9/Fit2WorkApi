using System.Data.Entity;
using AnvilGroup.Services.Fit2Work.Models;
//using Microsoft.EntityFrameworkCore;

namespace AnvilGroup.Services.Fit2Work.Data {
    public class Fit2WorkDb : System.Data.Entity.DbContext, IFit2WorkDb {
        public Fit2WorkDb() : base("name=Fit2Work") { }
        public virtual DbSet<ClientModel> Clients { get; set; }
        public virtual DbSet<LogModel> Logs { get; set; }
        public virtual DbSet<UserInfoModel> Users { get; set; }
        public virtual DbSet<UserQuestionnaireModel> UserQuestionnaires { get; set; }
        public virtual DbSet<ResourceModel> Resources { get; set; }
        public virtual DbSet<ResourceUrlModel> ResourceUrls { get; set; }
        public override int SaveChanges() {
            return base.SaveChanges();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            // Nothing to override
        }
    }
}