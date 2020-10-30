using AnvilGroup.Services.Fit2Work.Data;

namespace AnvilGroup.Services.Fit2Work.Services {
    public class BaseService {
        protected IFit2WorkDb Fit2WorkDb { get; private set; }
        public BaseService() {
            Fit2WorkDb = new Fit2WorkDb();
        }
        public BaseService(IFit2WorkDb fit2WorkDb) {
            Fit2WorkDb = fit2WorkDb;
        }
    }
}