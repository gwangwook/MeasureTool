using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MeasureTool
{
    public partial class MainForm
    {
        private void MenuSaveOriginalImage_Click(object sender, EventArgs e)
        {
            if (_originalImage == null)
            {
                MessageBox.Show("저장할 원본 이미지가 없습니다.");
                return;
            }

            SaveBitmapWithDialog(
                _originalImage,
                GetDefaultSaveFileName("Original"),
                "원본 이미지 저장");
        }

        private void MenuSaveBinaryImage_Click(object sender, EventArgs e)
        {
            // tabPage3에서는 현재 View 모드에 해당하는 Preview를 저장
            if (tabControl1.SelectedTab == tabPage3)
            {
                Bitmap target = null;
                string suffix = "";

                switch (_hvViewMode)
                {
                    case HvViewImageMode.HoleBinary:
                        target = _holeBinaryPreview;
                        suffix = "HoleBinary";
                        break;

                    case HvViewImageMode.HoleBinaryDetect:
                        target = _holeBinaryDetectPreview;
                        suffix = "HoleBinaryDetect";
                        break;

                    case HvViewImageMode.VentBinary:
                        target = _ventBinaryPreview;
                        suffix = "VentBinary";
                        break;

                    case HvViewImageMode.VentBinaryDetect:
                        target = _ventBinaryDetectPreview;
                        suffix = "VentBinaryDetect";
                        break;

                    case HvViewImageMode.Original:
                    default:
                        MessageBox.Show(
                            "현재 View가 Original입니다.\r\nView를 Preprocess 또는 Binary로 변경 후 저장하세요.");
                        return;
                }

                if (target == null)
                {
                    MessageBox.Show(
                        "저장할 Preview 이미지가 없습니다.\r\nRun Measure를 먼저 실행하세요.");
                    return;
                }

                SaveBitmapWithDialog(
                    target,
                    GetDefaultSaveFileName(suffix),
                    suffix + " 이미지 저장");

                return;
            }

            // 그 외 탭은 기존 동작 유지 (Preprocess > Binary)
            if (_binaryImage == null)
            {
                MessageBox.Show("저장할 이진화 이미지가 없습니다.\r\n먼저 전처리 → 이진화를 적용하세요.");
                return;
            }

            SaveBitmapWithDialog(
                _binaryImage,
                GetDefaultSaveFileName(string.Format("Binary_T{0}", _binaryThreshold)),
                "이진화 이미지 저장");
        }

        private void MenuSaveCurrentImage_Click(object sender, EventArgs e)
        {
            if (_loadedImage == null)
            {
                MessageBox.Show("저장할 현재 이미지가 없습니다.");
                return;
            }

            // tabPage3에서는 현재 View 모드(콤보 선택)에 해당하는 이미지를 저장
            Bitmap viewImage = GetViewerDrawImage();

            SaveBitmapWithDialog(
                viewImage,
                GetDefaultSaveFileName(GetHvViewSuffix("Current")),
                "현재 이미지 저장");
        }

        private void MenuSaveCurrentOverlayImage_Click(object sender, EventArgs e)
        {
            if (_loadedImage == null)
            {
                MessageBox.Show("저장할 현재 이미지가 없습니다.");
                return;
            }

            using (Bitmap overlayImage = CreateCurrentImageWithOverlay())
            {
                if (overlayImage == null)
                {
                    MessageBox.Show("Overlay 이미지 생성에 실패했습니다.");
                    return;
                }

                SaveBitmapWithDialog(
                    overlayImage,
                    GetDefaultSaveFileName(GetHvViewSuffix("ResultOverlay") + "_Overlay"),
                    "현재 이미지 + 결과 Overlay 저장");
            }
        }

        private void SaveBitmapWithDialog(Bitmap source, string defaultFileName, string dialogTitle)
        {
            if (source == null)
            {
                MessageBox.Show("저장할 이미지가 없습니다.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = dialogTitle;
                sfd.FileName = defaultFileName;
                sfd.Filter =
                    "PNG Image|*.png|" +
                    "Bitmap Image|*.bmp|" +
                    "JPEG Image|*.jpg;*.jpeg|" +
                    "TIFF Image|*.tif;*.tiff";

                sfd.FilterIndex = 1;
                sfd.OverwritePrompt = true;
                sfd.AddExtension = true;

                if (sfd.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    ImageFormat format = GetImageFormatFromFileName(sfd.FileName);

                    // 원본 Bitmap을 직접 Save하면 파일 잠금/PixelFormat 문제 가능성이 있으므로 Clone 저장
                    using (Bitmap saveImage = CloneBitmapForSave(source))
                    {
                        saveImage.Save(sfd.FileName, format);
                    }

                    MessageBox.Show("이미지 저장이 완료되었습니다.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "이미지 저장 실패\r\n" + ex.Message,
                        "Save Image",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private Bitmap CloneBitmapForSave(Bitmap source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            // 8bppIndexed Binary 이미지는 8bpp 그대로 저장하는 것이 좋음
            if (source.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                return Clone8bppIndexedBitmap(source);
            }

            // 그 외 포맷은 24bpp로 안전하게 저장
            Bitmap clone = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(clone))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(source, 0, 0, source.Width, source.Height);
            }

            return clone;
        }

        private Bitmap Clone8bppIndexedBitmap(Bitmap source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (source.PixelFormat != PixelFormat.Format8bppIndexed)
                return CloneBitmapForSave(source);

            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

            Bitmap clone = new Bitmap(source.Width, source.Height, PixelFormat.Format8bppIndexed);

            // Palette 복사
            clone.Palette = source.Palette;

            BitmapData srcData = null;
            BitmapData dstData = null;

            try
            {
                srcData = source.LockBits(
                    rect,
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format8bppIndexed);

                dstData = clone.LockBits(
                    rect,
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);

                int srcStride = srcData.Stride;
                int dstStride = dstData.Stride;

                int srcBytes = Math.Abs(srcStride) * source.Height;
                int dstBytes = Math.Abs(dstStride) * source.Height;

                byte[] srcBuffer = new byte[srcBytes];
                byte[] dstBuffer = new byte[dstBytes];

                Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBytes);

                int copyWidth = source.Width;

                for (int y = 0; y < source.Height; y++)
                {
                    int srcRow = y * srcStride;
                    int dstRow = y * dstStride;

                    Buffer.BlockCopy(srcBuffer, srcRow, dstBuffer, dstRow, copyWidth);
                }

                Marshal.Copy(dstBuffer, 0, dstData.Scan0, dstBytes);
            }
            finally
            {
                if (srcData != null)
                    source.UnlockBits(srcData);

                if (dstData != null)
                    clone.UnlockBits(dstData);
            }

            return clone;
        }

        private ImageFormat GetImageFormatFromFileName(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(ext))
                return ImageFormat.Png;

            ext = ext.ToLowerInvariant();

            if (ext == ".bmp")
                return ImageFormat.Bmp;

            if (ext == ".jpg" || ext == ".jpeg")
                return ImageFormat.Jpeg;

            if (ext == ".tif" || ext == ".tiff")
                return ImageFormat.Tiff;

            return ImageFormat.Png;
        }

        private Bitmap CreateCurrentImageWithOverlay()
        {
            if (_loadedImage == null)
                return null;

            Bitmap baseImage = _loadedImage;

            // Hole / Vent Measure에서는 현재 View 모드(콤보 선택)에 해당하는 이미지를
            // Overlay base로 사용. (Original 선택 시 원본, Binary 선택 시 해당 Preview)
            if (tabControl1.SelectedTab == tabPage3)
            {
                Bitmap viewImage = GetViewerDrawImage();

                if (viewImage != null)
                    baseImage = viewImage;
            }

            Bitmap result = null;

            try
            {
                result = new Bitmap(
                    baseImage.Width,
                    baseImage.Height,
                    PixelFormat.Format24bppRgb);

                using (Graphics g = Graphics.FromImage(result))
                {
                    g.Clear(Color.Black);

                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // 저장용 이미지는 화면 캡처가 아니라 원본 해상도 기준으로 그림
                    g.DrawImage(baseImage, 0, 0, baseImage.Width, baseImage.Height);

                    if (tabControl1.SelectedTab == tabPage1)
                    {
                        DrawManualMeasureOverlayForSave(g);
                    }
                    else if (tabControl1.SelectedTab == tabPage2)
                    {
                        DrawRoiOverlayForSave(g);
                    }
                    else if (tabControl1.SelectedTab == tabPage3)
                    {
                        DrawHoleVentOverlayForSave(g);
                    }
                }

                return result;
            }
            catch
            {
                if (result != null)
                    result.Dispose();

                return null;
            }
        }

        private string GetDefaultSaveFileName(string suffix)
        {
            string baseName = "Image";

            try
            {
                if (!string.IsNullOrWhiteSpace(lblFileName.Text))
                {
                    string text = lblFileName.Text.Trim();

                    // lblFileName.Text 형식: "File : 파일명.ext"
                    string prefix = "File :";

                    if (text.StartsWith(prefix))
                    {
                        string file = text.Substring(prefix.Length).Trim();

                        if (!string.IsNullOrWhiteSpace(file) && file != "-")
                        {
                            baseName = System.IO.Path.GetFileNameWithoutExtension(file);
                        }
                    }
                }
            }
            catch
            {
                baseName = "Image";
            }

            string time = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            if (string.IsNullOrWhiteSpace(suffix))
                return string.Format("{0}_{1}.png", baseName, time);

            return string.Format("{0}_{1}_{2}.png", baseName, suffix, time);
        }

        private string GetHvViewSuffix(string baseSuffix)
        {
            // tabPage3가 아니면 기본 suffix 그대로
            if (tabControl1.SelectedTab != tabPage3)
                return baseSuffix;

            switch (_hvViewMode)
            {
                case HvViewImageMode.HoleBinary:
                    return "HoleBinary";

                case HvViewImageMode.HoleBinaryDetect:
                    return "HoleBinaryDetect";

                case HvViewImageMode.VentBinary:
                    return "VentBinary";

                case HvViewImageMode.VentBinaryDetect:
                    return "VentBinaryDetect";

                case HvViewImageMode.Original:
                default:
                    return baseSuffix;
            }
        }

        private void DrawManualMeasureOverlayForSave(Graphics g)
        {
            if (_measurePoint1Raw == null)
                return;

            using (Pen rawPointPen = new Pen(Color.Lime, 2))
            using (Pen measureLinePen = new Pen(Color.Red, 2))
            using (Pen usedPointPen = new Pen(Color.Orange, 2))
            using (Font font = new Font("맑은 고딕", GetSaveFontSize(), FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.Cyan))
            using (Brush usedTextBrush = new SolidBrush(Color.Orange))
            {
                Point p1Raw = _measurePoint1Raw.Value;
                PointF p1RawImage = ImagePixelCenterToImagePoint(p1Raw);

                DrawCross(g, rawPointPen, p1RawImage, GetSaveCrossSize());
                g.DrawString(
                    string.Format("P1 ({0}, {1})", p1Raw.X, p1Raw.Y),
                    font,
                    textBrush,
                    p1RawImage.X + 8,
                    p1RawImage.Y + 8);

                if (_measurePoint2Raw == null)
                    return;

                Point p2Raw = _measurePoint2Raw.Value;
                PointF p2RawImage = ImagePixelCenterToImagePoint(p2Raw);

                DrawCross(g, rawPointPen, p2RawImage, GetSaveCrossSize());
                g.DrawString(
                    string.Format("P2 ({0}, {1})", p2Raw.X, p2Raw.Y),
                    font,
                    textBrush,
                    p2RawImage.X + 8,
                    p2RawImage.Y + 8);

                Point p1Calc;
                Point p2Calc;

                if (!TryGetCalculatedMeasurePoints(out p1Calc, out p2Calc))
                    return;

                PointF p1CalcImage = ImagePixelCenterToImagePoint(p1Calc);
                PointF p2CalcImage = ImagePixelCenterToImagePoint(p2Calc);

                g.DrawLine(measureLinePen, p1CalcImage, p2CalcImage);

                if (p2Calc != p2Raw)
                {
                    DrawCross(g, usedPointPen, p2CalcImage, GetSaveCrossSize() - 2);
                    g.DrawString(
                        "Used",
                        font,
                        usedTextBrush,
                        p2CalcImage.X + 8,
                        p2CalcImage.Y - 18);
                }

                int dx = p2Calc.X - p1Calc.X;
                int dy = p2Calc.Y - p1Calc.Y;
                double distancePx = GetDistance(p1Calc, p2Calc);

                string displayText = string.Format(
                    "D={0:F3}px, dX={1}px, dY={2}px",
                    distancePx,
                    dx,
                    dy);

                PointF mid = new PointF(
                    (p1CalcImage.X + p2CalcImage.X) / 2.0f,
                    (p1CalcImage.Y + p2CalcImage.Y) / 2.0f);

                g.DrawString(displayText, font, textBrush, mid.X + 10, mid.Y + 10);
            }
        }

        private void DrawRoiOverlayForSave(Graphics g)
        {
            if (_loadedImage == null)
                return;

            _roiMeasureType = GetCurrentRoiMeasureType();

            if (_roiMeasureType == RoiMeasureType.SingleRoiDualEdge)
            {
                if (_roi1Rect.HasValue)
                    DrawSingleRoiForSave(g, _roi1Rect.Value, "Single ROI", Color.DeepSkyBlue, _selectedRoiIndex == 1);
            }
            else
            {
                if (_roi1Rect.HasValue)
                    DrawSingleRoiForSave(g, _roi1Rect.Value, "ROI1", Color.DeepSkyBlue, _selectedRoiIndex == 1);

                if (_roi2Rect.HasValue)
                    DrawSingleRoiForSave(g, _roi2Rect.Value, "ROI2", Color.Orange, _selectedRoiIndex == 2);
            }

            if (_roi1EdgeResult != null && _roi1EdgeResult.Found)
                DrawSingleRoiEdgeResultForSave(g, _roi1EdgeResult, Color.Cyan);

            if (_roi2EdgeResult != null && _roi2EdgeResult.Found)
                DrawSingleRoiEdgeResultForSave(g, _roi2EdgeResult, Color.Yellow);

            DrawRoiMeasureOverlayForSave(g);
        }

        private void DrawHoleVentOverlayForSave(Graphics g)
        {
            if (_originalImage == null)
                return;

            // 1. ROI 표시
            if (_holeRoiRect.HasValue)
            {
                DrawSingleHoleVentRoiForSave(
                    g,
                    _holeRoiRect.Value,
                    "Hole ROI",
                    Color.DeepSkyBlue,
                    _selectedHvRoiIndex == 1);
            }

            if (_ventRoiRect.HasValue)
            {
                DrawSingleHoleVentRoiForSave(
                    g,
                    _ventRoiRect.Value,
                    "Vent ROI",
                    Color.Orange,
                    _selectedHvRoiIndex == 2);
            }

            // 2. Blob 검출 결과 표시
            if (_holeDetectResult != null)
            {
                DrawBlobResultOverlayForSave(
                    g,
                    _holeDetectResult.Candidates,
                    _holeDetectResult.SelectedCandidate,
                    _holeDetectResult.Found,
                    Color.Yellow,
                    Color.Lime,
                    Color.Cyan);
            }

            if (_ventDetectResult != null)
            {
                DrawBlobResultOverlayForSave(
                    g,
                    _ventDetectResult.Candidates,
                    _ventDetectResult.SelectedCandidate,
                    _ventDetectResult.Found,
                    Color.Lime,
                    Color.Lime,
                    Color.Cyan);
            }

            // 3. Center 표시
            if (_holeCenter.HasValue)
                DrawCenterTextForSave(g, _holeCenter.Value, "Hole");

            if (_ventCenter.HasValue)
                DrawCenterTextForSave(g, _ventCenter.Value, "Vent");

            // 4. 거리선 표시
            DrawHoleVentDistanceOverlayForSave(g);
        }

        private void DrawSingleHoleVentRoiForSave(
            Graphics g,
            Rectangle roi,
            string name,
            Color color,
            bool selected)
        {
            float penWidth = selected ? 3.0f : 2.0f;

            using (Pen pen = new Pen(color, penWidth))
            using (Brush textBrush = new SolidBrush(color))
            using (Font font = new Font("맑은 고딕", GetSaveFontSize(), FontStyle.Bold))
            {
                g.DrawRectangle(
                    pen,
                    roi.X,
                    roi.Y,
                    roi.Width,
                    roi.Height);

                string text = string.Format(
                    "{0} ({1},{2},{3},{4})",
                    name,
                    roi.X,
                    roi.Y,
                    roi.Width,
                    roi.Height);

                g.DrawString(
                    text,
                    font,
                    textBrush,
                    roi.X + 4,
                    roi.Y - 22);
            }

            DrawRoiHandlesForSave(g, roi, color);
        }

        private void DrawBlobResultOverlayForSave(
            Graphics g,
            List<BlobCandidate> candidates,
            BlobCandidate selected,
            bool found,
            Color candidateColor,
            Color selectedColor,
            Color rectColor)
        {
            float baseWidth = GetSaveLineWidth();

            if (candidates != null)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    BlobCandidate c = candidates[i];

                    Color color = c.Selected ? selectedColor : candidateColor;
                    float width = c.Selected ? baseWidth : Math.Max(0.5f, baseWidth * 0.5f);

                    DrawContourOverlayForSave(g, c.ContourPoints, color, width);
                }
            }

            if (found && selected != null)
            {
                Rectangle br = selected.BoundingRect;

                using (Pen pen = new Pen(rectColor, baseWidth))
                {
                    g.DrawRectangle(pen, br);
                }
            }
        }

        private void DrawContourOverlayForSave(
            Graphics g,
            List<PointF> contour,
            Color color,
            float width)
        {
            if (contour == null || contour.Count < 2)
                return;

            using (Pen pen = new Pen(color, width))
            {
                if (contour.Count >= 3)
                    g.DrawPolygon(pen, contour.ToArray());
                else
                    g.DrawLines(pen, contour.ToArray());
            }
        }

        private void DrawCenterTextForSave(Graphics g, PointF center, string label)
        {
            using (Pen centerPen = new Pen(Color.Lime, GetSaveLineWidth()))
            using (Brush textBrush = new SolidBrush(Color.Lime))
            using (Font font = new Font("맑은 고딕", GetSaveFontSize(), FontStyle.Bold))
            {
                DrawCross(g, centerPen, center, GetSaveCrossSize());

                string text = string.Format(
                    "{0} ({1:F1}, {2:F1})",
                    label,
                    center.X,
                    center.Y);

                g.DrawString(text, font, textBrush, center.X + 10, center.Y + 10);
            }
        }

        private void DrawHoleVentDistanceOverlayForSave(Graphics g)
        {
            if (!_hvMeasureCalcPoint1.HasValue || !_hvMeasureCalcPoint2.HasValue)
                return;

            PointF p1 = _hvMeasureCalcPoint1.Value;
            PointF p2 = _hvMeasureCalcPoint2.Value;

            bool hasActualHole = _holeCenter.HasValue;
            bool hasActualVent = _ventCenter.HasValue;

            bool p1IsDifferentFromHole =
                hasActualHole &&
                GetPointDistance(p1, _holeCenter.Value) > 0.01;

            bool p2IsDifferentFromVent =
                hasActualVent &&
                GetPointDistance(p2, _ventCenter.Value) > 0.01;

            using (Pen measureLinePen = new Pen(Color.Cyan, 2))
            using (Pen usedPointPen = new Pen(Color.Cyan, 2))
            using (Brush textBrush = new SolidBrush(Color.Cyan))
            using (Brush usedTextBrush = new SolidBrush(Color.Cyan))
            using (Font font = new Font("맑은 고딕", GetSaveFontSize(), FontStyle.Bold))
            {
                g.DrawLine(measureLinePen, p1, p2);

                if (p1IsDifferentFromHole)
                {
                    DrawCross(g, usedPointPen, p1, GetSaveCrossSize());
                    g.DrawString(
                        "Used",
                        font,
                        usedTextBrush,
                        p1.X + 8,
                        p1.Y - 18);
                }

                if (p2IsDifferentFromVent)
                {
                    DrawCross(g, usedPointPen, p2, GetSaveCrossSize());
                    g.DrawString(
                        "Used",
                        font,
                        usedTextBrush,
                        p2.X + 8,
                        p2.Y - 18);
                }

                PointF mid = new PointF(
                    (p1.X + p2.X) / 2.0f,
                    (p1.Y + p2.Y) / 2.0f);

                string text = string.Format(
                    "D={0:F3}px, {1:F6}mm, {2:F3}um",
                    _hvDistancePx,
                    _hvDistanceMm,
                    _hvDistanceUm);

                g.DrawString(
                    text,
                    font,
                    textBrush,
                    mid.X + 10,
                    mid.Y + 10);
            }
        }

        private void DrawSingleRoiForSave(
            Graphics g,
            Rectangle roi,
            string name,
            Color color,
            bool selected)
        {
            float penWidth = selected ? 3.0f : 2.0f;

            using (Pen pen = new Pen(color, penWidth))
            using (Brush textBrush = new SolidBrush(color))
            using (Font font = new Font("맑은 고딕", GetSaveFontSize(), FontStyle.Bold))
            {
                g.DrawRectangle(
                    pen,
                    roi.X,
                    roi.Y,
                    roi.Width,
                    roi.Height);

                g.DrawString(
                    string.Format("{0} ({1},{2},{3},{4})", name, roi.X, roi.Y, roi.Width, roi.Height),
                    font,
                    textBrush,
                    roi.X + 4,
                    roi.Y - 22);
            }

            DrawRoiHandlesForSave(g, roi, color);
        }

        private void DrawRoiHandlesForSave(Graphics g, Rectangle roi, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            using (Pen pen = new Pen(Color.Black, 1))
            {
                RectangleF[] handles = GetRoiHandleRectsForSave(roi);

                for (int i = 0; i < handles.Length; i++)
                {
                    g.FillRectangle(brush, handles[i]);
                    g.DrawRectangle(
                        pen,
                        handles[i].X,
                        handles[i].Y,
                        handles[i].Width,
                        handles[i].Height);
                }
            }
        }

        private RectangleF[] GetRoiHandleRectsForSave(Rectangle roi)
        {
            float s = RoiHandleSize;
            float hs = s / 2.0f;

            float left = roi.Left;
            float right = roi.Right;
            float top = roi.Top;
            float bottom = roi.Bottom;
            float cx = roi.Left + roi.Width / 2.0f;
            float cy = roi.Top + roi.Height / 2.0f;

            return new RectangleF[]
            {
        new RectangleF(left - hs, top - hs, s, s),
        new RectangleF(cx - hs, top - hs, s, s),
        new RectangleF(right - hs, top - hs, s, s),

        new RectangleF(left - hs, cy - hs, s, s),
        new RectangleF(right - hs, cy - hs, s, s),

        new RectangleF(left - hs, bottom - hs, s, s),
        new RectangleF(cx - hs, bottom - hs, s, s),
        new RectangleF(right - hs, bottom - hs, s, s)
            };
        }

        private void DrawSingleRoiEdgeResultForSave(
            Graphics g,
            RoiEdgeResult result,
            Color color)
        {
            if (result == null || !result.Found)
                return;

            using (Brush pointBrush = new SolidBrush(color))
            using (Pen linePen = new Pen(Color.Red, 2))
            {
                // Inlier Edge Points
                for (int i = 0; i < result.InlierPoints.Count; i++)
                {
                    PointF pt = result.InlierPoints[i];

                    g.FillEllipse(
                        pointBrush,
                        pt.X - 2,
                        pt.Y - 2,
                        4,
                        4);
                }

                // Fitted Line
                g.DrawLine(linePen, result.LineStart, result.LineEnd);

                // Representative Point
                g.DrawEllipse(
                    linePen,
                    result.RepresentativePoint.X - 5,
                    result.RepresentativePoint.Y - 5,
                    10,
                    10);
            }
        }

        private void DrawRoiMeasureOverlayForSave(Graphics g)
        {
            if (!_roiMeasureCalcPoint1.HasValue || !_roiMeasureCalcPoint2.HasValue)
                return;

            PointF p1 = _roiMeasureCalcPoint1.Value;
            PointF p2 = _roiMeasureCalcPoint2.Value;

            using (Pen pen = new Pen(Color.Lime, 2))
            using (Font font = new Font("맑은 고딕", GetSaveFontSize(), FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Lime))
            {
                g.DrawLine(pen, p1, p2);

                DrawCross(g, pen, p1, GetSaveCrossSize());
                DrawCross(g, pen, p2, GetSaveCrossSize());

                PointF mid = new PointF(
                    (p1.X + p2.X) / 2.0f,
                    (p1.Y + p2.Y) / 2.0f);

                string text = string.Format("D={0:F3}px", _roiDistancePx);

                g.DrawString(text, font, brush, mid.X + 8, mid.Y + 8);
            }
        }

        private PointF ImagePixelCenterToImagePoint(Point imagePixel)
        {
            return new PointF(imagePixel.X + 0.5f, imagePixel.Y + 0.5f);
        }

        private float GetSaveCrossSize()
        {
            if (_loadedImage == null)
                return 6.0f;

            // 이미지 해상도가 커도 너무 작게 보이지 않도록 약간 보정
            float baseSize = Math.Max(_loadedImage.Width, _loadedImage.Height) / 350.0f;

            if (baseSize < 6.0f)
                baseSize = 6.0f;

            if (baseSize > 14.0f)
                baseSize = 14.0f;

            return baseSize;
        }

        private float GetSaveLineWidth()
        {
            if (_loadedImage == null)
                return 1.0f;

            // Cross 크기보다 더 얇게. 해상도 비례.
            float width = Math.Max(_loadedImage.Width, _loadedImage.Height) / 1200.0f;

            if (width < 1.0f)
                width = 1.0f;

            if (width > 3.0f)
                width = 3.0f;

            return width;
        }

        private float GetSaveFontSize()
        {
            if (_loadedImage == null)
                return 7.0f;

            float fontSize = Math.Max(_loadedImage.Width, _loadedImage.Height) / 220.0f;

            if (fontSize < 7.0f)
                fontSize = 7.0f;

            if (fontSize > 14.0f)
                fontSize = 14.0f;

            return fontSize;
        }
    }
}