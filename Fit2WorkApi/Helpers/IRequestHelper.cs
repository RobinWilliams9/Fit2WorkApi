using System.Net.Http;

namespace AnvilGroup.Services.Fit2WorkApi.Helpers {
    public interface IRequestHelper {
        string GetTokenFromHeader(HttpRequestMessage request);
        bool HasTokenInHeader(HttpRequestMessage request);
        bool ValidateToken(string token, string key);
    }
}