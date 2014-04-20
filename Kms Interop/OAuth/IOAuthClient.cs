using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kms.Interop.OAuth {
    public interface IOAuthClient {
        OAuthCryptoSet ConsumerCredentials {
            get;
            set;
        }
        OAuthCryptoSet Token {
            get;
            set;
        }
    }
}
