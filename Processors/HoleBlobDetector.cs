using System;
using System.Collections.Generic;
using System.Drawing;
using OpenCvSharp;
using CvRect = OpenCvSharp.Rect;
using CvPoint = OpenCvSharp.Point;
using CvSize = OpenCvSharp.Size;

namespace MeasureTool
{
    internal static class HoleBlobDetector
    {
        public static HoleDetectResult Detect(
            Bitmap sourceImage,
            Rectangle holeRoi,
            HoleBlobSettings settings)
        {
            HoleDetectResult result = new HoleDetectResult();
            result.Found = false;
            result.Message = "Not Found";
            result.Roi = holeRoi;

            if (sourceImage == null)
            {
                result.Message = "Source image is null";
                return result;
            }

            if (settings == null)
                settings = new HoleBlobSettings();

            Rectangle validRoi = ClampRectangleToImage(
                holeRoi,
                sourceImage.Width,
                sourceImage.Height);

            if (validRoi.Width <= 0 || validRoi.Height <= 0)
            {
                result.Message = "Invalid ROI";
                return result;
            }

            using (Mat fullGray = BlobUtility.CreateGrayMatFromBitmap(sourceImage))
            {
                if (fullGray == null || fullGray.Empty())
                {
                    result.Message = "Failed to create gray Mat";
                    return result;
                }

                Mat preprocess;
                Mat binary;
                Mat detectMask;

                CreateHoleProcessImages(
                    fullGray,
                    validRoi,
                    settings,
                    out preprocess,
                    out binary,
                    out detectMask);

                using (preprocess)
                using (binary)
                using (detectMask)
                {
                    result.BinaryPreview = BlobUtility.CreatePreviewBitmap(sourceImage, validRoi, binary);
                    result.DetectPreview = BlobUtility.CreatePreviewBitmap(sourceImage, validRoi, detectMask);

                    List<BlobCandidate> candidates =
                        BlobUtility.ExtractCandidatesFromBinary(detectMask, validRoi);

                    result.Candidates = candidates;

                    if (candidates.Count <= 0)
                    {
                        result.Message = "No contour";
                        return result;
                    }

                    BlobCandidate best = SelectBestEnhancedHoleCandidate(
                        candidates,
                        validRoi,
                        detectMask,
                        settings);

                    if (best == null)
                    {
                        result.Message = "No valid contour";
                        return result;
                    }

                    for (int i = 0; i < candidates.Count; i++)
                        candidates[i].Selected = false;

                    best.Selected = true;
                    best.IsVirtual = false;

                    // ── P2: fill된 형상으로 중심 계산 ──
                    // fill 결과(ConvexHull+구멍채움)의 BoundingRect/contour로 후보를 갱신해야
                    // CenterMethod(MinEnclosingCircle/BoundingRect)가 "꽉 찬 홀" 기준으로 동작.
                    ApplyFilledShapeToCandidate(best, validRoi, detectMask, settings);

                    PointF finalCenter = GetHoleCenterByMethod(best, settings.CenterMethod);
                    best.Center = finalCenter;

                    Bitmap enhanced = BlobUtility.CreateEnhancedHoleBinaryPreview(
                        sourceImage, validRoi, detectMask, best);  // ← binary 아니라 detectMask 기반

                    if (enhanced != null)
                    {
                        if (result.DetectPreview != null)
                            result.DetectPreview.Dispose();
                        result.DetectPreview = enhanced;
                    }

                    result.SelectedCandidate = best;
                    result.Center = finalCenter;
                    result.Found = true;
                    result.Message = "OK EnhancedBlob";

                    return result;
                }
            }
        }

