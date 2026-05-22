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
        private void DrawMeasureOverlay(Graphics g)
        {
            if (_measurePoint1Raw == null)
                return;

            using (Pen rawPointPen = new Pen(Color.Lime, 2))
            using (Pen measureLinePen = new Pen(Color.Red, 2))
            using (Pen usedPointPen = new Pen(Color.Orange, 2))
            using (Font font = new Font("맑은 고딕", 9, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.Cyan))
            using (Brush usedTextBrush = new SolidBrush(Color.Orange))
            {
                Point p1Raw = _measurePoint1Raw.Value;
                PointF p1RawScreen = ImagePixelCenterToScreen(p1Raw);

                // 사용자가 찍은 Point1 표시
                DrawCross(g, rawPointPen, p1RawScreen, 6);
                g.DrawString(
                    string.Format("P1 ({0}, {1})", p1Raw.X, p1Raw.Y),
                    font,
                    textBrush,
                    p1RawScreen.X + 8,
                    p1RawScreen.Y + 8);

                if (_measurePoint2Raw != null)
                {
                    Point p2Raw = _measurePoint2Raw.Value;
                    PointF p2RawScreen = ImagePixelCenterToScreen(p2Raw);

                    // 사용자가 찍은 Point2 표시
                    DrawCross(g, rawPointPen, p2RawScreen, 6);
                    g.DrawString(
                        string.Format("P2 ({0}, {1})", p2Raw.X, p2Raw.Y),
                        font,
                        textBrush,
                        p2RawScreen.X + 8,
                        p2RawScreen.Y + 8);

                    Point p1Calc;
                    Point p2Calc;

                    if (TryGetCalculatedMeasurePoints(out p1Calc, out p2Calc))
                    {
                        PointF p1CalcScreen = ImagePixelCenterToScreen(p1Calc);
                        PointF p2CalcScreen = ImagePixelCenterToScreen(p2Calc);

                        // 실제 거리 계산에 사용하는 선
                        g.DrawLine(measureLinePen, p1CalcScreen, p2CalcScreen);

                        // Fixed 모드에서는 계산에 쓰이는 P2 위치가 Raw P2와 다를 수 있으므로 보조 표시
                        if (p2Calc != p2Raw)
                        {
                            DrawCross(g, usedPointPen, p2CalcScreen, 4);
                            g.DrawString(
                                "Used",
                                font,
                                usedTextBrush,
                                p2CalcScreen.X + 8,
                                p2CalcScreen.Y - 18);
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
                            (p1CalcScreen.X + p2CalcScreen.X) / 2.0f,
                            (p1CalcScreen.Y + p2CalcScreen.Y) / 2.0f);

                        g.DrawString(displayText, font, textBrush, mid.X + 10, mid.Y + 10);
                    }
                }
            }
        }

        private void AddMeasurePoint(Point screenPoint)
        {
            Point imagePixel = ScreenToImagePixel(screenPoint);

            if (!IsImagePixelValid(imagePixel))
                return;

            if (_measurePoint1Raw == null)
            {
                _measurePoint1Raw = imagePixel;
                _measurePoint2Raw = null;
            }
            else if (_measurePoint2Raw == null)
            {
                _measurePoint2Raw = imagePixel;

                // 두 점 측정 후 측정 모드 종료
                _viewerMode = ViewerMode.None;
                pnlViewer.Cursor = Cursors.Default;
            }
            else
            {
                _measurePoint1Raw = imagePixel;
                _measurePoint2Raw = null;
            }

            RefreshMeasureDisplay();
        }

        private void RefreshMeasureDisplay()
        {
            if (_measurePoint1Raw == null)
            {
                lblPoint1.Text = "Point1 : -";
                lblPoint2.Text = "Point2 : -";
                lblDistancePx.Text = "Distance px : -";
                lblDistanceMm.Text = "Distance mm : -";
                lblDistanceUm.Text = "Distance um : -";

                pnlViewer.Invalidate();
                return;
            }

            Point p1Raw = _measurePoint1Raw.Value;

            lblPoint1.Text = string.Format(
                "Point1 : X={0}, Y={1}",
                p1Raw.X,
                p1Raw.Y);

            if (_measurePoint2Raw == null)
            {
                lblPoint2.Text = "Point2 : -";
                lblDistancePx.Text = "Distance px : -";
                lblDistanceMm.Text = "Distance mm : -";
                lblDistanceUm.Text = "Distance um : -";

                pnlViewer.Invalidate();
                return;
            }

            Point p2Raw = _measurePoint2Raw.Value;

            lblPoint2.Text = string.Format(
                "Point2 : X={0}, Y={1}",
                p2Raw.X,
                p2Raw.Y);

            CalculateMeasureResult();

            pnlViewer.Invalidate();
        }

        private void CalculateMeasureResult()
        {
            Point p1Calc;
            Point p2Calc;

            if (!TryGetCalculatedMeasurePoints(out p1Calc, out p2Calc))
                return;

            int dx = p2Calc.X - p1Calc.X;
            int dy = p2Calc.Y - p1Calc.Y;
            double distancePx = GetDistance(p1Calc, p2Calc);

            MeasureCalcMode mode = GetCurrentMeasureCalcMode();

            string modeText = "Free";
            if (mode == MeasureCalcMode.XFixed) modeText = "X Fixed";
            if (mode == MeasureCalcMode.YFixed) modeText = "Y Fixed";

            lblDistancePx.Text = string.Format(
                "Distance px : {0:F3} / dX={1}, dY={2} / {3}",
                distancePx,
                dx,
                dy,
                modeText);

            double resolution = _resolutionMmPerPx;

            double distanceMm = distancePx * resolution;
            double distanceUm = distanceMm * 1000.0;

            double dxMm = dx * resolution;
            double dyMm = dy * resolution;

            lblDistanceMm.Text = string.Format(
                "Distance mm : {0:F6} / dX={1:F6}, dY={2:F6}",
                distanceMm,
                dxMm,
                dyMm);

            lblDistanceUm.Text = string.Format(
                "Distance um : {0:F3}",
                distanceUm);
        }

        private bool TryGetCalculatedMeasurePoints(out Point p1Calc, out Point p2Calc)
        {
            p1Calc = Point.Empty;
            p2Calc = Point.Empty;

            if (_measurePoint1Raw == null || _measurePoint2Raw == null)
                return false;

            Point p1Raw = _measurePoint1Raw.Value;
            Point p2Raw = _measurePoint2Raw.Value;

            p1Calc = p1Raw;

            MeasureCalcMode mode = GetCurrentMeasureCalcMode();

            if (mode == MeasureCalcMode.Free)
            {
                p2Calc = p2Raw;
            }
            else if (mode == MeasureCalcMode.XFixed)
            {
                p2Calc = new Point(p1Raw.X, p2Raw.Y);
            }
            else if (mode == MeasureCalcMode.YFixed)
            {
                p2Calc = new Point(p2Raw.X, p1Raw.Y);
            }

            return IsImagePixelValid(p1Calc) && IsImagePixelValid(p2Calc);
        }

        private MeasureCalcMode GetCurrentMeasureCalcMode()
        {
            if (rbMeasureXFixed.Checked)
                return MeasureCalcMode.XFixed;

            if (rbMeasureYFixed.Checked)
                return MeasureCalcMode.YFixed;

            return MeasureCalcMode.Free;
        }

        private void MeasureMode_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb == null)
                return;

            if (!rb.Checked)
                return;

            // RadioButton 변경 시 기존 Raw Point는 유지하고
            // 계산 결과와 Display 선만 갱신
            RefreshMeasureDisplay();
        }

        private void MenuMeasureDistance_Click(object sender, EventArgs e)
        {
            if (_loadedImage == null)
            {
                MessageBox.Show("이미지를 먼저 로드하세요.");
                return;
            }

            _viewerMode = ViewerMode.MeasureDistance;
            _measurePoint1Raw = null;
            _measurePoint2Raw = null;

            lblPoint1.Text = "Point1 : 첫 번째 픽셀을 선택하세요.";
            lblPoint2.Text = "Point2 : -";
            lblDistancePx.Text = "Distance px : -";
            lblDistanceMm.Text = "Distance mm : -";
            lblDistanceUm.Text = "Distance um : -";

            pnlViewer.Cursor = Cursors.Cross;
            pnlViewer.Invalidate();
        }

        private void MenuClearMeasure_Click(object sender, EventArgs e)
        {
            ClearMeasure();
            pnlViewer.Invalidate();
        }

        private void MenuFitImage_Click(object sender, EventArgs e)
        {
            if (_loadedImage == null)
                return;

            FitImageToViewer();
            pnlViewer.Invalidate();
        }

        private void ClearMeasure()
        {
            _viewerMode = ViewerMode.None;

            _measurePoint1Raw = null;
            _measurePoint2Raw = null;

            lblPoint1.Text = "Point1 : -";
            lblPoint2.Text = "Point2 : -";
            lblDistancePx.Text = "Distance px : -";
            lblDistanceMm.Text = "Distance mm : -";
            lblDistanceUm.Text = "Distance um : -";

            pnlViewer.Cursor = Cursors.Default;
        }

        private void TbResolution_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CommitResolutionText();
            }
        }

        private void TbResolution_Leave(object sender, EventArgs e)
        {
            CommitResolutionText();
        }

        private void CommitResolutionText()
        {
            if (_isRestoringResolutionText)
                return;

            double newResolution;
            string errorMessage;

            if (!TryParseResolutionText(tbResolution.Text, out newResolution, out errorMessage))
            {
                MessageBox.Show(
                    errorMessage,
                    "Resolution 입력 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                RestoreLastValidResolutionText();
                return;
            }

            _resolutionMmPerPx = newResolution;
            _lastValidResolutionText = newResolution.ToString("0.################", CultureInfo.InvariantCulture);

            if (tbResolution.Text != _lastValidResolutionText)
            {
                _isRestoringResolutionText = true;
                tbResolution.Text = _lastValidResolutionText;
                tbResolution.SelectionStart = tbResolution.Text.Length;
                _isRestoringResolutionText = false;
            }

            // 이미 측정된 Point가 있으면 Resolution 변경 즉시 결과 재계산
            RefreshMeasureDisplay();
        }

        private bool TryParseResolutionText(string text, out double resolution, out string errorMessage)
        {
            resolution = 0.0;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(text))
            {
                errorMessage = "Resolution 값을 입력하세요.";
                return false;
            }

            string value = text.Trim();

            // 부호 금지
            if (value.Contains("+") || value.Contains("-"))
            {
                errorMessage = "Resolution에는 + 또는 - 부호를 입력할 수 없습니다.\r\n0보다 큰 숫자만 입력하세요.";
                return false;
            }

            // 숫자와 소수점만 허용
            int dotCount = 0;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];

                if (char.IsDigit(ch))
                    continue;

                if (ch == '.')
                {
                    dotCount++;

                    if (dotCount > 1)
                    {
                        errorMessage = "소수점은 한 번만 입력할 수 있습니다.";
                        return false;
                    }

                    continue;
                }

                errorMessage = "Resolution에는 숫자와 소수점만 입력할 수 있습니다.";
                return false;
            }

            if (!double.TryParse(
                value,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out resolution))
            {
                errorMessage = "Resolution 값을 숫자로 변환할 수 없습니다.";
                return false;
            }

            if (resolution <= 0)
            {
                errorMessage = "Resolution은 0보다 큰 값이어야 합니다.";
                return false;
            }

            return true;
        }

        private void RestoreLastValidResolutionText()
        {
            _isRestoringResolutionText = true;

            tbResolution.Text = _lastValidResolutionText;
            tbResolution.SelectionStart = tbResolution.Text.Length;

            _isRestoringResolutionText = false;
        }
    }
}
