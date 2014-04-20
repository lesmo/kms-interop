using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMS.Comm.InnerCore {
    public abstract class ICoreCommand<T>{
        protected abstract InnerCoreCommand Command {
            get;
        }
        public abstract ICoreCommandContent<T> CommandContent {
            get;
            set;
        }
        public bool IsValid {
            get {
                return this._isValid;
            }
        }
        private bool _isValid = false;

        public ICoreCommand() {
        }

        public ICoreCommand(T value) {
            this.CommandContent.Content = value;
            this._isValid = true;
        }

        public ICoreCommand(byte[] deserializeBytes) {
            this.Deserialize(deserializeBytes);
        }

        public byte[] Serialize() {
            byte[] contentBytes
                = this.CommandContent.Serialize();
            byte[] commandBytes
                = new byte[contentBytes.Length + 3];
            int crc
                = contentBytes.Length + 2;

            commandBytes[0]
                = (byte)this.Command;
            commandBytes[1]
                = (byte)contentBytes.Length;

            if ( contentBytes.Length == 0 ) {
                commandBytes[1] = 0;
                commandBytes[2] = 0;
                return commandBytes;
            }

            contentBytes.CopyTo(commandBytes, 2);

            commandBytes[crc]
                = contentBytes[0];

            for ( int i = 1; i < contentBytes.Length; i++ )
                commandBytes[crc]
                    = (byte)(commandBytes[crc] ^ contentBytes[i]);

            return commandBytes;
        }

        public void Deserialize(byte[] input) {
            if ( input == null )
                return;

            if ( input.Length < 3 )
                throw new ArgumentException();

            InnerCoreCommand command
                = (InnerCoreCommand)Enum.ToObject(
                    typeof(InnerCoreCommand), 
                    input[0]
                );
            
            if ( command != this.Command )
                throw new CoreCommandException(command, input);

            byte[] deserializeBytes
                = new byte[input[1]];

            for ( short i = 2, d = 0; i < input[1]; d++, i++ )
                deserializeBytes[d] = input[i];

            this.CommandContent.Deserialize(
                deserializeBytes
            );

            byte tmpCrc
                = input[2];
            for ( int i = 1; i < input.Length; i++ )
                tmpCrc = (byte)(tmpCrc ^ input[i]);

            this._isValid
                = tmpCrc == input[input.Length - 1];
        }
    }
}
