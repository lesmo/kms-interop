using System;

namespace KMS.Interop.Blockity {

    internal class BlockityHelpers {
        /// <summary>
        ///     Devuelve el CRC (XOR de todos los Bytes).
        /// </summary>
        /// <param name="input">
        ///     Bytes de los cuales calcular el CRC.
        /// </param>
        public static Byte GetCrc(Byte[] input) {
            if ( input == null || input.Length == 0 ) {
                return 0;
            }

            if ( input.Length == 1 ) {
                return input[0];
            }

            byte crc = input[0];
            for ( short i = 1; i < input.Length; i++ )
                crc = (Byte)(crc ^ input[i]);

            return crc;
        }
    }

}