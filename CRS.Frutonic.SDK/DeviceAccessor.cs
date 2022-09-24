using CRS.Domain;
using CRS.Futronic.SDK;

using System;

namespace CRS.Frutronic.SDK
{
    public class DeviceAccessor
    {
        public IResult<FingerprintDevice> AccessFingerprintDevice()
        {
            var handle = LibScanApi.ftrScanOpenDevice();

            if (handle != IntPtr.Zero)
            {
                return Result<FingerprintDevice>.Success(new FingerprintDevice(handle));
            }

            return Result<FingerprintDevice>.Fail("Could not access fingerprint device.");
        }
    }
}