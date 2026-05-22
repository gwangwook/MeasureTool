using System;
using System.Collections.Generic;
using System.Drawing;

namespace MeasureTool
{
    internal static class RoiEdgeDetector
    {
        public static RoiEdgeResult Detect(
            byte[] grayBuffer,
            int imageWidth,
            int imageHeight,
            Rectangle roi,
            RoiScanDirection direction,
            RoiEdgePolarity polarity,
            RoiEdgeSelection selection,
            int strengthThresholdPercent,
            int scanStep,
            int minValidPercent)
        {
            RoiEdgeResult result = new RoiEdgeResult();
            result.Roi = roi;
            result.ScanDirection = direction;
            result.Polarity = polarity;
            result.Found = false;
            result.Message = string.Empty;

            if (grayBuffer == null)
            {
                result.Message = "Gray buffer is null.";
                return result;
            }

            int totalScanLines = GetTotalScanLineCount(roi, direction, scanStep);
            result.TotalScanLines = totalScanLines;

            if (totalScanLines <= 0)
            {
                result.Message = "No scan lines.";
                return result;
            }

            int minValidCount = (int)Math.Ceiling(totalScanLines * (minValidPercent / 100.0));
            if (minValidCount < 1)
                minValidCount = 1;

            List<double> strengths = new List<double>();

            if (direction == RoiScanDirection.TopToBottom ||
                direction == RoiScanDirection.BottomToTop)
            {
                for (int x = roi.Left; x < roi.Right; x += scanStep)
                {
                    double[] profile = BuildVerticalProfile(
                        grayBuffer,
                        imageWidth,
                        imageHeight,
                        roi,
                        x,
                        direction);

                    EdgeCandidate candidate;
                    if (TryFindEdgeOnProfile(
                        profile,
                        polarity,
                        strengthThresholdPercent,
                        selection,
                        out candidate))
                    {
                        int edgeY = GetEdgeYFromProfileIndex(roi, direction, candidate.Index);

                        result.EdgePoints.Add(new PointF(x + 0.5f, edgeY + 0.5f));
                        strengths.Add(candidate.StrengthPercent);
                    }
                }
            }
            else
            {
                for (int y = roi.Top; y < roi.Bottom; y += scanStep)
                {
                    double[] profile = BuildHorizontalProfile(
                        grayBuffer,
                        imageWidth,
                        imageHeight,
                        roi,
                        y,
                        direction);

                    EdgeCandidate candidate;
                    if (TryFindEdgeOnProfile(
                        profile,
                        polarity,
                        strengthThresholdPercent,
                        selection,
                        out candidate))
                    {
                        int edgeX = GetEdgeXFromProfileIndex(roi, direction, candidate.Index);

                        result.EdgePoints.Add(new PointF(edgeX + 0.5f, y + 0.5f));
                        strengths.Add(candidate.StrengthPercent);
                    }
                }
            }

            result.FoundScanLines = result.EdgePoints.Count;

            if (result.EdgePoints.Count < minValidCount)
            {
                result.Message = string.Format(
                    "Not enough edge points. Found={0}, Required={1}",
                    result.EdgePoints.Count,
                    minValidCount);
                return result;
            }

            result.InlierPoints = RemoveRoiEdgeOutliers(
                result.EdgePoints,
                direction,
                3.0);

            if (result.InlierPoints.Count < minValidCount)
            {
                result.Message = string.Format(
                    "Not enough inlier points. Inlier={0}, Required={1}",
                    result.InlierPoints.Count,
                    minValidCount);
                return result;
            }

            if (!BuildRoiEdgeLine(result, direction))
            {
                result.Message = "Line fitting failed.";
                return result;
            }

            if (strengths.Count > 0)
            {
                double sum = 0.0;

                for (int i = 0; i < strengths.Count; i++)
                    sum += strengths[i];

                result.AverageStrengthPercent = sum / strengths.Count;
            }

            result.Found = true;
            result.Message = "OK";

            return result;
        }

        private static int GetTotalScanLineCount(Rectangle roi, RoiScanDirection direction, int scanStep)
        {
            if (scanStep < 1)
                scanStep = 1;

            if (direction == RoiScanDirection.TopToBottom ||
                direction == RoiScanDirection.BottomToTop)
            {
                return (int)Math.Ceiling(roi.Width / (double)scanStep);
            }

            return (int)Math.Ceiling(roi.Height / (double)scanStep);
        }

