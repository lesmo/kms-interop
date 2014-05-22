using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KMS.Interop.Blockity {
    public static class ResponseCommands {
        public static Boolean SimpleResponse(CommandByte expectedCommand, Byte[] input) {
            if ( input == null || input.Length != 3 )
                return false;

            if ( input[0] != (Byte)expectedCommand || input[1] != 0 || input[2] != 1 )
                return false;

            return true;
        }

        private static Boolean ValidateResponse(CommandByte expectedCommand, Byte[] input) {
            if ( input.Length < 3 || input.Length != input[1] + 3 )
                return false;

            var crcBytes = input.Skip(2).Take(input[1]).ToArray();
            return BlockityHelpers.GetCrc(crcBytes) == input.Last();
        }

        public static DateTime DateTimeResponse(Byte[] input) {
            if ( input == null )
                throw new ArgumentNullException("input");

            if ( input.Length != 10 || ! ValidateResponse(CommandByte.GetDateResponse, input) )
                throw new FormatException("Command response is corrupted");

            try {
                var returnDate = new DateTime(2000 + input[2], input[3], input[4], input[5], input[6], input[7]);
                var dayOfWeek  = (DayOfWeek)(input[8] == 7 ? 0 : input[8]);

                if ( returnDate.DayOfWeek == dayOfWeek )
                    return returnDate;
                else
                    throw new FormatException("Day of Week reported by device doesn't match DateTime Day of Week.");
            } catch ( Exception ex ) {
                throw new TypeInitializationException("Command response is invalid", ex);
            }
        }

        public static BlockityPin SetDateTimeResponse(Byte[] input) {
            if ( input == null )
                throw new ArgumentNullException("input");

            if ( input.Length != 7 || ! ValidateResponse(CommandByte.SetDateResponse, input) )
                throw new FormatException("Command response is corrupted");

            return new BlockityPin(input[2], input[3], input[4], input[5]);
        }

        public static IEnumerable<Data> GetDataResponse(DateTime baseDate, Byte[] input) {
            if ( baseDate == null )
                throw new ArgumentNullException("baseDate");

            if ( input == null )
                throw new ArgumentNullException("input");

            if ( baseDate > DateTime.UtcNow )
                throw new ArgumentException("Date cannot be in the future.", "baseDate");

            if ( baseDate < DateTime.UtcNow.AddDays(-6) )
                throw new ArgumentException("Date cannot be more than a week in the past.", "baseDate");
            
            if ( ! ValidateResponse(CommandByte.ReadDataResponse, input) )
                throw new FormatException("Command response is corrupted");

            try {
                var deviceDate = new DateTime(2000 + input[2], input[3], input[4]);

                if ( deviceDate.Year != baseDate.Year || deviceDate.Month != baseDate.Month || deviceDate.Day != baseDate.Month )
                    throw new FormatException("Device DateTime is out of sync.");
            } catch ( Exception ex ) {
                throw new FormatException("Device DateTime is invalid. See Inner Exception for details.", ex);
            }

            for ( int m = 0, s = 3; s < input.Length; m+= 2, s += 2 ) {
                DataActivity activity;
                try {
                    activity = (DataActivity)(Int32)input[s];
                } catch ( Exception ex ) {
                    throw new FormatException(
                        "Device reported data contains an unrecognized Activity Type. See inner exception for details.",
                        ex
                    );
                }

                yield return new Data {
                    Activity = activity,
                    Steps    = (Int16)input[s + 1],
                    Timestamp = baseDate = baseDate.AddMinutes(m)
                };
            }
        }

        public static Byte[] RawResponse(Byte[] input) {
            if ( input == null )
                throw new ArgumentNullException("input");

            if ( input.Length < 4 || ! ValidateResponse(CommandByte.SetDateResponse, input) )
                throw new FormatException("Command response is corrupted");

            return input.Skip(2).Take(input[1]).ToArray();
        }
    }
}
