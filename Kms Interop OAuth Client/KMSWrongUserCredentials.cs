using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Kms.Interop.CloudClient {
    public class KMSWrongUserCredentials : HttpListenerException {
        private const int UnauthorizedErrorCode = 401;

        public KMSWrongUserCredentials()
            : base(UnauthorizedErrorCode) {
        }

        public KMSWrongUserCredentials(string message)
            : base(UnauthorizedErrorCode, message) {
        }
    }
}
