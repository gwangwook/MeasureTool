using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MeasureTool
{
    public partial class MainForm
    {
        private double GetPointDistance(PointF p1, PointF p2)
        {
            return ImageProcessingHelper.GetPointDistance(p1, p2);
        }

        private double GetDistance(Point p1, Point p2)
        {
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        private Rectangle ClampRectangleToImage(Rectangle rect, int imageWidth, int imageHeight)
        {
            return ImageProcessingHelper.ClampRectangleToImage(
                rect,
                imageWidth,
                imageHeight,
                RoiMinWidth,
                RoiMinHeight);
        }

        private Rectangle ClampRectangleToImage(Rectangle rect)
        {
            if (_loadedImage == null)
                return rect;

            return ClampRectangleToImage(rect, _loadedImage.Width, _loadedImage.Height);
        }

        private Rectangle NormalizeRectangle(Rectangle rect)
        {
            int left = Math.Min(rect.Left, rect.Right);
            int right = Math.Max(rect.Left, rect.Right);
            int top = Math.Min(rect.Top, rect.Bottom);
            int bottom = Math.Max(rect.Top, rect.Bottom);

            int width = right - left;
            int height = bottom - top;

            if (width < RoiMinWidth)
                width = RoiMinWidth;

            if (height < RoiMinHeight)
                height = RoiMinHeight;

            return new Rectangle(left, top, width, height);
        }

        private Rectangle ExpandRectangleToImage(Rectangle rect, int padding, int imageWidth, int imageHeight)
        {
            return ImageProcessingHelper.ExpandRectangleToImage(rect, padding, imageWidth, imageHeight);
        }

        private bool IsPointInsideRectangle(PointF point, Rectangle rect)
        {
            return ImageProcessingHelper.IsPointInsideRectangle(point, rect);
        }

        private bool IsInsideImage(double x, double y, int imageWidth, int imageHeight)
        {
            return ImageProcessingHelper.IsInsideImage(x, y, imageWidth, imageHeight);
        }

        private double GetBilinearGrayValue(byte[] grayBuffer, int imageWidth, int imageHeight, double x, double y)
        {
            return ImageProcessingHelper.GetBilinearGrayValue(grayBuffer, imageWidth, imageHeight, x, y);
        }

        private bool TryCreateGrayBuffer(Bitmap source, out byte[] grayBuffer, out int width, out int height)
        {
            return ImageProcessingHelper.TryCreateGrayBuffer(source, out grayBuffer, out width, out height);
        }

        private OpenCvSharp.Mat CreateGrayMatFromBuffer(byte[] grayBuffer, int imageWidth, int imageHeight, Rectangle roi)
        {
            return ImageProcessingHelper.CreateGrayMatFromBuffer(grayBuffer, imageWidth, imageHeight, roi);
        }

        private byte GetGrayValue(byte[] grayBuffer, int width, int height, int x, int y)
        {
            return ImageProcessingHelper.GetGrayValue(grayBuffer, width, height, x, y);
        }
    }
}
