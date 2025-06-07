using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0xB0)]
    public unsafe struct SpyroTag_Skylander
    {
        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x40)]
        public struct MagicMomentRegion0
        {
            [FieldOffset(0x00)] public ushort experience2011;
            [FieldOffset(0x02)] public byte experience2011High;
            [FieldOffset(0x03)] public ushort money;
            [FieldOffset(0x05)] public uint cumulativeTime;
            [FieldOffset(0x09)] public byte areaSequence;
            [FieldOffset(0x0A)] public ushort crcType3;
            [FieldOffset(0x0C)] public ushort crcType2;
            [FieldOffset(0x0E)] public ushort crcType1;
            [FieldOffset(0x10)] public ushort flags;
            [FieldOffset(0x12)] public byte flagsHigh;
            [FieldOffset(0x13)] public byte platforms2011;
            [FieldOffset(0x14)] public ushort hat2011;
            [FieldOffset(0x16)] public byte regionCountIdentifier;
            [FieldOffset(0x17)] public byte platforms2013;
            [FieldOffset(0x18)] public ulong ownerID2011_12;
            [FieldOffset(0x20)] public fixed ushort nickname[16];
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x30)]
        public struct RemainingDataRegion0
        {
            [FieldOffset(0x00)] public TagTimeDate lastUsed;
            [FieldOffset(0x06)] public uint heroicChallenges2011;
            [FieldOffset(0x0A)] public ushort heroPoints;
            [FieldOffset(0x0C)] public fixed byte lastUsageID[3];
            [FieldOffset(0x0F)] public byte ownerCount;
            [FieldOffset(0x10)] public TagTimeDate firstUsed;
            [FieldOffset(0x20)] public fixed byte unknownUsageIdentifier[15];
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x10)]
        public struct MagicMomentRegion1
        {
            [FieldOffset(0x00)] public ushort crcType6;
            [FieldOffset(0x02)] public byte areaSequence;
            [FieldOffset(0x03)] public ushort experience2012;
            [FieldOffset(0x05)] public byte hat2012;
            [FieldOffset(0x06)] public ushort flags;
            [FieldOffset(0x08)] public uint experience2013;
            [FieldOffset(0x0C)] public byte hat2013_14;
            [FieldOffset(0x0D)] public byte trinket;
            [FieldOffset(0x0E)] public byte hat2015;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x30)]
        public struct RemainingDataRegion1
        {
            [FieldOffset(0x00)] public uint battlegroundsFlags;
            [FieldOffset(0x04)] public ushort heroicChallenges2012;
            [FieldOffset(0x06)] public byte heroicChallenges2012High;
            [FieldOffset(0x07)] public fixed byte quests2012[9];
            [FieldOffset(0x17)] public fixed byte quests2013[9];
        }

        [FieldOffset(0x00)] public MagicMomentRegion0 magicMomentRegion0;
        [FieldOffset(0x40)] public RemainingDataRegion0 remainingDataRegion0;
        [FieldOffset(0x70)] public MagicMomentRegion1 magicMomentRegion1;
        [FieldOffset(0x80)] public RemainingDataRegion1 remainingDataRegion1;

        public override readonly string ToString() => SpyroTagToHexString(this);
    }

    public class FigType_Skylander : FigType
    {
        public unsafe SpyroTag_Skylander* SpyroTag { get; protected set; }
        public byte DataArea0 = 0;
        public byte DataArea1 = 0;

#region FigType Info
        public unsafe FigType_Skylander(PortalFigure portalFigure) : base(portalFigure)
        {
            int size = sizeof(SpyroTag_Skylander);
            SpyroTag = (SpyroTag_Skylander*)Marshal.AllocHGlobal(size);

            for (int i = 0; i < size; i++)
                ((byte*)SpyroTag)[i] = 0;
        }

        public override unsafe byte* GetBlock(int block)
        {
            int offset = block * BLOCK_SIZE;

            if (offset < 0 || offset >= sizeof(SpyroTag_Skylander))
                throw new ArgumentOutOfRangeException(nameof(block), "Block index is out of range");

            return (byte*)SpyroTag + offset;
        }

        public override unsafe byte[] GetBlockManaged(int block)
        {
            int offset = block * BLOCK_SIZE;

            if (offset < 0 || offset >= sizeof(SpyroTag_Skylander))
                throw new ArgumentOutOfRangeException(nameof(block), "Block index is out of range");

            byte[] data = new byte[BLOCK_SIZE];

            fixed (byte* dataPtr = data)
                Buffer.MemoryCopy((byte*)SpyroTag + offset, dataPtr, BLOCK_SIZE, BLOCK_SIZE);

            return data;
        }

        public override unsafe void SetBlock(int block, byte* data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Pointer to block data cannot be null");

            int offset = block * BLOCK_SIZE;

            if (offset < 0 || offset >= sizeof(SpyroTag_Skylander))
                throw new ArgumentOutOfRangeException(nameof(block), "Block index is out of range");

            byte* destination = (byte*)SpyroTag + offset;
            Buffer.MemoryCopy(data, destination, BLOCK_SIZE, BLOCK_SIZE);
        }

        public override unsafe void SetBlockManaged(int block, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Pointer to block data cannot be null");

            int offset = block * BLOCK_SIZE;

            if (offset < 0 || offset >= sizeof(SpyroTag_Skylander))
                throw new ArgumentOutOfRangeException(nameof(block), "Block index is out of range");

            fixed (byte* dataPtr = data)
            {
                byte* destination = (byte*)SpyroTag + offset;
                Buffer.MemoryCopy(dataPtr, destination, BLOCK_SIZE, BLOCK_SIZE);
            }
        }

        public override unsafe void Dispose()
        {
            if (SpyroTag != null)
            {
                Marshal.FreeHGlobal((IntPtr)SpyroTag);
                SpyroTag = null;
            }
        }
#endregion

        public async Task FetchMagicMoment0()
        {
            byte[] block0 = await portalFigure.FetchBlock(DATA_REGION0_OFFSET);
            byte[] block1 = await portalFigure.FetchBlock(DATA_REGION1_OFFSET);

            if ((byte)(block0[0x09] + 1) == block1[0x09])
            {
                DataArea0 = 1;
                SetBlockManaged(0x00, block1);
            }
            else
            {
                if ((byte)(block1[0x09] + 1) != block0[0x09])
                    PTUManager.LogWarning("Data areas out of sync. First area chosen by default.", LogPriority.High);

                DataArea0 = 0;
                SetBlockManaged(0x00, block0);
            }

            int size = BlockSizeOf<SpyroTag_Skylander.MagicMomentRegion0>();
            int areaOffset = (DataArea0 == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0;

            for (int i = 1; i < size; i++)
            {
                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                SetBlockManaged(i, await portalFigure.FetchBlock((byte)(i + areaOffset + offset)));
            }
        }

        public async Task FetchRemainingData0()
        {
            int size = BlockSizeOf<SpyroTag_Skylander.RemainingDataRegion0>();
            int areaOffset = (DataArea0 == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0x05;

            for (int i = 0; i < size; i++)
            {
                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                SetBlockManaged(i + 0x04, await portalFigure.FetchBlock((byte)(i + areaOffset + offset)));
            }
        }

        public async Task FetchMagicMoment1()
        {
            byte[] block0 = await portalFigure.FetchBlock(DATA_REGION0_OFFSET + 0x09);
            byte[] block1 = await portalFigure.FetchBlock(DATA_REGION1_OFFSET + 0x09);

            if ((byte)(block0[0x02] + 1) == block1[0x02])
            {
                DataArea1 = 1;
                SetBlockManaged(0x07, block1);
            }
            else
            {
                if ((byte)(block1[0x02] + 1) != block0[0x02])
                    PTUManager.LogWarning("Data areas out of sync. First area chosen by default.", LogPriority.High);

                DataArea1 = 0;
                SetBlockManaged(0x07, block0);
            }
        }

        public async Task FetchRemainingData1()
        {
            int size = BlockSizeOf<SpyroTag_Skylander.RemainingDataRegion1>();
            int areaOffset = (DataArea1 == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0x0A;

            for (int i = 0; i < size; i++)
            {
                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                SetBlockManaged(i + 0x08, await portalFigure.FetchBlock((byte)(i + areaOffset + offset)));
            }
        }

        public unsafe void SetUpRegionHeader0()
        {
            SpyroTag->magicMomentRegion0.crcType3 = CalculateCRCType3();
            SpyroTag->magicMomentRegion0.crcType2 = CalculateCRCType2();
            SpyroTag->magicMomentRegion0.areaSequence++;
            SpyroTag->magicMomentRegion0.crcType1 = CalculateCRCType1();
        }

        public unsafe void SetUpRegionHeader1()
        {
            SpyroTag->magicMomentRegion1.areaSequence++;
            SpyroTag->magicMomentRegion1.crcType6 = CalculateCRCType6();
        }

        private unsafe ushort CalculateCRCType3()
        {
            Checksum crcType3 = new Checksum(sizeof(SpyroTag_Skylander.RemainingDataRegion0), (IntPtr)(&SpyroTag->magicMomentRegion0.crcType3));
            return crcType3.Calculate((IntPtr)(&SpyroTag->remainingDataRegion0), 0xE0);
        }

        private unsafe ushort CalculateCRCType2()
        {
            Checksum crcType2 = new Checksum(BLOCK_SIZE * 3, (IntPtr)(&SpyroTag->magicMomentRegion0.crcType2));
            return crcType2.Calculate((IntPtr)(&SpyroTag->magicMomentRegion0) + BLOCK_SIZE);
        }

        private unsafe ushort CalculateCRCType1()
        {
            Checksum crcType1 = new Checksum(BLOCK_SIZE, (IntPtr)(&SpyroTag->magicMomentRegion0.crcType1), 0x0500);
            return crcType1.Calculate((IntPtr)(&SpyroTag->magicMomentRegion0));
        }

        private unsafe ushort CalculateCRCType6()
        {
            Checksum crcType6 = new Checksum(sizeof(SpyroTag_Skylander.MagicMomentRegion1) + sizeof(SpyroTag_Skylander.RemainingDataRegion1), (IntPtr)(&SpyroTag->magicMomentRegion1.crcType6), 0x0601);
            return crcType6.Calculate((IntPtr)(&SpyroTag->magicMomentRegion1));
        }

        public unsafe void EvaluateChecksums()
        {
            PTUManager.LogWarning(SpyroTag->magicMomentRegion0.crcType3.ToString(), LogPriority.Normal);
            PTUManager.LogError(CalculateCRCType3().ToString(), LogPriority.Normal);
            PTUManager.LogWarning(SpyroTag->magicMomentRegion0.crcType2.ToString(), LogPriority.Normal);
            PTUManager.LogError(CalculateCRCType2().ToString(), LogPriority.Normal);
            PTUManager.LogWarning(SpyroTag->magicMomentRegion0.crcType1.ToString(), LogPriority.Normal);
            PTUManager.LogError(CalculateCRCType1().ToString(), LogPriority.Normal);
            PTUManager.LogWarning(SpyroTag->magicMomentRegion1.crcType6.ToString(), LogPriority.Normal);
            PTUManager.LogError(CalculateCRCType6().ToString(), LogPriority.Normal);
        }

        public async Task SetMagicMoment0()
        {
            SetUpRegionHeader0();
            DataArea0 = (byte)((DataArea0 + 1) & 1);
            int size = BlockSizeOf<SpyroTag_Skylander.MagicMomentRegion0>();
            int areaOffset = (DataArea0 == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0;

            for (int i = 0; i < size; i++)
            {
                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                await portalFigure.WriteBlock((byte)(i + areaOffset + offset), GetBlockManaged(i));
            }
        }

        public async Task SetRemainingData0()
        {
            int size = BlockSizeOf<SpyroTag_Skylander.RemainingDataRegion0>();
            int areaOffset = (DataArea0 == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0x05;

            for (int i = 0; i < size; i++)
            {
                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                await portalFigure.WriteBlock((byte)(i + areaOffset + offset), GetBlockManaged(i + 0x04));
            }
        }

        public async Task SetMagicMoment1()
        {
            SetUpRegionHeader1();
            DataArea1 = (byte)((DataArea1 + 1) & 1);
            int areaOffset = (DataArea1 == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            await portalFigure.WriteBlock((byte)(areaOffset + 0x09), GetBlockManaged(0x07));
        }

        public async Task SetRemainingData1()
        {
            int size = BlockSizeOf<SpyroTag_Skylander.RemainingDataRegion1>();
            int areaOffset = (DataArea1 == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0x0A;

            for (int i = 0; i < size; i++)
            {
                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                await portalFigure.WriteBlock((byte)(i + areaOffset + offset), GetBlockManaged(i + 0x08));
            }
        }
    }
}