using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MeasureTool
{
    internal class HoleLocalFillInfo
    {
        public bool Valid;
        public Rectangle FilledRect;
        public PointF FilledCenter;
        public double FilledArea;
        public double WhitePixelArea;
        public double WhiteToFilledRatio;
    }
}
