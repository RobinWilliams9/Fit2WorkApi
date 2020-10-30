using System;
using System.Web;

namespace AnvilGroup.Applications.Fit2Work.Helpers {
    public static class ExceptionHelper {
        public static string HtmlMessage(this Exception ex) {
            return ex.GetBaseException().Message;
        }
    }
}