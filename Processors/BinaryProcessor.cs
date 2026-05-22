using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MeasureTool
{
    public static class BinaryProcessor
    {
        public static Bitmap CloneAs24Bpp(Bitmap source)
        {
            return ImageProcessingHelper.CloneAs24Bpp(source);
        }

        public static Bitmap CreatePreviewSource(Bitmap source, int maxWidth, int maxHeight)
        {
            return ImageProcessingHelper.CreatePreviewSource(source, maxWidth, maxHeight);
        }

        public static Bitmap CreateBinaryBitmap(Bitmap source, int threshold, bool inverse)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            threshold = ImageProcessingHelper.Clamp(threshold, 0, 255);

            int width = source.Width;
            int height = source.Height;

            Bitmap src24 = ImageProcessingHelper.CloneAs24Bpp(source);

            Bitmap dst8 = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ImageProcessingHelper.SetGrayPalette(dst8);

            Rectangle rect = new Rectangle(0, 0, width, height);

            BitmapData srcData = null;
            BitmapData dstData = null;

            try
            {
                srcData = src24.LockBits(
                    rect,
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);

                dstData = dst8.LockBits(
                    rect,
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);

                int srcStride = srcData.Stride;
                int dstStride = dstData.Stride;

                int srcBytes = Math.Abs(srcStride) * height;
                int dstBytes = Math.Abs(dstStride) * height;

                byte[] srcBuffer = new byte[srcBytes];
                byte[] dstBuffer = new byte[dstBytes];

                Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBytes);

                for (int y = 0; y < height; y++)
                {
                    int srcRow = y * srcStride;
                    int dstRow = y * dstStride;

                    for (int x = 0; x < width; x++)
                    {
                        int srcIdx = srcRow + x * 3;
                        int dstIdx = dstRow + x;

                        byte b = srcBuffer[srcIdx + 0];
                        byte g = srcBuffer[srcIdx + 1];
                        byte r = srcBuffer[srcIdx + 2];

                        int gray = (r * 299 + g * 587 + b * 114 + 500) / 1000;

                        bool white = gray > threshold;

                        if (inverse)
                            white = !white;

                        dstBuffer[dstIdx] = white ? (byte)255 : (byte)0;
                    }
                }

                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBytes);
            }
            finally
            {
                if (srcData != null)
                    src24.UnlockBits(srcData);

                if (dstData != null)
                    dst8.UnlockBits(dstData);

                src24.Dispose();
            }

            return dst8;
        }

        public static void SetGrayPalette(Bitmap bitmap)
        {
            ImageProcessingHelper.SetGrayPalette(bitmap);
        }
    }
}
