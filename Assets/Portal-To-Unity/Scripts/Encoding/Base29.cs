using System;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public static class Base29
    {
        public static readonly char[] Alphabet = "23456789BCDFGHJKLMNPQRSTVWXYZ".ToCharArray();

        public static char[] EncodeWebCode(ulong value)
        {
            if (!ValidTradingCardID(value))
                return null;

            char[] result = new char[11];

            for (int i = 0; i < 10; i++, value /= 29)
            {
                ulong index = value % 29;
                result[10 - (i + (i / 5))] = Alphabet[index];
            }
            result[5] = '-';
            return result;
        }

        public static ulong DecodeWebCode(char[] value)
        {
            if (value.Length != 11 || value[5] != '-')
                throw new ArgumentException("Invalid Web Code format");

            ulong cardNumber = 0;

            for (int i = 0; i < 11; i++)
            {
                if (i == 5) continue;

                char c = value[i];
                int index = Array.IndexOf(Alphabet, c);

                if (index == -1)
                    throw new ArgumentException($"Invalid character in Web Code: {c}");

                cardNumber = cardNumber * 29 + (ulong)index;
            }
            return cardNumber;
        }
    }
}