        public static HoleDetectResult Preview(
            Bitmap sourceImage,
            Rectangle holeRoi,
            HoleBlobSettings settings)
        {
            HoleDetectResult result = new HoleDetectResult();
            result.Found = false;
            result.Message = "Preview Only";
            result.Roi = holeRoi;

            if (sourceImage == null)
            {
                result.Message = "Source image is null";
                return result;
            }

            if (settings == null)
                settings = new HoleBlobSettings();

            Rectangle validRoi = ClampRectangleToImage(
                holeRoi, sourceImage.Width, sourceImage.Height);

            if (validRoi.Width <= 0 || validRoi.Height <= 0)
            {
                result.Message = "Invalid ROI";
                return result;
            }

            using (Mat fullGray = BlobUtility.CreateGrayMatFromBitmap(sourceImage))
            {
                if (fullGray == null || fullGray.Empty())
                {
                    result.Message = "Failed to create gray Mat";
                    return result;
                }

                Mat preprocess;
                Mat binary;
                Mat detectMask;

                CreateHoleProcessImages(
                    fullGray, validRoi, settings,
                    out preprocess, out binary, out detectMask);

                using (preprocess)
                using (binary)
                using (detectMask)
                {
                    result.BinaryPreview = BlobUtility.CreatePreviewBitmap(
                        sourceImage, validRoi, binary);
                }
            }

            return result;
        }

        private static void CreateHoleProcessImages(
            Mat fullGray,
            Rectangle roi,
            HoleBlobSettings settings,
            out Mat preprocess,
            out Mat binary,
            out Mat detectMask)
        {
            CvRect cvRoi = new CvRect(roi.X, roi.Y, roi.Width, roi.Height);

            using (Mat roiGrayView = new Mat(fullGray, cvRoi))
            using (Mat roiGray = roiGrayView.Clone())
            {
                preprocess = new Mat();

                Cv2.GaussianBlur(
                    roiGray,
                    preprocess,
                    new CvSize(3, 3),
                    0);

                binary = new Mat();

                Cv2.Threshold(
                    preprocess,
                    binary,
                    settings.Threshold,
                    255,
                    ThresholdTypes.Binary);

                detectMask = BlobUtility.CreateDetectMaskFromBinary(
                    binary,
                    settings.Polarity,
                    settings.Morph);
            }
        }

        private static BlobCandidate SelectBestEnhancedHoleCandidate(
            List<BlobCandidate> candidates,
            Rectangle roi,
            Mat detectMask,
            HoleBlobSettings settings)
        {
            int minArea = settings.MinArea;
            int maxArea = settings.MaxArea;

            PointF roiCenter = BlobUtility.GetRectangleCenter(roi);
            int roiMinSize = Math.Min(roi.Width, roi.Height);

            double minDiameter = roiMinSize * 0.12;
            double maxDiameter = roiMinSize * 0.85;
            double maxCenterDistance = roiMinSize * 0.50;

            BlobCandidate best = null;
            double bestScore = double.MinValue;
            PointF bestCenter = PointF.Empty;

            for (int i = 0; i < candidates.Count; i++)
            {
                BlobCandidate c = candidates[i];

                if (c.Perimeter <= 0.0)
                    continue;

                Rectangle br = c.BoundingRect;

                if (br.Width <= 0 || br.Height <= 0)
                    continue;

                int diameter = Math.Max(br.Width, br.Height);

                if (diameter < minDiameter || diameter > maxDiameter)
                    continue;

                if (c.AspectRatio < 0.5 || c.AspectRatio > 2.0)
                    continue;

                double centerDist = BlobUtility.GetDistance(
                    BlobUtility.GetRectangleCenter(br),
                    roiCenter);

                if (centerDist > maxCenterDistance)
                    continue;

                HoleLocalFillInfo fillInfo = EvaluateHoleCandidateByLocalFill(c, roi, detectMask, settings);

                if (!fillInfo.Valid)
                    continue;

                Rectangle filledRect = fillInfo.FilledRect;
                int filledDiameter = Math.Max(filledRect.Width, filledRect.Height);

                if (filledDiameter < minDiameter || filledDiameter > maxDiameter)
                    continue;

                double filledAspect = BlobUtility.GetAspectRatio(filledRect);

                if (filledAspect < 0.55 || filledAspect > 1.85)
                    continue;

                double widthGrow = filledRect.Width / (double)Math.Max(1, br.Width);
                double heightGrow = filledRect.Height / (double)Math.Max(1, br.Height);

                if (widthGrow > 1.08 || heightGrow > 1.08)
                    continue;

                PointF originalRectCenter = BlobUtility.GetRectangleCenter(br);
                double fillCenterShift = BlobUtility.GetDistance(originalRectCenter, fillInfo.FilledCenter);

                if (fillCenterShift > roiMinSize * 0.08)
                    continue;

                double brArea = Math.Max(1.0, br.Width * br.Height);
                double filledToRectRatio = fillInfo.FilledArea / brArea;

                if (filledToRectRatio > 1.05)
                    continue;

                if (c.Area < minArea && fillInfo.FilledArea < minArea)
                    continue;

                if (c.Area > maxArea && fillInfo.FilledArea > maxArea)
                    continue;

                double circularityScore = c.Circularity * 700.0;

                double targetDiameter = roiMinSize * 0.38;
                double diameterError = Math.Abs(filledDiameter - targetDiameter) / Math.Max(1.0, targetDiameter);
                double diameterScore = Math.Max(0.0, 1.0 - diameterError) * 240.0;

                double aspectScore = Math.Max(0.0, 1.0 - Math.Abs(filledAspect - 1.0)) * 220.0;

                double centerScore = Math.Max(0.0, 1.0 - centerDist / Math.Max(1.0, maxCenterDistance)) * 100.0;

                double fillRatioScore = GetHoleFillRatioScore(fillInfo.WhiteToFilledRatio) * 220.0;

                double extentScore = GetHoleExtentScore(c.Extent) * 100.0;

                double shiftPenalty = fillCenterShift * 4.0;

                double score =
                    circularityScore +
                    diameterScore +
                    aspectScore +
                    centerScore +
                    fillRatioScore +
                    extentScore -
                    shiftPenalty;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = c;
                    bestCenter = fillInfo.FilledCenter;
                }
            }

