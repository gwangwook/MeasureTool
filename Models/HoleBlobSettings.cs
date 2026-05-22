namespace MeasureTool
{
    internal class HoleBlobSettings
    {
        public int Threshold = 120;
        public int Morph = 0;
        public int MinArea = 100;
        public int MaxArea = 50000;

        public BlobPolarity Polarity = BlobPolarity.BrightBlob;
        public HoleCenterMethod CenterMethod = HoleCenterMethod.BoundingRect;

        public bool UseConvexHullFill = true;   // EnhancedBlob fill 시 ConvexHull+FillHoles 사용
    }
}