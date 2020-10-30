using System.Web.Http;

namespace AnvilGroup.Services.Fit2WorkApi {
    public class WebApiApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
