using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore {
    public enum InnerCoreCommand {
        SetDateRequest
            = 0xC2,
        SetDateResponse
            = 0x22,
        ReadDataRequest
            = 0xC4,
        ReadDataResponse
            = 0x24,
        ReadDataResponseComplete
            = 0x04,
        FactoryResetRequest
            = 0x87,
        FactoryResetResponse
            = 0x27,

        Void
            = 0
    }

    public interface ICoreCommand {
        ICoreCommandContent CommandContent {
            get;
            set;
        }

        byte[] Serialize();
        void Deserialize(byte[] input);
    }
}
