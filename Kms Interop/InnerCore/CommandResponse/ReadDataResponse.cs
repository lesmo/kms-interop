using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore.CommandResponse {
    public enum DataActivity {
        Invalid = 255,
        Running = 128,
        Sleep = 64,
        Walking = 0
    }

    public class Data {
        public DateTime Timestamp;
        public short Steps;
        public DataActivity Activity;
    }

    public class ReadDataResponse: ICoreCommand<Data[]> {
        protected override InnerCoreCommand Command {
            get {
                return InnerCoreCommand.ReadDataResponse;
            }
        }

        public override ICoreCommandContent<Data[]> CommandContent {
            get {
                return this._commandContent;
            }
            set {
                this._commandContent = (ReadDataResponseContent)value;
            }
        }
        private ReadDataResponseContent _commandContent;

        public ReadDataResponse() {
        }
        public ReadDataResponse(DateTime baseDate) {
            this._commandContent
                = new ReadDataResponseContent(baseDate);
        }
    }

    public class ReadDataResponseContent : ICoreCommandContent<Data[]> {
        private DateTime _baseDate;

        public ReadDataResponseContent(Data[] o) {
            throw new NotImplementedException();
        }

        public ReadDataResponseContent(DateTime baseDate) {
            this._baseDate
                = baseDate;
        }

        public override byte[] Serialize() {
            throw new NotImplementedException();
        }

        public override void Deserialize(byte[] input) {
            if ( input.Length % 2 != 0 )
                throw new ArgumentException();

            Data[] dataTmp
                = new Data[input.Length / 2];

            for ( int i = 0, s = 0; s < input.Length; i++, s += 2 ) {
                dataTmp[i]
                    = new Data() {
                        Timestamp
                            = this._baseDate.AddMinutes(2 * i),
                        Steps
                            = (short)input[s + 1],
                        Activity
                            = (DataActivity)(int)input[s]
                    };
            }

            this.Content
                = dataTmp;
        }
    }
}
