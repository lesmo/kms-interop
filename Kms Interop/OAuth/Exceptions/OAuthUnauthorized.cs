using System;
using System.Collections.Generic;

using System.Text;

namespace Kms.Interop.OAuth {
    public class OAuthUnauthorized : OAuthUnexpectedResponse {
        private const Int32 UnauthorizedCode = 401;

        public OAuthUnauthorized()
            : base(UnauthorizedCode) {
        }

        public OAuthUnauthorized(string message)
            : base(UnauthorizedCode, message) {
        }

        public OAuthUnauthorized(string message, Exception innerException)
            : base(UnauthorizedCode, message, innerException) {
        }
    }

    public class OAuthUnauthorized<T> : OAuthUnexpectedResponse<T> {
        public OAuthUnauthorized() : base() {
        }

        public OAuthUnauthorized(OAuthResponse<T> response) : base(response) {
        }

        public OAuthUnauthorized(OAuthResponse<T> response, string message) : base(response, message) {
        }
    }
}
