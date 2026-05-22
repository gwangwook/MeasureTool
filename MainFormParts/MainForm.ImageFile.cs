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
        private void MenuLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "이미지 선택";
                ofd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All Files|*.*";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                LoadImage(ofd.FileName);
            }
        }

        private void DisposeImages()
        {
            if (_binaryImage != null)
            {
                _binaryImage.Dispose();
                _binaryImage = null;
            }

            if (_originalImage != null)
            {
                _originalImage.Dispose();
                _originalImage = null;
            }

            _loadedImage = null;

            DisposeHoleVentPreviewImages();
        }

        private void UpdateImageInfoLabel(string mode)
        {
            if (_loadedImage == null)
            {
                lblImageSize.Text = "Size : -";
                return;
            }

            lblImageSize.Text = string.Format(
                "Size : {0} x {1}, {2}, View={3}",
                _loadedImage.Width,
                _loadedImage.Height,
                _loadedImage.PixelFormat,
                mode);
        }

        private void LoadImage(string filePath)
        {
            try
            {
                DisposeImages();

                _originalImage = new Bitmap(filePath);
                _binaryImage = null;
                _loadedImage = _originalImage;

                lblFileName.Text = "File : " + System.IO.Path.GetFileName(filePath);
                UpdateImageInfoLabel("Original");

                ClearMeasure();
                ClearHoleVentRoiAndResult();

                FitImageToViewer();

                pnlViewer.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("이미지 로드 실패\r\n" + ex.Message);
            }
        }
    }
}
