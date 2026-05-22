using System;

namespace MeasureTool
{
    internal enum RoiHitType
    {
        None,
        Body,
        Left,
        Right,
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    internal enum ViewerMode
    {
        None,
        MeasureDistance
    }

    internal enum MeasureCalcMode
    {
        Free,
        XFixed, // P2.X = P1.X, Y 거리 측정
        YFixed  // P2.Y = P1.Y, X 거리 측정
    }

    internal enum RoiMeasureType
    {
        DualRoi,
        SingleRoiDualEdge
    }

    internal enum RoiScanDirection
    {
        TopToBottom,
        BottomToTop,
        LeftToRight,
        RightToLeft
    }

    internal enum RoiEdgePolarity
    {
        WhiteToBlack,
        BlackToWhite,
        Any
    }

    internal enum RoiEdgeSelection
    {
        Strongest,
        First
    }

    internal enum BlobPolarity
    {
        DarkBlob = 0,
        BrightBlob = 1
    }

    internal enum HoleCenterMethod
    {
        MinEnclosingCircle = 0,
        BoundingRect = 1
    }

    internal enum HvViewImageMode
    {
        Original = 0,
        HoleBinary = 1,
        HoleBinaryDetect = 2,
        VentBinary = 3,
        VentBinaryDetect = 4
    }

    internal enum VentThresholdMode
    {
        Manual = 0,
        Otsu = 1,
        Adaptive = 2
    }
}