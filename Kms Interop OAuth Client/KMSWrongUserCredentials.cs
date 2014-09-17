using System.Net;

namespace Kms.Interop.CloudClient {

    public class KMSWrongUserCredentials : HttpListenerException {
        private const int UnauthorizedErrorCode = 401;

        public KMSWrongUserCredentials() : base(UnauthorizedErrorCode) {}

        public KMSWrongUserCredentials(string message) : base(UnauthorizedErrorCode, message) {}
    }

}