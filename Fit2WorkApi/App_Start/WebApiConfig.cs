using System.Net.Http.Headers;
using System.Web.Http;
using AnvilGroup.Services.Fit2WorkApi.Filters;

namespace AnvilGroup.Services.Fit2WorkApi {
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {
            // Web API configuration and services
            config.MessageHandlers.Add(new AuthenticationHandler());
            config.Filters.Add(new ValidateModelAttribute());
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("application/json"));
            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
