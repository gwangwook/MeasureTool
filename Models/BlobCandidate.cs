using System.Collections.Generic;
using System.Drawing;

namespace MeasureTool
{
    internal class BlobCandidate
    {
        public List<PointF> ContourPoints = new List<PointF>();

        public Rectangle BoundingRect;
        public PointF Center;

        public double Area;
        public double Perimeter;
        public double Circularity;
        public double AspectRatio;
        public double Solidity;
        public double Extent;

        public float Radius;

        public bool HasEllipse;
        public PointF EllipseCenter;
        public SizeF EllipseSize;
        public float EllipseAngle;

        public bool Selected;
        public string Message;

        public bool IsVirtual;
        public List<BlobCandidate> GroupPieces = new List<BlobCandidate>();
    }
}
