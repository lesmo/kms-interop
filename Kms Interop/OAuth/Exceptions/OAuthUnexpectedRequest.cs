using System;
using System.Collections.Generic;

using System.Text;

namespace Kms.Interop.OAuth {
    public class OAuthUnexpectedRequest : Exception {
        public OAuthUnexpectedRequest() : base() {
        }

        public OAuthUnexpectedRequest(string message) : base(message) {
        }

        public OAuthUnexpectedRequest(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
