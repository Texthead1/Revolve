using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static PortalToUnity.Global;

namespace PortalToUnity
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x150)]
    public unsafe struct SpyroTag_Trap
    {
        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x40)]
        public struct MagicMoment
        {
            [FieldOffset(0x00)] public byte containsVariantVillain;
            [FieldOffset(0x01)] public byte villainCount;
            [FieldOffset(0x07)] public byte variantVillainID;
            [FieldOffset(0x09)] public byte areaSequence;
            [FieldOffset(0x0A)] public ushort crcTrap;
            [FieldOffset(0x0C)] public ushort crcType2;
            [FieldOffset(0x0E)] public ushort crcType1;
            [FieldOffset(0x10)] public SpyroTag_Villain primaryVillain;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x110)]
        public struct RemainingData
        {
            [FieldOffset(0x00)] public SpyroTag_Villain cachedVillain0;
            [FieldOffset(0x30)] public SpyroTag_Villain cachedVillain1;
            [FieldOffset(0x60)] public SpyroTag_Villain cachedVillain2;
            [FieldOffset(0x90)] public SpyroTag_Villain cachedVillain3;
            [FieldOffset(0xB0)] public SpyroTag_Villain cachedVillain4;
            [FieldOffset(0xE0)] public fixed byte unknownUsageIdentifier[15];
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x30)]
        public unsafe struct SpyroTag_Villain
        {
            [FieldOffset(0x00)] public byte villainID;
            [FieldOffset(0x01)] public byte isEvolved;
            [FieldOffset(0x02)] public byte hat;
            [FieldOffset(0x03)] public byte trinket;
            [FieldOffset(0x04)] public fixed ushort nickname[16];
        }

        [FieldOffset(0x00)] public MagicMoment magicMoment;
        [FieldOffset(0x40)] public RemainingData remainingData;
    }

    public class FigType_Trap : FigType
    {
        public unsafe SpyroTag_Trap* SpyroTag { get; protected set; }
        public byte DataArea = 0;

        public unsafe SpyroTag_Trap.SpyroTag_Villain[] GetCachedVillains() => new SpyroTag_Trap.SpyroTag_Villain[5] { SpyroTag->remainingData.cachedVillain0, SpyroTag->remainingData.cachedVillain1, SpyroTag->remainingData.cachedVillain2, SpyroTag->remainingData.cachedVillain3, SpyroTag->remainingData.cachedVillain4 };

#region FigType Info
        public unsafe FigType_Trap(PortalFigure portalFigure) : base(portalFigure)
        {
            int size = sizeof(SpyroTag_Trap);
            SpyroTag = (SpyroTag_Trap*)Marshal.AllocHGlobal(size);

            for (int i = 0; i < size; i++)
                ((byte*)SpyroTag)[i] = 0;
        }

        public override unsafe byte* GetBlock(int block)
        {
            int offset = block * BLOCK_SIZE;

            if (offset < 0 || offset >= sizeof(SpyroTag_Trap))
                throw new ArgumentOutOfRangeException(nameof(block), "Block index is out of range");

            return (byte*)SpyroTag + offset;
        }

        public override unsafe byte[] GetBlockManaged(int block)
        {
            int offset = block * BLOCK_SIZE;

            if (offset < 0 || offset >= sizeof(SpyroTag_Trap))
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

            if (offset < 0 || offset >= sizeof(SpyroTag_Trap))
                throw new ArgumentOutOfRangeException(nameof(block), "Block index is out of range");

            byte* destination = (byte*)SpyroTag + offset;
            Buffer.MemoryCopy(data, destination, BLOCK_SIZE, BLOCK_SIZE);
        }

        public override unsafe void SetBlockManaged(int block, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Pointer to block data cannot be null");

            int offset = block * BLOCK_SIZE;

            if (offset < 0 || offset >= sizeof(SpyroTag_Trap))
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

        public async Task FetchMagicMoment()
        {
            byte[] block0 = await portalFigure.FetchBlock(DATA_REGION0_OFFSET);
            byte[] block1 = await portalFigure.FetchBlock(DATA_REGION1_OFFSET);

            if ((byte)(block0[0x09] + 1) == block1[0x09])
            {
                DataArea = 1;
                SetBlockManaged(0x00, block1);
            }
            else
            {
                if ((byte)(block1[0x09] + 1) != block0[0x09])
                    PTUManager.LogWarning("Data areas out of sync. First area chosen by default.", LogPriority.High);

                DataArea = 0;
                SetBlockManaged(0x00, block0);
            }

            int size = BlockSizeOf<SpyroTag_Trap.MagicMoment>();
            int areaOffset = (DataArea == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0;

            for (int i = 1; i < size; i++)
            {
                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                SetBlockManaged(i, await portalFigure.FetchBlock((byte)(i + areaOffset + offset)));
            }
        }

        public async Task FetchRemainingData()
        {
            int size = BlockSizeOf<SpyroTag_Trap.RemainingData>();
            int areaOffset = (DataArea == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0x05;

            for (int i = 0; i < size; i++)
            {
                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                SetBlockManaged(i + 0x04, await portalFigure.FetchBlock((byte)(i + areaOffset + offset)));
            }
        }

        private unsafe void SetUpRegionHeader()
        {
            SpyroTag->magicMoment.crcTrap = CalculateCRCTrap();
            SpyroTag->magicMoment.crcType2 = CalculateCRCType2();
            SpyroTag->magicMoment.areaSequence++;
            SpyroTag->magicMoment.crcType1 = CalculateCRCType1();
        }

        private unsafe ushort CalculateCRCTrap()
        {
            Checksum crcTrap = new Checksum(sizeof(SpyroTag_Trap.RemainingData), (IntPtr)(&SpyroTag->magicMoment.crcTrap));
            return crcTrap.Calculate((IntPtr)(&SpyroTag->remainingData));
        }

        private unsafe ushort CalculateCRCType2()
        {
            Checksum crcType2 = new Checksum(BLOCK_SIZE * 3, (IntPtr)(&SpyroTag->magicMoment.crcType2));
            return crcType2.Calculate((IntPtr)(&SpyroTag->magicMoment) + BLOCK_SIZE);
        }

        private unsafe ushort CalculateCRCType1()
        {
            Checksum crcType1 = new Checksum(BLOCK_SIZE, (IntPtr)(&SpyroTag->magicMoment.crcType1), 0x0500);
            return crcType1.Calculate((IntPtr)(&SpyroTag->magicMoment));
        }

        public async Task SetMagicMoment()
        {
            SetUpRegionHeader();
            DataArea = (byte)((DataArea + 1) & 1);
            int size = BlockSizeOf<SpyroTag_Trap.MagicMoment>();
            int areaOffset = (DataArea == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0;

            for (int i = 0; i < size; i++)
            {
                byte[] data = GetBlockManaged(i);

                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                await portalFigure.WriteBlock((byte)(i + areaOffset + offset), data);
            }
        }

        public async Task SetRemainingData()
        {
            int size = BlockSizeOf<SpyroTag_Trap.RemainingData>();
            int areaOffset = (DataArea == 1) ? DATA_REGION1_OFFSET : DATA_REGION0_OFFSET;
            int offset = 0x05;

            for (int i = 0; i < size; i++)
            {
                byte[] data = GetBlockManaged(i + 0x04);

                if (IsAccessControlBlock((byte)(i + areaOffset + offset)))
                    offset++;

                await portalFigure.WriteBlock((byte)(i + areaOffset + offset), data);
            }
        }
    }
}