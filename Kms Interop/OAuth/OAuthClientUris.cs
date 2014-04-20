using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kms.Interop.OAuth {
    public class OAuthClientUris {
        public Uri BaseUri {
            get;
            set;
        }
        public string RequestTokenResource {
            get;
            set;
        }
        public Uri CallbackRequestTokenUri {
            get;
            set;
        }
        public string ExchangeTokenResource {
            get;
            set;
        }
        public string AuthorizationResource {
            get;
            set;
        }
    }
}
