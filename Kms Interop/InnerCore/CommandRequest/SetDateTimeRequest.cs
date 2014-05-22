using KMS.Comm.InnerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KMS.Interop.InnerCore.CommandRequest {
    public sealed class SetDateTimeRequest : ICoreCommand<DateTime> {
        protected override InnerCoreCommand Command {
            get {
                return InnerCoreCommand.SetDateRequest;
            }
        }

        public SetDateTimeRequest(DateTime newTime) {
            CommandContent2 = new SetDateTimeRequestContent(newTime);
        }
    }

    public sealed class SetDateTimeRequestContent : ICoreCommandContent<DateTime> {
        public SetDateTimeRequestContent(DateTime o) : base(o) { }
        public SetDateTimeRequestContent(byte[] b) : base(b) { }

        public override Byte[] Serialize() {
            return new Byte[] {
                (Byte)Content.Year,
                (Byte)Content.Month,
                (Byte)Content.Day,
                (Byte)Content.Hour,
                (Byte)Content.Minute,
                17 // TODO: ASK WHAT THE FUCK KIND OF WEEK THIS IS
            };
        }

        public override void Deserialize(Byte[] input) {
            throw new NotImplementedException();
        }
    }
}
