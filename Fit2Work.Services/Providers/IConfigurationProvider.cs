using System.Collections.Generic;

namespace AnvilGroup.Services.Fit2Work.Services.Providers {
    public interface IConfigurationProvider {
        List<SMSConfig> SMSConfigs { get; }
        string ClientEmailFromAddress { get; }
        string SupportEmailFromAddress { get; }
        string SupportEmailToAddress { get; }
        string SupportEmailSubjectPrefix { get; }
    }
}