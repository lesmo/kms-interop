using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore {
    public class CoreCommandException : Exception {
        public readonly InnerCoreCommand Command;
        public readonly byte[] Response;

        public CoreCommandException(InnerCoreCommand command, byte[] response) {
            this.Command
                = command;
            this.Response
                = response;
        }
    }
}
