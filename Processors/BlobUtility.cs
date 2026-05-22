using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenCvSharp;
using CvRect = OpenCvSharp.Rect;
using CvPoint = OpenCvSharp.Point;
using CvSize = OpenCvSharp.Size;

namespace MeasureTool
{
    internal static class BlobUtility
    {
        public static int GetMorphKernelSize(int value)
        {
            if (value <= 0)
                return 0;

            if (value % 2 == 0)
                value += 1;

            return value;
        }

        public static PointF GetRectangleCenter(Rectangle rect)
        {
            return new PointF(
                rect.Left + rect.Width / 2.0f,
                rect.Top + rect.Height / 2.0f);
        }

        public static double GetDistance(PointF p1, PointF p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetAspectRatio(Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return 999.0;

            if (rect.Width >= rect.Height)
                return rect.Width / (double)rect.Height;

            return rect.Height / (double)rect.Width;
        }

        public static double GetCandidateAreaSum(List<BlobCandidate> candidates)
        {
            if (candidates == null)
                return 0.0;

            double sum = 0.0;

            for (int i = 0; i < candidates.Count; i++)
                sum += candidates[i].Area;

            return sum;
        }

        public static CvPoint[] ConvertPointFListToCvPoints(List<PointF> points)
        {
            CvPoint[] result = new CvPoint[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                result[i] = new CvPoint(
                    (int)Math.Round(points[i].X),
                    (int)Math.Round(points[i].Y));
            }

            return result;
        }

        public static void ApplyPostMorphology(Mat binary, int morphValue)
        {
            int k = GetMorphKernelSize(morphValue);

            if (k <= 1)
                return;

            using (Mat kernel = Cv2.GetStructuringElement(
                MorphShapes.Ellipse,
                new CvSize(k, k)))
            {
                Cv2.MorphologyEx(
                    binary,
                    binary,
                    MorphTypes.Close,
                    kernel);

                Cv2.MorphologyEx(
                    binary,
                    binary,
                    MorphTypes.Open,
                    kernel);
            }
        }

        public static Mat CreateDetectMaskFromBinary(
            Mat binary,
            BlobPolarity target,
            int morphValue)
        {
            Mat detectMask = new Mat();

            if (target == BlobPolarity.BrightBlob)
            {
                binary.CopyTo(detectMask);
            }
            else
            {
                Cv2.BitwiseNot(binary, detectMask);
            }

            ApplyPostMorphology(detectMask, morphValue);

            return detectMask;
        }

        public static List<BlobCandidate> ExtractCandidatesFromBinary(Mat binaryRoi, Rectangle imageRoi)
        {
            List<BlobCandidate> list = new List<BlobCandidate>();

            CvPoint[][] contours;
            HierarchyIndex[] hierarchy;

            using (Mat temp = binaryRoi.Clone())
            {
                Cv2.FindContours(
                    temp,
                    out contours,
                    out hierarchy,
                    RetrievalModes.External,
                    ContourApproximationModes.ApproxSimple);
            }

            if (contours == null)
                return list;

            for (int i = 0; i < contours.Length; i++)
            {
                CvPoint[] contour = contours[i];

                if (contour == null || contour.Length < 3)
                    continue;

                double area = Math.Abs(Cv2.ContourArea(contour));

                if (area <= 0.0)
                    continue;

                double perimeter = Cv2.ArcLength(contour, true);

                if (perimeter <= 0.0)
                    continue;

                CvRect br = Cv2.BoundingRect(contour);

                if (br.Width <= 0 || br.Height <= 0)
                    continue;

                BlobCandidate c = new BlobCandidate();

                c.Area = area;
                c.Perimeter = perimeter;
                c.Circularity = 4.0 * Math.PI * area / (perimeter * perimeter);
                c.AspectRatio = br.Width >= br.Height
                    ? br.Width / (double)br.Height
                    : br.Height / (double)br.Width;

                c.Extent = area / (double)(br.Width * br.Height);

                CvPoint[] hull = Cv2.ConvexHull(contour);
                double hullArea = Math.Abs(Cv2.ContourArea(hull));
                c.Solidity = hullArea > 0.0 ? area / hullArea : 0.0;

                c.BoundingRect = new Rectangle(
                    imageRoi.X + br.X,
                    imageRoi.Y + br.Y,
                    br.Width,
                    br.Height);

                Point2f center;
                float radius;
                Cv2.MinEnclosingCircle(contour, out center, out radius);

                c.Center = new PointF(
                    imageRoi.X + center.X,
                    imageRoi.Y + center.Y);

                c.Radius = radius;

                for (int p = 0; p < contour.Length; p++)
                {
                    c.ContourPoints.Add(
                        new PointF(
                            imageRoi.X + contour[p].X,
                            imageRoi.Y + contour[p].Y));
                }

                if (contour.Length >= 5)
                {
                    try
                    {
                        RotatedRect ellipse = Cv2.FitEllipse(contour);

                        c.HasEllipse = true;
                        c.EllipseCenter = new PointF(
                            imageRoi.X + ellipse.Center.X,
                            imageRoi.Y + ellipse.Center.Y);

                        c.EllipseSize = new SizeF(
                            ellipse.Size.Width,
                            ellipse.Size.Height);

                        c.EllipseAngle = ellipse.Angle;
                    }
                    catch
                    {
                        c.HasEllipse = false;
                    }
                }

                list.Add(c);
            }

            return list;
        }

        public static Mat CreateGrayMatFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;

            using (Bitmap src = CloneBitmapTo24bpp(bitmap))
            {
                Rectangle rect = new Rectangle(0, 0, src.Width, src.Height);

                BitmapData data = src.LockBits(
                    rect,
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);

                try
                {
                    int width = src.Width;
                    int height = src.Height;
                    int stride = data.Stride;

                    byte[] srcBuffer = new byte[stride * height];
                    Marshal.Copy(data.Scan0, srcBuffer, 0, srcBuffer.Length);

                    byte[] grayBuffer = new byte[width * height];

                    for (int y = 0; y < height; y++)
                    {
                        int srcRow = y * stride;
                        int dstRow = y * width;

                        for (int x = 0; x < width; x++)
                        {
                            int si = srcRow + x * 3;

                            byte b = srcBuffer[si + 0];
                            byte g = srcBuffer[si + 1];
                            byte r = srcBuffer[si + 2];

                            int gray = (int)(0.114 * b + 0.587 * g + 0.299 * r);

                            if (gray < 0)
                                gray = 0;

                            if (gray > 255)
                                gray = 255;

                            grayBuffer[dstRow + x] = (byte)gray;
                        }
                    }

                    Mat mat = new Mat(height, width, MatType.CV_8UC1);
                    Marshal.Copy(grayBuffer, 0, mat.Data, grayBuffer.Length);

                    return mat;
                }
                finally
                {
                    src.UnlockBits(data);
                }
            }
        }

