using System;

namespace PortalToUnity
{
    public readonly struct Checksum
    {
        readonly int length;
        readonly IntPtr selfPtr;
        readonly ushort placeholder;

        public Checksum(int length, IntPtr selfPtr, ushort placeholder = 0)
        {
            this.length = length;
            this.selfPtr = selfPtr;
            this.placeholder = placeholder;
        }

        public static unsafe void Update(ushort* crc, byte data)
        {
            const ushort polynomial = 0x1021;
            *crc ^= (ushort)(data << 8);

            for (int j = 0; j < 8; j++)
                *crc = (ushort)(((*crc & 0x8000) != 0) ? (*crc << 1) ^ polynomial : *crc << 1);
        }

        public readonly unsafe ushort Calculate(IntPtr ptr, uint padAmount = 0)
        {
            ushort crc = ushort.MaxValue;
            for (int i = 0; i < length; i++)
            {
                byte* bytePtr = (byte*)ptr + i;
                byte input = *bytePtr;

                if ((IntPtr)bytePtr == selfPtr)
                    input = (byte)((placeholder >> 8) & 0xFF);
                else if ((IntPtr)bytePtr == selfPtr + 1)
                    input = (byte)(placeholder & 0xFF);

                Update(&crc, input);
            }

            for (int i = 0; i < padAmount; i++)
                Update(&crc, 0x00);

            return crc;
        }
    }
}