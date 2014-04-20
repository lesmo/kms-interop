using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kms.Interop.CloudClient {
    public class KMSScrewYou : Exception {
        public override string Message {
            get {
                return this._message;
            }
        }
        private string _message;

        public KMSScrewYou(string message = "General screw you") {
            this._message
                = message;
        }
    }
}
