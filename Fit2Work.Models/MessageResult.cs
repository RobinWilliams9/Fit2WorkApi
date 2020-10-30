using System;

namespace AnvilGroup.Services.Fit2Work.Models {
    public class MessageResult {
        public MessageResult() {
            IsOk = true;
        }
        public string Recipient { get; set; }
        public string MessageContent { get; set; }
        public bool IsOk { get; set; }        
        public Exception Exception{ get; set; }        
    }
}
