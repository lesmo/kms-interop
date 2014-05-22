using System;
using System.Collections.Generic;

using System.Text;

namespace KMS.Comm.InnerCore.CommandRequest {
    public sealed class FactoryResetRequest : ICoreCommand<Object> {
        protected override InnerCoreCommand Command {
            get {
                return InnerCoreCommand.FactoryResetRequest;
            }
        }

        public override ICoreCommandContent<Object> CommandContent2 {
            get {
                return new FactoryResetRequestContent();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public FactoryResetRequest() {
        }
    }

    public sealed class FactoryResetRequestContent : ICoreCommandContent<Object> {
        public override byte[] Serialize() {
            return new byte[0];
        }

        public override void Deserialize(byte[] input) {
            throw new NotImplementedException();
        }
    }
}
