using System.Net;

namespace Kms.Interop.OAuth {

    public class OAuthResponse<T> {
        public readonly WebHeaderCollection Headers;
        public readonly string RawResponse;
        public readonly T Response;
        public readonly HttpStatusCode StatusCode;

        public OAuthResponse(HttpStatusCode statusCode, WebHeaderCollection headers, T response, string rawResponse) {
            StatusCode  = statusCode;
            Headers     = headers;
            Response    = response;
            RawResponse = rawResponse;
        }
    }

}