using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AnvilGroup.Services.Fit2Work.Services.Providers {
    public class ConfigurationProvider : IConfigurationProvider {
        public List<SMSConfig> SMSConfigs {
            get {
                return ConfigurationManager.AppSettings.AllKeys
                    .Where(key => key.StartsWith("SMSRouting"))
                    .Select(key => new SMSConfig(ConfigurationManager.AppSettings[key]))
                    .ToList();
            }
        }        
        public string ClientEmailFromAddress {
            get {
                return ConfigurationManager.AppSettings["ClientEmail.FromAddress"];
            }
        }
        public string SupportEmailFromAddress {
            get {
                return ConfigurationManager.AppSettings["SupportEmail.FromAddress"];
            }
        }
        public string SupportEmailToAddress {
            get {
                return ConfigurationManager.AppSettings["SupportEmail.ToAddress"];
            }
        }
        public string SupportEmailSubjectPrefix {
            get {
                return ConfigurationManager.AppSettings["SupportEmail.SubjectPrefix"];
            }
        }
    }
}
