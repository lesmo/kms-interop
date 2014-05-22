using System;
using System.Collections.Generic;

using System.Text;

namespace KMS.Comm.InnerCore {
    public class CoreCommandException : Exception {
        public InnerCoreCommand Command {
            get;
            private set;
        }

        public Byte[] Response {
            get;
            private set;
        }

        public CoreCommandException(InnerCoreCommand command, byte[] response) {
            Command  = command;
            Response = response;
        }
    }
}
