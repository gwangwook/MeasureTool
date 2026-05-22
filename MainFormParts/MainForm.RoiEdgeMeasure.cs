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
        private void InitRoiEdgeControls()
        {
            tabPage1.Text = "Manual Measure";
            tabPage2.Text = "ROI Edge Measure";

            if (tabPage3 != null)
                tabPage3.Text = "Hole / Vent Measure";

            // ROI Measure Type 기본값
            cmbRoiMeasureType.DropDownStyle = ComboBoxStyle.DropDownList;

            if (cmbRoiMeasureType.Items.Count == 0)
            {
                cmbRoiMeasureType.Items.Add("Dual ROI");
                cmbRoiMeasureType.Items.Add("Single ROI Dual Edge");
            }

            cmbRoiMeasureType.SelectedIndex = 1;
            _roiMeasureType = RoiMeasureType.SingleRoiDualEdge;

            // ROI Measure Mode 기본값
            rbRoiMeasureVertical.Checked = true;

            // ROI Resolution 기본값
            _roiResolutionMmPerPx = 0.001;
            _lastValidRoiResolutionText = _roiResolutionMmPerPx.ToString(
                "0.################",
                CultureInfo.InvariantCulture);

            tbRoiResolution.Text = _lastValidRoiResolutionText;

            // ComboBox 기본값
            // Designer 기준:
            // cmbRoiEdgeSelection: 0=Strongest, 1=First
            if (cmbRoiEdgeSelection.Items.Count > 0)
                cmbRoiEdgeSelection.SelectedIndex = 0;

            // Scan Direction: 0=T->B, 1=B->T, 2=L->R, 3=R->L
            if (cmbRoi1ScanDirection.Items.Count > 0)
                cmbRoi1ScanDirection.SelectedIndex = 0;

            if (cmbRoi2ScanDirection.Items.Count > 1)
                cmbRoi2ScanDirection.SelectedIndex = 1; // B -> T

            // Edge Polarity: 0=W->B, 1=B->W, 2=Any
            if (cmbRoi1EdgePolarity.Items.Count > 0)
                cmbRoi1EdgePolarity.SelectedIndex = 0;

            if (cmbRoi2EdgePolarity.Items.Count > 0)
                cmbRoi2EdgePolarity.SelectedIndex = 1; // W -> B

            nudRoiEdgeThresholdPercent.Minimum = 1;
            nudRoiEdgeThresholdPercent.Maximum = 100;
            nudRoiEdgeThresholdPercent.Value = 30;

            nudRoiScanStep.Minimum = 1;
            nudRoiScanStep.Maximum = 20;
            nudRoiScanStep.Value = 1;

            nudRoiMinValidPointsPercent.Minimum = 1;
            nudRoiMinValidPointsPercent.Maximum = 100;
            nudRoiMinValidPointsPercent.Value = 60;

            UpdateRoiMeasureTypeUi();
            ClearRoiResultLabels();
        }

        private RoiMeasureType GetCurrentRoiMeasureType()
        {
            if (cmbRoiMeasureType.SelectedIndex == 1)
                return RoiMeasureType.SingleRoiDualEdge;

            return RoiMeasureType.DualRoi;
        }

        private void BtnCreateRoi_Click(object sender, EventArgs e)
        {
            if (_loadedImage == null)
            {
                MessageBox.Show("이미지를 먼저 로드하세요.");
                return;
            }

            _roiMeasureType = GetCurrentRoiMeasureType();

            if (_roiMeasureType == RoiMeasureType.SingleRoiDualEdge)
                CreateDefaultSingleRoiForDualEdge();
            else
                CreateDefaultRois();

            ClearRoiResultLabels();

            tabControl1.SelectedTab = tabPage2;
            pnlViewer.Invalidate();
        }

        private void BtnClearRoi_Click(object sender, EventArgs e)
        {
            ClearRois();
            ClearRoiResultLabels();
            pnlViewer.Invalidate();
        }

        private void BtnRunRoiMeasure_Click(object sender, EventArgs e)
        {
            _roiMeasureType = GetCurrentRoiMeasureType();

            if (_roiMeasureType == RoiMeasureType.SingleRoiDualEdge)
                RunSingleRoiDualEdgeMeasure();
            else
                RunDualRoiEdgeMeasure();
        }

        private void RunDualRoiEdgeMeasure()
        {
            if (_loadedImage == null)
            {
                MessageBox.Show("이미지를 먼저 로드하세요.");
                return;
            }

            if (!_roi1Rect.HasValue || !_roi2Rect.HasValue)
            {
                MessageBox.Show("ROI1 / ROI2를 먼저 생성하세요.");
                return;
            }

            int edgeThresholdPercent = (int)nudRoiEdgeThresholdPercent.Value;
            int scanStep = (int)nudRoiScanStep.Value;
            int minValidPercent = (int)nudRoiMinValidPointsPercent.Value;
            double roiResolution = _roiResolutionMmPerPx;

            if (edgeThresholdPercent < 1 || edgeThresholdPercent > 100)
            {
                MessageBox.Show("Edge Threshold [%]는 1~100 범위여야 합니다.");
                return;
            }

            if (scanStep < 1)
            {
                MessageBox.Show("Scan Step은 1 이상이어야 합니다.");
                return;
            }

            if (minValidPercent < 1 || minValidPercent > 100)
            {
                MessageBox.Show("Min Valid Points [%]는 1~100 범위여야 합니다.");
                return;
            }

            Rectangle roi1 = ClampRectangleToImage(_roi1Rect.Value);
            Rectangle roi2 = ClampRectangleToImage(_roi2Rect.Value);

            if (!IsRoiValidForDetection(roi1) || !IsRoiValidForDetection(roi2))
            {
                MessageBox.Show("ROI 크기가 너무 작거나 이미지 영역을 벗어났습니다.");
                return;
            }

            RoiScanDirection roi1Direction = GetRoiScanDirection(cmbRoi1ScanDirection);
            RoiScanDirection roi2Direction = GetRoiScanDirection(cmbRoi2ScanDirection);

            RoiEdgePolarity roi1Polarity = GetRoiEdgePolarity(cmbRoi1EdgePolarity);
            RoiEdgePolarity roi2Polarity = GetRoiEdgePolarity(cmbRoi2EdgePolarity);

            RoiEdgeSelection selection = GetRoiEdgeSelection();

            byte[] grayBuffer;
            int imageWidth;
            int imageHeight;

            if (!TryCreateGrayBuffer(_loadedImage, out grayBuffer, out imageWidth, out imageHeight))
            {
                MessageBox.Show("이미지 Gray Buffer 생성에 실패했습니다.");
                return;
            }

            _roi1EdgeResult = DetectEdgeInRoi(
                grayBuffer,
                imageWidth,
                imageHeight,
                roi1,
                roi1Direction,
                roi1Polarity,
                selection,
                edgeThresholdPercent,
                scanStep,
                minValidPercent);

            _roi2EdgeResult = DetectEdgeInRoi(
                grayBuffer,
                imageWidth,
                imageHeight,
                roi2,
                roi2Direction,
                roi2Polarity,
                selection,
                edgeThresholdPercent,
                scanStep,
                minValidPercent);

            bool roi1Ok = _roi1EdgeResult != null && _roi1EdgeResult.Found;
            bool roi2Ok = _roi2EdgeResult != null && _roi2EdgeResult.Found;

            if (!roi1Ok || !roi2Ok)
            {
                lblRoi1Edge.Text = roi1Ok
                    ? FormatRoiEdgeLabel("ROI1", _roi1EdgeResult)
                    : "ROI1 Edge : Not Found";

                lblRoi2Edge.Text = roi2Ok
                    ? FormatRoiEdgeLabel("ROI2", _roi2EdgeResult)
                    : "ROI2 Edge : Not Found";

                ClearRoiDistanceOnly();

                pnlViewer.Invalidate();

                MessageBox.Show(
                    "Edge 탐색에 실패했습니다.\r\nROI 위치, Edge Polarity, Edge Threshold [%], Binary 상태를 확인하세요.",
                    "ROI Edge Measure",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            _roiMeasurePoint1 = _roi1EdgeResult.RepresentativePoint;
            _roiMeasurePoint2 = _roi2EdgeResult.RepresentativePoint;

            CalculateRoiDistance(
                _roiMeasurePoint1.Value,
                _roiMeasurePoint2.Value,
                GetCurrentRoiMeasureCalcMode(),
                roiResolution);

            lblRoi1Edge.Text = FormatRoiEdgeLabel("ROI1", _roi1EdgeResult);
            lblRoi2Edge.Text = FormatRoiEdgeLabel("ROI2", _roi2EdgeResult);

            UpdateRoiDistanceLabels();

            pnlViewer.Invalidate();
        }

        private void RunSingleRoiDualEdgeMeasure()
        {
            if (_loadedImage == null)
            {
                MessageBox.Show("이미지를 먼저 로드하세요.");
                return;
            }

            if (!_roi1Rect.HasValue)
            {
                MessageBox.Show("Single ROI를 먼저 생성하세요.");
                return;
            }

            if (rbRoiMeasureFree.Checked)
            {
                MessageBox.Show("Single ROI Dual Edge에서는 Free 모드를 사용할 수 없습니다.\r\nVertical 또는 Horizontal을 선택하세요.");
                return;
            }

            int edgeThresholdPercent = (int)nudRoiEdgeThresholdPercent.Value;
            int scanStep = (int)nudRoiScanStep.Value;
            int minValidPercent = (int)nudRoiMinValidPointsPercent.Value;

            if (edgeThresholdPercent < 1 || edgeThresholdPercent > 100)
            {
                MessageBox.Show("Edge Threshold [%]는 1~100 범위여야 합니다.");
                return;
            }

            if (scanStep < 1)
            {
                MessageBox.Show("Scan Step은 1 이상이어야 합니다.");
                return;
            }

            if (minValidPercent < 1 || minValidPercent > 100)
            {
                MessageBox.Show("Min Valid Points [%]는 1~100 범위여야 합니다.");
                return;
            }

            Rectangle singleRoi = ClampRectangleToImage(_roi1Rect.Value);

            if (!IsRoiValidForDetection(singleRoi))
            {
                MessageBox.Show("Single ROI 크기가 너무 작거나 이미지 영역을 벗어났습니다.");
                return;
            }

            RoiScanDirection edgeADirection = GetRoiScanDirection(cmbRoi1ScanDirection);
            RoiScanDirection edgeBDirection = GetRoiScanDirection(cmbRoi2ScanDirection);

            RoiEdgePolarity edgeAPolarity = GetRoiEdgePolarity(cmbRoi1EdgePolarity);
            RoiEdgePolarity edgeBPolarity = GetRoiEdgePolarity(cmbRoi2EdgePolarity);

            RoiEdgeSelection selection = GetRoiEdgeSelection();

            byte[] grayBuffer;
            int imageWidth;
            int imageHeight;

            if (!TryCreateGrayBuffer(_loadedImage, out grayBuffer, out imageWidth, out imageHeight))
            {
                MessageBox.Show("이미지 Gray Buffer 생성에 실패했습니다.");
                return;
            }

            // 같은 Single ROI 안에서 Edge A / Edge B를 각각 검출
            _roi1EdgeResult = DetectEdgeInRoi(
                grayBuffer,
                imageWidth,
                imageHeight,
                singleRoi,
                edgeADirection,
                edgeAPolarity,
                selection,
                edgeThresholdPercent,
                scanStep,
                minValidPercent);

            _roi2EdgeResult = DetectEdgeInRoi(
                grayBuffer,
                imageWidth,
                imageHeight,
                singleRoi,
                edgeBDirection,
                edgeBPolarity,
                selection,
                edgeThresholdPercent,
                scanStep,
                minValidPercent);

            bool edgeAOk = _roi1EdgeResult != null && _roi1EdgeResult.Found;
            bool edgeBOk = _roi2EdgeResult != null && _roi2EdgeResult.Found;

            if (!edgeAOk || !edgeBOk)
            {
                lblRoi1Edge.Text = edgeAOk
                    ? FormatRoiEdgeLabel("Edge A", _roi1EdgeResult)
                    : "Edge A : Not Found";

                lblRoi2Edge.Text = edgeBOk
                    ? FormatRoiEdgeLabel("Edge B", _roi2EdgeResult)
                    : "Edge B : Not Found";

                ClearRoiDistanceOnly();

                pnlViewer.Invalidate();

                MessageBox.Show(
                    "Edge A/B 탐색에 실패했습니다.\r\nSingle ROI 위치, Scan Direction, Edge Polarity, Threshold를 확인하세요.",
                    "Single ROI Dual Edge",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // 다음 Step에서 RepresentativePoint가 아니라 Fitted Line + baseX/baseY 기준으로 계산하도록 구현
            if (!CalculateSingleRoiDualEdgeDistance(singleRoi))
            {
                ClearRoiDistanceOnly();

                pnlViewer.Invalidate();

                MessageBox.Show(
                    "Single ROI Dual Edge 거리 계산에 실패했습니다.\r\nMeasure Mode와 Edge 방향 조합을 확인하세요.",
                    "Single ROI Dual Edge",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            lblRoi1Edge.Text = FormatRoiEdgeLabel("Edge A", _roi1EdgeResult);
            lblRoi2Edge.Text = FormatRoiEdgeLabel("Edge B", _roi2EdgeResult);

            UpdateRoiDistanceLabels();

            if (rbRoiMeasureVertical.Checked)
            {
                lblRoiDistancePx.Text = lblRoiDistancePx.Text + "  (baseX)";
            }
            else if (rbRoiMeasureHorizontal.Checked)
            {
                lblRoiDistancePx.Text = lblRoiDistancePx.Text + "  (baseY)";
            }

            pnlViewer.Invalidate();
        }

        private bool CalculateSingleRoiDualEdgeDistance(Rectangle singleRoi)
        {
            if (_roi1EdgeResult == null || !_roi1EdgeResult.Found)
                return false;

            if (_roi2EdgeResult == null || !_roi2EdgeResult.Found)
                return false;

            if (rbRoiMeasureFree.Checked)
                return false;

            PointF p1;
            PointF p2;

            if (rbRoiMeasureVertical.Checked)
            {
                // Vertical 거리:
                // Single ROI 중심 X에서 Edge A/B Fitted Line의 Y값을 각각 계산
                float baseX = singleRoi.Left + singleRoi.Width / 2.0f;

                if (!TryGetPointOnFittedLineAtX(_roi1EdgeResult, baseX, out p1))
                    return false;

                if (!TryGetPointOnFittedLineAtX(_roi2EdgeResult, baseX, out p2))
                    return false;

                _roiMeasurePoint1 = p1;
                _roiMeasurePoint2 = p2;
                _roiMeasureCalcPoint1 = p1;
                _roiMeasureCalcPoint2 = p2;

                double dy = p2.Y - p1.Y;

                _roiDistancePx = Math.Abs(dy);
                _roiDistanceMm = _roiDistancePx * _roiResolutionMmPerPx;
                _roiDistanceUm = _roiDistanceMm * 1000.0;

                return true;
            }

            if (rbRoiMeasureHorizontal.Checked)
            {
                // Horizontal 거리:
                // Single ROI 중심 Y에서 Edge A/B Fitted Line의 X값을 각각 계산
                float baseY = singleRoi.Top + singleRoi.Height / 2.0f;

                if (!TryGetPointOnFittedLineAtY(_roi1EdgeResult, baseY, out p1))
                    return false;

                if (!TryGetPointOnFittedLineAtY(_roi2EdgeResult, baseY, out p2))
                    return false;

                _roiMeasurePoint1 = p1;
                _roiMeasurePoint2 = p2;
                _roiMeasureCalcPoint1 = p1;
                _roiMeasureCalcPoint2 = p2;

                double dx = p2.X - p1.X;

                _roiDistancePx = Math.Abs(dx);
                _roiDistanceMm = _roiDistancePx * _roiResolutionMmPerPx;
                _roiDistanceUm = _roiDistanceMm * 1000.0;

                return true;
            }

            return false;
        }

        private bool TryGetPointOnFittedLineAtX(
            RoiEdgeResult edgeResult,
            float x,
            out PointF point)
        {
            point = PointF.Empty;

            if (edgeResult == null || !edgeResult.Found)
                return false;

            PointF p1 = edgeResult.LineStart;
            PointF p2 = edgeResult.LineEnd;

            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            // 거의 수직 라인이면 y = ax + b 형태로 계산 불가
            if (Math.Abs(dx) < 0.0001f)
                return false;

            float t = (x - p1.X) / dx;
            float y = p1.Y + t * dy;

            point = new PointF(x, y);
            return true;
        }

        private bool TryGetPointOnFittedLineAtY(
            RoiEdgeResult edgeResult,
            float y,
            out PointF point)
        {
            point = PointF.Empty;

            if (edgeResult == null || !edgeResult.Found)
                return false;

            PointF p1 = edgeResult.LineStart;
            PointF p2 = edgeResult.LineEnd;

            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            // 거의 수평 라인이면 x = ay + b 형태로 계산 불가
            if (Math.Abs(dy) < 0.0001f)
                return false;

            float t = (y - p1.Y) / dy;
            float x = p1.X + t * dx;

            point = new PointF(x, y);
            return true;
        }

        private void ClearRoiDistanceOnly()
        {
            lblRoiDistancePx.Text = "Distance px : -";
            lblRoiDistanceMm.Text = "Distance mm : -";
            lblRoiDistanceUm.Text = "Distance um : -";

            _roiMeasurePoint1 = null;
            _roiMeasurePoint2 = null;
            _roiMeasureCalcPoint1 = null;
            _roiMeasureCalcPoint2 = null;

            _roiDistancePx = 0.0;
            _roiDistanceMm = 0.0;
            _roiDistanceUm = 0.0;
        }

        private void CreateDefaultRois()
        {
            if (_loadedImage == null)
                return;

            int imgW = _loadedImage.Width;
            int imgH = _loadedImage.Height;

            // 이미지 중앙 기준, 기존보다 작게 생성
            int roiW = Math.Max(80, imgW / 8);      // 이미지 폭의 약 12.5%
            int roiH = Math.Max(20, imgH / 35);     // 이미지 높이의 약 2.8%

            // 이미지보다 커지지 않도록 보정
            if (roiW > imgW)
                roiW = imgW;

            if (roiH > imgH)
                roiH = imgH;

            int centerX = imgW / 2;
            int centerY = imgH / 2;

            int x = centerX - roiW / 2;

            // ROI1은 중앙보다 약간 위, ROI2는 약간 아래
            int gap = Math.Max(10, roiH / 2);

            int y1 = centerY - roiH - gap / 2;
            int y2 = centerY + gap / 2;

            Rectangle roi1 = new Rectangle(x, y1, roiW, roiH);
            Rectangle roi2 = new Rectangle(x, y2, roiW, roiH);

            _roi1Rect = ClampRectangleToImage(roi1);
            _roi2Rect = ClampRectangleToImage(roi2);

            _selectedRoiIndex = 1;
        }

        private void CreateDefaultSingleRoiForDualEdge()
        {
            if (_loadedImage == null)
                return;

            int imgW = _loadedImage.Width;
            int imgH = _loadedImage.Height;

            // Single ROI는 하나의 Object 안에서 두 Edge를 동시에 찾는 용도이므로,
            // 기존 Dual ROI보다 높이를 조금 크게 잡는다.
            int roiW = Math.Max(120, imgW / 6);    // 이미지 폭의 약 16.7%
            int roiH = Math.Max(60, imgH / 12);    // 이미지 높이의 약 8.3%

            if (roiW > imgW)
                roiW = imgW;

            if (roiH > imgH)
                roiH = imgH;

            int centerX = imgW / 2;
            int centerY = imgH / 2;

            int x = centerX - roiW / 2;
            int y = centerY - roiH / 2;

            Rectangle roi = new Rectangle(x, y, roiW, roiH);

            _roi1Rect = ClampRectangleToImage(roi);

            // Single ROI Dual Edge에서는 ROI2를 사용하지 않음
            _roi2Rect = null;

            _selectedRoiIndex = 1;
        }

        private void ClearRois()
        {
            _roiMeasureType = GetCurrentRoiMeasureType();

            _roi1Rect = null;
            _roi2Rect = null;

            _selectedRoiIndex = 0;

            _isRoiEditing = false;
            _activeRoiIndex = 0;
            _activeRoiHitType = RoiHitType.None;

            ClearRoiResultLabels();
        }

        private void ClearRoiResultLabels()
        {
            if (_roiMeasureType == RoiMeasureType.SingleRoiDualEdge)
            {
                lblRoi1Edge.Text = "Edge A : -";
                lblRoi2Edge.Text = "Edge B : -";
            }
            else
            {
                lblRoi1Edge.Text = "ROI1 Edge : -";
                lblRoi2Edge.Text = "ROI2 Edge : -";
            }

            lblRoiDistancePx.Text = "Distance px : -";
            lblRoiDistanceMm.Text = "Distance mm : -";
            lblRoiDistanceUm.Text = "Distance um : -";

            _roi1EdgeResult = null;
            _roi2EdgeResult = null;

            _roiMeasurePoint1 = null;
            _roiMeasurePoint2 = null;
            _roiMeasureCalcPoint1 = null;
            _roiMeasureCalcPoint2 = null;

            _roiDistancePx = 0.0;
            _roiDistanceMm = 0.0;
            _roiDistanceUm = 0.0;
        }

        private void ClearRoiMeasureByTypeChange()
        {
            _roi1Rect = null;
            _roi2Rect = null;

            _selectedRoiIndex = 0;

            _isRoiEditing = false;
            _activeRoiIndex = 0;
            _activeRoiHitType = RoiHitType.None;

            _roi1EdgeResult = null;
            _roi2EdgeResult = null;

            _roiMeasurePoint1 = null;
            _roiMeasurePoint2 = null;
            _roiMeasureCalcPoint1 = null;
            _roiMeasureCalcPoint2 = null;

            _roiDistancePx = 0.0;
            _roiDistanceMm = 0.0;
            _roiDistanceUm = 0.0;

            if (_roiMeasureType == RoiMeasureType.SingleRoiDualEdge)
            {
                lblRoi1Edge.Text = "Edge A : -";
                lblRoi2Edge.Text = "Edge B : -";
            }
            else
            {
                lblRoi1Edge.Text = "ROI1 Edge : -";
                lblRoi2Edge.Text = "ROI2 Edge : -";
            }

            lblRoiDistancePx.Text = "Distance px : -";
            lblRoiDistanceMm.Text = "Distance mm : -";
            lblRoiDistanceUm.Text = "Distance um : -";
        }

        private void DrawRoiOverlay(Graphics g)
        {
            if (tabControl1.SelectedTab != tabPage2)
                return;

            if (_loadedImage == null)
                return;

            _roiMeasureType = GetCurrentRoiMeasureType();

            if (_roiMeasureType == RoiMeasureType.SingleRoiDualEdge)
            {
                if (_roi1Rect.HasValue)
                    DrawSingleRoi(g, _roi1Rect.Value, "Single ROI", Color.DeepSkyBlue);
            }
            else
            {
                if (_roi1Rect.HasValue)
                    DrawSingleRoi(g, _roi1Rect.Value, "ROI1", Color.DeepSkyBlue);

                if (_roi2Rect.HasValue)
                    DrawSingleRoi(g, _roi2Rect.Value, "ROI2", Color.Orange);
            }

            DrawRoiEdgeResultOverlay(g);
            DrawRoiMeasureOverlay(g);
        }

        private void DrawRoiEdgeResultOverlay(Graphics g)
        {
            if (_roi1EdgeResult != null && _roi1EdgeResult.Found)
                DrawSingleRoiEdgeResult(g, _roi1EdgeResult, Color.Cyan);

            if (_roi2EdgeResult != null && _roi2EdgeResult.Found)
                DrawSingleRoiEdgeResult(g, _roi2EdgeResult, Color.Yellow);
        }

        private void DrawSingleRoiEdgeResult(Graphics g, RoiEdgeResult result, Color color)
        {
            if (result == null || !result.Found)
                return;

            using (Pen pointPen = new Pen(color, 1))
            using (Pen linePen = new Pen(Color.Red, 2))
            using (Brush brush = new SolidBrush(color))
            {
                // Edge Points
                for (int i = 0; i < result.InlierPoints.Count; i++)
                {
                    PointF sp = ImagePointToScreen(result.InlierPoints[i]);

                    g.FillEllipse(
                        brush,
                        sp.X - 2,
                        sp.Y - 2,
                        4,
                        4);
                }

                // Fitted Line
                PointF lineStart = ImagePointToScreen(result.LineStart);
                PointF lineEnd = ImagePointToScreen(result.LineEnd);

                g.DrawLine(linePen, lineStart, lineEnd);

                // Representative Point
                PointF rep = ImagePointToScreen(result.RepresentativePoint);

                g.DrawEllipse(
                    linePen,
                    rep.X - 5,
                    rep.Y - 5,
                    10,
                    10);
            }
        }

        private void DrawRoiMeasureOverlay(Graphics g)
        {
            if (!_roiMeasureCalcPoint1.HasValue || !_roiMeasureCalcPoint2.HasValue)
                return;

            PointF p1 = ImagePointToScreen(_roiMeasureCalcPoint1.Value);
            PointF p2 = ImagePointToScreen(_roiMeasureCalcPoint2.Value);

            using (Pen pen = new Pen(Color.Lime, 2))
            using (Font font = new Font("맑은 고딕", 9, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Lime))
            {
                g.DrawLine(pen, p1, p2);

                DrawCross(g, pen, p1, 5);
                DrawCross(g, pen, p2, 5);

                PointF mid = new PointF(
                    (p1.X + p2.X) / 2.0f,
                    (p1.Y + p2.Y) / 2.0f);

                string text = string.Format("D={0:F3}px", _roiDistancePx);

                g.DrawString(text, font, brush, mid.X + 8, mid.Y + 8);
            }
        }

        private void DrawSingleRoi(Graphics g, Rectangle roi, string name, Color color)
        {
            RectangleF screenRect = ImageRectToScreenRect(roi);

            float penWidth = 2.0f;

            if ((name == "ROI1" && _selectedRoiIndex == 1) ||
                (name == "ROI2" && _selectedRoiIndex == 2))
            {
                penWidth = 3.0f;
            }

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

                g.DrawString(
                    string.Format("{0} ({1},{2},{3},{4})", name, roi.X, roi.Y, roi.Width, roi.Height),
                    font,
                    textBrush,
                    screenRect.X + 4,
                    screenRect.Y - 20);
            }

            DrawRoiHandles(g, screenRect, color);
        }

        private void DrawRoiHandles(Graphics g, RectangleF rect, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            using (Pen pen = new Pen(Color.Black, 1))
            {
                RectangleF[] handles = GetRoiHandleRects(rect);

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

        private RectangleF[] GetRoiHandleRects(RectangleF rect)
        {
            float s = RoiHandleSize;
            float hs = s / 2.0f;

            float left = rect.Left;
            float right = rect.Right;
            float top = rect.Top;
            float bottom = rect.Bottom;
            float cx = rect.Left + rect.Width / 2.0f;
            float cy = rect.Top + rect.Height / 2.0f;

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

        private RectangleF ImageRectToScreenRect(Rectangle roi)
        {
            float x = roi.X * _zoom + _panOffset.X;
            float y = roi.Y * _zoom + _panOffset.Y;
            float w = roi.Width * _zoom;
            float h = roi.Height * _zoom;

            return new RectangleF(x, y, w, h);
        }

        private bool TryBeginRoiEdit(Point screenPoint)
        {
            if (!IsRoiEditAvailable())
                return false;

            RoiHitType hit1 = RoiHitType.None;
            RoiHitType hit2 = RoiHitType.None;

            if (_roi1Rect.HasValue)
                hit1 = HitTestRoi(screenPoint, _roi1Rect.Value);

            if (_roi2Rect.HasValue)
                hit2 = HitTestRoi(screenPoint, _roi2Rect.Value);

            bool roi1Hit = hit1 != RoiHitType.None;
            bool roi2Hit = hit2 != RoiHitType.None;

            if (!roi1Hit && !roi2Hit)
                return false;

            // 1순위: 핸들 Resize 우선
            // 겹친 상태에서도 ROI1 핸들을 클릭하면 ROI1 Resize 가능
            if (IsResizeHit(hit1) && !IsResizeHit(hit2))
            {
                BeginRoiEdit(1, hit1, screenPoint, _roi1Rect.Value);
                return true;
            }

            if (IsResizeHit(hit2) && !IsResizeHit(hit1))
            {
                BeginRoiEdit(2, hit2, screenPoint, _roi2Rect.Value);
                return true;
            }

            // 둘 다 Resize 핸들에 걸린 경우
            // 현재 선택된 ROI 우선
            if (IsResizeHit(hit1) && IsResizeHit(hit2))
            {
                if (_selectedRoiIndex == 1 && _roi1Rect.HasValue)
                {
                    BeginRoiEdit(1, hit1, screenPoint, _roi1Rect.Value);
                    return true;
                }

                if (_selectedRoiIndex == 2 && _roi2Rect.HasValue)
                {
                    BeginRoiEdit(2, hit2, screenPoint, _roi2Rect.Value);
                    return true;
                }

                // 선택 ROI가 없으면 ROI2를 위에 있는 것으로 보고 우선
                BeginRoiEdit(2, hit2, screenPoint, _roi2Rect.Value);
                return true;
            }

            // 2순위: 현재 선택된 ROI 우선
            if (_selectedRoiIndex == 1 && roi1Hit && _roi1Rect.HasValue)
            {
                BeginRoiEdit(1, hit1, screenPoint, _roi1Rect.Value);
                return true;
            }

            if (_selectedRoiIndex == 2 && roi2Hit && _roi2Rect.HasValue)
            {
                BeginRoiEdit(2, hit2, screenPoint, _roi2Rect.Value);
                return true;
            }

            // 3순위: 둘 다 Body에 걸리면 ROI2를 위에 있는 ROI로 간주
            if (roi2Hit && _roi2Rect.HasValue)
            {
                BeginRoiEdit(2, hit2, screenPoint, _roi2Rect.Value);
                return true;
            }

            if (roi1Hit && _roi1Rect.HasValue)
            {
                BeginRoiEdit(1, hit1, screenPoint, _roi1Rect.Value);
                return true;
            }

            return false;
        }

        private bool IsResizeHit(RoiHitType hitType)
        {
            return hitType == RoiHitType.Left ||
                   hitType == RoiHitType.Right ||
                   hitType == RoiHitType.Top ||
                   hitType == RoiHitType.Bottom ||
                   hitType == RoiHitType.TopLeft ||
                   hitType == RoiHitType.TopRight ||
                   hitType == RoiHitType.BottomLeft ||
                   hitType == RoiHitType.BottomRight;
        }

        private bool IsRoiEditAvailable()
        {
            if (tabControl1.SelectedTab != tabPage2)
                return false;

            if (_loadedImage == null)
                return false;

            if (!_roi1Rect.HasValue && !_roi2Rect.HasValue)
                return false;

            return true;
        }

        private void BeginRoiEdit(int roiIndex, RoiHitType hitType, Point screenPoint, Rectangle startRect)
        {
            _isRoiEditing = true;
            _activeRoiIndex = roiIndex;
            _selectedRoiIndex = roiIndex;
            _activeRoiHitType = hitType;
            _roiEditStartImagePoint = ScreenToImagePixel(screenPoint);
            _roiEditStartRect = startRect;

            pnlViewer.Cursor = GetCursorForRoiHitType(hitType);
        }

        private void UpdateRoiEdit(Point screenPoint)
        {
            if (!_isRoiEditing)
                return;

            Point currentImagePoint = ScreenToImagePixel(screenPoint);

            int dx = currentImagePoint.X - _roiEditStartImagePoint.X;
            int dy = currentImagePoint.Y - _roiEditStartImagePoint.Y;

            Rectangle newRect = ApplyRoiEditDelta(_roiEditStartRect, _activeRoiHitType, dx, dy);
            newRect = NormalizeRectangle(newRect);
            newRect = ClampRectangleToImage(newRect);

            if (_activeRoiIndex == 1)
                _roi1Rect = newRect;
            else if (_activeRoiIndex == 2)
                _roi2Rect = newRect;

            ClearRoiResultLabels();
        }

        private Rectangle ApplyRoiEditDelta(Rectangle startRect, RoiHitType hitType, int dx, int dy)
        {
            int left = startRect.Left;
            int right = startRect.Right;
            int top = startRect.Top;
            int bottom = startRect.Bottom;

            if (hitType == RoiHitType.Body)
            {
                return new Rectangle(
                    startRect.X + dx,
                    startRect.Y + dy,
                    startRect.Width,
                    startRect.Height);
            }

            if (hitType == RoiHitType.Left || hitType == RoiHitType.TopLeft || hitType == RoiHitType.BottomLeft)
                left += dx;

            if (hitType == RoiHitType.Right || hitType == RoiHitType.TopRight || hitType == RoiHitType.BottomRight)
                right += dx;

            if (hitType == RoiHitType.Top || hitType == RoiHitType.TopLeft || hitType == RoiHitType.TopRight)
                top += dy;

            if (hitType == RoiHitType.Bottom || hitType == RoiHitType.BottomLeft || hitType == RoiHitType.BottomRight)
                bottom += dy;

            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        private RoiHitType HitTestRoi(Point screenPoint, Rectangle roi)
        {
            RectangleF screenRect = ImageRectToScreenRect(roi);
            RectangleF[] handles = GetRoiHandleRects(screenRect);

            // 순서:
            // 0 TopLeft, 1 Top, 2 TopRight, 3 Left, 4 Right, 5 BottomLeft, 6 Bottom, 7 BottomRight
            if (handles[0].Contains(screenPoint)) return RoiHitType.TopLeft;
            if (handles[1].Contains(screenPoint)) return RoiHitType.Top;
            if (handles[2].Contains(screenPoint)) return RoiHitType.TopRight;
            if (handles[3].Contains(screenPoint)) return RoiHitType.Left;
            if (handles[4].Contains(screenPoint)) return RoiHitType.Right;
            if (handles[5].Contains(screenPoint)) return RoiHitType.BottomLeft;
            if (handles[6].Contains(screenPoint)) return RoiHitType.Bottom;
            if (handles[7].Contains(screenPoint)) return RoiHitType.BottomRight;

            if (screenRect.Contains(screenPoint))
                return RoiHitType.Body;

            return RoiHitType.None;
        }

        private void UpdateRoiCursor(Point screenPoint)
        {
            if (!IsRoiEditAvailable())
                return;

            RoiHitType hit = RoiHitType.None;

            if (_roi2Rect.HasValue)
                hit = HitTestRoi(screenPoint, _roi2Rect.Value);

            if (hit == RoiHitType.None && _roi1Rect.HasValue)
                hit = HitTestRoi(screenPoint, _roi1Rect.Value);

            if (hit != RoiHitType.None)
                pnlViewer.Cursor = GetCursorForRoiHitType(hit);
            else if (_viewerMode == ViewerMode.MeasureDistance)
                pnlViewer.Cursor = Cursors.Cross;
            else
                pnlViewer.Cursor = Cursors.Default;
        }

        private Cursor GetCursorForRoiHitType(RoiHitType hitType)
        {
            if (hitType == RoiHitType.Body)
                return Cursors.SizeAll;

            if (hitType == RoiHitType.Left || hitType == RoiHitType.Right)
                return Cursors.SizeWE;

            if (hitType == RoiHitType.Top || hitType == RoiHitType.Bottom)
                return Cursors.SizeNS;

            if (hitType == RoiHitType.TopLeft || hitType == RoiHitType.BottomRight)
                return Cursors.SizeNWSE;

            if (hitType == RoiHitType.TopRight || hitType == RoiHitType.BottomLeft)
                return Cursors.SizeNESW;

            return Cursors.Default;
        }

        private void UpdateRoiMeasureTypeUi()
        {
            _roiMeasureType = GetCurrentRoiMeasureType();

            bool isSingle = _roiMeasureType == RoiMeasureType.SingleRoiDualEdge;

            if (isSingle)
            {
                gbRoi1Setting.Text = "Edge A Setting";
                gbRoi2Setting.Text = "Edge B Setting";

                lblRoi1ScanDirection.Text = "A Scan Direction :";
                lblRoi1EdgePolarity.Text = "A Edge Polarity :";

                lblRoi2ScanDirection.Text = "B Scan Direction :";
                lblRoi2EdgePolarity.Text = "B Edge Polarity :";

                lblRoi1Edge.Text = "Edge A : -";
                lblRoi2Edge.Text = "Edge B : -";

                // Single ROI Dual Edge에서는 Free 비활성화
                rbRoiMeasureFree.Enabled = false;

                if (rbRoiMeasureFree.Checked)
                    rbRoiMeasureVertical.Checked = true;
            }
            else
            {
                gbRoi1Setting.Text = "ROI1 Setting";
                gbRoi2Setting.Text = "ROI2 Setting";

                lblRoi1ScanDirection.Text = "Scan Direction :";
                lblRoi1EdgePolarity.Text = "Edge Polarity :";

                lblRoi2ScanDirection.Text = "Scan Direction :";
                lblRoi2EdgePolarity.Text = "Edge Polarity :";

                lblRoi1Edge.Text = "ROI1 Edge : -";
                lblRoi2Edge.Text = "ROI2 Edge : -";

                rbRoiMeasureFree.Enabled = true;
            }
        }

        private RoiScanDirection GetRoiScanDirection(ComboBox comboBox)
        {
            // Designer 기준:
            // 0 = T -> B
            // 1 = B -> T
            // 2 = L -> R
            // 3 = R -> L

            if (comboBox.SelectedIndex == 1)
                return RoiScanDirection.BottomToTop;

            if (comboBox.SelectedIndex == 2)
                return RoiScanDirection.LeftToRight;

            if (comboBox.SelectedIndex == 3)
                return RoiScanDirection.RightToLeft;

            return RoiScanDirection.TopToBottom;
        }

        private RoiEdgePolarity GetRoiEdgePolarity(ComboBox comboBox)
        {
            // Designer 기준:
            // 0 = W -> B
            // 1 = B -> W
            // 2 = Any

            if (comboBox.SelectedIndex == 1)
                return RoiEdgePolarity.BlackToWhite;

            if (comboBox.SelectedIndex == 2)
                return RoiEdgePolarity.Any;

            return RoiEdgePolarity.WhiteToBlack;
        }

        private RoiEdgeSelection GetRoiEdgeSelection()
        {
            // Designer 기준:
            // 0 = Strongest
            // 1 = First

            if (cmbRoiEdgeSelection.SelectedIndex == 1)
                return RoiEdgeSelection.First;

            return RoiEdgeSelection.Strongest;
        }

        private MeasureCalcMode GetCurrentRoiMeasureCalcMode()
        {
            if (rbRoiMeasureVertical.Checked)
                return MeasureCalcMode.XFixed;

            if (rbRoiMeasureHorizontal.Checked)
                return MeasureCalcMode.YFixed;

            return MeasureCalcMode.Free;
        }

        private bool TryGetRoiResolution(out double resolution)
        {
            resolution = 0.0;

            if (string.IsNullOrWhiteSpace(tbRoiResolution.Text))
                return false;

            string text = tbRoiResolution.Text.Trim();

            if (text.Contains("+") || text.Contains("-"))
                return false;

            if (!double.TryParse(
                text,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out resolution))
            {
                return false;
            }

            if (resolution <= 0)
                return false;

            return true;
        }

        private void RunRoiEdgeMeasure()
        {
            if (_loadedImage == null)
            {
                MessageBox.Show("이미지를 먼저 로드하세요.");
                return;
            }

            if (!_roi1Rect.HasValue || !_roi2Rect.HasValue)
            {
                MessageBox.Show("ROI를 먼저 생성하세요.");
                return;
            }

            if (!CommitRoiResolutionText())
                return;

            double roiResolution = _roiResolutionMmPerPx;

            int edgeThresholdPercent = (int)nudRoiEdgeThresholdPercent.Value;
            int scanStep = (int)nudRoiScanStep.Value;
            int minValidPercent = (int)nudRoiMinValidPointsPercent.Value;

            if (edgeThresholdPercent < 1 || edgeThresholdPercent > 100)
            {
                MessageBox.Show("Edge Threshold [%]는 1~100 범위여야 합니다.");
                return;
            }

            if (scanStep < 1)
            {
                MessageBox.Show("Scan Step은 1 이상이어야 합니다.");
                return;
            }

            if (minValidPercent < 1 || minValidPercent > 100)
            {
                MessageBox.Show("Min Valid Points [%]는 1~100 범위여야 합니다.");
                return;
            }

            Rectangle roi1 = ClampRectangleToImage(_roi1Rect.Value);
            Rectangle roi2 = ClampRectangleToImage(_roi2Rect.Value);

            if (!IsRoiValidForDetection(roi1) || !IsRoiValidForDetection(roi2))
            {
                MessageBox.Show("ROI 크기가 너무 작거나 이미지 영역을 벗어났습니다.");
                return;
            }

            RoiScanDirection roi1Direction = GetRoiScanDirection(cmbRoi1ScanDirection);
            RoiScanDirection roi2Direction = GetRoiScanDirection(cmbRoi2ScanDirection);

            RoiEdgePolarity roi1Polarity = GetRoiEdgePolarity(cmbRoi1EdgePolarity);
            RoiEdgePolarity roi2Polarity = GetRoiEdgePolarity(cmbRoi2EdgePolarity);

            RoiEdgeSelection selection = GetRoiEdgeSelection();

            byte[] grayBuffer;
            int imageWidth;
            int imageHeight;

            if (!TryCreateGrayBuffer(_loadedImage, out grayBuffer, out imageWidth, out imageHeight))
            {
                MessageBox.Show("이미지 Gray Buffer 생성에 실패했습니다.");
                return;
            }

            _roi1EdgeResult = DetectEdgeInRoi(
                grayBuffer,
                imageWidth,
                imageHeight,
                roi1,
                roi1Direction,
                roi1Polarity,
                selection,
                edgeThresholdPercent,
                scanStep,
                minValidPercent);

            _roi2EdgeResult = DetectEdgeInRoi(
                grayBuffer,
                imageWidth,
                imageHeight,
                roi2,
                roi2Direction,
                roi2Polarity,
                selection,
                edgeThresholdPercent,
                scanStep,
                minValidPercent);

            bool roi1Ok = _roi1EdgeResult != null && _roi1EdgeResult.Found;
            bool roi2Ok = _roi2EdgeResult != null && _roi2EdgeResult.Found;

            if (!roi1Ok || !roi2Ok)
            {
                lblRoi1Edge.Text = roi1Ok
                    ? FormatRoiEdgeLabel("ROI1", _roi1EdgeResult)
                    : "ROI1 Edge : Not Found";

                lblRoi2Edge.Text = roi2Ok
                    ? FormatRoiEdgeLabel("ROI2", _roi2EdgeResult)
                    : "ROI2 Edge : Not Found";

                lblRoiDistancePx.Text = "Distance px : -";
                lblRoiDistanceMm.Text = "Distance mm : -";
                lblRoiDistanceUm.Text = "Distance um : -";

                _roiMeasurePoint1 = null;
                _roiMeasurePoint2 = null;
                _roiMeasureCalcPoint1 = null;
                _roiMeasureCalcPoint2 = null;

                pnlViewer.Invalidate();

                MessageBox.Show(
                    "Edge 탐색에 실패했습니다.\r\nROI 위치, Edge Polarity, Edge Threshold [%], Binary 상태를 확인하세요.",
                    "ROI Edge Measure",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            _roiMeasurePoint1 = _roi1EdgeResult.RepresentativePoint;
            _roiMeasurePoint2 = _roi2EdgeResult.RepresentativePoint;

            CalculateRoiDistance(
                _roiMeasurePoint1.Value,
                _roiMeasurePoint2.Value,
                GetCurrentRoiMeasureCalcMode(),
                roiResolution);

            lblRoi1Edge.Text = FormatRoiEdgeLabel("ROI1", _roi1EdgeResult);
            lblRoi2Edge.Text = FormatRoiEdgeLabel("ROI2", _roi2EdgeResult);

            UpdateRoiDistanceLabels();

            pnlViewer.Invalidate();
        }

        private bool IsRoiValidForDetection(Rectangle roi)
        {
            if (_loadedImage == null)
                return false;

            if (roi.Width < RoiMinWidth || roi.Height < RoiMinHeight)
                return false;

            if (roi.X < 0 || roi.Y < 0)
                return false;

            if (roi.Right > _loadedImage.Width || roi.Bottom > _loadedImage.Height)
                return false;

            return true;
        }

        private string FormatRoiEdgeLabel(string roiName, RoiEdgeResult result)
        {
            if (result == null || !result.Found)
                return roiName + " Edge : Not Found";

            string posText;

            if (result.ScanDirection == RoiScanDirection.TopToBottom ||
                result.ScanDirection == RoiScanDirection.BottomToTop)
            {
                posText = string.Format("Y={0:F2}", result.RepresentativePoint.Y);
            }
            else
            {
                posText = string.Format("X={0:F2}", result.RepresentativePoint.X);
            }

            return string.Format(
                "{0} Edge : {1}, Pts={2}/{3}, Str={4:F1}%",
                roiName,
                posText,
                result.InlierPoints.Count,
                result.TotalScanLines,
                result.AverageStrengthPercent);
        }

        private byte[] CreateBinaryBufferFromGray(
            byte[] grayBuffer,
            int imageWidth,
            int imageHeight,
            int threshold,
            bool inverse)
        {
            if (grayBuffer == null)
                return null;

            if (imageWidth <= 0 || imageHeight <= 0)
                return null;

            int length = imageWidth * imageHeight;

            if (grayBuffer.Length < length)
                return null;

            byte[] binaryBuffer = new byte[length];

            for (int i = 0; i < length; i++)
            {
                bool white = grayBuffer[i] > threshold;

                if (inverse)
                    white = !white;

                binaryBuffer[i] = white ? (byte)255 : (byte)0;
            }

            return binaryBuffer;
        }

        private RoiEdgeResult DetectEdgeInRoi(
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
            return RoiEdgeDetector.Detect(
                grayBuffer,
                imageWidth,
                imageHeight,
                roi,
                direction,
                polarity,
                selection,
                strengthThresholdPercent,
                scanStep,
                minValidPercent);
        }

        private void CalculateRoiDistance(
            PointF p1,
            PointF p2,
            MeasureCalcMode mode,
            double resolutionMmPerPx)
        {
            PointF calcP1 = p1;
            PointF calcP2 = p2;

            if (mode == MeasureCalcMode.XFixed)
            {
                calcP2 = new PointF(p1.X, p2.Y);
            }
            else if (mode == MeasureCalcMode.YFixed)
            {
                calcP2 = new PointF(p2.X, p1.Y);
            }

            double dx = calcP2.X - calcP1.X;
            double dy = calcP2.Y - calcP1.Y;

            _roiDistancePx = Math.Sqrt(dx * dx + dy * dy);
            _roiDistanceMm = _roiDistancePx * resolutionMmPerPx;
            _roiDistanceUm = _roiDistanceMm * 1000.0;

            _roiMeasureCalcPoint1 = calcP1;
            _roiMeasureCalcPoint2 = calcP2;
        }

        private void TbRoiResolution_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CommitRoiResolutionText();
            }
        }

        private void TbRoiResolution_Leave(object sender, EventArgs e)
        {
            CommitRoiResolutionText();
        }

        private bool CommitRoiResolutionText()
        {
            if (_isRestoringRoiResolutionText)
                return false;

            double newResolution;
            string errorMessage;

            if (!TryParseResolutionText(tbRoiResolution.Text, out newResolution, out errorMessage))
            {
                MessageBox.Show(
                    errorMessage,
                    "ROI Resolution 입력 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                RestoreLastValidRoiResolutionText();
                return false;
            }

            _roiResolutionMmPerPx = newResolution;
            _lastValidRoiResolutionText = newResolution.ToString(
                "0.################",
                CultureInfo.InvariantCulture);

            if (tbRoiResolution.Text != _lastValidRoiResolutionText)
            {
                _isRestoringRoiResolutionText = true;
                tbRoiResolution.Text = _lastValidRoiResolutionText;
                tbRoiResolution.SelectionStart = tbRoiResolution.Text.Length;
                _isRestoringRoiResolutionText = false;
            }

            // Run 이후 Edge 결과가 있으면 거리만 재계산
            RecalculateRoiDistanceFromExistingEdges();

            return true;
        }

        private void RestoreLastValidRoiResolutionText()
        {
            _isRestoringRoiResolutionText = true;

            tbRoiResolution.Text = _lastValidRoiResolutionText;
            tbRoiResolution.SelectionStart = tbRoiResolution.Text.Length;

            _isRestoringRoiResolutionText = false;
        }

        private void CmbRoiMeasureType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _roiMeasureType = GetCurrentRoiMeasureType();

            UpdateRoiMeasureTypeUi();

            // Measure Type이 바뀌면 기존 ROI/결과는 의미가 달라지므로 초기화
            ClearRoiMeasureByTypeChange();

            pnlViewer.Invalidate();
        }

        private void RoiMeasureMode_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null)
                return;

            if (!rb.Checked)
                return;

            // Run 이후 Edge 결과가 있으면 거리만 재계산
            RecalculateRoiDistanceFromExistingEdges();
        }

        private void RecalculateRoiDistanceFromExistingEdges()
        {
            if (_roi1EdgeResult == null || !_roi1EdgeResult.Found)
                return;

            if (_roi2EdgeResult == null || !_roi2EdgeResult.Found)
                return;

            _roiMeasureType = GetCurrentRoiMeasureType();

            if (_roiMeasureType == RoiMeasureType.SingleRoiDualEdge)
            {
                if (!_roi1Rect.HasValue)
                    return;

                Rectangle singleRoi = ClampRectangleToImage(_roi1Rect.Value);

                if (!CalculateSingleRoiDualEdgeDistance(singleRoi))
                    return;
            }
            else
            {
                PointF p1 = _roi1EdgeResult.RepresentativePoint;
                PointF p2 = _roi2EdgeResult.RepresentativePoint;

                _roiMeasurePoint1 = p1;
                _roiMeasurePoint2 = p2;

                CalculateRoiDistance(
                    p1,
                    p2,
                    GetCurrentRoiMeasureCalcMode(),
                    _roiResolutionMmPerPx);
            }

            UpdateRoiDistanceLabels();

            pnlViewer.Invalidate();
        }

        private void UpdateRoiDistanceLabels()
        {
            lblRoiDistancePx.Text = string.Format(
                "Distance px : {0:F3}",
                _roiDistancePx);

            lblRoiDistanceMm.Text = string.Format(
                "Distance mm : {0:F6}",
                _roiDistanceMm);

            lblRoiDistanceUm.Text = string.Format(
                "Distance um : {0:F3}",
                _roiDistanceUm);
        }
    }
}
