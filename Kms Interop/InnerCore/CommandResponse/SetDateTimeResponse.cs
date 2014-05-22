using KMS.Comm.InnerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KMS.Interop.InnerCore.CommandResponse {
    public class SetDateTimeResponse : ICoreCommand<Int16[]> {
        protected override InnerCoreCommand Command {
            get {
                return InnerCoreCommand.SetDateResponse;
            }
        }

        public SetDateTimeResponse(Byte[] input) {
            CommandContent2 = new SetDateTimeResponseContent();
            Deserialize(input);
        }
    }

    public class SetDateTimeResponseContent : ICoreCommandContent<Int16[]> {
        public override Byte[] Serialize() {
            throw new NotImplementedException();
        }

        public override void Deserialize(Byte[] input) {
            if ( input == null || input.Length != 4 )
                throw new ArgumentException();

            Content = new Int16[] {
                input[0],
                input[1],
                input[2],
                input[3]
            };
        }
    }
}
