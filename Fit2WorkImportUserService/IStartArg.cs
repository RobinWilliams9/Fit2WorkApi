using System.Threading;

namespace Fit2WorkImportUserService {
    public interface IStartArg {
        void StartWatcher();
        void StopWatcher();        
    }
}