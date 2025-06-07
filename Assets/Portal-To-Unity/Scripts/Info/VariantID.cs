using System;

namespace PortalToUnity
{
    [Serializable]
    public struct VariantID
    {
        public SkylandersGame YearCode;
        public bool IsRepose, IsAltDeco, IsLightCore, IsSuperCharger;
        public int DecoID;

        public VariantID(ushort variantID)
        {
            YearCode = (SkylandersGame)((variantID >> 0xC) & 0xF);
            IsRepose = ((variantID >> 0xB) & 0x1) != 0;
            IsAltDeco = ((variantID >> 0xA) & 0x1) != 0;
            IsLightCore = ((variantID >> 0x9) & 0x1) != 0;
            IsSuperCharger = ((variantID >> 0x8) & 0x1) != 0;
            DecoID = variantID & 0xFF;
        }

        public VariantID(SkylandersGame yearCode, bool isRepose, bool isAltDeco, bool isLightCore, bool isSuperCharger, byte decoID)
        {
            YearCode = yearCode;
            IsRepose = isRepose;
            IsAltDeco = isAltDeco;
            IsLightCore = isLightCore;
            IsSuperCharger = isSuperCharger;
            DecoID = decoID;
        }

        public VariantID(SkylandersGame yearCode, bool isRepose, bool isAltDeco, bool isLightCore, bool isSuperCharger, int decoID)
        {
            YearCode = yearCode;
            IsRepose = isRepose;
            IsAltDeco = isAltDeco;
            IsLightCore = isLightCore;
            IsSuperCharger = isSuperCharger;
            DecoID = decoID;
        }

        public readonly ushort Encode() => (ushort)((byte)DecoID | ((IsSuperCharger ? 1 : 0) << 0x8) | ((IsLightCore ? 1 : 0) << 0x9) | ((IsAltDeco ? 1 : 0) << 0xA) | ((IsRepose ? 1 : 0) << 0xB) | ((int)YearCode << 0xC));

        public override readonly string ToString() => $"YearCode: {YearCode}, IsRepose: {IsRepose}, IsAltDeco: {IsAltDeco}, IsLightCore: {IsLightCore}, IsSuperCharger: {IsSuperCharger}, DecoID: {DecoID}, VariantShort: {Encode()}";
    }
}