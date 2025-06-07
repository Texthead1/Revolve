using libusbK;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public class Xbox360PortalOfPower : PortalOfPower
    {
        public Xbox360PortalOfPower(KLST_DEVINFO_HANDLE handle) : base()
        {
            kDevice = new UsbK(handle);
            kHandle = handle;
        }

        public override string GetName() => PortalDatabase.NameFromID(ID) + " (Xbox 360)";

        public override bool PushTransfer(byte[] data, byte length = 0)
        {
            Cache(data);
            PTUManager.LogWarning($"OUTPUT ({(char)data[0]}): {BytesToHexString(data)}", LogPriority.Low);
            byte[] formatData = new byte[data.Length + XBOX360_DATA_HEADER.Length];
            Array.Copy(XBOX360_DATA_HEADER, formatData, XBOX360_DATA_HEADER.Length);
            Array.Copy(data, 0, formatData, XBOX360_DATA_HEADER.Length, data.Length);
            return kDevice.WritePipe(0x02, formatData, (uint)formatData.Length, out _, IntPtr.Zero);
        }

        public override bool WriteRaw(byte[] data) => false;

        public async override void StartReading()
        {
            isReading = true;
            byte[] buffer = new byte[REPORT_SIZE];

            while (isReading)
            {
                /*
                 * hopefully fixes an issue where the c# libusbK wrapper can return multiple stale pipe reads when the device is unplugged but before the removal is detected
                 * the small delay should negate the race condition and the minimal delay shouldn't be impactful
                 * now check if this occurs for 360 portals. the only portal i remember this issue happening with was the traptanium one, so you might need to get one of those :)
                 */
                await Task.Delay(3);

                if (await Task.Run(() => kDevice.ReadPipe(0x81, buffer, (uint)buffer.Length, out _, IntPtr.Zero)))
                {
                    if (buffer[0] == XBOX360_DATA_HEADER[0] && buffer[1] == XBOX360_DATA_HEADER[1])
                    {
                        byte[] strippedBuffer = new byte[buffer.Length - 2];
                        Array.Copy(buffer, 2, strippedBuffer, 0, strippedBuffer.Length);
                        ReportReceived(strippedBuffer);
                    }
                }
                Array.Clear(buffer, 0, buffer.Length);
            }
        }
    }
}