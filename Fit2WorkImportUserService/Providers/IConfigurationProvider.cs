namespace Fit2WorkImportUserService.Providers {
    public interface IConfigurationProvider {
        string WatchPath { get; }
        int InternalBufferSize { get; }
        string ArchiveWatchPath { get; }
        int WaitSeconds { get; }
    }
}