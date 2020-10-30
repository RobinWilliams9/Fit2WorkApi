using System.Configuration;

namespace Fit2WorkImportUserService.Providers {
    public class ConfigurationProvider : IConfigurationProvider {
        public string WatchPath => ConfigurationManager.AppSettings["WatchPath"];        
        public string ArchiveWatchPath => ConfigurationManager.AppSettings["ArchiveWatchPath"];
        public int InternalBufferSize => int.Parse(ConfigurationManager.AppSettings["InternalBufferSize"]);
        public int WaitSeconds => int.Parse(ConfigurationManager.AppSettings["WaitSeconds"]);
    }
}
