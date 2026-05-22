namespace MeasureTool
{
    partial class FormBinarySetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.lblBinaryThreshold = new System.Windows.Forms.Label();
            this.trbBinaryThreshold = new System.Windows.Forms.TrackBar();
            this.nudBinaryThreshold = new System.Windows.Forms.NumericUpDown();
            this.chkBinaryInverse = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbBinaryThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBinaryThreshold)).BeginInit();
            this.SuspendLayout();
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pbPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPreview.Location = new System.Drawing.Point(27, 24);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(651, 388);
            this.pbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPreview.TabIndex = 0;
            this.pbPreview.TabStop = false;
            // 
            // lblBinaryThreshold
            // 
            this.lblBinaryThreshold.AutoSize = true;
            this.lblBinaryThreshold.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblBinaryThreshold.Location = new System.Drawing.Point(62, 440);
            this.lblBinaryThreshold.Name = "lblBinaryThreshold";
            this.lblBinaryThreshold.Size = new System.Drawing.Size(104, 15);
            this.lblBinaryThreshold.TabIndex = 1;
            this.lblBinaryThreshold.Text = "Binary Threshold";
            // 
            // trbBinaryThreshold
            // 
            this.trbBinaryThreshold.Location = new System.Drawing.Point(211, 440);
            this.trbBinaryThreshold.Maximum = 255;
            this.trbBinaryThreshold.Name = "trbBinaryThreshold";
            this.trbBinaryThreshold.Size = new System.Drawing.Size(330, 45);
            this.trbBinaryThreshold.TabIndex = 2;
            this.trbBinaryThreshold.TickFrequency = 10;
            this.trbBinaryThreshold.Value = 128;
            // 
            // nudBinaryThreshold
            // 
            this.nudBinaryThreshold.Location = new System.Drawing.Point(547, 440);
            this.nudBinaryThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBinaryThreshold.Name = "nudBinaryThreshold";
            this.nudBinaryThreshold.Size = new System.Drawing.Size(62, 21);
            this.nudBinaryThreshold.TabIndex = 3;
            this.nudBinaryThreshold.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            // 
            // chkBinaryInverse
            // 
            this.chkBinaryInverse.AutoSize = true;
            this.chkBinaryInverse.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chkBinaryInverse.Location = new System.Drawing.Point(65, 505);
            this.chkBinaryInverse.Name = "chkBinaryInverse";
            this.chkBinaryInverse.Size = new System.Drawing.Size(108, 19);
            this.chkBinaryInverse.TabIndex = 4;
            this.chkBinaryInverse.Text = "Binary Inverse";
            this.chkBinaryInverse.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnApply.Location = new System.Drawing.Point(522, 545);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(73, 23);
            this.btnApply.TabIndex = 5;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.Location = new System.Drawing.Point(603, 545);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // FormBinarySetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 593);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.chkBinaryInverse);
            this.Controls.Add(this.nudBinaryThreshold);
            this.Controls.Add(this.trbBinaryThreshold);
            this.Controls.Add(this.lblBinaryThreshold);
            this.Controls.Add(this.pbPreview);
            this.Name = "FormBinarySetting";
            this.Text = "FormBinarySetting";
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbBinaryThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBinaryThreshold)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.Label lblBinaryThreshold;
        private System.Windows.Forms.TrackBar trbBinaryThreshold;
        private System.Windows.Forms.NumericUpDown nudBinaryThreshold;
        private System.Windows.Forms.CheckBox chkBinaryInverse;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}