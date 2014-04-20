using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore.CommandRequest {
    public sealed class DataReadTimeSpan {
        public DayOfWeek DayOfWeek
            = DayOfWeek.Monday;
        public short Hour
            = 0;
    }

    public sealed class ReadDataRequest : ICoreCommand<DataReadTimeSpan> {
        protected override InnerCoreCommand Command {
            get {
                return InnerCoreCommand.ReadDataRequest;
            }
        }

        public override ICoreCommandContent<DataReadTimeSpan> CommandContent {
            get {
                return this._commandContent;
            }
            set {
                this._commandContent = (ReadDataRequestContent)value;
            }
        }
        private ReadDataRequestContent _commandContent;

        public ReadDataRequest(DataReadTimeSpan input) {
            this.CommandContent
                = new ReadDataRequestContent(input);
        }
    }

    public sealed class ReadDataRequestContent : ICoreCommandContent<DataReadTimeSpan> {
        public ReadDataRequestContent(DataReadTimeSpan o) : base(o) {
        }
        public ReadDataRequestContent(byte[] b) : base(b) {
        }

        public override byte[] Serialize() {
            byte[] tmp
                = new byte[3];

            tmp[0]
                = this.Content.DayOfWeek == DayOfWeek.Sunday
                ? (byte)0
                : (byte)(int)this.Content.DayOfWeek;
            tmp[1]
                = (byte)this.Content.Hour;
            tmp[2]
                = (byte)3;

            return tmp;
        }

        public override void Deserialize(byte[] input) {
            throw new NotImplementedException();
        }
    }
}
