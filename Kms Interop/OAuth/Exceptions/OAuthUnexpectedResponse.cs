using System;
using System.Net;

namespace Kms.Interop.OAuth {

    public class OAuthUnexpectedResponse : HttpListenerException {

        public OAuthUnexpectedResponse() {}

        public OAuthUnexpectedResponse(Exception innerException) {
            InnerException = innerException;
        }

        public OAuthUnexpectedResponse(int errorCode) : base(errorCode) {}

        public OAuthUnexpectedResponse(int errorCode, Exception innerException) : base(errorCode) {
            InnerException = innerException;
        }

        public OAuthUnexpectedResponse(int errorCode, string message) : base(errorCode, message) {}

        public OAuthUnexpectedResponse(int errorCode, string message, Exception innerException)
            : base(errorCode, message) {
            InnerException = innerException;
        }

        public new Exception InnerException { get; private set; }
    }

    public class OAuthUnexpectedResponse<T> : OAuthUnexpectedResponse {

        public OAuthUnexpectedResponse() {}

        public OAuthUnexpectedResponse(OAuthResponse<T> response)
            : base((Int32)response.StatusCode, response.Headers[HttpResponseHeader.Warning]) {
            OAuthResponse = response;
        }

        public OAuthUnexpectedResponse(OAuthResponse<T> response, string message)
            : base((Int32)response.StatusCode, message) {
            OAuthResponse = response;
        }

        public OAuthResponse<T> OAuthResponse { get; private set; }
    }

}