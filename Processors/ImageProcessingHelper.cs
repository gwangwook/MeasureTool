using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MeasureTool
{
    internal static class ImageProcessingHelper
    {
        public static Bitmap CloneAs24Bpp(Bitmap source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Bitmap clone = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(clone))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            return clone;
        }

        public static Bitmap CreatePreviewSource(Bitmap source, int maxWidth, int maxHeight)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (maxWidth <= 0)
                maxWidth = source.Width;

            if (maxHeight <= 0)
                maxHeight = source.Height;

            int srcW = source.Width;
            int srcH = source.Height;

            double scaleX = (double)maxWidth / srcW;
            double scaleY = (double)maxHeight / srcH;
            double scale = Math.Min(scaleX, scaleY);

            if (scale > 1.0)
                scale = 1.0;

            int dstW = Math.Max(1, (int)(srcW * scale));
            int dstH = Math.Max(1, (int)(srcH * scale));

            Bitmap preview = new Bitmap(dstW, dstH, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(preview))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(source, 0, 0, dstW, dstH);
            }

            return preview;
        }

        public static void SetGrayPalette(Bitmap bitmap)
        {
            if (bitmap == null)
                return;

            if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                return;

            ColorPalette palette = bitmap.Palette;

            for (int i = 0; i < 256; i++)
                palette.Entries[i] = Color.FromArgb(i, i, i);

            bitmap.Palette = palette;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        public static double GetPointDistance(PointF p1, PointF p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Rectangle ClampRectangleToImage(Rectangle rect, int imageWidth, int imageHeight)
        {
            return ClampRectangleToImage(rect, imageWidth, imageHeight, 1, 1);
        }

        public static Rectangle ClampRectangleToImage(
            Rectangle rect,
            int imageWidth,
            int imageHeight,
            int minWidth,
            int minHeight)
        {
            if (imageWidth <= 0 || imageHeight <= 0)
                return Rectangle.Empty;

            if (minWidth < 1)
                minWidth = 1;

            if (minHeight < 1)
                minHeight = 1;

            int x = rect.X;
            int y = rect.Y;
            int width = rect.Width;
            int height = rect.Height;

            if (width < minWidth)
                width = minWidth;

            if (height < minHeight)
                height = minHeight;

            if (width > imageWidth)
                width = imageWidth;

            if (height > imageHeight)
                height = imageHeight;

            if (x < 0)
                x = 0;

            if (y < 0)
                y = 0;

            if (x + width > imageWidth)
                x = imageWidth - width;

            if (y + height > imageHeight)
                y = imageHeight - height;

            if (x < 0)
                x = 0;

            if (y < 0)
                y = 0;

            return new Rectangle(x, y, width, height);
        }

        public static Rectangle ExpandRectangleToImage(Rectangle rect, int padding, int imageWidth, int imageHeight)
        {
            if (imageWidth <= 0 || imageHeight <= 0)
                return Rectangle.Empty;

            if (padding < 0)
                padding = 0;

            int left = rect.Left - padding;
            int top = rect.Top - padding;
            int right = rect.Right + padding;
            int bottom = rect.Bottom + padding;

            if (left < 0)
                left = 0;

            if (top < 0)
                top = 0;

            if (right > imageWidth)
                right = imageWidth;

            if (bottom > imageHeight)
                bottom = imageHeight;

            if (right <= left || bottom <= top)
                return Rectangle.Empty;

            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        public static bool IsPointInsideRectangle(PointF point, Rectangle rect)
        {
            if (point.X < rect.Left)
                return false;

            if (point.X > rect.Right)
                return false;

            if (point.Y < rect.Top)
                return false;

            if (point.Y > rect.Bottom)
                return false;

            return true;
        }

        public static bool IsInsideImage(double x, double y, int imageWidth, int imageHeight)
        {
            if (x < 0.0 || y < 0.0)
                return false;

            if (x >= imageWidth - 1 || y >= imageHeight - 1)
                return false;

            return true;
        }

        public static double GetBilinearGrayValue(byte[] grayBuffer, int imageWidth, int imageHeight, double x, double y)
        {
            if (grayBuffer == null)
                return 0.0;

            if (!IsInsideImage(x, y, imageWidth, imageHeight))
                return 0.0;

            int x0 = (int)Math.Floor(x);
            int y0 = (int)Math.Floor(y);
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            double dx = x - x0;
            double dy = y - y0;

            double v00 = grayBuffer[y0 * imageWidth + x0];
            double v10 = grayBuffer[y0 * imageWidth + x1];
            double v01 = grayBuffer[y1 * imageWidth + x0];
            double v11 = grayBuffer[y1 * imageWidth + x1];

            double v0 = v00 * (1.0 - dx) + v10 * dx;
            double v1 = v01 * (1.0 - dx) + v11 * dx;

            return v0 * (1.0 - dy) + v1 * dy;
        }

        public static bool TryCreateGrayBuffer(Bitmap source, out byte[] grayBuffer, out int width, out int height)
        {
            grayBuffer = null;
            width = 0;
            height = 0;

            if (source == null)
                return false;

            width = source.Width;
            height = source.Height;

            Bitmap src24 = null;
            BitmapData data = null;

            try
            {
                src24 = CloneAs24Bpp(source);
                Rectangle rect = new Rectangle(0, 0, width, height);

                data = src24.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                int stride = data.Stride;
                int bytes = Math.Abs(stride) * height;

                byte[] buffer = new byte[bytes];
                Marshal.Copy(data.Scan0, buffer, 0, bytes);

                grayBuffer = new byte[width * height];

                for (int y = 0; y < height; y++)
                {
                    int srcRow = y * stride;
                    int dstRow = y * width;

                    for (int x = 0; x < width; x++)
                    {
                        int srcIdx = srcRow + x * 3;

                        byte b = buffer[srcIdx + 0];
                        byte g = buffer[srcIdx + 1];
                        byte r = buffer[srcIdx + 2];

                        int gray = (r * 299 + g * 587 + b * 114 + 500) / 1000;

                        if (gray < 0)
                            gray = 0;

                        if (gray > 255)
                            gray = 255;

                        grayBuffer[dstRow + x] = (byte)gray;
                    }
                }

                return true;
            }
            catch
            {
                grayBuffer = null;
                width = 0;
                height = 0;
                return false;
            }
            finally
            {
                if (data != null && src24 != null)
                    src24.UnlockBits(data);

                if (src24 != null)
                    src24.Dispose();
            }
        }

        public static OpenCvSharp.Mat CreateGrayMatFromBuffer(byte[] grayBuffer, int imageWidth, int imageHeight, Rectangle roi)
        {
            if (grayBuffer == null)
                return null;

            if (imageWidth <= 0 || imageHeight <= 0)
                return null;

            Rectangle validRoi = ClampRectangleToImage(roi, imageWidth, imageHeight);

            if (validRoi.Width <= 0 || validRoi.Height <= 0)
                return null;

            byte[] roiBuffer = new byte[validRoi.Width * validRoi.Height];

            for (int y = 0; y < validRoi.Height; y++)
            {
                int srcY = validRoi.Y + y;
                int srcRow = srcY * imageWidth + validRoi.X;
                int dstRow = y * validRoi.Width;

                Buffer.BlockCopy(grayBuffer, srcRow, roiBuffer, dstRow, validRoi.Width);
            }

            OpenCvSharp.Mat mat = new OpenCvSharp.Mat(validRoi.Height, validRoi.Width, OpenCvSharp.MatType.CV_8UC1);

            Marshal.Copy(roiBuffer, 0, mat.Data, roiBuffer.Length);

            return mat;
        }

        public static byte GetGrayValue(byte[] grayBuffer, int width, int height, int x, int y)
        {
            if (grayBuffer == null)
                return 0;

            if (width <= 0 || height <= 0)
                return 0;

            if (x < 0 || y < 0 || x >= width || y >= height)
                return 0;

            int index = y * width + x;

            if (index < 0 || index >= grayBuffer.Length)
                return 0;

            return grayBuffer[index];
        }
    }
}
