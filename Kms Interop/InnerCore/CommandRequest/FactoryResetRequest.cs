using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore.CommandRequest {
    public sealed class FactoryResetRequest : ICoreCommand<object> {
        protected override InnerCoreCommand Command {
            get {
                return InnerCoreCommand.FactoryResetRequest;
            }
        }

        public override ICoreCommandContent<object> CommandContent {
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

    public sealed class FactoryResetRequestContent : ICoreCommandContent<object> {
        public override byte[] Serialize() {
            return new byte[0];
        }

        public override void Deserialize(byte[] input) {
            throw new NotImplementedException();
        }
    }
}
