using System.Collections.Generic;
using AnvilGroup.Services.Fit2Work.Models;

namespace AnvilGroup.Applications.Fit2Work.Models {
    public class SMSModel {
        public ClientModel Client { get; set; }
        public List<UserInfoModel> Users { get; set; }
        public string MessageText { get; set; }
        public string DownloandLinkMessageText { get; set; }
    }    
}