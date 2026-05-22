using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeasureTool
{
    public partial class FormBinarySetting : Form
    {
        private Bitmap _previewSource = null;
        private Bitmap _previewBinary = null;

        private bool _isSyncing = false;

        // Preview Zoom / Pan
        private float _previewZoom = 1.0f;
        private PointF _previewPanOffset = new PointF(0, 0);
        private bool _isPreviewPanning = false;
        private Point _lastPreviewMousePoint;
        private bool _previewViewInitialized = false;

        public int BinaryThreshold { get; private set; }
        public bool BinaryInverse { get; private set; }

        // Preview Hover Info
        private bool _isPreviewHoverValid = false;
        private Point _previewHoverPixel = Point.Empty;

        public FormBinarySetting(Bitmap sourceImage, int currentThreshold, bool currentInverse)
        {
            InitializeComponent();

            if (sourceImage == null)
                throw new ArgumentNullException("sourceImage");

            BinaryThreshold = ImageProcessingHelper.Clamp(currentThreshold, 0, 255);
            BinaryInverse = currentInverse;

            // Preview용 Source 생성
            // 실제 Apply 시에는 MainForm에서 _originalImage 기준으로 다시 Binary 생성하는 구조를 유지
            _previewSource = BinaryProcessor.CloneAs24Bpp(sourceImage);

            InitControlValues();
            InitEvents();
            UpdatePreview();
        }

        private void InitControlValues()
        {
            this.Text = "Binary 설정";
            this.StartPosition = FormStartPosition.CenterParent;

            trbBinaryThreshold.Minimum = 0;
            trbBinaryThreshold.Maximum = 255;
            trbBinaryThreshold.TickFrequency = 10;
            trbBinaryThreshold.Value = BinaryThreshold;

            nudBinaryThreshold.Minimum = 0;
            nudBinaryThreshold.Maximum = 255;
            nudBinaryThreshold.Value = BinaryThreshold;

            chkBinaryInverse.Checked = BinaryInverse;

            // PictureBox.Image / SizeMode.Zoom 방식 사용하지 않음
            // Paint 이벤트에서 직접 Draw해서 Zoom/Pan 구현
            pbPreview.BackColor = Color.Black;
            pbPreview.SizeMode = PictureBoxSizeMode.Normal;
            pbPreview.Image = null;
            pbPreview.TabStop = true;
        }

        private void InitEvents()
        {
            trbBinaryThreshold.ValueChanged += TrbBinaryThreshold_ValueChanged;
            nudBinaryThreshold.ValueChanged += NudBinaryThreshold_ValueChanged;
            chkBinaryInverse.CheckedChanged += ChkBinaryInverse_CheckedChanged;
            btnApply.Click += BtnApply_Click;
            btnCancel.Click += BtnCancel_Click;

            // Preview Viewer 이벤트
            pbPreview.Paint += PbPreview_Paint;
            pbPreview.MouseWheel += PbPreview_MouseWheel;
            pbPreview.MouseDown += PbPreview_MouseDown;
            pbPreview.MouseMove += PbPreview_MouseMove;
            pbPreview.MouseUp += PbPreview_MouseUp;
            pbPreview.MouseEnter += PbPreview_MouseEnter;
            pbPreview.MouseLeave += PbPreview_MouseLeave;
            pbPreview.DoubleClick += PbPreview_DoubleClick;
            pbPreview.Resize += PbPreview_Resize;

            this.FormClosed += FormBinarySetting_FormClosed;
        }
        private void TrbBinaryThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (_isSyncing)
                return;

            _isSyncing = true;
            nudBinaryThreshold.Value = trbBinaryThreshold.Value;
            _isSyncing = false;

            BinaryThreshold = trbBinaryThreshold.Value;
            UpdatePreview();
        }

        private void NudBinaryThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (_isSyncing)
                return;

            _isSyncing = true;
            trbBinaryThreshold.Value = (int)nudBinaryThreshold.Value;
            _isSyncing = false;

            BinaryThreshold = (int)nudBinaryThreshold.Value;
            UpdatePreview();
        }

        private void ChkBinaryInverse_CheckedChanged(object sender, EventArgs e)
        {
            BinaryInverse = chkBinaryInverse.Checked;
            UpdatePreview();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            BinaryThreshold = trbBinaryThreshold.Value;
            BinaryInverse = chkBinaryInverse.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UpdatePreview()
        {
            if (_previewSource == null)
                return;

            if (_previewBinary != null)
            {
                _previewBinary.Dispose();
                _previewBinary = null;
            }

            _previewBinary = BinaryProcessor.CreateBinaryBitmap(_previewSource, BinaryThreshold, BinaryInverse);

            // 중요:
            // 최초 1회만 Fit.
            // 이후 Threshold / Inverse 변경 시에는 현재 Zoom/Pan 상태 유지.
            if (!_previewViewInitialized)
            {
                FitPreviewImageToBox();
                _previewViewInitialized = true;
            }

            pbPreview.Invalidate();
        }

        private void FitPreviewImageToBox()
        {
            if (_previewBinary == null)
                return;

            if (pbPreview.Width <= 0 || pbPreview.Height <= 0)
                return;

            float scaleX = (float)pbPreview.Width / _previewBinary.Width;
            float scaleY = (float)pbPreview.Height / _previewBinary.Height;

            _previewZoom = Math.Min(scaleX, scaleY);

            float displayWidth = _previewBinary.Width * _previewZoom;
            float displayHeight = _previewBinary.Height * _previewZoom;

            _previewPanOffset.X = (pbPreview.Width - displayWidth) / 2.0f;
            _previewPanOffset.Y = (pbPreview.Height - displayHeight) / 2.0f;
        }

        private void PbPreview_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);

            if (_previewBinary == null)
                return;

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            RectangleF destRect = new RectangleF(
                _previewPanOffset.X,
                _previewPanOffset.Y,
                _previewBinary.Width * _previewZoom,
                _previewBinary.Height * _previewZoom);

            e.Graphics.DrawImage(_previewBinary, destRect);

            DrawPreviewInfo(e.Graphics);
        }

        private void DrawPreviewInfo(Graphics g)
        {
            string infoText;

            if (_previewBinary == null)
            {
                infoText = "X=-, Y=- | Pixel=- | Zoom=-";
            }
            else if (!_isPreviewHoverValid)
            {
                infoText = string.Format(
                    "X=-, Y=- | Pixel=- | Zoom={0:F2}x",
                    _previewZoom);
            }
            else
            {
                string pixelText = GetPixelValueText(_previewBinary, _previewHoverPixel);

                infoText = string.Format(
                    "X={0}, Y={1} | {2} | Zoom={3:F2}x",
                    _previewHoverPixel.X,
                    _previewHoverPixel.Y,
                    pixelText,
                    _previewZoom);
            }

            DrawInfoBox(g, infoText, pbPreview.ClientSize);
        }

        private string GetPixelValueText(Bitmap bmp, Point pixel)
        {
            if (bmp == null)
                return "Pixel=-";

            if (pixel.X < 0 || pixel.Y < 0 || pixel.X >= bmp.Width || pixel.Y >= bmp.Height)
                return "Pixel=-";

            Color color = bmp.GetPixel(pixel.X, pixel.Y);

            if (color.R == color.G && color.G == color.B)
            {
                return string.Format("Pixel=Gray({0})", color.R);
            }

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

        private void PbPreview_MouseEnter(object sender, EventArgs e)
        {
            pbPreview.Focus();
        }

        private void PbPreview_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_previewBinary == null)
                return;

            PointF imageBefore = PreviewScreenToImage(e.Location);

            if (e.Delta > 0)
                _previewZoom *= 1.1f;
            else
                _previewZoom /= 1.1f;

            if (_previewZoom < 0.01f)
                _previewZoom = 0.01f;

            if (_previewZoom > 100.0f)
                _previewZoom = 100.0f;

            _previewPanOffset.X = e.X - imageBefore.X * _previewZoom;
            _previewPanOffset.Y = e.Y - imageBefore.Y * _previewZoom;

            pbPreview.Invalidate();
        }

        private void PbPreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (_previewBinary == null)
                return;

            if (e.Button == MouseButtons.Left)
            {
                _isPreviewPanning = true;
                _lastPreviewMousePoint = e.Location;
                pbPreview.Cursor = Cursors.Hand;
            }
        }

        private void PbPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (_previewBinary == null)
            {
                _isPreviewHoverValid = false;
                return;
            }

            Point hoverPixel = PreviewScreenToImagePixel(e.Location);

            if (IsPreviewPixelValid(hoverPixel))
            {
                _previewHoverPixel = hoverPixel;
                _isPreviewHoverValid = true;
            }
            else
            {
                _isPreviewHoverValid = false;
            }

            if (_isPreviewPanning)
            {
                int dx = e.X - _lastPreviewMousePoint.X;
                int dy = e.Y - _lastPreviewMousePoint.Y;

                _previewPanOffset.X += dx;
                _previewPanOffset.Y += dy;

                _lastPreviewMousePoint = e.Location;
            }

            pbPreview.Invalidate();
        }

        private void PbPreview_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPreviewPanning = false;
                pbPreview.Cursor = Cursors.Default;
            }
        }

        private void PbPreview_MouseLeave(object sender, EventArgs e)
        {
            _isPreviewHoverValid = false;
            pbPreview.Invalidate();
        }

        private void PbPreview_DoubleClick(object sender, EventArgs e)
        {
            FitPreviewImageToBox();
            pbPreview.Invalidate();
        }

        private void PbPreview_Resize(object sender, EventArgs e)
        {
            // Resize 때는 임의로 Fit 하지 않음.
            // 사용자가 잡아둔 Zoom/Pan 상태를 최대한 유지.
            pbPreview.Invalidate();
        }

        private PointF PreviewScreenToImage(Point screenPoint)
        {
            if (_previewBinary == null)
                return PointF.Empty;

            float imageX = (screenPoint.X - _previewPanOffset.X) / _previewZoom;
            float imageY = (screenPoint.Y - _previewPanOffset.Y) / _previewZoom;

            return new PointF(imageX, imageY);
        }

        private Point PreviewScreenToImagePixel(Point screenPoint)
        {
            PointF imagePoint = PreviewScreenToImage(screenPoint);

            const double EPS = 1e-6;

            int x = (int)Math.Floor(imagePoint.X + EPS);
            int y = (int)Math.Floor(imagePoint.Y + EPS);

            return new Point(x, y);
        }

        private bool IsPreviewPixelValid(Point pt)
        {
            if (_previewBinary == null)
                return false;

            if (pt.X < 0 || pt.Y < 0)
                return false;

            if (pt.X >= _previewBinary.Width || pt.Y >= _previewBinary.Height)
                return false;

            return true;
        }
        private void FormBinarySetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisposePreviewImages();
        }

        private void DisposePreviewImages()
        {
            if (pbPreview != null)
            {
                pbPreview.Image = null;
            }

            if (_previewBinary != null)
            {
                _previewBinary.Dispose();
                _previewBinary = null;
            }

            if (_previewSource != null)
            {
                _previewSource.Dispose();
                _previewSource = null;
            }
        }
    }
}