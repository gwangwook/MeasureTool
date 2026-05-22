using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace MeasureTool
{
    public partial class MainForm
    {
        // =========================================================
        // Initialization
        // =========================================================
        private void InitHoleVentControls()
        {
            if (tabPage3 != null)
                tabPage3.Text = "Hole / Vent Measure";

            // Measure Mode 기본값
            rbHvMeasureFree.Checked = true;
            chkHvShowOverlay.Checked = true;

            // Resolution 기본값
            _hvResolutionMmPerPx = 0.001;
            _lastValidHvResolutionText = _hvResolutionMmPerPx.ToString(
                "0.################",
                CultureInfo.InvariantCulture);

            tbHvResolution.Text = _lastValidHvResolutionText;

            // Hole Setting 기본값
            nudHoleThreshold.Value = 150;
            nudHoleMorph.Value = 0;
            nudHoleMinArea.Value = 3000;
            nudHoleMaxArea.Value = 50000;
            cmbHolePolarity.SelectedIndex = (int)BlobPolarity.BrightBlob;
            cmbHoleCenterMethod.SelectedIndex = (int)HoleCenterMethod.BoundingRect;

            // Vent Setting 기본값
            nudVentThreshold.Value = 150;
            cmbVentThresholdMode.SelectedIndex = (int)VentThresholdMode.Adaptive;

            // Vent Threshold는 ThresholdMode가 Manual일 때만 유효
            UpdateVentThresholdEnabled();

            // View Image 기본값
            cmbHvViewImage.SelectedIndex = (int)HvViewImageMode.Original;
            _hvViewMode = HvViewImageMode.Original;

            ClearHoleVentResultLabels();
        }

        // =========================================================
        // Settings Build
        // =========================================================
        private HoleBlobSettings BuildHoleSettings()
        {
            HoleBlobSettings s = new HoleBlobSettings();

            s.Threshold = (int)nudHoleThreshold.Value;
            s.Morph = (int)nudHoleMorph.Value;
            s.MinArea = (int)nudHoleMinArea.Value;
            s.MaxArea = (int)nudHoleMaxArea.Value;

            s.Polarity = (BlobPolarity)Math.Max(0, cmbHolePolarity.SelectedIndex);
            s.CenterMethod = (HoleCenterMethod)Math.Max(0, cmbHoleCenterMethod.SelectedIndex);

            return s;
        }

        private VentBlobSettings BuildVentSettings()
        {
            VentBlobSettings s = new VentBlobSettings();

            s.Threshold = (int)nudVentThreshold.Value;
            s.ThresholdMode = (VentThresholdMode)Math.Max(0, cmbVentThresholdMode.SelectedIndex);

            return s;
        }

        // =========================================================
        // Run
        // =========================================================
        private void BtnRunHvMeasure_Click(object sender, EventArgs e)
        {
            if (_originalImage == null)
            {
                MessageBox.Show("원본 이미지를 먼저 로드하세요.");
                return;
            }

            if (!_holeRoiRect.HasValue)
            {
                MessageBox.Show("Hole ROI를 먼저 생성하세요.");
                return;
            }

            if (!_ventRoiRect.HasValue)
            {
                MessageBox.Show("Vent ROI를 먼저 생성하세요.");
                return;
            }

            ClearHoleVentResultLabels();

            // 1. Hole 검출
            _holeDetectResult = HoleBlobDetector.Detect(
                _originalImage,
                _holeRoiRect.Value,
                BuildHoleSettings());

            ApplyDetectPreviews(_holeDetectResult, isHole: true);

            if (_holeDetectResult != null && _holeDetectResult.Found)
            {
                _holeCenter = _holeDetectResult.Center;

                BlobCandidate hc = _holeDetectResult.SelectedCandidate;

                lblHvHoleCenter.Text = string.Format(
                    "Hole Center : X={0:F3}, Y={1:F3}",
                    _holeCenter.Value.X,
                    _holeCenter.Value.Y);

                lblHvHoleStatus.Text = string.Format(
                    "Hole : {0}, Area={1:F0}, Circ={2:F3}, Candidates={3}",
                    _holeDetectResult.Message,
                    hc != null ? hc.Area : 0,
                    hc != null ? hc.Circularity : 0,
                    _holeDetectResult.Candidates != null ? _holeDetectResult.Candidates.Count : 0);
            }
            else
            {
                _holeCenter = null;

                string message = "Not Found";

                if (_holeDetectResult != null && !string.IsNullOrWhiteSpace(_holeDetectResult.Message))
                    message = _holeDetectResult.Message;

                lblHvHoleCenter.Text = "Hole Center : -";
                lblHvHoleStatus.Text = "Hole : " + message;
            }

            // 2. Vent 검출
            _ventDetectResult = VentBlobDetector.Detect(
                _originalImage,
                _ventRoiRect.Value,
                BuildVentSettings());

            ApplyDetectPreviews(_ventDetectResult, isHole: false);

            if (_ventDetectResult != null && _ventDetectResult.Found)
            {
                _ventCenter = _ventDetectResult.Center;

                BlobCandidate vc = _ventDetectResult.SelectedCandidate;

                lblHvVentCenter.Text = string.Format(
                    "Vent Center : X={0:F3}, Y={1:F3}",
                    _ventCenter.Value.X,
                    _ventCenter.Value.Y);

                lblHvVentStatus.Text = string.Format(
                    "Vent : {0}, Area={1:F0}, AR={2:F2}",
                    _ventDetectResult.Message,
                    vc != null ? vc.Area : 0,
                    vc != null ? vc.AspectRatio : 0);
            }
            else
            {
                _ventCenter = null;

                string message = "Not Found";

                if (_ventDetectResult != null && !string.IsNullOrWhiteSpace(_ventDetectResult.Message))
                    message = _ventDetectResult.Message;

                lblHvVentCenter.Text = "Vent Center : -";
                lblHvVentStatus.Text = "Vent : " + message;
            }

            // 3. 거리 계산
            if (_holeCenter.HasValue && _ventCenter.HasValue)
            {
                CalculateHoleVentDistance(
                    _holeCenter.Value,
                    _ventCenter.Value,
                    GetCurrentHvMeasureCalcMode(),
                    _hvResolutionMmPerPx);

                UpdateHoleVentDistanceLabels();
            }
            else
            {
                _hvMeasureCalcPoint1 = null;
                _hvMeasureCalcPoint2 = null;

                _hvDistancePx = 0.0;
                _hvDistanceMm = 0.0;
                _hvDistanceUm = 0.0;

                lblHvDistancePx.Text = "Distance px : -";
                lblHvDistanceMm.Text = "Distance mm : -";
                lblHvDistanceUm.Text = "Distance um : -";
            }

            pnlViewer.Invalidate();
        }

        // =========================================================
        // Preview Bitmap 관리
        // =========================================================
        private void ApplyDetectPreviews(HoleDetectResult result, bool isHole)
        {
            if (result == null || !isHole)
                return;

            ReplacePreviewBitmap(ref _holeBinaryPreview, result.BinaryPreview);
            ReplacePreviewBitmap(ref _holeBinaryDetectPreview, result.DetectPreview);

            result.BinaryPreview = null;
            result.DetectPreview = null;
        }

        private void ApplyDetectPreviews(VentDetectResult result, bool isHole)
        {
            if (result == null || isHole)
                return;

            ReplacePreviewBitmap(ref _ventBinaryPreview, result.BinaryPreview);
            ReplacePreviewBitmap(ref _ventBinaryDetectPreview, result.DetectPreview);

            result.BinaryPreview = null;
            result.DetectPreview = null;
        }


        private void ReplacePreviewBitmap(ref Bitmap target, Bitmap newBitmap)
        {
            if (target != null)
            {
                target.Dispose();
                target = null;
            }

            target = newBitmap;
        }

        private void DisposeHoleVentPreviewImages()
        {
            ReplacePreviewBitmap(ref _holeBinaryPreview, null);
            ReplacePreviewBitmap(ref _holeBinaryDetectPreview, null);
            ReplacePreviewBitmap(ref _ventBinaryPreview, null);
            ReplacePreviewBitmap(ref _ventBinaryDetectPreview, null);
        }

        // =========================================================
        // View Image 전환
        // =========================================================
        private void CmbHvViewImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cmbHvViewImage.SelectedIndex;
            if (idx < 0) idx = 0;

            HvViewImageMode requested = (HvViewImageMode)idx;

            // Detect 모드는 Run 후에만 가능
            if (requested == HvViewImageMode.HoleBinaryDetect && _holeDetectResult == null)
            {
                // 일반 Binary로 fallback
                SetViewModeSilently(HvViewImageMode.HoleBinary);
                EnsureHoleBinaryPreview();
                pnlViewer.Invalidate();
                return;
            }

            if (requested == HvViewImageMode.VentBinaryDetect && _ventDetectResult == null)
            {
                SetViewModeSilently(HvViewImageMode.VentBinary);
                EnsureVentBinaryPreview();
                pnlViewer.Invalidate();
                return;
            }

            _hvViewMode = requested;

            // Run 전 Binary 항목 선택 시, Preview 없으면 즉시 생성
            if (requested == HvViewImageMode.HoleBinary)
                EnsureHoleBinaryPreview();
            else if (requested == HvViewImageMode.VentBinary)
                EnsureVentBinaryPreview();

            pnlViewer.Invalidate();
        }

        private bool _isUpdatingViewMode = false;

        private void SetViewModeSilently(HvViewImageMode mode)
        {
            _isUpdatingViewMode = true;
            cmbHvViewImage.SelectedIndex = (int)mode;
            _hvViewMode = mode;
            _isUpdatingViewMode = false;
        }

        private void EnsureHoleBinaryPreview()
        {
            if (_holeBinaryPreview != null)
                return;

            if (_originalImage == null || !_holeRoiRect.HasValue)
                return;

            HoleDetectResult preview = HoleBlobDetector.Preview(
                _originalImage, _holeRoiRect.Value, BuildHoleSettings());

            if (preview != null)
            {
                ReplacePreviewBitmap(ref _holeBinaryPreview, preview.BinaryPreview);
                preview.BinaryPreview = null;
            }
        }

        private void EnsureVentBinaryPreview()
        {
            if (_ventBinaryPreview != null)
                return;

            if (_originalImage == null || !_ventRoiRect.HasValue)
                return;

            VentDetectResult preview = VentBlobDetector.Preview(
                _originalImage, _ventRoiRect.Value, BuildVentSettings());

            if (preview != null)
            {
                ReplacePreviewBitmap(ref _ventBinaryPreview, preview.BinaryPreview);
                preview.BinaryPreview = null;
            }
        }

        private void NudHoleValueChanged_ForLivePreview(object sender, EventArgs e)
        {
            // 현재 View가 Hole Binary면 즉시 재계산
            if (_hvViewMode != HvViewImageMode.HoleBinary)
                return;

            if (_originalImage == null || !_holeRoiRect.HasValue)
                return;

            // 기존 Preview는 stale, 다시 만들기
            ReplacePreviewBitmap(ref _holeBinaryPreview, null);
            EnsureHoleBinaryPreview();
            pnlViewer.Invalidate();
        }

        private void NudVentValueChanged_ForLivePreview(object sender, EventArgs e)
        {
            if (_hvViewMode != HvViewImageMode.VentBinary)
                return;

            if (_originalImage == null || !_ventRoiRect.HasValue)
                return;

            ReplacePreviewBitmap(ref _ventBinaryPreview, null);
            EnsureVentBinaryPreview();
            pnlViewer.Invalidate();
        }

        private void CmbHvViewImage_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();

            string text = cmbHvViewImage.Items[e.Index].ToString();

            HvViewImageMode mode = (HvViewImageMode)e.Index;
            bool isDisabled = false;

            if (mode == HvViewImageMode.HoleBinaryDetect && _holeDetectResult == null)
                isDisabled = true;
            else if (mode == HvViewImageMode.VentBinaryDetect && _ventDetectResult == null)
                isDisabled = true;

            Color textColor;
            if (isDisabled)
                textColor = SystemColors.GrayText;
            else if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                textColor = SystemColors.HighlightText;
            else
                textColor = e.ForeColor;

            using (Brush brush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void ChkHvShowOverlay_CheckedChanged(object sender, EventArgs e)
        {
            pnlViewer.Invalidate();
        }

        private void BtnClearHvResult_Click(object sender, EventArgs e)
        {
            ClearHoleVentResultLabels();
            pnlViewer.Invalidate();
        }

        // =========================================================
        // Setting 변경 시 결과 무효화
        // =========================================================
        private void HvSettingChanged(object sender, EventArgs e)
        {
            // ThresholdMode 등 설정 변경에 따라 Vent Threshold 활성 상태 갱신
            UpdateVentThresholdEnabled();

            // 검출 설정이 바뀌면 이전 검출 결과는 더 이상 유효하지 않음.
            // 단, ROI는 보존.
            if (_holeDetectResult == null && _ventDetectResult == null)
                return;

            ClearHoleVentResultLabels();
            pnlViewer.Invalidate();
        }

        // Vent Threshold는 ThresholdMode가 Manual일 때만 의미가 있으므로
        // 그 외 모드(Otsu/Adaptive)에서는 컨트롤을 비활성화한다.
        private void UpdateVentThresholdEnabled()
        {
            bool isManual =
                (VentThresholdMode)Math.Max(0, cmbVentThresholdMode.SelectedIndex)
                == VentThresholdMode.Manual;

            nudVentThreshold.Enabled = isManual;

            if (lblVentThreshold != null)
                lblVentThreshold.Enabled = isManual;
        }

        // =========================================================
        // ROI Edit
        // =========================================================
        private void ClearHoleVentRoiAndResult()
        {
            _holeRoiRect = null;
            _ventRoiRect = null;

            _selectedHvRoiIndex = 0;

            _isHvRoiEditing = false;
            _activeHvRoiIndex = 0;
            _activeHvRoiHitType = RoiHitType.None;

            ClearHoleVentResultLabels();
        }

        private void BtnCreateHvRoi_Click(object sender, EventArgs e)
        {
            if (_loadedImage == null)
            {
                MessageBox.Show("이미지를 먼저 로드하세요.");
                return;
            }

            int imageWidth = _loadedImage.Width;
            int imageHeight = _loadedImage.Height;

            if (imageWidth <= 0 || imageHeight <= 0)
                return;

            // 이미지 크기 기준 기본 ROI 생성
            // Hole : 좌측 영역
            // Vent : 중앙 영역
            Rectangle holeRoi = new Rectangle(
                (int)(imageWidth * 0.19),
                (int)(imageHeight * 0.47),
                (int)(imageWidth * 0.1),
                (int)(imageHeight * 0.12));

            Rectangle ventRoi = new Rectangle(
                (int)(imageWidth * 0.44),
                (int)(imageHeight * 0.47),
                (int)(imageWidth * 0.17),
                (int)(imageHeight * 0.1));

            _holeRoiRect = ClampRectangleToImage(holeRoi, imageWidth, imageHeight);
            _ventRoiRect = ClampRectangleToImage(ventRoi, imageWidth, imageHeight);

            _selectedHvRoiIndex = 1; // 기본 선택은 Hole ROI

            ClearHoleVentResultLabels();

            pnlViewer.Invalidate();
        }

        private void BtnClearHvRoi_Click(object sender, EventArgs e)
        {
            _holeRoiRect = null;
            _ventRoiRect = null;

            _selectedHvRoiIndex = 0;

            _isHvRoiEditing = false;
            _activeHvRoiIndex = 0;
            _activeHvRoiHitType = RoiHitType.None;

            ClearHoleVentResultLabels();

            pnlViewer.Cursor = Cursors.Default;
            pnlViewer.Invalidate();
        }

        private bool TryBeginHvRoiEdit(Point screenPoint)
        {
            if (!IsHvRoiEditAvailable())
                return false;

            RoiHitType holeHit = RoiHitType.None;
            RoiHitType ventHit = RoiHitType.None;

            if (_holeRoiRect.HasValue)
                holeHit = HitTestRoi(screenPoint, _holeRoiRect.Value);

            if (_ventRoiRect.HasValue)
                ventHit = HitTestRoi(screenPoint, _ventRoiRect.Value);

            bool holeRoiHit = holeHit != RoiHitType.None;
            bool ventRoiHit = ventHit != RoiHitType.None;

            if (!holeRoiHit && !ventRoiHit)
                return false;

            // 1순위: Resize 핸들 우선
            if (IsResizeHit(holeHit) && !IsResizeHit(ventHit))
            {
                BeginHvRoiEdit(1, holeHit, screenPoint, _holeRoiRect.Value);
                return true;
            }

            if (IsResizeHit(ventHit) && !IsResizeHit(holeHit))
            {
                BeginHvRoiEdit(2, ventHit, screenPoint, _ventRoiRect.Value);
                return true;
            }

            if (IsResizeHit(holeHit) && IsResizeHit(ventHit))
            {
                if (_selectedHvRoiIndex == 1 && _holeRoiRect.HasValue)
                {
                    BeginHvRoiEdit(1, holeHit, screenPoint, _holeRoiRect.Value);
                    return true;
                }

                if (_selectedHvRoiIndex == 2 && _ventRoiRect.HasValue)
                {
                    BeginHvRoiEdit(2, ventHit, screenPoint, _ventRoiRect.Value);
                    return true;
                }

                BeginHvRoiEdit(2, ventHit, screenPoint, _ventRoiRect.Value);
                return true;
            }

            // 2순위: 현재 선택된 ROI 우선
            if (_selectedHvRoiIndex == 1 && holeRoiHit && _holeRoiRect.HasValue)
            {
                BeginHvRoiEdit(1, holeHit, screenPoint, _holeRoiRect.Value);
                return true;
            }

            if (_selectedHvRoiIndex == 2 && ventRoiHit && _ventRoiRect.HasValue)
            {
                BeginHvRoiEdit(2, ventHit, screenPoint, _ventRoiRect.Value);
                return true;
            }

            // 3순위: Vent를 위에 있는 ROI로 간주
            if (ventRoiHit && _ventRoiRect.HasValue)
            {
                BeginHvRoiEdit(2, ventHit, screenPoint, _ventRoiRect.Value);
                return true;
            }

            if (holeRoiHit && _holeRoiRect.HasValue)
            {
                BeginHvRoiEdit(1, holeHit, screenPoint, _holeRoiRect.Value);
                return true;
            }

            return false;
        }

        private bool IsHvRoiEditAvailable()
        {
            if (tabControl1.SelectedTab != tabPage3)
                return false;

            if (_loadedImage == null)
                return false;

            if (!_holeRoiRect.HasValue && !_ventRoiRect.HasValue)
                return false;

            return true;
        }

        private void BeginHvRoiEdit(int roiIndex, RoiHitType hitType, Point screenPoint, Rectangle startRect)
        {
            _isHvRoiEditing = true;
            _activeHvRoiIndex = roiIndex;
            _selectedHvRoiIndex = roiIndex;
            _activeHvRoiHitType = hitType;
            _hvRoiEditStartImagePoint = ScreenToImagePixel(screenPoint);
            _hvRoiEditStartRect = startRect;

            // ROI를 수정하면 기존 검출 결과는 무효
            ClearHoleVentResultLabels();

            pnlViewer.Cursor = GetCursorForRoiHitType(hitType);
        }

        private void UpdateHvRoiEdit(Point screenPoint)
        {
            if (!_isHvRoiEditing)
                return;

            Point currentImagePoint = ScreenToImagePixel(screenPoint);

            int dx = currentImagePoint.X - _hvRoiEditStartImagePoint.X;
            int dy = currentImagePoint.Y - _hvRoiEditStartImagePoint.Y;

            Rectangle newRect = ApplyRoiEditDelta(_hvRoiEditStartRect, _activeHvRoiHitType, dx, dy);
            newRect = NormalizeRectangle(newRect);
            newRect = ClampRectangleToImage(newRect);

            if (_activeHvRoiIndex == 1)
                _holeRoiRect = newRect;
            else if (_activeHvRoiIndex == 2)
                _ventRoiRect = newRect;

            ClearHoleVentResultLabels();
        }

        private void UpdateHvRoiCursor(Point screenPoint)
        {
            if (!IsHvRoiEditAvailable())
                return;

            RoiHitType hit = RoiHitType.None;

            if (_ventRoiRect.HasValue)
                hit = HitTestRoi(screenPoint, _ventRoiRect.Value);

            if (hit == RoiHitType.None && _holeRoiRect.HasValue)
                hit = HitTestRoi(screenPoint, _holeRoiRect.Value);

            if (hit != RoiHitType.None)
                pnlViewer.Cursor = GetCursorForRoiHitType(hit);
            else if (_viewerMode == ViewerMode.MeasureDistance)
                pnlViewer.Cursor = Cursors.Cross;
            else
                pnlViewer.Cursor = Cursors.Default;
        }

        // =========================================================
        // Overlay - Screen
        // =========================================================
        private void DrawHoleVentOverlay(Graphics g)
        {
            if (_loadedImage == null)
                return;

            if (tabControl1.SelectedTab != tabPage3)
                return;

            // 1. ROI 표시
            if (_holeRoiRect.HasValue)
            {
                DrawSingleHoleVentRoi(
                    g,
                    _holeRoiRect.Value,
                    "Hole ROI",
                    Color.DeepSkyBlue,
                    _selectedHvRoiIndex == 1);
            }

            if (_ventRoiRect.HasValue)
            {
                DrawSingleHoleVentRoi(
                    g,
                    _ventRoiRect.Value,
                    "Vent ROI",
                    Color.Orange,
                    _selectedHvRoiIndex == 2);
            }

            // 2. Blob 검출 결과 표시
            if (_holeDetectResult != null)
            {
                DrawBlobResultOverlay(
                    g,
                    _holeDetectResult.Candidates,
                    _holeDetectResult.SelectedCandidate,
                    _holeDetectResult.Found,
                    Color.Yellow,   // candidateColor (선택 안 된 후보 contour)
                    Color.Lime,   // selectedColor  (선택된 contour)
                    Color.Cyan);     // rectColor       (Rect)
            }

            if (_ventDetectResult != null)
            {
                DrawBlobResultOverlay(
                    g,
                    _ventDetectResult.Candidates,
                    _ventDetectResult.SelectedCandidate,
                    _ventDetectResult.Found,
                    Color.Lime,   // candidateColor
                    Color.Lime,   // selectedColor
                    Color.Cyan);     // rectColor
            }

            // 3. Center 표시
            if (_holeCenter.HasValue)
            {
                PointF screen = ImagePointToScreen(_holeCenter.Value);

                using (Pen pen = new Pen(Color.Lime, 2))
                using (Brush textBrush = new SolidBrush(Color.Lime))
                using (Font font = new Font("맑은 고딕", 9, FontStyle.Bold))
                {
                    DrawCross(g, pen, screen, 8);

                    string text = string.Format(
                        "Hole ({0:F1}, {1:F1})",
                        _holeCenter.Value.X,
                        _holeCenter.Value.Y);

                    g.DrawString(
                        text,
                        font,
                        textBrush,
                        screen.X + 10,
                        screen.Y + 10);
                }
            }

            if (_ventCenter.HasValue)
            {
                PointF screen = ImagePointToScreen(_ventCenter.Value);

                using (Pen pen = new Pen(Color.Lime, 2))
                using (Brush textBrush = new SolidBrush(Color.Lime))
                using (Font font = new Font("맑은 고딕", 9, FontStyle.Bold))
                {
                    DrawCross(g, pen, screen, 8);

                    string text = string.Format(
                        "Vent ({0:F1}, {1:F1})",
                        _ventCenter.Value.X,
                        _ventCenter.Value.Y);

                    g.DrawString(
                        text,
                        font,
                        textBrush,
                        screen.X + 10,
                        screen.Y + 10);
                }
            }

            // 4. 거리선 표시
            DrawHoleVentDistanceOverlay(g);
        }

        private void DrawBlobResultOverlay(
            Graphics g,
            List<BlobCandidate> candidates,
            BlobCandidate selected,
            bool found,
            Color candidateColor,
            Color selectedColor,
            Color rectColor)
        {
            if (candidates != null)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    BlobCandidate c = candidates[i];

                    Color color = c.Selected ? selectedColor : candidateColor;
                    float width = c.Selected ? 3.0f : 1.0f;

                    DrawContourOverlay(g, c.ContourPoints, color, width);
                }
            }

            if (found && selected != null)
            {
                Rectangle br = selected.BoundingRect;

                PointF p1 = ImagePointToScreen(new PointF(br.Left, br.Top));
                PointF p2 = ImagePointToScreen(new PointF(br.Right, br.Bottom));

                using (Pen pen = new Pen(rectColor, 2))
                {
                    g.DrawRectangle(
                        pen,
                        p1.X,
                        p1.Y,
                        p2.X - p1.X,
                        p2.Y - p1.Y);
                }
            }
        }

        private void DrawContourOverlay(Graphics g, List<PointF> contour, Color color, float width)
        {
            if (contour == null || contour.Count < 2)
                return;

            PointF[] pts = new PointF[contour.Count];

            for (int i = 0; i < contour.Count; i++)
                pts[i] = ImagePointToScreen(contour[i]);

            using (Pen pen = new Pen(color, width))
            {
                if (pts.Length >= 3)
                    g.DrawPolygon(pen, pts);
                else
                    g.DrawLines(pen, pts);
            }
        }

        private void DrawSingleHoleVentRoi(
            Graphics g,
            Rectangle roi,
            string name,
            Color color,
            bool selected)
        {
            RectangleF screenRect = ImageRectToScreenRect(roi);

            float penWidth = selected ? 3.0f : 2.0f;

            using (Pen pen = new Pen(color, penWidth))
            using (Brush textBrush = new SolidBrush(color))
            using (Font font = new Font("맑은 고딕", 9, FontStyle.Bold))
            {
                g.DrawRectangle(
                    pen,
                    screenRect.X,
                    screenRect.Y,
                    screenRect.Width,
                    screenRect.Height);

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
                    screenRect.X + 4,
                    screenRect.Y - 22);
            }

            DrawRoiHandles(g, screenRect, color);
        }

        private void DrawHoleVentDistanceOverlay(Graphics g)
        {
            if (!_hvMeasureCalcPoint1.HasValue || !_hvMeasureCalcPoint2.HasValue)
                return;

            PointF p1Image = _hvMeasureCalcPoint1.Value;
            PointF p2Image = _hvMeasureCalcPoint2.Value;

            PointF p1Screen = ImagePointToScreen(p1Image);
            PointF p2Screen = ImagePointToScreen(p2Image);

            bool hasActualHole = _holeCenter.HasValue;
            bool hasActualVent = _ventCenter.HasValue;

            bool p1IsDifferentFromHole =
                hasActualHole &&
                GetPointDistance(p1Image, _holeCenter.Value) > 0.01;

            bool p2IsDifferentFromVent =
                hasActualVent &&
                GetPointDistance(p2Image, _ventCenter.Value) > 0.01;

            using (Pen measureLinePen = new Pen(Color.Cyan, 2))
            using (Pen usedPointPen = new Pen(Color.Cyan, 2))
            using (Brush textBrush = new SolidBrush(Color.Cyan))
            using (Brush usedTextBrush = new SolidBrush(Color.Cyan))
            using (Font font = new Font("맑은 고딕", 9, FontStyle.Bold))
            {
                g.DrawLine(measureLinePen, p1Screen, p2Screen);

                if (p1IsDifferentFromHole)
                {
                    DrawCross(g, usedPointPen, p1Screen, 6);
                    g.DrawString(
                        "Used",
                        font,
                        usedTextBrush,
                        p1Screen.X + 8,
                        p1Screen.Y - 18);
                }

                if (p2IsDifferentFromVent)
                {
                    DrawCross(g, usedPointPen, p2Screen, 6);
                    g.DrawString(
                        "Used",
                        font,
                        usedTextBrush,
                        p2Screen.X + 8,
                        p2Screen.Y - 18);
                }

                PointF mid = new PointF(
                    (p1Screen.X + p2Screen.X) / 2.0f,
                    (p1Screen.Y + p2Screen.Y) / 2.0f);

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

        // =========================================================
        // Measure Mode / Distance
        // =========================================================
        private void HvMeasureMode_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null)
                return;

            if (!rb.Checked)
                return;

            RecalculateHoleVentDistanceFromExistingCenters();
        }

        private MeasureCalcMode GetCurrentHvMeasureCalcMode()
        {
            if (rbHvMeasureVertical.Checked)
                return MeasureCalcMode.XFixed;

            if (rbHvMeasureHorizontal.Checked)
                return MeasureCalcMode.YFixed;

            return MeasureCalcMode.Free;
        }

        private void CalculateHoleVentDistance(
            PointF holePoint,
            PointF ventPoint,
            MeasureCalcMode mode,
            double resolutionMmPerPx)
        {
            PointF calcP1 = holePoint;
            PointF calcP2 = ventPoint;

            if (mode == MeasureCalcMode.XFixed)
            {
                calcP2 = new PointF(holePoint.X, ventPoint.Y);
            }
            else if (mode == MeasureCalcMode.YFixed)
            {
                calcP2 = new PointF(ventPoint.X, holePoint.Y);
            }

            double dx = calcP2.X - calcP1.X;
            double dy = calcP2.Y - calcP1.Y;

            _hvDistancePx = Math.Sqrt(dx * dx + dy * dy);
            _hvDistanceMm = _hvDistancePx * resolutionMmPerPx;
            _hvDistanceUm = _hvDistanceMm * 1000.0;

            _hvMeasureCalcPoint1 = calcP1;
            _hvMeasureCalcPoint2 = calcP2;
        }

        private void UpdateHoleVentDistanceLabels()
        {
            lblHvDistancePx.Text = string.Format(
                "Distance px : {0:F3}",
                _hvDistancePx);

            lblHvDistanceMm.Text = string.Format(
                "Distance mm : {0:F6}",
                _hvDistanceMm);

            lblHvDistanceUm.Text = string.Format(
                "Distance um : {0:F3}",
                _hvDistanceUm);
        }

        private void RecalculateHoleVentDistanceFromExistingCenters()
        {
            if (!_holeCenter.HasValue)
                return;

            if (!_ventCenter.HasValue)
                return;

            CalculateHoleVentDistance(
                _holeCenter.Value,
                _ventCenter.Value,
                GetCurrentHvMeasureCalcMode(),
                _hvResolutionMmPerPx);

            UpdateHoleVentDistanceLabels();

            pnlViewer.Invalidate();
        }

        // =========================================================
        // Resolution Text Handling
        // =========================================================
        private void TbHvResolution_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyHvResolutionText();
                e.SuppressKeyPress = true;
            }
        }

        private void TbHvResolution_Leave(object sender, EventArgs e)
        {
            ApplyHvResolutionText();
        }

        private void ApplyHvResolutionText()
        {
            if (_isRestoringHvResolutionText)
                return;

            double value;

            if (!double.TryParse(
                tbHvResolution.Text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value))
            {
                RestoreHvResolutionText();
                return;
            }

            if (value <= 0.0)
            {
                RestoreHvResolutionText();
                return;
            }

            _hvResolutionMmPerPx = value;
            _lastValidHvResolutionText = value.ToString(
                "0.################",
                CultureInfo.InvariantCulture);

            tbHvResolution.Text = _lastValidHvResolutionText;

            RecalculateHoleVentDistanceFromExistingCenters();
        }

        private void RestoreHvResolutionText()
        {
            _isRestoringHvResolutionText = true;
            tbHvResolution.Text = _lastValidHvResolutionText;
            _isRestoringHvResolutionText = false;
        }

        // =========================================================
        // Clear Result
        // =========================================================
        private void ClearHoleVentResultLabels()
        {
            _holeCenter = null;
            _ventCenter = null;

            _holeDetectResult = null;
            _ventDetectResult = null;

            _hvMeasureCalcPoint1 = null;
            _hvMeasureCalcPoint2 = null;

            _hvDistancePx = 0.0;
            _hvDistanceMm = 0.0;
            _hvDistanceUm = 0.0;

            lblHvHoleCenter.Text = "Hole Center : -";
            lblHvVentCenter.Text = "Vent Center : -";
            lblHvDistancePx.Text = "Distance px : -";
            lblHvDistanceMm.Text = "Distance mm : -";
            lblHvDistanceUm.Text = "Distance um : -";
            lblHvHoleStatus.Text = "Hole : -";
            lblHvVentStatus.Text = "Vent : -";

            // Preview Bitmap도 함께 정리 (ROI 자체는 유지)
            DisposeHoleVentPreviewImages();

            // 현재 View가 Detect 모드면 일반 Binary로 fallback
            if (_hvViewMode == HvViewImageMode.HoleBinaryDetect)
                SetViewModeSilently(HvViewImageMode.Original);
            else if (_hvViewMode == HvViewImageMode.VentBinaryDetect)
                SetViewModeSilently(HvViewImageMode.Original);
        }
    }
}