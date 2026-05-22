using System.Collections.Generic;
using System.Drawing;

namespace MeasureTool
{
    internal class RoiEdgeResult
    {
        public bool Found;
        public string Message;

        public Rectangle Roi;
        public RoiScanDirection ScanDirection;
        public RoiEdgePolarity Polarity;

        public List<PointF> EdgePoints = new List<PointF>();
        public List<PointF> InlierPoints = new List<PointF>();

        public PointF RepresentativePoint;
        public PointF LineStart;
        public PointF LineEnd;

        public double AverageStrengthPercent;
        public int TotalScanLines;
        public int FoundScanLines;
    }
}