        public static Bitmap CreatePreviewBitmap(Bitmap original, Rectangle roi, Mat roiImage)
        {
            if (original == null || roiImage == null || roiImage.Empty())
                return null;

            Bitmap preview = CloneBitmapTo24bpp(original);

            using (Mat roi8u = new Mat())
            {
                if (roiImage.Type() != MatType.CV_8UC1)
                    roiImage.ConvertTo(roi8u, MatType.CV_8UC1);
                else
                    roiImage.CopyTo(roi8u);

                byte[] roiBuffer = new byte[roi.Width * roi.Height];
                Marshal.Copy(roi8u.Data, roiBuffer, 0, roiBuffer.Length);

                Rectangle rect = new Rectangle(0, 0, preview.Width, preview.Height);

                BitmapData data = preview.LockBits(
                    rect,
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                try
                {
                    int stride = data.Stride;
                    byte[] buffer = new byte[stride * preview.Height];

                    Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

                    for (int y = 0; y < roi.Height; y++)
                    {
                        int imageY = roi.Y + y;

                        if (imageY < 0 || imageY >= preview.Height)
                            continue;

                        for (int x = 0; x < roi.Width; x++)
                        {
                            int imageX = roi.X + x;

                            if (imageX < 0 || imageX >= preview.Width)
                                continue;

                            byte v = roiBuffer[y * roi.Width + x];

                            int index = imageY * stride + imageX * 3;

                            buffer[index + 0] = v;
                            buffer[index + 1] = v;
                            buffer[index + 2] = v;
                        }
                    }

                    Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
                }
                finally
                {
                    preview.UnlockBits(data);
                }
            }

            return preview;
        }

        public static Bitmap CreateEnhancedHoleBinaryPreview(
            Bitmap baseImage,
            Rectangle imageRoi,
            Mat binaryRoi,
            BlobCandidate selectedCandidate)
        {
            if (baseImage == null)
                return null;

            if (selectedCandidate == null)
                return CreatePreviewBitmap(baseImage, imageRoi, binaryRoi);

            if (binaryRoi == null || binaryRoi.Empty())
                return CreatePreviewBitmap(baseImage, imageRoi, binaryRoi);

            using (Mat debugBinary = binaryRoi.Clone())
            {
                if (selectedCandidate.ContourPoints != null &&
                    selectedCandidate.ContourPoints.Count >= 3)
                {
                    CvPoint[] localContour = new CvPoint[selectedCandidate.ContourPoints.Count];

                    for (int i = 0; i < selectedCandidate.ContourPoints.Count; i++)
                    {
                        PointF p = selectedCandidate.ContourPoints[i];

                        localContour[i] = new CvPoint(
                            (int)Math.Round(p.X - imageRoi.X),
                            (int)Math.Round(p.Y - imageRoi.Y));
                    }

                    CvPoint[][] contours = new CvPoint[][] { localContour };

                    Cv2.DrawContours(
                        debugBinary,
                        contours,
                        0,
                        Scalar.White,
                        -1);
                }

                return CreatePreviewBitmap(baseImage, imageRoi, debugBinary);
            }
        }

        // 전경에 둘러싸인 내부 빈 영역(구멍)을 채운다.
        // 모서리에서 floodfill로 "바깥 배경"을 찾고, 그 여집합을 원본에 OR.
        public static void FillBinaryHoles(Mat bin)
        {
            if (bin == null || bin.Empty())
                return;

            int w = bin.Width;
            int h = bin.Height;

            if (w < 2 || h < 2)
                return;

            CvPoint[] corners =
            {
        new CvPoint(0, 0),
        new CvPoint(w - 1, 0),
        new CvPoint(0, h - 1),
        new CvPoint(w - 1, h - 1)
    };

            bool seedFound = false;
            CvPoint seed = new CvPoint(0, 0);

            for (int i = 0; i < corners.Length; i++)
            {
                if (bin.At<byte>(corners[i].Y, corners[i].X) == 0)
                {
                    seed = corners[i];
                    seedFound = true;
                    break;
                }
            }

            if (!seedFound)
                return;

            using (Mat flood = bin.Clone())
            using (Mat holes = new Mat())
            {
                Cv2.FloodFill(flood, seed, new Scalar(255));
                Cv2.BitwiseNot(flood, holes);
                Cv2.BitwiseOr(bin, holes, bin);
            }
        }

        // 주어진 (로컬좌표) contour를 ConvexHull로 채운 마스크를 filledMask에 그린다.
        // 홀은 볼록 도형에 가까워, hull 채우기로 내부 빈 영역까지 한 번에 메워진다.
        public static void FillContourConvex(Mat filledMask, CvPoint[] localContour)
        {
            if (filledMask == null || localContour == null || localContour.Length < 3)
                return;

            CvPoint[] hull = Cv2.ConvexHull(localContour);

            if (hull == null || hull.Length < 3)
            {
                // hull 실패 시 원본 contour라도 채움
                Cv2.DrawContours(
                    filledMask,
                    new CvPoint[][] { localContour },
                    0, Scalar.White, -1);
                return;
            }

            Cv2.FillPoly(filledMask, new CvPoint[][] { hull }, Scalar.White);
        }

        private static Bitmap CloneBitmapTo24bpp(Bitmap source)
        {
            Bitmap clone = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(clone))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            return clone;
        }
    }
}