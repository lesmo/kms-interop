using System;
using System.Collections.Generic;

using System.Text;

namespace KMS.Comm.InnerCore {
    public enum InnerCoreCommand {
        SetBluetoothNameRequest  = 0x81,
        SetBluetoothNameResponse = 0x21,
        SetBluetoothNameError    = 0x01,

        SetBluetoothPinRequest  = 0x85,
        SetBluetoothPinResponse = 0x25,
        SetBluetoothPinError    = 0x05,

        SetPersonalParametersRequest  = 0x83,
        SetPersonalParametersResponse = 0x23,
        SetPersonalParametersError    = 0x03,

        ReadDataRequest       = 0xC4,
        ReadDataResponse      = 0x24,
        ReadDataResponseError = 0x04,

        ReadDateDataRequest  = 0xC7,
        ReadDateDataResponse = 0x31,
        ReadDateDataError    = 0x0B,

        ReadDeviceDataRequest  = 0xC6,
        ReadDeviceDataResponse = 0x26,
        ReadDeviceDataError    = 0x06,

        ReadTodayDataRequest  = 0xC8,
        ReadTodayDataResponse = 0x30,
        ReadTodayDataError    = 0x0A,
        
        FactoryResetRequest  = 0x87,
        FactoryResetResponse = 0x27,
        FactoryResetError    = 0x07,

        ClearDataRequest = 0x88,
        ClearDataResponse = 0x28,
        ClearDataError = 0x08,

        SetDateRequest  = 0xC2,
        SetDateResponse = 0x22,
        SetDateError    = 0x02,

        GetDateRequest = 0x89,
        GetDateResponse = 0x29,
        GetDateError    = 0x09
    }

    public interface ICoreCommand {
        ICoreCommandContent CommandContent {
            get;
            set;
        }

        byte[] Serialize();
        void Deserialize(byte[] input);
    }
}