            // 의도적으로 best.Center를 FilledCenter로 덮어쓰지 않음.
            // 최종 Center는 GetHoleCenterByMethod에서 결정.

            return best;
        }

        private static double GetHoleExtentScore(double extent)
        {
            if (extent <= 0.0)
                return 0.0;

            if (extent < 0.03)
                return 0.0;

            if (extent > 0.95)
                return 0.2;

            if (extent >= 0.10 && extent <= 0.75)
                return 1.0;

            if (extent < 0.10)
                return extent / 0.10;

            return Math.Max(0.0, 1.0 - (extent - 0.75) / 0.20);
        }

        private static double GetHoleFillRatioScore(double ratio)
        {
            if (ratio <= 0.0)
                return 0.0;

            if (ratio < 0.05)
                return 0.0;

            if (ratio > 0.98)
                return 0.2;

            double target = 0.35;
            double error = Math.Abs(ratio - target);

            return Math.Max(0.0, 1.0 - error / 0.40);
        }

        private static HoleLocalFillInfo EvaluateHoleCandidateByLocalFill(
            BlobCandidate candidate,
            Rectangle imageRoi,
            Mat detectMask,
            HoleBlobSettings settings)
        {
            HoleLocalFillInfo info = new HoleLocalFillInfo();
            info.Valid = false;

            if (candidate == null)
                return info;

            Rectangle br = candidate.BoundingRect;

            if (br.Width <= 0 || br.Height <= 0)
                return info;

            Rectangle localImageRect = br;
            int pad = Math.Max(3, (int)Math.Round(Math.Max(br.Width, br.Height) * 0.15));
            localImageRect.Inflate(pad, pad);
            localImageRect = Rectangle.Intersect(localImageRect, imageRoi);

            if (localImageRect.Width <= 0 || localImageRect.Height <= 0)
                return info;

            CvRect localRoi = new CvRect(
                localImageRect.X - imageRoi.X,
                localImageRect.Y - imageRoi.Y,
                localImageRect.Width,
                localImageRect.Height);

            if (localRoi.X < 0 || localRoi.Y < 0)
                return info;

            if (localRoi.X + localRoi.Width > detectMask.Width)
                return info;

            if (localRoi.Y + localRoi.Height > detectMask.Height)
                return info;

            using (Mat localMaskView = new Mat(detectMask, localRoi))
            using (Mat localMask = localMaskView.Clone())
            using (Mat filledMask = Mat.Zeros(localMask.Rows, localMask.Cols, MatType.CV_8UC1))
            {
                double whitePixels = Cv2.CountNonZero(localMask);

                if (whitePixels <= 0.0)
                    return info;

                if (candidate.ContourPoints == null || candidate.ContourPoints.Count < 3)
                    return info;

                CvPoint[] localContour = new CvPoint[candidate.ContourPoints.Count];

                for (int i = 0; i < candidate.ContourPoints.Count; i++)
                {
                    PointF p = candidate.ContourPoints[i];

                    localContour[i] = new CvPoint(
                        (int)Math.Round(p.X - localImageRect.X),
                        (int)Math.Round(p.Y - localImageRect.Y));
                }

                // ── P2: ConvexHull + 내부 구멍 채우기 ──
                // 기존 DrawContours(-1)는 선택 contour 외곽선 안만 채워서
                // 홀 내부가 링/조각이면 가운데 빈 영역이 남았다(케이스 7.2).
                // ConvexHull로 외곽을 볼록하게 확정 + FillBinaryHoles로 내부 잔여 구멍 제거.
                if (settings != null && settings.UseConvexHullFill)
                {
                    BlobUtility.FillContourConvex(filledMask, localContour);
                    BlobUtility.FillBinaryHoles(filledMask);
                }
                else
                {
                    CvPoint[][] contours0 = new CvPoint[][] { localContour };
                    Cv2.DrawContours(filledMask, contours0, 0, Scalar.White, -1);
                    BlobUtility.FillBinaryHoles(filledMask);
                }

                CvPoint[][] filledContours;
                HierarchyIndex[] hierarchy;

                using (Mat temp = filledMask.Clone())
                {
                    Cv2.FindContours(
                        temp,
                        out filledContours,
                        out hierarchy,
                        RetrievalModes.External,
                        ContourApproximationModes.ApproxSimple);
                }

                if (filledContours == null || filledContours.Length <= 0)
                    return info;

                CvRect bestRect = new CvRect();
                double bestArea = 0.0;

                for (int i = 0; i < filledContours.Length; i++)
                {
                    double area = Math.Abs(Cv2.ContourArea(filledContours[i]));

                    if (area > bestArea)
                    {
                        bestArea = area;
                        bestRect = Cv2.BoundingRect(filledContours[i]);
                    }
                }

                if (bestArea <= 0.0 || bestRect.Width <= 0 || bestRect.Height <= 0)
                    return info;

                Rectangle filledImageRect = new Rectangle(
                    localImageRect.X + bestRect.X,
                    localImageRect.Y + bestRect.Y,
                    bestRect.Width,
                    bestRect.Height);

                double ratio = whitePixels / Math.Max(1.0, bestArea);

                info.Valid = true;
                info.FilledRect = filledImageRect;
                info.FilledCenter = BlobUtility.GetRectangleCenter(filledImageRect);
                info.FilledArea = bestArea;
                info.WhitePixelArea = whitePixels;
                info.WhiteToFilledRatio = ratio;

                return info;
            }
        }