        private static double[] BuildVerticalProfile(
            byte[] grayBuffer,
            int imageWidth,
            int imageHeight,
            Rectangle roi,
            int x,
            RoiScanDirection direction)
        {
            double[] profile = new double[roi.Height];

            for (int i = 0; i < roi.Height; i++)
            {
                int y;

                if (direction == RoiScanDirection.TopToBottom)
                    y = roi.Top + i;
                else
                    y = roi.Bottom - 1 - i;

                profile[i] = ImageProcessingHelper.GetGrayValue(grayBuffer, imageWidth, imageHeight, x, y);
            }

            return profile;
        }

        private static double[] BuildHorizontalProfile(
            byte[] grayBuffer,
            int imageWidth,
            int imageHeight,
            Rectangle roi,
            int y,
            RoiScanDirection direction)
        {
            double[] profile = new double[roi.Width];

            for (int i = 0; i < roi.Width; i++)
            {
                int x;

                if (direction == RoiScanDirection.LeftToRight)
                    x = roi.Left + i;
                else
                    x = roi.Right - 1 - i;

                profile[i] = ImageProcessingHelper.GetGrayValue(grayBuffer, imageWidth, imageHeight, x, y);
            }

            return profile;
        }

        private static bool TryFindEdgeOnProfile(
            double[] profile,
            RoiEdgePolarity polarity,
            int strengthThresholdPercent,
            RoiEdgeSelection selection,
            out EdgeCandidate selected)
        {
            selected = null;

            if (profile == null || profile.Length < 2)
                return false;

            double min = profile[0];
            double max = profile[0];

            for (int i = 1; i < profile.Length; i++)
            {
                if (profile[i] < min) min = profile[i];
                if (profile[i] > max) max = profile[i];
            }

            double range = max - min;

            if (range < 1.0)
                return false;

            List<EdgeCandidate> candidates = new List<EdgeCandidate>();

            for (int i = 0; i < profile.Length - 1; i++)
            {
                double before = profile[i];
                double after = profile[i + 1];
                double diff = after - before;

                if (!IsPolarityMatched(diff, polarity))
                    continue;

                double strengthPercent = Math.Abs(diff) / range * 100.0;

                if (strengthPercent < strengthThresholdPercent)
                    continue;

                EdgeCandidate candidate = new EdgeCandidate();
                candidate.Index = i + 1;
                candidate.BeforeValue = before;
                candidate.AfterValue = after;
                candidate.Diff = diff;
                candidate.StrengthPercent = strengthPercent;

                candidates.Add(candidate);

                if (selection == RoiEdgeSelection.First)
                    break;
            }

            if (candidates.Count <= 0)
                return false;

            if (selection == RoiEdgeSelection.First)
            {
                selected = candidates[0];
                return true;
            }

            EdgeCandidate strongest = candidates[0];

            for (int i = 1; i < candidates.Count; i++)
            {
                if (candidates[i].StrengthPercent > strongest.StrengthPercent)
                {
                    strongest = candidates[i];
                }
                else if (Math.Abs(candidates[i].StrengthPercent - strongest.StrengthPercent) < 0.0001)
                {
                    if (Math.Abs(candidates[i].Diff) > Math.Abs(strongest.Diff))
                        strongest = candidates[i];
                }
            }

            selected = strongest;
            return true;
        }

        private static bool IsPolarityMatched(double diff, RoiEdgePolarity polarity)
        {
            if (polarity == RoiEdgePolarity.WhiteToBlack)
                return diff < 0;

            if (polarity == RoiEdgePolarity.BlackToWhite)
                return diff > 0;

            return Math.Abs(diff) > 0.0001;
        }

        private static int GetEdgeYFromProfileIndex(Rectangle roi, RoiScanDirection direction, int profileIndex)
        {
            if (profileIndex < 0)
                profileIndex = 0;

            if (profileIndex >= roi.Height)
                profileIndex = roi.Height - 1;

            if (direction == RoiScanDirection.TopToBottom)
                return roi.Top + profileIndex;

            return roi.Bottom - 1 - profileIndex;
        }

        private static int GetEdgeXFromProfileIndex(Rectangle roi, RoiScanDirection direction, int profileIndex)
        {
            if (profileIndex < 0)
                profileIndex = 0;

            if (profileIndex >= roi.Width)
                profileIndex = roi.Width - 1;

            if (direction == RoiScanDirection.LeftToRight)
                return roi.Left + profileIndex;

            return roi.Right - 1 - profileIndex;
        }

