using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(AnvilGroup.Applications.Fit2Work.Startup))]
namespace AnvilGroup.Applications.Fit2Work {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            app.MapSignalR();
        }
    }
}