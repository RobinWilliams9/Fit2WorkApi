using System;
using System.Collections.Generic;
using System.Linq;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;
using AnvilGroup.Services.Fit2Work.Services.Providers;

namespace AnvilGroup.Services.Fit2Work.Services {
    public class LoggingService : BaseService, ILoggingService {
        private readonly IEmailProvider _emailProvider;
        private readonly IConfigurationProvider _configurationProvider;

        public LoggingService() {
            _emailProvider = new EmailProvider();
            _configurationProvider = new ConfigurationProvider();
        }
        public LoggingService(IFit2WorkDb fit2WorkDb) : base(fit2WorkDb) {
            _emailProvider = new EmailProvider();
            _configurationProvider = new ConfigurationProvider();
        }
        public LoggingService(IFit2WorkDb fit2WorkDb,
            IEmailProvider emailProvider,
            IConfigurationProvider configurationProvider) : base(fit2WorkDb) {
            _emailProvider = emailProvider;
            _configurationProvider = configurationProvider;
        }

        public void LogMessage(string message, string details = null) {
            try {
                Fit2WorkDb.Logs.Add(new LogModel {
                    CreatedDate = DateTime.UtcNow,
                    Message = message,
                    Details = details
                });
                Fit2WorkDb.SaveChanges();
            } catch (Exception ex) {
                NotifyDevTeam("Unable to write to db log", ex.ToString());
            }
        }

        public void LogError(Exception exception) {
            try {
                var log = new LogModel {
                    CreatedDate = DateTime.UtcNow,
                    Message = exception.Message,
                    Details = exception.ToString()
                };
                Fit2WorkDb.Logs.Add(log);
                Fit2WorkDb.SaveChanges();
            } catch (Exception ex) {
                NotifyDevTeam("Unable to write to db log", ex.ToString());
            }
        }

        public void LogCriticalError(Exception exception) {
            try {
                NotifyDevTeam(exception.Message, exception.ToString());
                Fit2WorkDb.Logs.Add(new LogModel {
                    CreatedDate = DateTime.UtcNow,
                    Message = exception.Message,
                    Details = exception.ToString()
                });
                Fit2WorkDb.SaveChanges();
            } catch (Exception ex) {
                NotifyDevTeam("Unable to write to db log", ex.ToString());
            }
        }

        private void NotifyDevTeam(string subject, string body) {
            _emailProvider.SendMail(
                    _configurationProvider.SupportEmailFromAddress,
                    _configurationProvider.SupportEmailToAddress,
                    $"{_configurationProvider.SupportEmailSubjectPrefix} : {subject}", 
                    body);
        }

        public IEnumerable<LogModel> GetLogs(DateTime date, int pageNumber, int maxPerPage) {

            var start = date.Date;
            var end = date.Date.AddDays(1);
            
            if(pageNumber < 1) {
                pageNumber = 1;
            }

            return Fit2WorkDb.Logs
                .Where(l => l.CreatedDate > start)
                .Where(l => l.CreatedDate < end)
                .OrderByDescending(l => l.CreatedDate)
                .Skip((pageNumber - 1) * maxPerPage)
                .Take(maxPerPage)
                .ToList();
        }

        public LogModel GetLogById(int id) {
            return Fit2WorkDb.Logs
                .FirstOrDefault(l => l.Id  == id);
        }

        public int GetLogCount(DateTime date) {
            var start = date.Date;
            var end = date.Date.AddDays(1);

            return Fit2WorkDb.Logs
                .Where(l => l.CreatedDate > start)
                .Where(l => l.CreatedDate < end)
                .Count();
        }
    }
}
