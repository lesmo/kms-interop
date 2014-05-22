using System;
using System.Collections.Generic;

using System.Text;

namespace KMS.Comm.InnerCore {
    public abstract class ICoreCommand<T> : ICoreCommand {
        protected abstract InnerCoreCommand Command {
            get;
        }

        public virtual ICoreCommandContent CommandContent {
            get {
                return (ICoreCommandContent)CommandContent2;
            }
            set {
                CommandContent2 = (ICoreCommandContent<T>)value;
            }
        }

        public virtual ICoreCommandContent<T> CommandContent2 {
            get;
            set;
        }

        public bool IsValid {
            get;
            private set;
        }

        public ICoreCommand() {
        }

        public ICoreCommand(T value) {
            CommandContent2.Content = value;
            IsValid = true;
        }

        public ICoreCommand(byte[] deserializeBytes) {
            Deserialize(deserializeBytes);
        }

        public byte[] Serialize() {
            var contentBytes = CommandContent2.Serialize();
            var commandBytes = new byte[contentBytes.Length + 3];
            var crcPosition  = contentBytes.Length + 2;

            commandBytes[0] = (byte)this.Command;
            commandBytes[1] = (byte)contentBytes.Length;

            if ( contentBytes.Length == 0 ) {
                commandBytes[1] = 0;
                commandBytes[2] = 0;
                return commandBytes;
            }

            contentBytes.CopyTo(commandBytes, 2);

            commandBytes[crcPosition] = contentBytes[0];

            for ( int i = 1; i < contentBytes.Length; i++ )
                commandBytes[crcPosition] =
                    (byte)(commandBytes[crcPosition] ^ contentBytes[i]);

            return commandBytes;
        }

        public void Deserialize(byte[] input) {
            if ( input == null || input.Length < 3 )
                throw new ArgumentException();
            
            var inputCommand = (InnerCoreCommand)Enum.ToObject(
                typeof(InnerCoreCommand), 
                input[0]
            );
            
            if ( inputCommand != Command )
                throw new CoreCommandException(inputCommand, input);

            var deserializeBytes = new byte[input[1]];

            for ( short i = 2, d = 0; i < input[1]; d++, i++ )
                deserializeBytes[d] = input[i];

            CommandContent2.Deserialize(deserializeBytes);

            var tmpCrc = input[2];
            for ( int i = 1; i < input.Length; i++ )
                tmpCrc = (byte)(tmpCrc ^ input[i]);

            IsValid = tmpCrc == input[input.Length - 1];
        }
    }
}
