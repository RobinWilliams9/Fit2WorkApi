using System;
using System.IO;
using System.Text;
using System.Threading;
using AnvilGroup.Library;
using AnvilGroup.Library.LoggingManager;
using AnvilGroup.Services.Fit2Work.Services;
using Fit2WorkImportUserService.Providers;

namespace Fit2WorkImportUserService {
    public class StartArg : IStartArg {
        //IHeartBeat _heartBeat;
        IUserService _userService;
        ILogger _logger;
        IFileProvider _fileProvider;
        IConfigurationProvider _configurationProvider;

        //BeatArg _beatArg;
        FileSystemWatcher _fileWatcher;

        public StartArg(ILogger logger) {
            _logger = logger;
            //_heartBeat = new HeartBeat();
            _userService = new UserService();
            _fileProvider = new FileProvider();
            _configurationProvider = new ConfigurationProvider();
        }

        //public StartArg(IUserService userService,
        //    IFileProvider fileProvider,
        //    IConfigurationProvider configProvider,
        //    ILogger logger,
        //    IHeartBeat heartBeat) {
        //    _fileProvider = fileProvider;
        //    _configurationProvider = configProvider;
        //    _userService = userService;
        //    _logger = logger;
        //    _heartBeat = heartBeat;            
        //}

        public void StartWatcher() {
            _logger.Info(this, "Preparing StartArg");

            _fileWatcher = new FileSystemWatcher {
                Path = _configurationProvider.WatchPath,
                Filter = "*.*",
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                InternalBufferSize = _configurationProvider.InternalBufferSize                              
            };

            _fileWatcher.Changed += new FileSystemEventHandler(ProcessFile);
            _fileWatcher.Created += new FileSystemEventHandler(ProcessFile);

            //_beatArg = new BeatArg();
            //_beatArg.ApplicationName = "AnvilGroup - Fit2Work User Import Service";

            //_logger.LogMessage(_beatArg.ToString());
        }
        public void StopWatcher() {
            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Dispose();
            _fileWatcher = null;
        }

        public void ProcessFile(object sender, FileSystemEventArgs e) {
            if(e.ChangeType != WatcherChangeTypes.Changed 
                && e.ChangeType != WatcherChangeTypes.Created) {
                return;
            }
            _logger.Info(this, $"Checking if file/folder '{e.Name}' is ready to process");
            SendHeartBeat();
            if (IsFileReady(e.FullPath)) {
                try {
                    // Disable further events whilst processing
                    _fileWatcher.EnableRaisingEvents = false;
                    switch (e.ChangeType) {
                        case WatcherChangeTypes.Changed:
                        case WatcherChangeTypes.Created:
                            _fileProvider.Archive(
                                e.FullPath, _configurationProvider.ArchiveWatchPath);
                            _logger.Info(this, $"Beginning processing of file '{e.Name}'");
                            var importLog = new StringBuilder();
                            _userService.ImportUsers(e.FullPath, ((entry) => {
                                importLog.AppendLine(entry);
                            }));
                            _logger.Info(
                                this, $"File '{e.Name}' processed.\r\n{importLog.ToString()}");                            
                            break;
                        default:
                            break;
                    }
                } catch (Exception err) {                    
                    _logger.LogException(
                        new Exception($"Error processing file {e.FullPath}", err), true);
                } finally {
                    // Re-enable events after processing
                    _fileWatcher.EnableRaisingEvents = true;
                }
            } else {
                _logger.Info(this, String.Format("File/folder '{0}' NOT ready to process", e.Name));
            }
        }        

        private bool IsFileReady(string path) {
            // Wait for a defined number of seconds before applying check
            Thread.Sleep(_configurationProvider.WaitSeconds * 1000);
            // One exception per file rather than several like in the polling pattern
            try {
                // If we can't open the file, it's still copying
                using (var file =
                    _fileProvider.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    return true;
                }
            } catch (IOException) {
                return false;
            }
        }

        private void SendHeartBeat() {
            //_heartBeat.Beat(ref _beatArg);
            //if (_beatArg.Status == BeatResult.Error) {
            //    _logger.LogMessage(_beatArg.ErrorMessage);
            //}
        }
    }
}