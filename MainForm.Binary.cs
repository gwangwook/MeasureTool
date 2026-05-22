using System;
using System.Drawing;
using System.Windows.Forms;

namespace MeasureTool
{
    public partial class MainForm
    {
        private void MenuBinarySetting_Click(object sender, EventArgs e)
        {
            if (_originalImage == null)
            {
                MessageBox.Show("이미지를 먼저 로드하세요.");
                return;
            }

            using (FormBinarySetting frm = new FormBinarySetting(
                _originalImage,
                _binaryThreshold,
                _binaryInverse))
            {
                if (frm.ShowDialog(this) != DialogResult.OK)
                    return;

                _binaryThreshold = frm.BinaryThreshold;
                _binaryInverse = frm.BinaryInverse;

                if (_binaryImage != null)
                {
                    _binaryImage.Dispose();
                    _binaryImage = null;
                }

                _binaryImage = BinaryProcessor.CreateBinaryBitmap(
                    _originalImage,
                    _binaryThreshold,
                    _binaryInverse);

                _loadedImage = _binaryImage;

                UpdateImageInfoLabel("Binary");
                pnlViewer.Invalidate();
            }
        }

        private void MenuShowOriginal_Click(object sender, EventArgs e)
        {
            if (_originalImage == null)
            {
                MessageBox.Show("이미지를 먼저 로드하세요.");
                return;
            }

            _loadedImage = _originalImage;
            UpdateImageInfoLabel("Original");
            pnlViewer.Invalidate();
        }

        private void MenuShowBinary_Click(object sender, EventArgs e)
        {
            if (_binaryImage == null)
            {
                MessageBox.Show("Binary 이미지가 없습니다.\r\n먼저 Binary 설정을 적용하세요.");
                return;
            }

            _loadedImage = _binaryImage;
            UpdateImageInfoLabel("Binary");
            pnlViewer.Invalidate();
        }
    }
}
