using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Kms.Interop.OAuth {
    public class OAuthResponse<T> {
        public readonly HttpStatusCode StatusCode;
        public readonly WebHeaderCollection Headers;
        public readonly T Response;
        public readonly string RawResponse;

        public OAuthResponse(
            HttpStatusCode statusCode,
            WebHeaderCollection headers,
            T response,
            string rawResponse
        ) {
            this.StatusCode
                = statusCode;
            this.Headers
                = headers;
            this.Response
                = response;
            this.RawResponse
                = rawResponse;
        }
    }
}
