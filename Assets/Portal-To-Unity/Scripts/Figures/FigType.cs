namespace PortalToUnity
{
    public abstract class FigType
    {
        public PortalFigure portalFigure;

        public FigType(PortalFigure portalFigure)
        {
            this.portalFigure = portalFigure;
        }

        public abstract unsafe byte* GetBlock(int block);

        public abstract unsafe byte[] GetBlockManaged(int block);

        public abstract unsafe void SetBlock(int block, byte* data);

        public abstract unsafe void SetBlockManaged(int block, byte[] data);

        public abstract unsafe void Dispose();
    }
}