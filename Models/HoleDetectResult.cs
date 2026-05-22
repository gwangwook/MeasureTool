using System;
using System.Collections.Generic;
using System.Drawing;

namespace MeasureTool
{
    internal class HoleDetectResult : IDisposable
    {
        public bool Found;
        public string Message;

        public Rectangle Roi;
        public PointF Center;

        public BlobCandidate SelectedCandidate;
        public List<BlobCandidate> Candidates = new List<BlobCandidate>();

        // 검출 과정에서 만든 Preview 이미지.
        // 호출 측이 Result를 보관하는 동안 함께 보관/Dispose 한다.
        public Bitmap BinaryPreview;
        public Bitmap DetectPreview;

        public void Dispose()
        {
            if (BinaryPreview != null) { BinaryPreview.Dispose(); BinaryPreview = null; }
            if (DetectPreview != null) { DetectPreview.Dispose(); DetectPreview = null; }
        }
    }
}