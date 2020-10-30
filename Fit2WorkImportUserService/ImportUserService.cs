using System;
using System.ServiceProcess;
using AnvilGroup.Library.LoggingManager;

namespace Fit2WorkImportUserService {
    public partial class ImportUserService : ServiceBase {
        private IStartArg _arg;
        private ILogger _logger;
        public ImportUserService() {
            InitializeComponent();
            _logger = new Logger();
        }

        protected override void OnStart(string[] args) {
            try {
                _arg = new StartArg(_logger);
                _logger.Info(_arg, "Entered On Start");
                try {
                    _arg.StartWatcher();
                } catch (Exception ex1) {
                    _logger.Error(_arg, ex1);
                }
                _logger.Info(_arg, "Running...");
            } catch (Exception ex2) {
                _logger.Error(this, ex2);
            }
        }

        protected override void OnStop() {
            try {
                _arg.StopWatcher();
            } catch (Exception ex) {
                _logger.Error(this, ex);
                throw;
            } finally {
                _logger.Info(_arg, "Stopped");
                _arg = null;
            }
        }

        public void DoRun() {
            OnStart(new string[] { });
        }
        public void DoStop() {
            OnStop();
        }
    }
}
