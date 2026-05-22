using System.Collections.Generic;
using System.Drawing;

namespace MeasureTool
{
    internal class CircleFitResult
    {
        public bool Found;
        public string Message;

        public PointF Center;
        public float Radius;

        public double RmsError;
        public int ValidPointCount;
        public int SampleCount;

        public List<PointF> EdgePoints = new List<PointF>();
    }
}
