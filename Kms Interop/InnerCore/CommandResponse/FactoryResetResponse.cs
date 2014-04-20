using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore.CommandResponse {
    public sealed class FactoryResetResponse : ICoreCommand<object> {
        protected override InnerCoreCommand Command {
            get {
                return InnerCoreCommand.FactoryResetResponse;
            }
        }

        public override ICoreCommandContent<object> CommandContent {
            get {
                return null;
            }
            set {
                throw new NotImplementedException();
            }
        }
    }
}
