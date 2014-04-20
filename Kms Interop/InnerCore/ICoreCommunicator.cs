using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore {
    public abstract class ICoreCommunicator {
        public abstract T Request<T>(ICoreCommand requestCommand, ICoreCommand responseCommand);
        public abstract byte[] Request(byte[] writeCommand);
    }
}
