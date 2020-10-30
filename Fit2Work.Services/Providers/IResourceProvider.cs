using System.Collections.Generic;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Services.Fit2Work.Services.Providers {
    public interface IResourceProvider {
        string EmailFooter { get; }
        string EmailHeader { get; }
        string PrimaryEmailBodyFormat { get; }
        string PrimaryEmailSubjectFormat { get; }
        string SecondaryEmailBodyFormat { get; }
        string SecondaryEmailSubjectFormat { get; }
        string UserMessageText { get; }
        string UserPrimaryMessageText { get; }
        string UserSecondaryMessageText { get; }
        string DownloadMessageText { get; }
        List<ResourceUrlModel> ResourceUrls { get; }
    }
}