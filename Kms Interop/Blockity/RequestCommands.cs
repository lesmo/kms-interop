using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KMS.Interop.Blockity {
    public interface IBlockityCommand<T> {
        Byte[] RequestBytes { get; }
        BlockityCommandDelegate<T> ResponseCommand { get; }
    }

    public struct BlockityCommand<T> : IBlockityCommand<T> {
        public Byte[] RequestBytes {
            get;
            internal set;
        }

        public BlockityCommandDelegate<T> ResponseCommand {
            get;
            internal set;
        }
    }

    public delegate T BlockityCommandDelegate<T>(Byte[] input);

    /// <summary>
    ///     Contiene métodos que devuelven los bytes que deben escribirse en la
    ///     Pulsera KMS para el comando.
    /// </summary>
    public static class RequestCommands {
        public static BlockityCommand<Boolean> SetName(String newName) {
            if ( String.IsNullOrEmpty(newName) )
                throw new ArgumentException("Name is either null or empty.", "newName");

            Byte[] newNameBytes = Encoding.ASCII.GetBytes(newName.PadRight(15, '#'));
            if ( newNameBytes.Length != 15 )
                throw new ArgumentException("Name is greater than 15 bytes in length.", "newName");

            Byte[] command = new Byte[18];
            command[0] = (Byte)CommandByte.SetBluetoothNameRequest;
            command[1] = 0x0F;

            newNameBytes.CopyTo(command, 2);
            command[17] = BlockityHelpers.GetCrc(newNameBytes);

            return new BlockityCommand<Boolean> {
                RequestBytes = command,
                ResponseCommand = new BlockityCommandDelegate<Boolean>((Byte[] input) => {
                    return ResponseCommands.SimpleResponse(CommandByte.SetBluetoothNameResponse, input);
                })
            };
        }

        public static BlockityCommand<Boolean> SetPassword(BlockityPin pin) {
            return new BlockityCommand<Boolean> {
                RequestBytes = new Byte[] {
                    (Byte)CommandByte.SetBluetoothPinRequest,
                    4, pin.A, pin.B, pin.C, pin.D,
                    BlockityHelpers.GetCrc(new Byte[] { pin.A, pin.B, pin.C, pin.D })
                },
                ResponseCommand = new BlockityCommandDelegate<Boolean>((Byte[] input) => {
                    return ResponseCommands.SimpleResponse(CommandByte.SetBluetoothPinResponse, input);
                })
            };
        }

        public static BlockityCommand<DateTime> GetDateTime() {
            return new BlockityCommand<DateTime> {
                RequestBytes = new Byte[] { (Byte)CommandByte.GetDateRequest, 0, 0 },
                ResponseCommand = new BlockityCommandDelegate<DateTime>(ResponseCommands.DateTimeResponse)
            };
        }

        public static BlockityCommand<BlockityPin> SetDateTime(DateTime now) {
            var dateBytes = new Byte[] {
                (Byte)Int32.Parse(now.ToString("yy")),
                (Byte)now.Month,
                (Byte)now.Day,
                (Byte)now.Hour,
                (Byte)now.Minute,
                (Byte)now.Second,
                now.DayOfWeek == 0 ? (Byte)7 : (Byte)now.DayOfWeek
            };

            var requestBytes = new Byte[dateBytes.Length + 3];
            requestBytes[0]  = (Byte)CommandByte.SetDateRequest;
            requestBytes[1]  = 7;
            requestBytes[9]  = BlockityHelpers.GetCrc(dateBytes);

            dateBytes.CopyTo(requestBytes, 2);

            return new BlockityCommand<BlockityPin> {
                RequestBytes    = requestBytes,
                ResponseCommand = new BlockityCommandDelegate<BlockityPin>(ResponseCommands.SetDateTimeResponse)
            };
        }

        public static BlockityCommand<IEnumerable<Data>> GetData(DateTime date, Int32 hoursToRead) {
            return GetData(date, (Byte)hoursToRead);
        }

        public static BlockityCommand<IEnumerable<Data>> GetData(DateTime date, Byte hoursToRead) {
            if ( date > DateTime.UtcNow )
                throw new ArgumentException("Date to start Data Read cannot be in the future.", "date");

            if ( date < DateTime.UtcNow.AddDays(-6) )
                throw new ArgumentException("Date to start Data Read cannot be greater than 7 days in the past.", "date");

            if ( hoursToRead > 3 )
                throw new ArgumentException("Hours to Read cannot be greater than 3.", "hoursToRead");

            if ( hoursToRead < 1 )
                throw new ArgumentException("Hours to Read cannot be less than 1.", "hoursToRead");

            var dayOfWeek = (Byte)(date.DayOfWeek == 0 ? 7 : (Int32)date.DayOfWeek);

            return new BlockityCommand<IEnumerable<Data>> {
                RequestBytes = new Byte[] {
                    (Byte)CommandByte.ReadDataRequest,
                    3, dayOfWeek, (Byte)date.Hour, hoursToRead,
                    BlockityHelpers.GetCrc(new Byte[] { dayOfWeek, (Byte)date.Hour, hoursToRead })
                },
                ResponseCommand = new BlockityCommandDelegate<IEnumerable<Data>>((Byte[] input) => {
                    return ResponseCommands.GetDataResponse(date, input);
                })
            };
        }

        public static BlockityCommand<Byte[]> GetDeviceData(Byte lid) {
            return new BlockityCommand<Byte[]> {
                RequestBytes = new Byte[] { (Byte)CommandByte.ReadDeviceDataRequest, 1, lid, lid },
                ResponseCommand = new BlockityCommandDelegate<Byte[]>(ResponseCommands.RawResponse)
            };
        }

        public static BlockityCommand<Byte> GetBatteryLevel() {
            return new BlockityCommand<Byte> {
                RequestBytes = new Byte[] { (Byte)CommandByte.ReadDeviceDataRequest, 1, },
                ResponseCommand = new BlockityCommandDelegate<Byte>((Byte[] input) => {
                    return ResponseCommands.RawResponse(input)[0];
                })
            };
        }

        public static BlockityCommand<Boolean> ClearData() {
            return new BlockityCommand<Boolean> {
                RequestBytes = new Byte[] { (Byte)CommandByte.ClearDataRequest, 0, 0 },
                ResponseCommand = new BlockityCommandDelegate<Boolean>((Byte[] input) => {
                    return ResponseCommands.SimpleResponse(CommandByte.ClearDataRequest, input);
                })
            };
        }
        
        public static BlockityCommand<Boolean> FactoryReset() {
            return new BlockityCommand<Boolean> {
                RequestBytes = new Byte[] { (Byte)CommandByte.FactoryResetRequest, 0, 0 },
                ResponseCommand = new BlockityCommandDelegate<Boolean>((Byte[] input) => {
                    return ResponseCommands.SimpleResponse(CommandByte.FactoryResetResponse, input);
                })
            };
        }
    }
}