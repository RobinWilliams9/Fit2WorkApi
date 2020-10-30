using System;
using System.Linq;
using System.Net.Http;
using AnvilGroup.Library.Encryption;
using AnvilGroup.Services.Fit2Work.Data;
using AnvilGroup.Services.Fit2Work.Services;

namespace AnvilGroup.Services.Fit2WorkApi.Helpers {
    public class RequestHelper : IRequestHelper {
        const string AuthHeader = "Authorization";

        IFit2WorkDb _fit2WorkDb;
        ILoggingService _loggingService;

        public RequestHelper() {
            _fit2WorkDb = new Fit2WorkDb();
            _loggingService = new LoggingService(_fit2WorkDb);
        }
        public RequestHelper(IFit2WorkDb fit2WorkDb, ILoggingService loggingService) {
            _fit2WorkDb = fit2WorkDb;
            _loggingService = loggingService;
        }

        public bool HasTokenInHeader(HttpRequestMessage request) {
            return request.Headers.Contains(AuthHeader);
        }
        public string GetTokenFromHeader(HttpRequestMessage request) {
            if (request == null) {
                throw new NullReferenceException("Request is null");
            }
            if (!request.Headers.Contains(AuthHeader)) {
                throw new ApplicationException("Auth Token is not provided in the header");
            }
            return request.Headers.GetValues(AuthHeader).FirstOrDefault();
        }
        public bool ValidateToken(string token, string key) {
            try {
                var decryptedToken = new Cryptography().DecryptString(key, token);
                var decryptedArray = decryptedToken.Split('#');
                var clientId = int.Parse(decryptedArray[0]);
                var userPhoneNumber = decryptedArray[1];
                var timestamp = DateTime.Parse(decryptedArray[2]);
                if (string.IsNullOrEmpty(userPhoneNumber)) {
                    throw new ArgumentNullException("phoneNumber");
                }
                // Valid if request is within 30 minutes AND clientId and phone number
                // match an existing user in the system
                return timestamp.AddMinutes(30) >= DateTime.UtcNow
                    && _fit2WorkDb.Users.Any((u => u.ClientId == clientId
                        && u.PhoneNumber.Equals(userPhoneNumber)));               
            } catch(Exception ex) {
                _loggingService.LogCriticalError(ex);
                return false;
            }
        }        
    }
}