        private static PointF GetHoleCenterByMethod(BlobCandidate candidate, HoleCenterMethod method)
        {
            if (candidate == null)
                return PointF.Empty;

            if (method == HoleCenterMethod.MinEnclosingCircle)
                return candidate.Center;

            return BlobUtility.GetRectangleCenter(candidate.BoundingRect);
        }

        // EnhancedBlob 선택 후, fill된 형상(ConvexHull+구멍채움)의
        // BoundingRect / MinEnclosingCircle 중심 / contour를 후보에 반영.
        // 이렇게 해야 CenterMethod 옵션이 "채워진 홀" 기준으로 중심을 산출한다.
        private static void ApplyFilledShapeToCandidate(
            BlobCandidate candidate,
            Rectangle imageRoi,
            Mat detectMask,
            HoleBlobSettings settings)
        {
            if (candidate == null)
                return;

            if (candidate.ContourPoints == null || candidate.ContourPoints.Count < 3)
                return;

            Rectangle br = candidate.BoundingRect;
            if (br.Width <= 0 || br.Height <= 0)
                return;

            Rectangle localImageRect = br;
            int pad = Math.Max(3, (int)Math.Round(Math.Max(br.Width, br.Height) * 0.15));
            localImageRect.Inflate(pad, pad);
            localImageRect = Rectangle.Intersect(localImageRect, imageRoi);

            if (localImageRect.Width <= 0 || localImageRect.Height <= 0)
                return;

            using (Mat filledMask = Mat.Zeros(localImageRect.Height, localImageRect.Width, MatType.CV_8UC1))
            {
                CvPoint[] localContour = new CvPoint[candidate.ContourPoints.Count];
                for (int i = 0; i < candidate.ContourPoints.Count; i++)
                {
                    PointF p = candidate.ContourPoints[i];
                    localContour[i] = new CvPoint(
                        (int)Math.Round(p.X - localImageRect.X),
                        (int)Math.Round(p.Y - localImageRect.Y));
                }

                if (settings != null && settings.UseConvexHullFill)
                {
                    BlobUtility.FillContourConvex(filledMask, localContour);
                    BlobUtility.FillBinaryHoles(filledMask);
                }
                else
                {
                    Cv2.DrawContours(filledMask, new CvPoint[][] { localContour }, 0, Scalar.White, -1);
                    BlobUtility.FillBinaryHoles(filledMask);
                }

                CvPoint[][] filledContours;
                HierarchyIndex[] hierarchy;
                using (Mat temp = filledMask.Clone())
                {
                    Cv2.FindContours(temp, out filledContours, out hierarchy,
                        RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                }

                if (filledContours == null || filledContours.Length == 0)
                    return;

                int bestIdx = 0;
                double bestArea = -1.0;
                for (int i = 0; i < filledContours.Length; i++)
                {
                    double a = Math.Abs(Cv2.ContourArea(filledContours[i]));
                    if (a > bestArea) { bestArea = a; bestIdx = i; }
                }

                CvPoint[] fc = filledContours[bestIdx];

                // BoundingRect 갱신 (이미지 좌표)
                CvRect fbr = Cv2.BoundingRect(fc);
                candidate.BoundingRect = new Rectangle(
                    localImageRect.X + fbr.X,
                    localImageRect.Y + fbr.Y,
                    fbr.Width, fbr.Height);

                // MinEnclosingCircle 중심/반지름 갱신
                Point2f mc; float mr;
                Cv2.MinEnclosingCircle(fc, out mc, out mr);
                candidate.Center = new PointF(localImageRect.X + mc.X, localImageRect.Y + mc.Y);
                candidate.Radius = mr;

                // contour 갱신 (오버레이도 채워진 형상으로)
                List<PointF> newContour = new List<PointF>();
                for (int i = 0; i < fc.Length; i++)
                    newContour.Add(new PointF(localImageRect.X + fc[i].X, localImageRect.Y + fc[i].Y));
                candidate.ContourPoints = newContour;
            }
        }

        private static Rectangle ClampRectangleToImage(Rectangle rect, int imageWidth, int imageHeight)
        {
            int left = Math.Max(0, rect.Left);
            int top = Math.Max(0, rect.Top);
            int right = Math.Min(imageWidth, rect.Right);
            int bottom = Math.Min(imageHeight, rect.Bottom);

            if (right <= left || bottom <= top)
                return Rectangle.Empty;

            return Rectangle.FromLTRB(left, top, right, bottom);
        }
    }
}