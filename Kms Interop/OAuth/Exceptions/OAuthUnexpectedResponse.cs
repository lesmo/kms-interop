using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Kms.Interop.OAuth {
    public class OAuthUnexpectedResponse : HttpListenerException {
        public new Exception InnerException {
            get;
            private set;
        }

        public OAuthUnexpectedResponse() : base() {
        }
        public OAuthUnexpectedResponse(Exception innerException) : base() {
            InnerException = innerException;
        }
        public OAuthUnexpectedResponse(int errorCode) : base(errorCode) {
        }
        public OAuthUnexpectedResponse(int errorCode, Exception innerException) : base(errorCode) {
            InnerException = innerException;
        }
        public OAuthUnexpectedResponse(int errorCode, string message) : base(errorCode, message) {
        }
        public OAuthUnexpectedResponse(int errorCode, string message, Exception innerException) : base(errorCode, message) {
            InnerException = innerException;
        }
    }

    public class OAuthUnexpectedResponse<T> : OAuthUnexpectedResponse {
        public OAuthResponse<T> OAuthResponse {
            get;
            private set;
        }

        public OAuthUnexpectedResponse() : base() {
        }

        public OAuthUnexpectedResponse(OAuthResponse<T> response) : base((Int32)response.StatusCode, response.Headers[HttpResponseHeader.Warning]) {
            OAuthResponse = response;
        }

        public OAuthUnexpectedResponse(OAuthResponse<T> response, string message) : base((Int32)response.StatusCode, message) {
            OAuthResponse = response;
        }
    }
}
