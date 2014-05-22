using System;
using System.Collections.Generic;

using System.Text;

namespace KMS.Comm.InnerCore.CommandResponse {
    public enum DataActivity {
        Invalid = 255,
        Running = 128,
        Sleep = 64,
        Walking = 0,
        Reserved = 192
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

        public ReadDataResponse() {
            throw new NotSupportedException("Cannot create instance of object without a base date.");
        }
        public ReadDataResponse(DateTime baseDate) {
            CommandContent = new ReadDataResponseContent(baseDate);
        }
    }

    public class ReadDataResponseContent : ICoreCommandContent<Data[]> {
        private DateTime m_baseDate;

        public ReadDataResponseContent(Data[] o) {
            throw new NotImplementedException();
        }

        public ReadDataResponseContent(DateTime baseDate) {
            m_baseDate = baseDate;
        }

        public override byte[] Serialize() {
            throw new NotImplementedException();
        }

        public override void Deserialize(byte[] input) {
            if ( input.Length != 183 )
                throw new ArgumentException();

            var dataTmp = new Data[180];
            
            for ( int i = 3, s = 3; s < 183; i++, s += 2 ) {
                dataTmp[i] = new Data() {
                    Timestamp = this.m_baseDate.AddMinutes(2 * i),
                    Steps     = (short)input[s + 1],
                    Activity  = (DataActivity)(int)input[s]
                };
            }

            Content = dataTmp;
        }
    }
}
