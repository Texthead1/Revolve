using System;

namespace PortalToUnity
{
    public static class EndianHelper
    {
        public enum Endianness
        {
            LittleEndian,
            BigEndian,
        }

        public static Endianness NativeEndianness =>  BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;

        public static ushort RawChangeEndianness(this ushort value) => (ushort)((value >> 8) | (value << 8));
        public static uint RawChangeEndianness(this uint value) => ((value >> 24) & 0x000000FF) | ((value >> 8) & 0x0000FF00) | ((value << 8) & 0x00FF0000) | ((value << 24) & 0xFF000000);
        public static ulong RawChangeEndianness(this ulong value) => ((value & 0x00000000000000FFul) << 56)
                                                                    | ((value & 0x000000000000FF00ul) << 40)
                                                                    | ((value & 0x0000000000FF0000ul) << 24)
                                                                    | ((value & 0x00000000FF000000ul) << 8)
                                                                    | ((value & 0x000000FF00000000ul) >> 8)
                                                                    | ((value & 0x0000FF0000000000ul) >> 24)
                                                                    | ((value & 0x00FF000000000000ul) >> 40)
                                                                    | ((value & 0xFF00000000000000ul) >> 56);

        public static ushort ChangeLittleEndianValue(this ushort value, Endianness targetEndian) => targetEndian == Endianness.LittleEndian ? value : value.RawChangeEndianness();
        public static uint ChangeLittleEndianValue(this uint value, Endianness targetEndian) => targetEndian == Endianness.LittleEndian ? value : value.RawChangeEndianness();
        public static ulong ChangeLittleEndianValue(this ulong value, Endianness targetEndian) => targetEndian == Endianness.LittleEndian ? value : value.RawChangeEndianness();

        public static ushort ChangeBigEndianValue(this ushort value, Endianness targetEndian) => value.ChangeLittleEndianValue(targetEndian == Endianness.LittleEndian ? Endianness.BigEndian : Endianness.LittleEndian);
        public static uint ChangeBigEndianValue(this uint value, Endianness targetEndian) => value.ChangeLittleEndianValue(targetEndian == Endianness.LittleEndian ? Endianness.BigEndian : Endianness.LittleEndian);
        public static ulong ChangeBigEndianValue(this ulong value, Endianness targetEndian) => value.ChangeLittleEndianValue(targetEndian == Endianness.LittleEndian ? Endianness.BigEndian : Endianness.LittleEndian);

        public static ushort ChangeNativeEndianValue(this ushort value, Endianness targetEndian) => NativeEndianness == targetEndian ? value : value.RawChangeEndianness();
        public static uint ChangeNativeEndianValue(this uint value, Endianness targetEndian) => NativeEndianness == targetEndian ? value : value.RawChangeEndianness();
        public static ulong ChangeNativeEndianValue(this ulong value, Endianness targetEndian) => NativeEndianness == targetEndian ? value : value.RawChangeEndianness();
    }
}