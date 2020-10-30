using System.Collections.Generic;
using System.Linq;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Services.Providers {
    public class ResourceProvider : IResourceProvider {
        private string _languageCode;
        private ResourceModel _resources;
        private List<ResourceUrlModel> _resourceUrls;
        private IFit2WorkDb _fit2WorkDb;        

        public ResourceProvider(IFit2WorkDb fit2WorkDb, string languageCode = "en-GB") {
            _languageCode = languageCode;
            _fit2WorkDb = fit2WorkDb;
        }        

        public string PrimaryEmailSubjectFormat {
            get {
                if (_resources == null) {
                    LoadResources();
                }
                return _resources.PrimaryEmailSubject;
            }
        }

        public string PrimaryEmailBodyFormat {
            get {
                if (_resources == null) {
                    LoadResources();
                }
                return _resources.PrimaryEmailBody;
            }
        }

        public string SecondaryEmailSubjectFormat {
            get {
                if (_resources == null) {
                    LoadResources();
                }
                return _resources.SecondaryEmailSubject;
            }
        }

        public string SecondaryEmailBodyFormat {
            get {
                if (_resources == null) {
                    LoadResources();
                }
                return _resources.SecondaryEmailBody;
            }
        }

        public string EmailHeader {
            get {
                if (_resources == null) {
                    LoadResources();
                }
                return _resources.EmailHeader;
            }
        }

        public string EmailFooter {
            get {
                if (_resources == null) {
                    LoadResources();
                }
                return _resources.EmailFooter;
            }
        }

        public string UserMessageText {
            get {
                if (_resources == null) {
                    LoadResources();
                }
                return _resources.UserMessageText;
            }
        }

        public string UserPrimaryMessageText {
            get {
                if (_resources == null) {
                    LoadResources();
                }
                return _resources.UserPrimaryMessageText;
            }
        }

        public string UserSecondaryMessageText {
            get {
                if (_resources == null) {
                    LoadResources();
                }
                return _resources.UserSecondaryMessageText;
            }
        }

        public string DownloadMessageText {
            get {
                if(_resources == null) {
                    LoadResources();
                }

                return _resources.DownloadMessageText;
            }
        }

        public List<ResourceUrlModel> ResourceUrls {
            get {
                if (_resourceUrls == null) {
                    LoadResources();
                }
                return _resourceUrls;
            }
        }

        private void LoadResources() {
            _resources = _fit2WorkDb.Resources
                .SingleOrDefault(r =>
                    r.CultureCode.ToUpper().Equals(_languageCode.ToUpper()));
            _resourceUrls = _fit2WorkDb.ResourceUrls
                .Where(r => r.CultureCode.ToUpper().Equals(_languageCode.ToUpper()))
                .ToList();
        }
    }
}
