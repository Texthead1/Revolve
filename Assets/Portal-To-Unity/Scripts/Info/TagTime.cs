using System;
using System.Runtime.InteropServices;

namespace PortalToUnity
{
    [StructLayout(LayoutKind.Sequential, Size = 0x06)]
    public struct TagTimeDate
    {
        public byte Minute;
        public byte Hour;
        public byte Day;
        public byte Month;
        public ushort Year;

        public TagTimeDate(byte minute, byte hour, byte day, byte month, ushort year)
        {
            Minute = minute;
            Hour = hour;
            Day = day;
            Month = month;
            Year = year;
        }

        public readonly string ToUSTimeAndDate() => $"{Hour:D2}:{Minute:D2}:00 {Month:D2}/{Day:D2}/{Year}";
        public readonly string ToEUTimeAndDate() => $"{Hour:D2}:{Minute:D2}:00 {Day:D2}/{Month:D2}/{Year}";
        public readonly string ToUSDate() => $"{Month:D2}/{Day:D2}/{Year}";
        public readonly string ToEUDate() => $"{Day:D2}/{Month:D2}/{Year}";
        public override readonly string ToString() => $"Minute: {Minute:D2}, Hour: {Hour:D2}, Day: {Day:D2}, Month: {Month:D2}, Year: {Year}";
    }

    public struct CumulativeSeconds
    {
        public uint RawSeconds;
        public TimeSpan TimeSpan;

        public CumulativeSeconds(uint seconds)
        {
            RawSeconds = seconds;
            TimeSpan = TimeSpan.FromSeconds(seconds);
        }

        public readonly string ToHoursMinutesSeconds() => TimeSpan.ToString("hh\\:mm\\:ss");
    }
}