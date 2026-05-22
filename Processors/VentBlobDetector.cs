using System;
using System.Collections.Generic;
using System.Drawing;
using OpenCvSharp;
using CvRect = OpenCvSharp.Rect;
using CvPoint = OpenCvSharp.Point;
using CvSize = OpenCvSharp.Size;

namespace MeasureTool
{
    internal static class VentBlobDetector
    {
        public static VentDetectResult Detect(
            Bitmap sourceImage,
            Rectangle ventRoi,
            VentBlobSettings settings)
        {
            return DetectByRegionBlob(sourceImage, ventRoi, settings);
        }

        public static VentDetectResult Preview(
            Bitmap sourceImage,
            Rectangle ventRoi,
            VentBlobSettings settings)
        {
            VentDetectResult result = new VentDetectResult();
            result.Found = false;
            result.Message = "Preview Only";
            result.Roi = ventRoi;

            if (sourceImage == null) { result.Message = "Source image is null"; return result; }
            if (settings == null) settings = new VentBlobSettings();

            Rectangle validRoi = ClampRectangleToImage(
                ventRoi, sourceImage.Width, sourceImage.Height);

            if (validRoi.Width <= 0 || validRoi.Height <= 0)
            {
                result.Message = "Invalid ROI";
                return result;
            }

            // 분석 영역 = ROI + margin (DetectByRegionBlob과 동일한 전처리 경로 사용)
            int margin = Math.Max(0, settings.RegionMargin);

            Rectangle analysisRoi = ClampRectangleToImage(
                new Rectangle(
                    validRoi.X - margin,
                    validRoi.Y - margin,
                    validRoi.Width + margin * 2,
                    validRoi.Height + margin * 2),
                sourceImage.Width, sourceImage.Height);

            if (analysisRoi.Width <= 0 || analysisRoi.Height <= 0)
            {
                result.Message = "Invalid analysis ROI";
                return result;
            }

            using (Mat fullGray = BlobUtility.CreateGrayMatFromBitmap(sourceImage))
            {
                if (fullGray == null || fullGray.Empty())
                {
                    result.Message = "Failed to create gray Mat";
                    return result;
                }

                CvRect cvRoi = new CvRect(
                    analysisRoi.X, analysisRoi.Y,
                    analysisRoi.Width, analysisRoi.Height);

                using (Mat roiView = new Mat(fullGray, cvRoi))
                using (Mat roiGray = roiView.Clone())
                {
                    Mat preprocess;
                    Mat binary;
                    Mat mask;

                    BuildRegionBlobImages(
                        roiGray, settings,
                        out preprocess, out binary, out mask);

                    using (preprocess)
                    using (binary)
                    using (mask)
                    {
                        // 사용자 ROI 기준(margin 제외)으로 잘라서 표시
                        result.BinaryPreview = CropPreviewToUserRoi(
                            sourceImage, validRoi, analysisRoi, binary);

                        result.DetectPreview = CropPreviewToUserRoi(
                            sourceImage, validRoi, analysisRoi, mask);
                    }
                }
            }

            return result;
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

        // =========================================================
        // P4: Region Blob 방식
        // 어두운 Vent 테두리를 닫힌 고리로 만들고, 그 안쪽 영역을
        // 하나의 채워진 Blob으로 잡아 MinAreaRect 중심을 산출.
        // =========================================================
        public static VentDetectResult DetectByRegionBlob(
            Bitmap sourceImage,
            Rectangle ventRoi,
            VentBlobSettings settings)
        {
            VentDetectResult result = new VentDetectResult();
            result.Found = false;
            result.Message = "Not Found";
            result.Roi = ventRoi;

            if (sourceImage == null)
            {
                result.Message = "Source image is null";
                return result;
            }

            if (settings == null)
                settings = new VentBlobSettings();

            Rectangle validRoi = ClampRectangleToImage(
                ventRoi, sourceImage.Width, sourceImage.Height);

            if (validRoi.Width <= 0 || validRoi.Height <= 0)
            {
                result.Message = "Invalid ROI";
                return result;
            }

            // 분석 영역 = ROI + margin (테두리가 경계에 닿지 않도록)
            int margin = Math.Max(0, settings.RegionMargin);

            Rectangle analysisRoi = ClampRectangleToImage(
                new Rectangle(
                    validRoi.X - margin,
                    validRoi.Y - margin,
                    validRoi.Width + margin * 2,
                    validRoi.Height + margin * 2),
                sourceImage.Width, sourceImage.Height);

            if (analysisRoi.Width <= 0 || analysisRoi.Height <= 0)
            {
                result.Message = "Invalid analysis ROI";
                return result;
            }

            double roiArea = (double)validRoi.Width * validRoi.Height;
            double minArea = roiArea * settings.MinAreaFraction;
            double maxArea = roiArea * settings.MaxAreaFraction;

            using (Mat fullGray = BlobUtility.CreateGrayMatFromBitmap(sourceImage))
            {
                if (fullGray == null || fullGray.Empty())
                {
                    result.Message = "Failed to create gray Mat";
                    return result;
                }

                CvRect cvRoi = new CvRect(
                    analysisRoi.X, analysisRoi.Y,
                    analysisRoi.Width, analysisRoi.Height);

                using (Mat roiView = new Mat(fullGray, cvRoi))
                using (Mat roiGray = roiView.Clone())
                {
                    Mat preprocess;
                    Mat binary;
                    Mat mask;

                    BuildRegionBlobImages(
                        roiGray, settings,
                        out preprocess, out binary, out mask);

                    using (preprocess)
                    using (binary)
                    using (mask)
                    {
                        // Preview는 사용자 ROI 기준(margin 제외)으로 잘라서 표시
                        result.BinaryPreview = CropPreviewToUserRoi(
                            sourceImage, validRoi, analysisRoi, binary);

                        result.DetectPreview = CropPreviewToUserRoi(
                            sourceImage, validRoi, analysisRoi, mask);

                        // 연결 요소 분석
                        using (Mat labels = new Mat())
                        using (Mat stats = new Mat())
                        using (Mat centroids = new Mat())
                        {
                            int count = Cv2.ConnectedComponentsWithStats(
                                mask, labels, stats, centroids,
                                PixelConnectivity.Connectivity8, MatType.CV_32S);

                            if (count <= 1)
                            {
                                result.Message = "No blob found";
                                return result;
                            }

                            int areaCol = (int)ConnectedComponentsTypes.Area;
                            int leftCol = (int)ConnectedComponentsTypes.Left;
                            int topCol = (int)ConnectedComponentsTypes.Top;
                            int widthCol = (int)ConnectedComponentsTypes.Width;
                            int heightCol = (int)ConnectedComponentsTypes.Height;

                            int bestLabel = -1;
                            double bestArea = -1.0;

                            for (int label = 1; label < count; label++)
                            {
                                double area = stats.At<int>(label, areaCol);

                                if (area < minArea || area > maxArea)
                                    continue;

                                if (area > bestArea)
                                {
                                    bestArea = area;
                                    bestLabel = label;
                                }
                            }

                            if (bestLabel < 0)
                            {
                                result.Message = string.Format(
                                    "No blob in area range. Min={0:F0}, Max={1:F0}",
                                    minArea, maxArea);
                                return result;
                            }

                            // 무게중심 (분석영역 기준)
                            double cx = centroids.At<double>(bestLabel, 0);
                            double cy = centroids.At<double>(bestLabel, 1);

                            PointF centroidImage = new PointF(
                                (float)(analysisRoi.X + cx),
                                (float)(analysisRoi.Y + cy));

                            int boxX = stats.At<int>(bestLabel, leftCol);
                            int boxY = stats.At<int>(bestLabel, topCol);
                            int boxW = stats.At<int>(bestLabel, widthCol);
                            int boxH = stats.At<int>(bestLabel, heightCol);

                            Rectangle blobRect = new Rectangle(
                                analysisRoi.X + boxX,
                                analysisRoi.Y + boxY,
                                boxW, boxH);

                            // 선택된 blob의 contour 추출 + MinAreaRect 중심
                            // ── 보완1: 선택된 blob만 분리 후 정제 ──
                            // C800처럼 안쪽이 노이즈로 갈라져 구멍이 남는 경우,
                            // best blob 마스크에 Close+FillHoles를 재적용해 매끈한 단일 영역으로 만든다.
                            List<PointF> contourImage;
                            PointF minAreaCenter;
                            bool gotMinArea;

                            using (Mat blobMask = new Mat())
                            {
                                Cv2.InRange(
                                    labels,
                                    new Scalar(bestLabel),
                                    new Scalar(bestLabel),
                                    blobMask);

                                int refine = settings.BlobRefineCloseSize;
                                if (refine < 3) refine = 3;
                                if (refine % 2 == 0) refine++;

                                using (Mat rk = Cv2.GetStructuringElement(
                                    MorphShapes.Ellipse, new CvSize(refine, refine)))
                                {
                                    // 내부 만입/구멍 제거
                                    Cv2.MorphologyEx(blobMask, blobMask, MorphTypes.Close, rk);
                                }

                                // 정제된 마스크 내부 빈 영역 한 번 더 채우기
                                FillBinaryHoles(blobMask);

                                gotMinArea = GetBlobMinAreaCenterFromMask(
                                    blobMask, analysisRoi,
                                    out contourImage, out minAreaCenter);
                            }

                            // 중심: MinAreaRect 우선, 실패 시 무게중심
                            PointF finalCenter = gotMinArea ? minAreaCenter : centroidImage;

                            // 대칭성 검증: 무게중심 vs MinAreaRect 중심 차이
                            double symDiff = BlobUtility.GetDistance(centroidImage, finalCenter);

                            BlobCandidate slot = new BlobCandidate();
                            slot.IsVirtual = false;
                            slot.BoundingRect = blobRect;
                            slot.Center = finalCenter;
                            slot.Area = bestArea;
                            slot.Selected = true;

                            if (contourImage != null && contourImage.Count > 0)
                                slot.ContourPoints = contourImage;

                            result.SelectedCandidate = slot;
                            result.Candidates = new List<BlobCandidate> { slot };
                            result.Center = finalCenter;
                            result.Found = true;

                            if (symDiff > settings.SymmetryTolerancePx)
                            {
                                result.Message = string.Format(
                                    "OK RegionBlob (asymmetry warn: {0:F1}px)", symDiff);
                            }
                            else
                            {
                                result.Message = "OK RegionBlob";
                            }

                            return result;
                        }
                    }
                }
            }
        }

        // Region Blob 전처리: 어두운 테두리 → 안쪽 영역만 채워진 마스크
        private static void BuildRegionBlobImages(
            Mat roiGray,
            VentBlobSettings settings,
            out Mat preprocess,
            out Mat binary,
            out Mat mask)
        {
            preprocess = new Mat();

            // ── 보완2: 노이즈 억제 전처리 ──
            if (settings.UseBilateralPreblur)
            {
                using (Mat tmp = new Mat())
                {
                    Cv2.BilateralFilter(roiGray, tmp, 7, 50, 50);
                    Cv2.GaussianBlur(tmp, preprocess, new CvSize(5, 5), 0);
                }
            }
            else
            {
                Cv2.GaussianBlur(roiGray, preprocess, new CvSize(5, 5), 0);
            }

            // ── 방향1: 배경(조명 음영) 평탄화 ──
            // Vent 한쪽에 깔린 음영이 AdaptiveThreshold의 국소 평균을 끌어내려
            // 그쪽 테두리를 비대칭하게 두껍게 만드는 문제 보정.
            // 큰 커널 Open으로 저주파 배경(음영)을 추정해 빼서, 테두리만 균일하게 남김.
            if (settings.UseBackgroundFlatten)
            {
                FlattenBackground(preprocess, settings.FlattenKernelScale);
            }

            binary = new Mat();

            if (settings.ThresholdMode == VentThresholdMode.Adaptive)
            {
                int block = settings.AdaptiveBlockSize;
                if (block < 3) block = 3;
                if (block % 2 == 0) block += 1;

                Cv2.AdaptiveThreshold(
                    preprocess, binary,
                    255,
                    AdaptiveThresholdTypes.MeanC,
                    ThresholdTypes.BinaryInv,
                    block,
                    settings.AdaptiveC);
            }
            else if (settings.ThresholdMode == VentThresholdMode.Otsu)
            {
                Cv2.Threshold(
                    preprocess, binary, 0, 255,
                    ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);
            }
            else
            {
                Cv2.Threshold(
                    preprocess, binary, settings.Threshold, 255,
                    ThresholdTypes.BinaryInv);
            }

            mask = binary.Clone();

            int closeSize = settings.MorphCloseSize;
            if (closeSize < 3) closeSize = 3;
            if (closeSize % 2 == 0) closeSize++;

            using (Mat kernel = Cv2.GetStructuringElement(
                MorphShapes.Ellipse, new CvSize(closeSize, closeSize)))
            {
                // 1차 봉합 (등방성)
                Cv2.MorphologyEx(mask, mask, MorphTypes.Close, kernel);

                // ── 보완3: 가로 방향 Close 추가 ──
                // Vent는 수평 슬롯. 좌우 끝(round)의 가로 끊김은 등방성 커널보다
                // 가로로 긴 커널이 훨씬 잘 메움. B(2)의 우측 끊김 대응.
                int hw = settings.HorizontalCloseWidth;
                if (hw < 3) hw = 3;
                if (hw % 2 == 0) hw++;

                using (Mat hKernel = Cv2.GetStructuringElement(
                    MorphShapes.Rect, new CvSize(hw, 3)))
                {
                    Cv2.MorphologyEx(mask, mask, MorphTypes.Close, hKernel);
                }

                // 반전: 테두리=배경, 안쪽/바깥 영역=전경
                Cv2.BitwiseNot(mask, mask);

                // 잡티 제거
                using (Mat small = Cv2.GetStructuringElement(
                    MorphShapes.Ellipse, new CvSize(3, 3)))
                {
                    Cv2.MorphologyEx(mask, mask, MorphTypes.Open, small);
                }

                // 경계 닿는 전경(바깥 영역) 제거 → 안쪽만
                ClearBorderForeground(mask);

                // 안쪽 빈 영역(각인 등) 채우기
                FillBinaryHoles(mask);

                Cv2.MorphologyEx(mask, mask, MorphTypes.Open, kernel);
                Cv2.MorphologyEx(mask, mask, MorphTypes.Close, kernel);
            }
        }

        // 조명 음영(저주파 배경) 추정 후 차감하여 평탄화.
        // 어두운 테두리(전경 대상)를 보존하기 위해 morphological "Close"로 배경을 추정한다.
        //   - 밝은 배경 + 어두운 구조(테두리) 상황에서, 큰 커널 Close는
        //     어두운 테두리를 메워 "테두리 없는 밝은 배경"을 만든다.
        //   - 그 배경에서 원본을 빼면(=배경 - 원본) 어두운 테두리가 밝게 부각된 영상이 되고,
        //     이를 다시 반전 처리 없이 쓰기 위해 background - src 로 계산한 뒤
        //     원래 극성(어두운 테두리)을 유지하도록 재반전한다.
        private static void FlattenBackground(Mat gray, double kernelScale)
        {
            if (gray == null || gray.Empty())
                return;

            int h = gray.Height;

            int k = (int)Math.Round(h * kernelScale);

            if (k < 15)
                k = 15;

            if (k % 2 == 0)
                k += 1;

            using (Mat kernel = Cv2.GetStructuringElement(
                MorphShapes.Ellipse, new CvSize(k, k)))
            using (Mat background = new Mat())
            using (Mat flattened = new Mat())
            {
                // 어두운 테두리를 메운 "밝은 배경" 추정
                Cv2.MorphologyEx(gray, background, MorphTypes.Close, kernel);

                // 배경 대비 어두워진 정도 = background - gray (테두리가 클수록 큰 양수)
                Cv2.Subtract(background, gray, flattened);

                // 결과는 "테두리가 밝은(높은 값)" 영상.
                // 이후 파이프라인은 BinaryInv(어두운 테두리=전경) 가정이므로,
                // 극성을 원래대로(어두운 테두리) 되돌리기 위해 반전.
                Cv2.BitwiseNot(flattened, flattened);

                flattened.CopyTo(gray);
            }
        }

        // 분석영역 마스크를 사용자 ROI 크기로 잘라 원본 위에 입힌 Preview
        private static Bitmap CropPreviewToUserRoi(
            Bitmap sourceImage,
            Rectangle userRoi,
            Rectangle analysisRoi,
            Mat analysisMask)
        {
            int offsetX = userRoi.X - analysisRoi.X;
            int offsetY = userRoi.Y - analysisRoi.Y;

            if (offsetX < 0 || offsetY < 0)
                return BlobUtility.CreatePreviewBitmap(sourceImage, analysisRoi, analysisMask);

            if (offsetX + userRoi.Width > analysisMask.Width ||
                offsetY + userRoi.Height > analysisMask.Height)
                return BlobUtility.CreatePreviewBitmap(sourceImage, analysisRoi, analysisMask);

            CvRect crop = new CvRect(offsetX, offsetY, userRoi.Width, userRoi.Height);

            using (Mat view = new Mat(analysisMask, crop))
            using (Mat cropped = view.Clone())
            {
                return BlobUtility.CreatePreviewBitmap(sourceImage, userRoi, cropped);
            }
        }

        // 선택된 label의 MinAreaRect 중심 + contour(이미지 좌표) 추출
        private static bool GetBlobMinAreaCenter(
            Mat labels,
            int targetLabel,
            Rectangle analysisRoi,
            out List<PointF> contourImage,
            out PointF center)
        {
            contourImage = new List<PointF>();
            center = PointF.Empty;

            using (Mat lblMask = new Mat())
            {
                Cv2.InRange(
                    labels,
                    new Scalar(targetLabel),
                    new Scalar(targetLabel),
                    lblMask);

                if (lblMask.Empty())
                    return false;

                CvPoint[][] contours;
                HierarchyIndex[] hierarchy;

                Cv2.FindContours(
                    lblMask, out contours, out hierarchy,
                    RetrievalModes.External,
                    ContourApproximationModes.ApproxSimple);

                if (contours == null || contours.Length == 0)
                    return false;

                int bestIdx = 0;
                double bestArea = -1.0;

                for (int i = 0; i < contours.Length; i++)
                {
                    double a = Cv2.ContourArea(contours[i]);
                    if (a > bestArea) { bestArea = a; bestIdx = i; }
                }

                CvPoint[] best = contours[bestIdx];

                for (int i = 0; i < best.Length; i++)
                {
                    contourImage.Add(new PointF(
                        analysisRoi.X + best[i].X,
                        analysisRoi.Y + best[i].Y));
                }

                RotatedRect rr = Cv2.MinAreaRect(best);

                center = new PointF(
                    analysisRoi.X + rr.Center.X,
                    analysisRoi.Y + rr.Center.Y);

                return true;
            }
        }

        // 정제된 단일 blob 마스크에서 MinAreaRect 중심 + contour 추출
        private static bool GetBlobMinAreaCenterFromMask(
            Mat blobMask,
            Rectangle analysisRoi,
            out List<PointF> contourImage,
            out PointF center)
        {
            contourImage = new List<PointF>();
            center = PointF.Empty;

            if (blobMask == null || blobMask.Empty())
                return false;

            CvPoint[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(
                blobMask, out contours, out hierarchy,
                RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);

            if (contours == null || contours.Length == 0)
                return false;

            int bestIdx = 0;
            double bestArea = -1.0;

            for (int i = 0; i < contours.Length; i++)
            {
                double a = Cv2.ContourArea(contours[i]);
                if (a > bestArea) { bestArea = a; bestIdx = i; }
            }

            CvPoint[] best = contours[bestIdx];

            for (int i = 0; i < best.Length; i++)
            {
                contourImage.Add(new PointF(
                    analysisRoi.X + best[i].X,
                    analysisRoi.Y + best[i].Y));
            }

            RotatedRect rr = Cv2.MinAreaRect(best);

            center = new PointF(
                analysisRoi.X + rr.Center.X,
                analysisRoi.Y + rr.Center.Y);

            return true;
        }

        // 경계에 닿는 전경 제거 (imclearborder)
        private static void ClearBorderForeground(Mat bin)
        {
            int w = bin.Width;
            int h = bin.Height;

            if (w < 2 || h < 2)
                return;

            Scalar zero = new Scalar(0);

            for (int x = 0; x < w; x++)
            {
                if (bin.At<byte>(0, x) != 0)
                    Cv2.FloodFill(bin, new CvPoint(x, 0), zero);

                if (bin.At<byte>(h - 1, x) != 0)
                    Cv2.FloodFill(bin, new CvPoint(x, h - 1), zero);
            }

            for (int y = 0; y < h; y++)
            {
                if (bin.At<byte>(y, 0) != 0)
                    Cv2.FloodFill(bin, new CvPoint(0, y), zero);

                if (bin.At<byte>(y, w - 1) != 0)
                    Cv2.FloodFill(bin, new CvPoint(w - 1, y), zero);
            }
        }

        // 전경으로 둘러싸인 내부 구멍 채우기
        private static void FillBinaryHoles(Mat bin)
        {
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
    }
}