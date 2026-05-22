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
        private void InitViewer()
        {
            pnlViewer.BackColor = Color.Black;
            pnlViewer.TabStop = true;
            pnlViewer.DoubleBuffered(true);

            lblFileName.Text = "File : -";
            lblImageSize.Text = "Size : -";

            lblPoint1.Text = "Point1 : -";
            lblPoint2.Text = "Point2 : -";
            lblDistancePx.Text = "Distance px : -";
            lblDistanceMm.Text = "Distance mm : -";
            lblDistanceUm.Text = "Distance um : -";

            // tbResolution.Text = "0.001"; // mm/px 기본값
            _resolutionMmPerPx = 0.001;
            _lastValidResolutionText = _resolutionMmPerPx.ToString("0.################", CultureInfo.InvariantCulture);
            tbResolution.Text = _lastValidResolutionText;

            // 디자이너에서 rbMeasureFree.Checked = true로 해도 되지만,
            // 코드에서도 한 번 더 보장
            rbMeasureFree.Checked = true;
        }

        private void FitImageToViewer()
        {
            if (_loadedImage == null)
                return;

            if (pnlViewer.Width <= 0 || pnlViewer.Height <= 0)
                return;

            float scaleX = (float)pnlViewer.Width / _loadedImage.Width;
            float scaleY = (float)pnlViewer.Height / _loadedImage.Height;

            _zoom = Math.Min(scaleX, scaleY);

            float displayWidth = _loadedImage.Width * _zoom;
            float displayHeight = _loadedImage.Height * _zoom;

            _panOffset.X = (pnlViewer.Width - displayWidth) / 2.0f;
            _panOffset.Y = (pnlViewer.Height - displayHeight) / 2.0f;
        }

        private void PnlViewer_Resize(object sender, EventArgs e)
        {
            pnlViewer.Invalidate();
        }

        private void PnlViewer_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);

            if (_loadedImage == null)
                return;

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Viewer에 그릴 실제 이미지 (tabPage3 View 모드에 따라 Preview로 교체)
            Bitmap drawImage = GetViewerDrawImage();

            RectangleF destRect = new RectangleF(
                _panOffset.X,
                _panOffset.Y,
                drawImage.Width * _zoom,
                drawImage.Height * _zoom);

            e.Graphics.DrawImage(drawImage, destRect);

            DrawMeasureOverlay(e.Graphics);
            DrawRoiOverlay(e.Graphics);

            if (tabControl1.SelectedTab != tabPage3 || chkHvShowOverlay.Checked)
            {
                DrawHoleVentOverlay(e.Graphics);
            }

            DrawViewerInfo(e.Graphics);
        }

        private Bitmap GetViewerDrawImage()
        {
            if (tabControl1.SelectedTab != tabPage3)
                return _loadedImage;

            switch (_hvViewMode)
            {
                case HvViewImageMode.HoleBinary:
                    return _holeBinaryPreview ?? _loadedImage;

                case HvViewImageMode.HoleBinaryDetect:
                    return _holeBinaryDetectPreview ?? _loadedImage;

                case HvViewImageMode.VentBinary:
                    return _ventBinaryPreview ?? _loadedImage;

                case HvViewImageMode.VentBinaryDetect:
                    return _ventBinaryDetectPreview ?? _loadedImage;

                case HvViewImageMode.Original:
                default:
                    return _loadedImage;
            }
        }

        private void DrawViewerInfo(Graphics g)
        {
            string infoText;

            if (_loadedImage == null)
            {
                infoText = "X=-, Y=- | Pixel=- | Zoom=-";
            }
            else if (!_isViewerHoverValid)
            {
                infoText = string.Format(
                    "X=-, Y=- | Pixel=- | Zoom={0:F2}x",
                    _zoom);
            }
            else
            {
                string pixelText = GetPixelValueText(_loadedImage, _viewerHoverPixel);

                infoText = string.Format(
                    "X={0}, Y={1} | {2} | Zoom={3:F2}x",
                    _viewerHoverPixel.X,
                    _viewerHoverPixel.Y,
                    pixelText,
                    _zoom);
            }

            DrawInfoBox(g, infoText, pnlViewer.ClientSize);
        }

        private string GetPixelValueText(Bitmap bmp, Point pixel)
        {
            if (bmp == null)
                return "Pixel=-";

            if (pixel.X < 0 || pixel.Y < 0 || pixel.X >= bmp.Width || pixel.Y >= bmp.Height)
                return "Pixel=-";

            Color color = bmp.GetPixel(pixel.X, pixel.Y);

            // Gray / Binary 이미지는 보통 R=G=B
            if (color.R == color.G && color.G == color.B)
            {
                return string.Format("Pixel=Gray({0})", color.R);
            }

            // 사용자 표시 기준은 RGB 순서
            return string.Format("Pixel=RGB({0},{1},{2})", color.R, color.G, color.B);
        }

        private void DrawInfoBox(Graphics g, string text, Size clientSize)
        {
            using (Font font = new Font("맑은 고딕", 9, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(text, font);

                float x = 8;
                float y = clientSize.Height - textSize.Height - 8;

                RectangleF bgRect = new RectangleF(
                    x - 4,
                    y - 3,
                    textSize.Width + 8,
                    textSize.Height + 6);

                using (Brush bgBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                using (Brush textBrush = new SolidBrush(Color.Yellow))
                {
                    g.FillRectangle(bgBrush, bgRect);
                    g.DrawString(text, font, textBrush, x, y);
                }
            }
        }

        private void DrawCross(Graphics g, Pen pen, PointF pt, float size)
        {
            g.DrawLine(pen, pt.X - size, pt.Y, pt.X + size, pt.Y);
            g.DrawLine(pen, pt.X, pt.Y - size, pt.X, pt.Y + size);
            g.DrawEllipse(pen, pt.X - size, pt.Y - size, size * 2, size * 2);
        }

        private void PnlViewer_MouseEnter(object sender, EventArgs e)
        {
            pnlViewer.Focus();
        }

        private void PnlViewer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_loadedImage == null)
                return;

            PointF mouseImageBefore = ScreenToImage(e.Location);

            if (e.Delta > 0)
                _zoom *= 1.1f;
            else
                _zoom /= 1.1f;

            if (_zoom < 0.01f)
                _zoom = 0.01f;

            if (_zoom > 100.0f)
                _zoom = 100.0f;

            _panOffset.X = e.X - mouseImageBefore.X * _zoom;
            _panOffset.Y = e.Y - mouseImageBefore.Y * _zoom;

            pnlViewer.Invalidate();
        }

        private void PnlViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (_loadedImage == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                if (_viewerMode == ViewerMode.MeasureDistance)
                {
                    AddMeasurePoint(e.Location);
                    return;
                }

                if (TryBeginHvRoiEdit(e.Location))
                {
                    return;
                }

                if (TryBeginRoiEdit(e.Location))
                {
                    return;
                }

                _isPanning = true;
                _lastMousePoint = e.Location;
                pnlViewer.Cursor = Cursors.Hand;
            }
        }

        private void PnlViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_loadedImage == null)
            {
                _isViewerHoverValid = false;
                return;
            }

            Point hoverPixel = ScreenToImagePixel(e.Location);

            if (IsImagePixelValid(hoverPixel))
            {
                _viewerHoverPixel = hoverPixel;
                _isViewerHoverValid = true;
            }
            else
            {
                _isViewerHoverValid = false;
            }

            if (_isHvRoiEditing)
            {
                UpdateHvRoiEdit(e.Location);
                pnlViewer.Invalidate();
                return;
            }

            if (_isRoiEditing)
            {
                UpdateRoiEdit(e.Location);
                pnlViewer.Invalidate();
                return;
            }

            if (tabControl1.SelectedTab == tabPage3)
                UpdateHvRoiCursor(e.Location);
            else
                UpdateRoiCursor(e.Location);

            if (_isPanning)
            {
                int dx = e.X - _lastMousePoint.X;
                int dy = e.Y - _lastMousePoint.Y;

                _panOffset.X += dx;
                _panOffset.Y += dy;

                _lastMousePoint = e.Location;
            }

            pnlViewer.Invalidate();
        }

        private void PnlViewer_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_isHvRoiEditing)
                {
                    _isHvRoiEditing = false;
                    _activeHvRoiIndex = 0;
                    _activeHvRoiHitType = RoiHitType.None;
                    pnlViewer.Cursor = Cursors.Default;
                    pnlViewer.Invalidate();
                    return;
                }

                if (_isRoiEditing)
                {
                    _isRoiEditing = false;
                    _activeRoiIndex = 0;
                    _activeRoiHitType = RoiHitType.None;
                    pnlViewer.Cursor = Cursors.Default;
                    pnlViewer.Invalidate();
                    return;
                }

                _isPanning = false;

                if (_viewerMode == ViewerMode.MeasureDistance)
                    pnlViewer.Cursor = Cursors.Cross;
                else
                    pnlViewer.Cursor = Cursors.Default;
            }
        }

        private void PnlViewer_MouseLeave(object sender, EventArgs e)
        {
            _isViewerHoverValid = false;
            pnlViewer.Invalidate();
        }

        private PointF ScreenToImage(Point screenPoint)
        {
            if (_loadedImage == null)
                return PointF.Empty;

            float imageX = (screenPoint.X - _panOffset.X) / _zoom;
            float imageY = (screenPoint.Y - _panOffset.Y) / _zoom;

            return new PointF(imageX, imageY);
        }

        private PointF ImagePointToScreen(PointF imagePoint)
        {
            float screenX = imagePoint.X * _zoom + _panOffset.X;
            float screenY = imagePoint.Y * _zoom + _panOffset.Y;

            return new PointF(screenX, screenY);
        }

        private Point ScreenToImagePixel(Point screenPoint)
        {
            PointF imagePoint = ScreenToImage(screenPoint);

            const double EPS = 1e-6;

            int x = (int)Math.Floor(imagePoint.X + EPS);
            int y = (int)Math.Floor(imagePoint.Y + EPS);

            return new Point(x, y);
        }

        private bool IsImagePixelValid(Point pt)
        {
            if (_loadedImage == null)
                return false;

            if (pt.X < 0 || pt.Y < 0)
                return false;

            if (pt.X >= _loadedImage.Width || pt.Y >= _loadedImage.Height)
                return false;

            return true;
        }

        private PointF ImagePixelCenterToScreen(Point imagePixel)
        {
            float screenX = (imagePixel.X + 0.5f) * _zoom + _panOffset.X;
            float screenY = (imagePixel.Y + 0.5f) * _zoom + _panOffset.Y;

            return new PointF(screenX, screenY);
        }
    }
}