        private static List<PointF> RemoveRoiEdgeOutliers(
            List<PointF> points,
            RoiScanDirection direction,
            double maxDistance)
        {
            List<PointF> inliers = new List<PointF>();

            if (points == null || points.Count <= 0)
                return inliers;

            if (points.Count < 3)
            {
                inliers.AddRange(points);
                return inliers;
            }

            if (direction == RoiScanDirection.TopToBottom ||
                direction == RoiScanDirection.BottomToTop)
            {
                double medianY = GetMedianCoordinate(points, false);

                for (int i = 0; i < points.Count; i++)
                {
                    if (Math.Abs(points[i].Y - medianY) <= maxDistance)
                        inliers.Add(points[i]);
                }
            }
            else
            {
                double medianX = GetMedianCoordinate(points, true);

                for (int i = 0; i < points.Count; i++)
                {
                    if (Math.Abs(points[i].X - medianX) <= maxDistance)
                        inliers.Add(points[i]);
                }
            }

            return inliers;
        }

        private static double GetMedianCoordinate(List<PointF> points, bool useX)
        {
            List<double> values = new List<double>();

            for (int i = 0; i < points.Count; i++)
            {
                values.Add(useX ? points[i].X : points[i].Y);
            }

            values.Sort();

            int mid = values.Count / 2;

            if (values.Count % 2 == 1)
                return values[mid];

            return (values[mid - 1] + values[mid]) / 2.0;
        }

        private static bool BuildRoiEdgeLine(RoiEdgeResult result, RoiScanDirection direction)
        {
            if (result == null || result.InlierPoints == null || result.InlierPoints.Count <= 0)
                return false;

            Rectangle roi = result.Roi;

            PointF avg = GetAveragePoint(result.InlierPoints);
            result.RepresentativePoint = avg;

            if (direction == RoiScanDirection.TopToBottom ||
                direction == RoiScanDirection.BottomToTop)
            {
                // y = a*x + b
                double a;
                double b;

                if (FitLineYFromX(result.InlierPoints, out a, out b))
                {
                    float x1 = roi.Left;
                    float x2 = roi.Right - 1;

                    float y1 = (float)(a * x1 + b);
                    float y2 = (float)(a * x2 + b);

                    result.LineStart = new PointF(x1 + 0.5f, y1);
                    result.LineEnd = new PointF(x2 + 0.5f, y2);
                }
                else
                {
                    result.LineStart = new PointF(roi.Left + 0.5f, avg.Y);
                    result.LineEnd = new PointF(roi.Right - 0.5f, avg.Y);
                }
            }
            else
            {
                // x = a*y + b
                double a;
                double b;

                if (FitLineXFromY(result.InlierPoints, out a, out b))
                {
                    float y1 = roi.Top;
                    float y2 = roi.Bottom - 1;

                    float x1 = (float)(a * y1 + b);
                    float x2 = (float)(a * y2 + b);

                    result.LineStart = new PointF(x1, y1 + 0.5f);
                    result.LineEnd = new PointF(x2, y2 + 0.5f);
                }
                else
                {
                    result.LineStart = new PointF(avg.X, roi.Top + 0.5f);
                    result.LineEnd = new PointF(avg.X, roi.Bottom - 0.5f);
                }
            }

            return true;
        }

        private static PointF GetAveragePoint(List<PointF> points)
        {
            if (points == null || points.Count <= 0)
                return PointF.Empty;

            double sx = 0.0;
            double sy = 0.0;

            for (int i = 0; i < points.Count; i++)
            {
                sx += points[i].X;
                sy += points[i].Y;
            }

            return new PointF(
                (float)(sx / points.Count),
                (float)(sy / points.Count));
        }

        private static bool FitLineYFromX(List<PointF> points, out double a, out double b)
        {
            a = 0.0;
            b = 0.0;

            if (points == null || points.Count < 2)
                return false;

            double sx = 0.0;
            double sy = 0.0;
            double sxx = 0.0;
            double sxy = 0.0;

            int n = points.Count;

            for (int i = 0; i < n; i++)
            {
                double x = points[i].X;
                double y = points[i].Y;

                sx += x;
                sy += y;
                sxx += x * x;
                sxy += x * y;
            }

            double denom = n * sxx - sx * sx;

            if (Math.Abs(denom) < 0.000001)
                return false;

            a = (n * sxy - sx * sy) / denom;
            b = (sy - a * sx) / n;

            return true;
        }

        private static bool FitLineXFromY(List<PointF> points, out double a, out double b)
        {
            a = 0.0;
            b = 0.0;

            if (points == null || points.Count < 2)
                return false;

            double sy = 0.0;
            double sx = 0.0;
            double syy = 0.0;
            double syx = 0.0;

            int n = points.Count;

            for (int i = 0; i < n; i++)
            {
                double y = points[i].Y;
                double x = points[i].X;

                sy += y;
                sx += x;
                syy += y * y;
                syx += y * x;
            }

            double denom = n * syy - sy * sy;

            if (Math.Abs(denom) < 0.000001)
                return false;

            a = (n * syx - sy * sx) / denom;
            b = (sx - a * sy) / n;

            return true;
        }
}
}
