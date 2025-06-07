using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    public static class Cryptography
    {
        public static byte[] Salt { get; private set; } = null;
        public static bool SaltIsReady() => Salt != null;

        private static readonly MD5 md5 = MD5.Create();
        private static readonly Aes cipher = Aes.Create();

        private static ICryptoTransform encryptor => cipher.CreateEncryptor();
        private static ICryptoTransform decryptor => cipher.CreateDecryptor();

        private static readonly byte[] saltHash = new byte[]
        {
            0x73, 0x5D, 0x9A, 0xB9, 0x4E, 0xCA, 0x74, 0xD8, 0xE2, 0x50, 0x67, 0xF5, 0x8C, 0x4C, 0xCA, 0x6B,
            0x8E, 0x92, 0x6F, 0x39, 0xC8, 0x8E, 0x36, 0xB9, 0x33, 0xF9, 0xF0, 0x9D, 0x6D, 0x95, 0x32, 0x60
        };

        static unsafe Cryptography()
        {
            cipher.Mode = CipherMode.ECB;
            cipher.Padding = PaddingMode.Zeros;
        }

        public static bool CheckForSalt()
        {
            try
            {
                string saltPath = Path.Combine(Application.streamingAssetsPath, "salt.txt");

                if (!File.Exists(saltPath))
                    return false;

                using FileStream fs = new FileStream(saltPath, FileMode.Open);
                using BinaryReader reader = new BinaryReader(fs);
                byte[] input = reader.ReadBytes((int)fs.Length);

                if (input.Length != SALT_LENGTH)
                    return false;

                if (ComputeHash(input).SequenceEqual(saltHash))
                {
                    Salt = input;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                PTUManager.LogException(ex, LogPriority.High);
                return false;
            }
        }

        public static unsafe byte[] ComposeKey(SpyroTag_TagHeader* tagHeader)
        {
            byte[] key = new byte[KEY_SIZE];

            fixed (byte* buffer = key, salt = Salt)
            {
                Buffer.MemoryCopy(tagHeader, buffer, KEY_SIZE, TAG_HEADER_SIZE);
                Buffer.MemoryCopy(salt, buffer + 0x21, KEY_SIZE - 0x21, SALT_LENGTH);
            }
            return key;
        }

        public static unsafe void EncryptSpyroTagBlock(SpyroTag_TagHeader* tagHeader, byte* data, byte block)
        {
            if (!SaltIsReady())
                throw new CryptographicException("No salt.txt or an incorrect one was provided");

            byte[] key = ComposeKey(tagHeader);
            key[0x20] = block;
            cipher.Key = md5.ComputeHash(key);

            byte[] buffer = new byte[BLOCK_SIZE];
            Marshal.Copy((IntPtr)data, buffer, 0, BLOCK_SIZE);

            encryptor.TransformBlock(buffer, 0, BLOCK_SIZE, buffer, 0);
            Marshal.Copy(buffer, 0, (IntPtr)data, BLOCK_SIZE);
        }

        public static unsafe void DecryptSpyroTagBlock(SpyroTag_TagHeader* tagHeader, byte* data, byte block)
        {
            if (!SaltIsReady())
                throw new CryptographicException("No salt.txt or an incorrect one was provided");

            ulong* ulongPtr = (ulong*)data;
            if (ulongPtr[0] == 0 && ulongPtr[1] == 0)
                return;

            byte[] key = ComposeKey(tagHeader);
            key[0x20] = block;
            cipher.Key = md5.ComputeHash(key);

            byte[] buffer = new byte[BLOCK_SIZE];
            Marshal.Copy((IntPtr)data, buffer, 0, BLOCK_SIZE);

            decryptor.TransformBlock(buffer, 0, BLOCK_SIZE, buffer, 0);
            Marshal.Copy(buffer, 0, (IntPtr)data, BLOCK_SIZE);
        }

        public static byte[] ComputeHash(byte[] value) => SHA256.Create().ComputeHash(value);

        public static byte CalculateBCC(uint serial)
        {
            byte[] serialBytes = BitConverter.GetBytes(serial);
            return (byte)(serialBytes[0] ^ serialBytes[1] ^ serialBytes[2] ^ serialBytes[3]);
        }

        public static ulong CalculateCRC48(byte[] data)
        {
            const ulong polynomial = 0x42F0E1EBA9EA3693;
            ulong crc = 2ul * 2ul * 3ul * 1103ul * 12868356821ul;

            foreach (byte b in data)
            {
                crc ^= (ulong)b << 40;

                for (int j = 0; j < 8; j++)
                    crc = ((crc & 0x800000000000) != 0) ? (crc << 1) ^ polynomial : crc << 1;
            }
            return crc & 0x0000FFFFFFFFFFFF;
        }

        public static ulong CalculateKeyA(byte sector, uint serial)
        {
            if (sector == 0)
                return 73ul * 2017ul * 560381651ul;

            byte[] data = new byte[5];
            data[4] = sector;
            Array.Copy(BitConverter.GetBytes(serial), data, 4);

            ulong result = CalculateCRC48(data);
            ulong converted = result.RawChangeEndianness() >> 16;

            return converted;
        }
    }
}