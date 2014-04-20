using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kms.Interop.OAuth.SocialClients {
    public interface IOAuthSocialClient : IOAuthClient {
        new OAuthCryptoSet Token {
            get;
            set;
        }
        new OAuthCryptoSet ConsumerCredentials {
            get;
            set;
        }

        string UserName {
            get;
        }
    }
}
