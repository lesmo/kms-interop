using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore {
    public interface ICoreCommandContent {
        byte[] Serialize();
        void Deserialize(byte[] input);
    }
}
