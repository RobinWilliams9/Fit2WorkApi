using System;
using System.Collections.Generic;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Services {
    public interface ILoggingService {
        void LogCriticalError(Exception exception);
        void LogError(Exception exception);
        void LogMessage(string message, string details = null);
        IEnumerable<LogModel> GetLogs(DateTime date, int pageNumber, int maxPerPage);
        LogModel GetLogById(int id);

        int GetLogCount(DateTime date);
    }
}