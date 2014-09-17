using System;

namespace Kms.Interop.OAuth {

    public class OAuthUnexpectedRequest : Exception {
        public OAuthUnexpectedRequest() {}

        public OAuthUnexpectedRequest(string message) : base(message) {}

        public OAuthUnexpectedRequest(string message, Exception innerException) : base(message, innerException) {}
    }

}