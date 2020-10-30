using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;

namespace AnvilGroup.Services.Fit2WorkApi.Controllers {
    public class VersionController : ApiController {
        // GET: api/Version
        public HttpResponseMessage Get() {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(
                Assembly.GetExecutingAssembly().GetName().Version.ToString(), 
                Encoding.UTF8, 
                "application/json");
            return response;
        }
    }
}
