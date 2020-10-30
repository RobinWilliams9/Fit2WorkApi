using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AnvilGroup.Services.Fit2WorkApi.Helpers;

namespace AnvilGroup.Services.Fit2WorkApi {
    /// <summary>
    /// Message handler to intercept all requests and authenticate
    /// </summary>
    public class AuthenticationHandler : DelegatingHandler {
        protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken) {            
            if (request.Method == HttpMethod.Post) {
                // Allows authentication to be turned off for testing purposes
                if (!bool.Parse(ConfigurationManager.AppSettings["Settings.Api.EncryptionEnabled"])) {                    
                    return base.SendAsync(request, cancellationToken);
                }
                // Encryption is enabled, so let's authenticate the request
                var sharedKey = ConfigurationManager.AppSettings["Settings.Api.EncryptionKey"].ToString();
                var helper = new RequestHelper();
                // Check for authorisation header
                if (!helper.HasTokenInHeader(request)) {
                    return ForbiddenResponse();
                }
                // Get header information and validate
                var token = helper.GetTokenFromHeader(request);
                //if (!helper.ValidateToken(token, sharedKey)) {
                //    return ForbiddenResponse();
                //}
            }
            return base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Returns 403 forbidden response
        /// </summary>
        private Task<HttpResponseMessage> ForbiddenResponse() {
            var source = new TaskCompletionSource<HttpResponseMessage>();
            source.SetResult(new HttpResponseMessage(HttpStatusCode.Forbidden));
            return source.Task;
        }
    }
}