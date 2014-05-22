using System;
using System.Collections.Generic;

using System.Text;

namespace KMS.Comm.InnerCore.CommandRequest {
    public sealed class DataReadTimeSpan {
        public DayOfWeek DayOfWeek = DayOfWeek.Monday;
        public UInt16 Hour = 0;
    }

    public sealed class ReadDataRequest : ICoreCommand<DataReadTimeSpan> {
        protected override InnerCoreCommand Command {
            get {
                return InnerCoreCommand.ReadDataRequest;
            }
        }

        public ReadDataRequest(DataReadTimeSpan input) {
            CommandContent2 = new ReadDataRequestContent(input);
        }
    }

    public sealed class ReadDataRequestContent : ICoreCommandContent<DataReadTimeSpan> {
        public ReadDataRequestContent(DataReadTimeSpan o) : base(o) { }
        public ReadDataRequestContent(byte[] b) : base(b) { }

        public override byte[] Serialize() {
            var tmp = new byte[3];

            tmp[0] = this.Content.DayOfWeek == DayOfWeek.Sunday
                ? (byte)0
                : (byte)(int)this.Content.DayOfWeek;
            tmp[1] = (byte)this.Content.Hour;
            tmp[2] = (byte)3;

            return tmp;
        }

        public override void Deserialize(byte[] input) {
            throw new NotImplementedException();
        }
    }
}
