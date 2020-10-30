using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Services;
using AnvilGroup.Services.Fit2Work.Services.Providers;
using Newtonsoft.Json;

namespace AnvilGroup.Services.Fit2WorkApi.Controllers {
    public class ResourcesController : ApiController {
        private IResourceProvider _resourceProvider;
        private ILoggingService _loggingService;

        public ResourcesController() {
            _resourceProvider = new ResourceProvider(new Fit2WorkDb());
            _loggingService = new LoggingService();
        }
        public ResourcesController(IResourceProvider resourceProvider, ILoggingService loggingService) {
            _resourceProvider = resourceProvider;
            _loggingService = loggingService;
        }

        /// <summary>
        /// GET: Gets the app resources.
        /// http://localhost:62780/api/resources
        /// </summary>
        /// <returns>JSON representation of known resources</returns>
        public HttpResponseMessage Get() {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            try {
                var resources = _resourceProvider.ResourceUrls;
                response.Content = new StringContent(JsonConvert.SerializeObject(resources));
            } catch (Exception ex) {
                _loggingService.LogError(ex);
                response = Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, ex);
            }
            return response;
        }
    }
}
