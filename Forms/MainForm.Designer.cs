namespace MeasureTool
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlViewer = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoadImage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveOriginalImage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveBinaryImage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveCurrentImage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveCurrentOverlayImage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPreprocess = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBinarySetting = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowOriginal = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowBinary = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gbImageInfo = new System.Windows.Forms.GroupBox();
            this.lblImageSize = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.gbMeasure = new System.Windows.Forms.GroupBox();
            this.lblDistanceUm = new System.Windows.Forms.Label();
            this.lblDistanceMm = new System.Windows.Forms.Label();
            this.lblDistancePx = new System.Windows.Forms.Label();
            this.lblPoint2 = new System.Windows.Forms.Label();
            this.lblPoint1 = new System.Windows.Forms.Label();
            this.gbMeasureMode = new System.Windows.Forms.GroupBox();
            this.rbMeasureYFixed = new System.Windows.Forms.RadioButton();
            this.rbMeasureXFixed = new System.Windows.Forms.RadioButton();
            this.rbMeasureFree = new System.Windows.Forms.RadioButton();
            this.gbResolution = new System.Windows.Forms.GroupBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.lblmm = new System.Windows.Forms.Label();
            this.tbResolution = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gbRoiResult = new System.Windows.Forms.GroupBox();
            this.lblRoiDistanceUm = new System.Windows.Forms.Label();
            this.lblRoiDistanceMm = new System.Windows.Forms.Label();
            this.lblRoiDistancePx = new System.Windows.Forms.Label();
            this.lblRoi2Edge = new System.Windows.Forms.Label();
            this.lblRoi1Edge = new System.Windows.Forms.Label();
            this.gbRoi2Setting = new System.Windows.Forms.GroupBox();
            this.cmbRoi2EdgePolarity = new System.Windows.Forms.ComboBox();
            this.cmbRoi2ScanDirection = new System.Windows.Forms.ComboBox();
            this.lblRoi2EdgePolarity = new System.Windows.Forms.Label();
            this.lblRoi2ScanDirection = new System.Windows.Forms.Label();
            this.gbRoi1Setting = new System.Windows.Forms.GroupBox();
            this.cmbRoi1EdgePolarity = new System.Windows.Forms.ComboBox();
            this.cmbRoi1ScanDirection = new System.Windows.Forms.ComboBox();
            this.lblRoi1EdgePolarity = new System.Windows.Forms.Label();
            this.lblRoi1ScanDirection = new System.Windows.Forms.Label();
            this.gbRoiEdgeSetting = new System.Windows.Forms.GroupBox();
            this.lblRoiMinValidPoints = new System.Windows.Forms.Label();
            this.nudRoiMinValidPointsPercent = new System.Windows.Forms.NumericUpDown();
            this.nudRoiScanStep = new System.Windows.Forms.NumericUpDown();
            this.lblRoiScanStep = new System.Windows.Forms.Label();
            this.cmbRoiEdgeSelection = new System.Windows.Forms.ComboBox();
            this.lblRoiEdgeSelection = new System.Windows.Forms.Label();
            this.nudRoiEdgeThresholdPercent = new System.Windows.Forms.NumericUpDown();
            this.lblRoiEdgeThreshold = new System.Windows.Forms.Label();
            this.gbRoiControl = new System.Windows.Forms.GroupBox();
            this.btnRunRoiMeasure = new System.Windows.Forms.Button();
            this.btnClearRoi = new System.Windows.Forms.Button();
            this.btnCreateRoi = new System.Windows.Forms.Button();
            this.gbRoiResolution = new System.Windows.Forms.GroupBox();
            this.lblRoiMmPerPx = new System.Windows.Forms.Label();
            this.tbRoiResolution = new System.Windows.Forms.TextBox();
            this.lblRoiResolution = new System.Windows.Forms.Label();
            this.gbRoiMeasureMode = new System.Windows.Forms.GroupBox();
            this.cmbRoiMeasureType = new System.Windows.Forms.ComboBox();
            this.lblRoiMeasureType = new System.Windows.Forms.Label();
            this.rbRoiMeasureHorizontal = new System.Windows.Forms.RadioButton();
            this.rbRoiMeasureVertical = new System.Windows.Forms.RadioButton();
            this.rbRoiMeasureFree = new System.Windows.Forms.RadioButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.gbHvResult = new System.Windows.Forms.GroupBox();
            this.lblHvVentStatus = new System.Windows.Forms.Label();
            this.lblHvHoleStatus = new System.Windows.Forms.Label();
            this.lblHvDistanceUm = new System.Windows.Forms.Label();
            this.lblHvDistanceMm = new System.Windows.Forms.Label();
            this.lblHvDistancePx = new System.Windows.Forms.Label();
            this.lblHvVentCenter = new System.Windows.Forms.Label();
            this.lblHvHoleCenter = new System.Windows.Forms.Label();
            this.gbViewImage = new System.Windows.Forms.GroupBox();
            this.chkHvShowOverlay = new System.Windows.Forms.CheckBox();
            this.cmbHvViewImage = new System.Windows.Forms.ComboBox();
            this.lblViewImage = new System.Windows.Forms.Label();
            this.gbVentSetting = new System.Windows.Forms.GroupBox();
            this.nudVentThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblVentThreshold = new System.Windows.Forms.Label();
            this.cmbVentThresholdMode = new System.Windows.Forms.ComboBox();
            this.lblVentThresholdMode = new System.Windows.Forms.Label();
            this.gbHoleSetting = new System.Windows.Forms.GroupBox();
            this.cmbHoleCenterMethod = new System.Windows.Forms.ComboBox();
            this.lblHoleCenterMethod = new System.Windows.Forms.Label();
            this.cmbHolePolarity = new System.Windows.Forms.ComboBox();
            this.lblHolePolarity = new System.Windows.Forms.Label();
            this.nudHoleMaxArea = new System.Windows.Forms.NumericUpDown();
            this.lblHoleMaxArea = new System.Windows.Forms.Label();
            this.nudHoleMinArea = new System.Windows.Forms.NumericUpDown();
            this.lblHoleMinArea = new System.Windows.Forms.Label();
            this.nudHoleMorph = new System.Windows.Forms.NumericUpDown();
            this.lblHoleMorph = new System.Windows.Forms.Label();
            this.nudHoleThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblHoleThreshold = new System.Windows.Forms.Label();
            this.gbHvRoiControl = new System.Windows.Forms.GroupBox();
            this.btnClearHvResult = new System.Windows.Forms.Button();
            this.btnRunHvMeasure = new System.Windows.Forms.Button();
            this.btnClearHvRoi = new System.Windows.Forms.Button();
            this.btnCreateHvRoi = new System.Windows.Forms.Button();
            this.gbHvResolution = new System.Windows.Forms.GroupBox();
            this.lblHvMmPerPx = new System.Windows.Forms.Label();
            this.tbHvResolution = new System.Windows.Forms.TextBox();
            this.lblHvResolution = new System.Windows.Forms.Label();
            this.gbHvMeasureMode = new System.Windows.Forms.GroupBox();
            this.rbHvMeasureHorizontal = new System.Windows.Forms.RadioButton();
            this.rbHvMeasureVertical = new System.Windows.Forms.RadioButton();
            this.rbHvMeasureFree = new System.Windows.Forms.RadioButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gbImageInfo.SuspendLayout();
            this.gbMeasure.SuspendLayout();
            this.gbMeasureMode.SuspendLayout();
            this.gbResolution.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.gbRoiResult.SuspendLayout();
            this.gbRoi2Setting.SuspendLayout();
            this.gbRoi1Setting.SuspendLayout();
            this.gbRoiEdgeSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRoiMinValidPointsPercent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRoiScanStep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRoiEdgeThresholdPercent)).BeginInit();
            this.gbRoiControl.SuspendLayout();
            this.gbRoiResolution.SuspendLayout();
            this.gbRoiMeasureMode.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.gbHvResult.SuspendLayout();
            this.gbViewImage.SuspendLayout();
            this.gbVentSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVentThreshold)).BeginInit();
            this.gbHoleSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMaxArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMinArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMorph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleThreshold)).BeginInit();
            this.gbHvRoiControl.SuspendLayout();
            this.gbHvResolution.SuspendLayout();
            this.gbHvMeasureMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlViewer);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(1475, 885);
            this.splitContainer1.SplitterDistance = 983;
            this.splitContainer1.TabIndex = 0;
            // 
            // pnlViewer
            // 
            this.pnlViewer.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pnlViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViewer.Location = new System.Drawing.Point(0, 24);
            this.pnlViewer.Name = "pnlViewer";
            this.pnlViewer.Size = new System.Drawing.Size(983, 861);
            this.pnlViewer.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuPreprocess});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(983, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLoadImage,
            this.menuSave,
            this.menuExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(43, 20);
            this.menuFile.Text = "파일";
            // 
            // menuLoadImage
            // 
            this.menuLoadImage.Name = "menuLoadImage";
            this.menuLoadImage.Size = new System.Drawing.Size(138, 22);
            this.menuLoadImage.Text = "이미지 로드";
            // 
            // menuSave
            // 
            this.menuSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSaveOriginalImage,
            this.menuSaveBinaryImage,
            this.menuSaveCurrentImage,
            this.menuSaveCurrentOverlayImage});
            this.menuSave.Name = "menuSave";
            this.menuSave.Size = new System.Drawing.Size(138, 22);
            this.menuSave.Text = "이미지 저장";
            // 
            // menuSaveOriginalImage
            // 
            this.menuSaveOriginalImage.Name = "menuSaveOriginalImage";
            this.menuSaveOriginalImage.Size = new System.Drawing.Size(150, 22);
            this.menuSaveOriginalImage.Text = "원본 이미지";
            // 
            // menuSaveBinaryImage
            // 
            this.menuSaveBinaryImage.Name = "menuSaveBinaryImage";
            this.menuSaveBinaryImage.Size = new System.Drawing.Size(150, 22);
            this.menuSaveBinaryImage.Text = "이진화 이미지";
            // 
            // menuSaveCurrentImage
            // 
            this.menuSaveCurrentImage.Name = "menuSaveCurrentImage";
            this.menuSaveCurrentImage.Size = new System.Drawing.Size(150, 22);
            this.menuSaveCurrentImage.Text = "뷰 이미지";
            // 
            // menuSaveCurrentOverlayImage
            // 
            this.menuSaveCurrentOverlayImage.Name = "menuSaveCurrentOverlayImage";
            this.menuSaveCurrentOverlayImage.Size = new System.Drawing.Size(150, 22);
            this.menuSaveCurrentOverlayImage.Text = "뷰 오버레이";
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(138, 22);
            this.menuExit.Text = "종료";
            // 
            // menuPreprocess
            // 
            this.menuPreprocess.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBinarySetting,
            this.menuShowOriginal,
            this.menuShowBinary});
            this.menuPreprocess.Name = "menuPreprocess";
            this.menuPreprocess.Size = new System.Drawing.Size(55, 20);
            this.menuPreprocess.Text = "전처리";
            // 
            // menuBinarySetting
            // 
            this.menuBinarySetting.Name = "menuBinarySetting";
            this.menuBinarySetting.Size = new System.Drawing.Size(138, 22);
            this.menuBinarySetting.Text = "이진화";
            // 
            // menuShowOriginal
            // 
            this.menuShowOriginal.Name = "menuShowOriginal";
            this.menuShowOriginal.Size = new System.Drawing.Size(138, 22);
            this.menuShowOriginal.Text = "원본 보기";
            // 
            // menuShowBinary
            // 
            this.menuShowBinary.Name = "menuShowBinary";
            this.menuShowBinary.Size = new System.Drawing.Size(138, 22);
            this.menuShowBinary.Text = "이진화 보기";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tabControl1.Location = new System.Drawing.Point(3, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(482, 870);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.gbImageInfo);
            this.tabPage1.Controls.Add(this.gbMeasure);
            this.tabPage1.Controls.Add(this.gbMeasureMode);
            this.tabPage1.Controls.Add(this.gbResolution);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(474, 845);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Manual Measure";
            // 
            // gbImageInfo
            // 
            this.gbImageInfo.Controls.Add(this.lblImageSize);
            this.gbImageInfo.Controls.Add(this.lblFileName);
            this.gbImageInfo.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gbImageInfo.Location = new System.Drawing.Point(10, 11);
            this.gbImageInfo.Name = "gbImageInfo";
            this.gbImageInfo.Size = new System.Drawing.Size(450, 99);
            this.gbImageInfo.TabIndex = 0;
            this.gbImageInfo.TabStop = false;
            this.gbImageInfo.Text = "Image Info";
            // 
            // lblImageSize
            // 
            this.lblImageSize.AutoSize = true;
            this.lblImageSize.Location = new System.Drawing.Point(27, 63);
            this.lblImageSize.Name = "lblImageSize";
            this.lblImageSize.Size = new System.Drawing.Size(71, 15);
            this.lblImageSize.TabIndex = 1;
            this.lblImageSize.Text = "Image Size";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(27, 27);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(65, 15);
            this.lblFileName.TabIndex = 0;
            this.lblFileName.Text = "File Name";
            // 
            // gbMeasure
            // 
            this.gbMeasure.Controls.Add(this.lblDistanceUm);
            this.gbMeasure.Controls.Add(this.lblDistanceMm);
            this.gbMeasure.Controls.Add(this.lblDistancePx);
            this.gbMeasure.Controls.Add(this.lblPoint2);
            this.gbMeasure.Controls.Add(this.lblPoint1);
            this.gbMeasure.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gbMeasure.Location = new System.Drawing.Point(10, 305);
            this.gbMeasure.Name = "gbMeasure";
            this.gbMeasure.Size = new System.Drawing.Size(450, 282);
            this.gbMeasure.TabIndex = 2;
            this.gbMeasure.TabStop = false;
            this.gbMeasure.Text = "Measure Result";
            // 
            // lblDistanceUm
            // 
            this.lblDistanceUm.AutoSize = true;
            this.lblDistanceUm.Location = new System.Drawing.Point(24, 238);
            this.lblDistanceUm.Name = "lblDistanceUm";
            this.lblDistanceUm.Size = new System.Drawing.Size(79, 15);
            this.lblDistanceUm.TabIndex = 4;
            this.lblDistanceUm.Text = "um Distance";
            // 
            // lblDistanceMm
            // 
            this.lblDistanceMm.AutoSize = true;
            this.lblDistanceMm.Location = new System.Drawing.Point(24, 191);
            this.lblDistanceMm.Name = "lblDistanceMm";
            this.lblDistanceMm.Size = new System.Drawing.Size(83, 15);
            this.lblDistanceMm.TabIndex = 3;
            this.lblDistanceMm.Text = "mm Distance";
            // 
            // lblDistancePx
            // 
            this.lblDistancePx.AutoSize = true;
            this.lblDistancePx.Location = new System.Drawing.Point(24, 141);
            this.lblDistancePx.Name = "lblDistancePx";
            this.lblDistancePx.Size = new System.Drawing.Size(74, 15);
            this.lblDistancePx.TabIndex = 2;
            this.lblDistancePx.Text = "Px Distance";
            // 
            // lblPoint2
            // 
            this.lblPoint2.AutoSize = true;
            this.lblPoint2.Location = new System.Drawing.Point(24, 91);
            this.lblPoint2.Name = "lblPoint2";
            this.lblPoint2.Size = new System.Drawing.Size(43, 15);
            this.lblPoint2.TabIndex = 1;
            this.lblPoint2.Text = "Point2";
            // 
            // lblPoint1
            // 
            this.lblPoint1.AutoSize = true;
            this.lblPoint1.Location = new System.Drawing.Point(24, 42);
            this.lblPoint1.Name = "lblPoint1";
            this.lblPoint1.Size = new System.Drawing.Size(43, 15);
            this.lblPoint1.TabIndex = 0;
            this.lblPoint1.Text = "Point1";
            // 
            // gbMeasureMode
            // 
            this.gbMeasureMode.Controls.Add(this.rbMeasureYFixed);
            this.gbMeasureMode.Controls.Add(this.rbMeasureXFixed);
            this.gbMeasureMode.Controls.Add(this.rbMeasureFree);
            this.gbMeasureMode.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gbMeasureMode.Location = new System.Drawing.Point(10, 125);
            this.gbMeasureMode.Name = "gbMeasureMode";
            this.gbMeasureMode.Size = new System.Drawing.Size(450, 70);
            this.gbMeasureMode.TabIndex = 3;
            this.gbMeasureMode.TabStop = false;
            this.gbMeasureMode.Text = "Measure Mode";
            // 
            // rbMeasureYFixed
            // 
            this.rbMeasureYFixed.AutoSize = true;
            this.rbMeasureYFixed.Location = new System.Drawing.Point(299, 32);
            this.rbMeasureYFixed.Name = "rbMeasureYFixed";
            this.rbMeasureYFixed.Size = new System.Drawing.Size(84, 19);
            this.rbMeasureYFixed.TabIndex = 2;
            this.rbMeasureYFixed.TabStop = true;
            this.rbMeasureYFixed.Text = "Horizontal";
            this.rbMeasureYFixed.UseVisualStyleBackColor = true;
            // 
            // rbMeasureXFixed
            // 
            this.rbMeasureXFixed.AutoSize = true;
            this.rbMeasureXFixed.Location = new System.Drawing.Point(167, 32);
            this.rbMeasureXFixed.Name = "rbMeasureXFixed";
            this.rbMeasureXFixed.Size = new System.Drawing.Size(69, 19);
            this.rbMeasureXFixed.TabIndex = 1;
            this.rbMeasureXFixed.TabStop = true;
            this.rbMeasureXFixed.Text = "Vertical";
            this.rbMeasureXFixed.UseVisualStyleBackColor = true;
            // 
            // rbMeasureFree
            // 
            this.rbMeasureFree.AutoSize = true;
            this.rbMeasureFree.Location = new System.Drawing.Point(47, 32);
            this.rbMeasureFree.Name = "rbMeasureFree";
            this.rbMeasureFree.Size = new System.Drawing.Size(50, 19);
            this.rbMeasureFree.TabIndex = 0;
            this.rbMeasureFree.TabStop = true;
            this.rbMeasureFree.Text = "Free";
            this.rbMeasureFree.UseVisualStyleBackColor = true;
            // 
            // gbResolution
            // 
            this.gbResolution.Controls.Add(this.lblResolution);
            this.gbResolution.Controls.Add(this.lblmm);
            this.gbResolution.Controls.Add(this.tbResolution);
            this.gbResolution.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.gbResolution.Location = new System.Drawing.Point(10, 211);
            this.gbResolution.Name = "gbResolution";
            this.gbResolution.Size = new System.Drawing.Size(450, 79);
            this.gbResolution.TabIndex = 1;
            this.gbResolution.TabStop = false;
            this.gbResolution.Text = "Resolution";
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Location = new System.Drawing.Point(27, 37);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(74, 15);
            this.lblResolution.TabIndex = 2;
            this.lblResolution.Text = "Resolution :";
            // 
            // lblmm
            // 
            this.lblmm.AutoSize = true;
            this.lblmm.Location = new System.Drawing.Point(240, 37);
            this.lblmm.Name = "lblmm";
            this.lblmm.Size = new System.Drawing.Size(48, 15);
            this.lblmm.TabIndex = 1;
            this.lblmm.Text = "mm/px";
            // 
            // tbResolution
            // 
            this.tbResolution.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.tbResolution.Location = new System.Drawing.Point(114, 34);
            this.tbResolution.Name = "tbResolution";
            this.tbResolution.Size = new System.Drawing.Size(111, 23);
            this.tbResolution.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.White;
            this.tabPage2.Controls.Add(this.gbRoiResult);
            this.tabPage2.Controls.Add(this.gbRoi2Setting);
            this.tabPage2.Controls.Add(this.gbRoi1Setting);
            this.tabPage2.Controls.Add(this.gbRoiEdgeSetting);
            this.tabPage2.Controls.Add(this.gbRoiControl);
            this.tabPage2.Controls.Add(this.gbRoiResolution);
            this.tabPage2.Controls.Add(this.gbRoiMeasureMode);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(474, 842);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ROI Edge Measure";
            // 
            // gbRoiResult
            // 
            this.gbRoiResult.Controls.Add(this.lblRoiDistanceUm);
            this.gbRoiResult.Controls.Add(this.lblRoiDistanceMm);
            this.gbRoiResult.Controls.Add(this.lblRoiDistancePx);
            this.gbRoiResult.Controls.Add(this.lblRoi2Edge);
            this.gbRoiResult.Controls.Add(this.lblRoi1Edge);
            this.gbRoiResult.Location = new System.Drawing.Point(16, 596);
            this.gbRoiResult.Name = "gbRoiResult";
            this.gbRoiResult.Size = new System.Drawing.Size(443, 235);
            this.gbRoiResult.TabIndex = 6;
            this.gbRoiResult.TabStop = false;
            this.gbRoiResult.Text = "Result";
            // 
            // lblRoiDistanceUm
            // 
            this.lblRoiDistanceUm.AutoSize = true;
            this.lblRoiDistanceUm.Location = new System.Drawing.Point(25, 199);
            this.lblRoiDistanceUm.Name = "lblRoiDistanceUm";
            this.lblRoiDistanceUm.Size = new System.Drawing.Size(95, 15);
            this.lblRoiDistanceUm.TabIndex = 4;
            this.lblRoiDistanceUm.Text = "Distance um : -";
            // 
            // lblRoiDistanceMm
            // 
            this.lblRoiDistanceMm.AutoSize = true;
            this.lblRoiDistanceMm.Location = new System.Drawing.Point(25, 157);
            this.lblRoiDistanceMm.Name = "lblRoiDistanceMm";
            this.lblRoiDistanceMm.Size = new System.Drawing.Size(99, 15);
            this.lblRoiDistanceMm.TabIndex = 3;
            this.lblRoiDistanceMm.Text = "Distance mm : -";
            // 
            // lblRoiDistancePx
            // 
            this.lblRoiDistancePx.AutoSize = true;
            this.lblRoiDistancePx.Location = new System.Drawing.Point(25, 115);
            this.lblRoiDistancePx.Name = "lblRoiDistancePx";
            this.lblRoiDistancePx.Size = new System.Drawing.Size(91, 15);
            this.lblRoiDistancePx.TabIndex = 2;
            this.lblRoiDistancePx.Text = "Distance px : -";
            // 
            // lblRoi2Edge
            // 
            this.lblRoi2Edge.AutoSize = true;
            this.lblRoi2Edge.Location = new System.Drawing.Point(25, 74);
            this.lblRoi2Edge.Name = "lblRoi2Edge";
            this.lblRoi2Edge.Size = new System.Drawing.Size(84, 15);
            this.lblRoi2Edge.TabIndex = 1;
            this.lblRoi2Edge.Text = "ROI2 Edge : -";
            // 
            // lblRoi1Edge
            // 
            this.lblRoi1Edge.AutoSize = true;
            this.lblRoi1Edge.Location = new System.Drawing.Point(25, 32);
            this.lblRoi1Edge.Name = "lblRoi1Edge";
            this.lblRoi1Edge.Size = new System.Drawing.Size(84, 15);
            this.lblRoi1Edge.TabIndex = 0;
            this.lblRoi1Edge.Text = "ROI1 Edge : -";
            // 
            // gbRoi2Setting
            // 
            this.gbRoi2Setting.Controls.Add(this.cmbRoi2EdgePolarity);
            this.gbRoi2Setting.Controls.Add(this.cmbRoi2ScanDirection);
            this.gbRoi2Setting.Controls.Add(this.lblRoi2EdgePolarity);
            this.gbRoi2Setting.Controls.Add(this.lblRoi2ScanDirection);
            this.gbRoi2Setting.Location = new System.Drawing.Point(14, 519);
            this.gbRoi2Setting.Name = "gbRoi2Setting";
            this.gbRoi2Setting.Size = new System.Drawing.Size(446, 70);
            this.gbRoi2Setting.TabIndex = 5;
            this.gbRoi2Setting.TabStop = false;
            this.gbRoi2Setting.Text = "ROI2 Setting";
            // 
            // cmbRoi2EdgePolarity
            // 
            this.cmbRoi2EdgePolarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoi2EdgePolarity.FormattingEnabled = true;
            this.cmbRoi2EdgePolarity.Items.AddRange(new object[] {
            "W -> B",
            "B -> W",
            "Any"});
            this.cmbRoi2EdgePolarity.Location = new System.Drawing.Point(335, 28);
            this.cmbRoi2EdgePolarity.Name = "cmbRoi2EdgePolarity";
            this.cmbRoi2EdgePolarity.Size = new System.Drawing.Size(72, 23);
            this.cmbRoi2EdgePolarity.TabIndex = 3;
            // 
            // cmbRoi2ScanDirection
            // 
            this.cmbRoi2ScanDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoi2ScanDirection.FormattingEnabled = true;
            this.cmbRoi2ScanDirection.Items.AddRange(new object[] {
            "T -> B",
            "B -> T",
            "L -> R",
            "R -> L"});
            this.cmbRoi2ScanDirection.Location = new System.Drawing.Point(128, 28);
            this.cmbRoi2ScanDirection.Name = "cmbRoi2ScanDirection";
            this.cmbRoi2ScanDirection.Size = new System.Drawing.Size(76, 23);
            this.cmbRoi2ScanDirection.TabIndex = 2;
            // 
            // lblRoi2EdgePolarity
            // 
            this.lblRoi2EdgePolarity.AutoSize = true;
            this.lblRoi2EdgePolarity.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRoi2EdgePolarity.Location = new System.Drawing.Point(230, 33);
            this.lblRoi2EdgePolarity.Name = "lblRoi2EdgePolarity";
            this.lblRoi2EdgePolarity.Size = new System.Drawing.Size(84, 13);
            this.lblRoi2EdgePolarity.TabIndex = 1;
            this.lblRoi2EdgePolarity.Text = "Edge Polarity :";
            // 
            // lblRoi2ScanDirection
            // 
            this.lblRoi2ScanDirection.AutoSize = true;
            this.lblRoi2ScanDirection.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRoi2ScanDirection.Location = new System.Drawing.Point(22, 33);
            this.lblRoi2ScanDirection.Name = "lblRoi2ScanDirection";
            this.lblRoi2ScanDirection.Size = new System.Drawing.Size(89, 13);
            this.lblRoi2ScanDirection.TabIndex = 0;
            this.lblRoi2ScanDirection.Text = "Scan Direction :";
            // 
            // gbRoi1Setting
            // 
            this.gbRoi1Setting.Controls.Add(this.cmbRoi1EdgePolarity);
            this.gbRoi1Setting.Controls.Add(this.cmbRoi1ScanDirection);
            this.gbRoi1Setting.Controls.Add(this.lblRoi1EdgePolarity);
            this.gbRoi1Setting.Controls.Add(this.lblRoi1ScanDirection);
            this.gbRoi1Setting.Location = new System.Drawing.Point(14, 437);
            this.gbRoi1Setting.Name = "gbRoi1Setting";
            this.gbRoi1Setting.Size = new System.Drawing.Size(445, 70);
            this.gbRoi1Setting.TabIndex = 4;
            this.gbRoi1Setting.TabStop = false;
            this.gbRoi1Setting.Text = "ROI1 Setting";
            // 
            // cmbRoi1EdgePolarity
            // 
            this.cmbRoi1EdgePolarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoi1EdgePolarity.FormattingEnabled = true;
            this.cmbRoi1EdgePolarity.Items.AddRange(new object[] {
            "W -> B",
            "B -> W",
            "Any"});
            this.cmbRoi1EdgePolarity.Location = new System.Drawing.Point(335, 28);
            this.cmbRoi1EdgePolarity.Name = "cmbRoi1EdgePolarity";
            this.cmbRoi1EdgePolarity.Size = new System.Drawing.Size(72, 23);
            this.cmbRoi1EdgePolarity.TabIndex = 8;
            // 
            // cmbRoi1ScanDirection
            // 
            this.cmbRoi1ScanDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoi1ScanDirection.FormattingEnabled = true;
            this.cmbRoi1ScanDirection.Items.AddRange(new object[] {
            "T -> B",
            "B -> T",
            "L -> R",
            "R -> L"});
            this.cmbRoi1ScanDirection.Location = new System.Drawing.Point(128, 28);
            this.cmbRoi1ScanDirection.Name = "cmbRoi1ScanDirection";
            this.cmbRoi1ScanDirection.Size = new System.Drawing.Size(76, 23);
            this.cmbRoi1ScanDirection.TabIndex = 7;
            // 
            // lblRoi1EdgePolarity
            // 
            this.lblRoi1EdgePolarity.AutoSize = true;
            this.lblRoi1EdgePolarity.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRoi1EdgePolarity.Location = new System.Drawing.Point(230, 33);
            this.lblRoi1EdgePolarity.Name = "lblRoi1EdgePolarity";
            this.lblRoi1EdgePolarity.Size = new System.Drawing.Size(84, 13);
            this.lblRoi1EdgePolarity.TabIndex = 6;
            this.lblRoi1EdgePolarity.Text = "Edge Polarity :";
            // 
            // lblRoi1ScanDirection
            // 
            this.lblRoi1ScanDirection.AutoSize = true;
            this.lblRoi1ScanDirection.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRoi1ScanDirection.Location = new System.Drawing.Point(22, 33);
            this.lblRoi1ScanDirection.Name = "lblRoi1ScanDirection";
            this.lblRoi1ScanDirection.Size = new System.Drawing.Size(89, 13);
            this.lblRoi1ScanDirection.TabIndex = 5;
            this.lblRoi1ScanDirection.Text = "Scan Direction :";
            // 
            // gbRoiEdgeSetting
            // 
            this.gbRoiEdgeSetting.Controls.Add(this.lblRoiMinValidPoints);
            this.gbRoiEdgeSetting.Controls.Add(this.nudRoiMinValidPointsPercent);
            this.gbRoiEdgeSetting.Controls.Add(this.nudRoiScanStep);
            this.gbRoiEdgeSetting.Controls.Add(this.lblRoiScanStep);
            this.gbRoiEdgeSetting.Controls.Add(this.cmbRoiEdgeSelection);
            this.gbRoiEdgeSetting.Controls.Add(this.lblRoiEdgeSelection);
            this.gbRoiEdgeSetting.Controls.Add(this.nudRoiEdgeThresholdPercent);
            this.gbRoiEdgeSetting.Controls.Add(this.lblRoiEdgeThreshold);
            this.gbRoiEdgeSetting.Location = new System.Drawing.Point(14, 303);
            this.gbRoiEdgeSetting.Name = "gbRoiEdgeSetting";
            this.gbRoiEdgeSetting.Size = new System.Drawing.Size(446, 126);
            this.gbRoiEdgeSetting.TabIndex = 3;
            this.gbRoiEdgeSetting.TabStop = false;
            this.gbRoiEdgeSetting.Text = "Edge Setting";
            // 
            // lblRoiMinValidPoints
            // 
            this.lblRoiMinValidPoints.AutoSize = true;
            this.lblRoiMinValidPoints.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRoiMinValidPoints.Location = new System.Drawing.Point(204, 84);
            this.lblRoiMinValidPoints.Name = "lblRoiMinValidPoints";
            this.lblRoiMinValidPoints.Size = new System.Drawing.Size(124, 13);
            this.lblRoiMinValidPoints.TabIndex = 7;
            this.lblRoiMinValidPoints.Text = "Min Valid Points [%] :";
            // 
            // nudRoiMinValidPointsPercent
            // 
            this.nudRoiMinValidPointsPercent.Location = new System.Drawing.Point(345, 80);
            this.nudRoiMinValidPointsPercent.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRoiMinValidPointsPercent.Name = "nudRoiMinValidPointsPercent";
            this.nudRoiMinValidPointsPercent.Size = new System.Drawing.Size(50, 23);
            this.nudRoiMinValidPointsPercent.TabIndex = 6;
            this.nudRoiMinValidPointsPercent.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // nudRoiScanStep
            // 
            this.nudRoiScanStep.Location = new System.Drawing.Point(135, 80);
            this.nudRoiScanStep.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudRoiScanStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRoiScanStep.Name = "nudRoiScanStep";
            this.nudRoiScanStep.Size = new System.Drawing.Size(50, 23);
            this.nudRoiScanStep.TabIndex = 5;
            this.nudRoiScanStep.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblRoiScanStep
            // 
            this.lblRoiScanStep.AutoSize = true;
            this.lblRoiScanStep.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRoiScanStep.Location = new System.Drawing.Point(35, 84);
            this.lblRoiScanStep.Name = "lblRoiScanStep";
            this.lblRoiScanStep.Size = new System.Drawing.Size(65, 13);
            this.lblRoiScanStep.TabIndex = 4;
            this.lblRoiScanStep.Text = "Scan Step :";
            // 
            // cmbRoiEdgeSelection
            // 
            this.cmbRoiEdgeSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoiEdgeSelection.FormattingEnabled = true;
            this.cmbRoiEdgeSelection.Items.AddRange(new object[] {
            "Strongest",
            "First"});
            this.cmbRoiEdgeSelection.Location = new System.Drawing.Point(301, 32);
            this.cmbRoiEdgeSelection.Name = "cmbRoiEdgeSelection";
            this.cmbRoiEdgeSelection.Size = new System.Drawing.Size(130, 23);
            this.cmbRoiEdgeSelection.TabIndex = 3;
            // 
            // lblRoiEdgeSelection
            // 
            this.lblRoiEdgeSelection.AutoSize = true;
            this.lblRoiEdgeSelection.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRoiEdgeSelection.Location = new System.Drawing.Point(204, 37);
            this.lblRoiEdgeSelection.Name = "lblRoiEdgeSelection";
            this.lblRoiEdgeSelection.Size = new System.Drawing.Size(91, 13);
            this.lblRoiEdgeSelection.TabIndex = 2;
            this.lblRoiEdgeSelection.Text = "Edge Selection :";
            // 
            // nudRoiEdgeThresholdPercent
            // 
            this.nudRoiEdgeThresholdPercent.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.nudRoiEdgeThresholdPercent.Location = new System.Drawing.Point(135, 33);
            this.nudRoiEdgeThresholdPercent.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRoiEdgeThresholdPercent.Name = "nudRoiEdgeThresholdPercent";
            this.nudRoiEdgeThresholdPercent.Size = new System.Drawing.Size(50, 23);
            this.nudRoiEdgeThresholdPercent.TabIndex = 1;
            this.nudRoiEdgeThresholdPercent.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lblRoiEdgeThreshold
            // 
            this.lblRoiEdgeThreshold.AutoSize = true;
            this.lblRoiEdgeThreshold.Font = new System.Drawing.Font("맑은 고딕", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblRoiEdgeThreshold.Location = new System.Drawing.Point(11, 37);
            this.lblRoiEdgeThreshold.Name = "lblRoiEdgeThreshold";
            this.lblRoiEdgeThreshold.Size = new System.Drawing.Size(118, 13);
            this.lblRoiEdgeThreshold.TabIndex = 0;
            this.lblRoiEdgeThreshold.Text = "Edge Threshold [%] :";
            // 
            // gbRoiControl
            // 
            this.gbRoiControl.Controls.Add(this.btnRunRoiMeasure);
            this.gbRoiControl.Controls.Add(this.btnClearRoi);
            this.gbRoiControl.Controls.Add(this.btnCreateRoi);
            this.gbRoiControl.Location = new System.Drawing.Point(12, 216);
            this.gbRoiControl.Name = "gbRoiControl";
            this.gbRoiControl.Size = new System.Drawing.Size(448, 75);
            this.gbRoiControl.TabIndex = 2;
            this.gbRoiControl.TabStop = false;
            this.gbRoiControl.Text = "ROI Control";
            // 
            // btnRunRoiMeasure
            // 
            this.btnRunRoiMeasure.Location = new System.Drawing.Point(308, 30);
            this.btnRunRoiMeasure.Name = "btnRunRoiMeasure";
            this.btnRunRoiMeasure.Size = new System.Drawing.Size(106, 23);
            this.btnRunRoiMeasure.TabIndex = 2;
            this.btnRunRoiMeasure.Text = "Run Measure";
            this.btnRunRoiMeasure.UseVisualStyleBackColor = true;
            // 
            // btnClearRoi
            // 
            this.btnClearRoi.Location = new System.Drawing.Point(167, 30);
            this.btnClearRoi.Name = "btnClearRoi";
            this.btnClearRoi.Size = new System.Drawing.Size(106, 23);
            this.btnClearRoi.TabIndex = 1;
            this.btnClearRoi.Text = "Clear ROI";
            this.btnClearRoi.UseVisualStyleBackColor = true;
            // 
            // btnCreateRoi
            // 
            this.btnCreateRoi.Location = new System.Drawing.Point(29, 30);
            this.btnCreateRoi.Name = "btnCreateRoi";
            this.btnCreateRoi.Size = new System.Drawing.Size(106, 23);
            this.btnCreateRoi.TabIndex = 0;
            this.btnCreateRoi.Text = "Create ROI";
            this.btnCreateRoi.UseVisualStyleBackColor = true;
            // 
            // gbRoiResolution
            // 
            this.gbRoiResolution.Controls.Add(this.lblRoiMmPerPx);
            this.gbRoiResolution.Controls.Add(this.tbRoiResolution);
            this.gbRoiResolution.Controls.Add(this.lblRoiResolution);
            this.gbRoiResolution.Location = new System.Drawing.Point(12, 127);
            this.gbRoiResolution.Name = "gbRoiResolution";
            this.gbRoiResolution.Size = new System.Drawing.Size(448, 78);
            this.gbRoiResolution.TabIndex = 1;
            this.gbRoiResolution.TabStop = false;
            this.gbRoiResolution.Text = "Resolution";
            // 
            // lblRoiMmPerPx
            // 
            this.lblRoiMmPerPx.AutoSize = true;
            this.lblRoiMmPerPx.Location = new System.Drawing.Point(235, 37);
            this.lblRoiMmPerPx.Name = "lblRoiMmPerPx";
            this.lblRoiMmPerPx.Size = new System.Drawing.Size(48, 15);
            this.lblRoiMmPerPx.TabIndex = 2;
            this.lblRoiMmPerPx.Text = "mm/px";
            // 
            // tbRoiResolution
            // 
            this.tbRoiResolution.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.tbRoiResolution.Location = new System.Drawing.Point(120, 34);
            this.tbRoiResolution.Name = "tbRoiResolution";
            this.tbRoiResolution.Size = new System.Drawing.Size(100, 23);
            this.tbRoiResolution.TabIndex = 1;
            // 
            // lblRoiResolution
            // 
            this.lblRoiResolution.AutoSize = true;
            this.lblRoiResolution.Location = new System.Drawing.Point(29, 37);
            this.lblRoiResolution.Name = "lblRoiResolution";
            this.lblRoiResolution.Size = new System.Drawing.Size(74, 15);
            this.lblRoiResolution.TabIndex = 0;
            this.lblRoiResolution.Text = "Resolution :";
            // 
            // gbRoiMeasureMode
            // 
            this.gbRoiMeasureMode.Controls.Add(this.cmbRoiMeasureType);
            this.gbRoiMeasureMode.Controls.Add(this.lblRoiMeasureType);
            this.gbRoiMeasureMode.Controls.Add(this.rbRoiMeasureHorizontal);
            this.gbRoiMeasureMode.Controls.Add(this.rbRoiMeasureVertical);
            this.gbRoiMeasureMode.Controls.Add(this.rbRoiMeasureFree);
            this.gbRoiMeasureMode.Location = new System.Drawing.Point(12, 13);
            this.gbRoiMeasureMode.Name = "gbRoiMeasureMode";
            this.gbRoiMeasureMode.Size = new System.Drawing.Size(448, 108);
            this.gbRoiMeasureMode.TabIndex = 0;
            this.gbRoiMeasureMode.TabStop = false;
            this.gbRoiMeasureMode.Text = "Measure Type / Mode";
            // 
            // cmbRoiMeasureType
            // 
            this.cmbRoiMeasureType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoiMeasureType.FormattingEnabled = true;
            this.cmbRoiMeasureType.Items.AddRange(new object[] {
            "Dual ROI",
            "Single ROI Dual Edge"});
            this.cmbRoiMeasureType.Location = new System.Drawing.Point(137, 33);
            this.cmbRoiMeasureType.Name = "cmbRoiMeasureType";
            this.cmbRoiMeasureType.Size = new System.Drawing.Size(193, 23);
            this.cmbRoiMeasureType.TabIndex = 4;
            // 
            // lblRoiMeasureType
            // 
            this.lblRoiMeasureType.AutoSize = true;
            this.lblRoiMeasureType.Location = new System.Drawing.Point(29, 36);
            this.lblRoiMeasureType.Name = "lblRoiMeasureType";
            this.lblRoiMeasureType.Size = new System.Drawing.Size(97, 15);
            this.lblRoiMeasureType.TabIndex = 3;
            this.lblRoiMeasureType.Text = "Measure Type :";
            // 
            // rbRoiMeasureHorizontal
            // 
            this.rbRoiMeasureHorizontal.AutoSize = true;
            this.rbRoiMeasureHorizontal.Location = new System.Drawing.Point(301, 72);
            this.rbRoiMeasureHorizontal.Name = "rbRoiMeasureHorizontal";
            this.rbRoiMeasureHorizontal.Size = new System.Drawing.Size(84, 19);
            this.rbRoiMeasureHorizontal.TabIndex = 2;
            this.rbRoiMeasureHorizontal.TabStop = true;
            this.rbRoiMeasureHorizontal.Text = "Horizontal";
            this.rbRoiMeasureHorizontal.UseVisualStyleBackColor = true;
            // 
            // rbRoiMeasureVertical
            // 
            this.rbRoiMeasureVertical.AutoSize = true;
            this.rbRoiMeasureVertical.Location = new System.Drawing.Point(170, 72);
            this.rbRoiMeasureVertical.Name = "rbRoiMeasureVertical";
            this.rbRoiMeasureVertical.Size = new System.Drawing.Size(69, 19);
            this.rbRoiMeasureVertical.TabIndex = 1;
            this.rbRoiMeasureVertical.TabStop = true;
            this.rbRoiMeasureVertical.Text = "Vertical";
            this.rbRoiMeasureVertical.UseVisualStyleBackColor = true;
            // 
            // rbRoiMeasureFree
            // 
            this.rbRoiMeasureFree.AutoSize = true;
            this.rbRoiMeasureFree.Location = new System.Drawing.Point(52, 72);
            this.rbRoiMeasureFree.Name = "rbRoiMeasureFree";
            this.rbRoiMeasureFree.Size = new System.Drawing.Size(50, 19);
            this.rbRoiMeasureFree.TabIndex = 0;
            this.rbRoiMeasureFree.TabStop = true;
            this.rbRoiMeasureFree.Text = "Free";
            this.rbRoiMeasureFree.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.White;
            this.tabPage3.Controls.Add(this.gbHvResult);
            this.tabPage3.Controls.Add(this.gbViewImage);
            this.tabPage3.Controls.Add(this.gbVentSetting);
            this.tabPage3.Controls.Add(this.gbHoleSetting);
            this.tabPage3.Controls.Add(this.gbHvRoiControl);
            this.tabPage3.Controls.Add(this.gbHvResolution);
            this.tabPage3.Controls.Add(this.gbHvMeasureMode);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(474, 842);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Hole / Vent Measure";
            // 
            // gbHvResult
            // 
            this.gbHvResult.Controls.Add(this.lblHvVentStatus);
            this.gbHvResult.Controls.Add(this.lblHvHoleStatus);
            this.gbHvResult.Controls.Add(this.lblHvDistanceUm);
            this.gbHvResult.Controls.Add(this.lblHvDistanceMm);
            this.gbHvResult.Controls.Add(this.lblHvDistancePx);
            this.gbHvResult.Controls.Add(this.lblHvVentCenter);
            this.gbHvResult.Controls.Add(this.lblHvHoleCenter);
            this.gbHvResult.Location = new System.Drawing.Point(12, 573);
            this.gbHvResult.Name = "gbHvResult";
            this.gbHvResult.Size = new System.Drawing.Size(445, 262);
            this.gbHvResult.TabIndex = 6;
            this.gbHvResult.TabStop = false;
            this.gbHvResult.Text = "Result";
            // 
            // lblHvVentStatus
            // 
            this.lblHvVentStatus.AutoSize = true;
            this.lblHvVentStatus.Location = new System.Drawing.Point(45, 227);
            this.lblHvVentStatus.Name = "lblHvVentStatus";
            this.lblHvVentStatus.Size = new System.Drawing.Size(50, 15);
            this.lblHvVentStatus.TabIndex = 6;
            this.lblHvVentStatus.Text = "Vent : -";
            // 
            // lblHvHoleStatus
            // 
            this.lblHvHoleStatus.AutoSize = true;
            this.lblHvHoleStatus.Location = new System.Drawing.Point(45, 194);
            this.lblHvHoleStatus.Name = "lblHvHoleStatus";
            this.lblHvHoleStatus.Size = new System.Drawing.Size(49, 15);
            this.lblHvHoleStatus.TabIndex = 5;
            this.lblHvHoleStatus.Text = "Hole : -";
            // 
            // lblHvDistanceUm
            // 
            this.lblHvDistanceUm.AutoSize = true;
            this.lblHvDistanceUm.Location = new System.Drawing.Point(45, 161);
            this.lblHvDistanceUm.Name = "lblHvDistanceUm";
            this.lblHvDistanceUm.Size = new System.Drawing.Size(95, 15);
            this.lblHvDistanceUm.TabIndex = 4;
            this.lblHvDistanceUm.Text = "Distance um : -";
            // 
            // lblHvDistanceMm
            // 
            this.lblHvDistanceMm.AutoSize = true;
            this.lblHvDistanceMm.Location = new System.Drawing.Point(45, 127);
            this.lblHvDistanceMm.Name = "lblHvDistanceMm";
            this.lblHvDistanceMm.Size = new System.Drawing.Size(99, 15);
            this.lblHvDistanceMm.TabIndex = 3;
            this.lblHvDistanceMm.Text = "Distance mm : -";
            // 
            // lblHvDistancePx
            // 
            this.lblHvDistancePx.AutoSize = true;
            this.lblHvDistancePx.Location = new System.Drawing.Point(45, 93);
            this.lblHvDistancePx.Name = "lblHvDistancePx";
            this.lblHvDistancePx.Size = new System.Drawing.Size(91, 15);
            this.lblHvDistancePx.TabIndex = 2;
            this.lblHvDistancePx.Text = "Distance px : -";
            // 
            // lblHvVentCenter
            // 
            this.lblHvVentCenter.AutoSize = true;
            this.lblHvVentCenter.Location = new System.Drawing.Point(45, 60);
            this.lblHvVentCenter.Name = "lblHvVentCenter";
            this.lblHvVentCenter.Size = new System.Drawing.Size(93, 15);
            this.lblHvVentCenter.TabIndex = 1;
            this.lblHvVentCenter.Text = "Vent Center : -";
            // 
            // lblHvHoleCenter
            // 
            this.lblHvHoleCenter.AutoSize = true;
            this.lblHvHoleCenter.Location = new System.Drawing.Point(45, 29);
            this.lblHvHoleCenter.Name = "lblHvHoleCenter";
            this.lblHvHoleCenter.Size = new System.Drawing.Size(92, 15);
            this.lblHvHoleCenter.TabIndex = 0;
            this.lblHvHoleCenter.Text = "Hole Center : -";
            // 
            // gbViewImage
            // 
            this.gbViewImage.Controls.Add(this.chkHvShowOverlay);
            this.gbViewImage.Controls.Add(this.cmbHvViewImage);
            this.gbViewImage.Controls.Add(this.lblViewImage);
            this.gbViewImage.Location = new System.Drawing.Point(12, 508);
            this.gbViewImage.Name = "gbViewImage";
            this.gbViewImage.Size = new System.Drawing.Size(445, 55);
            this.gbViewImage.TabIndex = 5;
            this.gbViewImage.TabStop = false;
            this.gbViewImage.Text = "View";
            // 
            // chkHvShowOverlay
            // 
            this.chkHvShowOverlay.AutoSize = true;
            this.chkHvShowOverlay.Checked = true;
            this.chkHvShowOverlay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHvShowOverlay.Location = new System.Drawing.Point(335, 24);
            this.chkHvShowOverlay.Name = "chkHvShowOverlay";
            this.chkHvShowOverlay.Size = new System.Drawing.Size(104, 19);
            this.chkHvShowOverlay.TabIndex = 2;
            this.chkHvShowOverlay.Text = "Show Overlay";
            this.chkHvShowOverlay.UseVisualStyleBackColor = true;
            // 
            // cmbHvViewImage
            // 
            this.cmbHvViewImage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbHvViewImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHvViewImage.FormattingEnabled = true;
            this.cmbHvViewImage.Items.AddRange(new object[] {
            "Original",
            "Hole Binary",
            "Hole Binary (Detect)",
            "Vent Binary",
            "Vent Binary (Detect)"});
            this.cmbHvViewImage.Location = new System.Drawing.Point(95, 22);
            this.cmbHvViewImage.Name = "cmbHvViewImage";
            this.cmbHvViewImage.Size = new System.Drawing.Size(220, 24);
            this.cmbHvViewImage.TabIndex = 1;
            // 
            // lblViewImage
            // 
            this.lblViewImage.AutoSize = true;
            this.lblViewImage.Location = new System.Drawing.Point(15, 25);
            this.lblViewImage.Name = "lblViewImage";
            this.lblViewImage.Size = new System.Drawing.Size(42, 15);
            this.lblViewImage.TabIndex = 0;
            this.lblViewImage.Text = "View :";
            // 
            // gbVentSetting
            // 
            this.gbVentSetting.Controls.Add(this.nudVentThreshold);
            this.gbVentSetting.Controls.Add(this.lblVentThreshold);
            this.gbVentSetting.Controls.Add(this.cmbVentThresholdMode);
            this.gbVentSetting.Controls.Add(this.lblVentThresholdMode);
            this.gbVentSetting.Location = new System.Drawing.Point(12, 391);
            this.gbVentSetting.Name = "gbVentSetting";
            this.gbVentSetting.Size = new System.Drawing.Size(445, 105);
            this.gbVentSetting.TabIndex = 4;
            this.gbVentSetting.TabStop = false;
            this.gbVentSetting.Text = "Vent Setting";
            // 
            // nudVentThreshold
            // 
            this.nudVentThreshold.Location = new System.Drawing.Point(96, 64);
            this.nudVentThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudVentThreshold.Name = "nudVentThreshold";
            this.nudVentThreshold.Size = new System.Drawing.Size(60, 23);
            this.nudVentThreshold.TabIndex = 1;
            this.nudVentThreshold.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // lblVentThreshold
            // 
            this.lblVentThreshold.AutoSize = true;
            this.lblVentThreshold.Location = new System.Drawing.Point(16, 67);
            this.lblVentThreshold.Name = "lblVentThreshold";
            this.lblVentThreshold.Size = new System.Drawing.Size(71, 15);
            this.lblVentThreshold.TabIndex = 0;
            this.lblVentThreshold.Text = "Threshold :";
            // 
            // cmbVentThresholdMode
            // 
            this.cmbVentThresholdMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVentThresholdMode.FormattingEnabled = true;
            this.cmbVentThresholdMode.Items.AddRange(new object[] {
            "Manual",
            "Otsu",
            "Adaptive"});
            this.cmbVentThresholdMode.Location = new System.Drawing.Point(84, 23);
            this.cmbVentThresholdMode.Name = "cmbVentThresholdMode";
            this.cmbVentThresholdMode.Size = new System.Drawing.Size(110, 23);
            this.cmbVentThresholdMode.TabIndex = 15;
            // 
            // lblVentThresholdMode
            // 
            this.lblVentThresholdMode.AutoSize = true;
            this.lblVentThresholdMode.Location = new System.Drawing.Point(16, 31);
            this.lblVentThresholdMode.Name = "lblVentThresholdMode";
            this.lblVentThresholdMode.Size = new System.Drawing.Size(62, 15);
            this.lblVentThresholdMode.TabIndex = 14;
            this.lblVentThresholdMode.Text = "ThMode :";
            // 
            // gbHoleSetting
            // 
            this.gbHoleSetting.Controls.Add(this.cmbHoleCenterMethod);
            this.gbHoleSetting.Controls.Add(this.lblHoleCenterMethod);
            this.gbHoleSetting.Controls.Add(this.cmbHolePolarity);
            this.gbHoleSetting.Controls.Add(this.lblHolePolarity);
            this.gbHoleSetting.Controls.Add(this.nudHoleMaxArea);
            this.gbHoleSetting.Controls.Add(this.lblHoleMaxArea);
            this.gbHoleSetting.Controls.Add(this.nudHoleMinArea);
            this.gbHoleSetting.Controls.Add(this.lblHoleMinArea);
            this.gbHoleSetting.Controls.Add(this.nudHoleMorph);
            this.gbHoleSetting.Controls.Add(this.lblHoleMorph);
            this.gbHoleSetting.Controls.Add(this.nudHoleThreshold);
            this.gbHoleSetting.Controls.Add(this.lblHoleThreshold);
            this.gbHoleSetting.Location = new System.Drawing.Point(12, 242);
            this.gbHoleSetting.Name = "gbHoleSetting";
            this.gbHoleSetting.Size = new System.Drawing.Size(445, 136);
            this.gbHoleSetting.TabIndex = 3;
            this.gbHoleSetting.TabStop = false;
            this.gbHoleSetting.Text = "Hole Setting";
            // 
            // cmbHoleCenterMethod
            // 
            this.cmbHoleCenterMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHoleCenterMethod.FormattingEnabled = true;
            this.cmbHoleCenterMethod.Items.AddRange(new object[] {
            "MinEnclosingCircle",
            "BoundingRect"});
            this.cmbHoleCenterMethod.Location = new System.Drawing.Point(95, 99);
            this.cmbHoleCenterMethod.Name = "cmbHoleCenterMethod";
            this.cmbHoleCenterMethod.Size = new System.Drawing.Size(150, 23);
            this.cmbHoleCenterMethod.TabIndex = 13;
            // 
            // lblHoleCenterMethod
            // 
            this.lblHoleCenterMethod.AutoSize = true;
            this.lblHoleCenterMethod.Location = new System.Drawing.Point(29, 102);
            this.lblHoleCenterMethod.Name = "lblHoleCenterMethod";
            this.lblHoleCenterMethod.Size = new System.Drawing.Size(53, 15);
            this.lblHoleCenterMethod.TabIndex = 12;
            this.lblHoleCenterMethod.Text = "Center :";
            // 
            // cmbHolePolarity
            // 
            this.cmbHolePolarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHolePolarity.FormattingEnabled = true;
            this.cmbHolePolarity.Items.AddRange(new object[] {
            "Dark Blob",
            "Bright Blob"});
            this.cmbHolePolarity.Location = new System.Drawing.Point(281, 61);
            this.cmbHolePolarity.Name = "cmbHolePolarity";
            this.cmbHolePolarity.Size = new System.Drawing.Size(100, 23);
            this.cmbHolePolarity.TabIndex = 9;
            // 
            // lblHolePolarity
            // 
            this.lblHolePolarity.AutoSize = true;
            this.lblHolePolarity.Location = new System.Drawing.Point(218, 65);
            this.lblHolePolarity.Name = "lblHolePolarity";
            this.lblHolePolarity.Size = new System.Drawing.Size(57, 15);
            this.lblHolePolarity.TabIndex = 8;
            this.lblHolePolarity.Text = "Polarity :";
            // 
            // nudHoleMaxArea
            // 
            this.nudHoleMaxArea.Location = new System.Drawing.Point(95, 62);
            this.nudHoleMaxArea.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudHoleMaxArea.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHoleMaxArea.Name = "nudHoleMaxArea";
            this.nudHoleMaxArea.Size = new System.Drawing.Size(70, 23);
            this.nudHoleMaxArea.TabIndex = 7;
            this.nudHoleMaxArea.Value = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            // 
            // lblHoleMaxArea
            // 
            this.lblHoleMaxArea.AutoSize = true;
            this.lblHoleMaxArea.Location = new System.Drawing.Point(15, 65);
            this.lblHoleMaxArea.Name = "lblHoleMaxArea";
            this.lblHoleMaxArea.Size = new System.Drawing.Size(70, 15);
            this.lblHoleMaxArea.TabIndex = 6;
            this.lblHoleMaxArea.Text = "Max Area :";
            // 
            // nudHoleMinArea
            // 
            this.nudHoleMinArea.Location = new System.Drawing.Point(362, 25);
            this.nudHoleMinArea.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudHoleMinArea.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHoleMinArea.Name = "nudHoleMinArea";
            this.nudHoleMinArea.Size = new System.Drawing.Size(70, 23);
            this.nudHoleMinArea.TabIndex = 5;
            this.nudHoleMinArea.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblHoleMinArea
            // 
            this.lblHoleMinArea.AutoSize = true;
            this.lblHoleMinArea.Location = new System.Drawing.Point(290, 28);
            this.lblHoleMinArea.Name = "lblHoleMinArea";
            this.lblHoleMinArea.Size = new System.Drawing.Size(67, 15);
            this.lblHoleMinArea.TabIndex = 4;
            this.lblHoleMinArea.Text = "Min Area :";
            // 
            // nudHoleMorph
            // 
            this.nudHoleMorph.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudHoleMorph.Location = new System.Drawing.Point(225, 25);
            this.nudHoleMorph.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudHoleMorph.Name = "nudHoleMorph";
            this.nudHoleMorph.Size = new System.Drawing.Size(50, 23);
            this.nudHoleMorph.TabIndex = 3;
            // 
            // lblHoleMorph
            // 
            this.lblHoleMorph.AutoSize = true;
            this.lblHoleMorph.Location = new System.Drawing.Point(170, 28);
            this.lblHoleMorph.Name = "lblHoleMorph";
            this.lblHoleMorph.Size = new System.Drawing.Size(53, 15);
            this.lblHoleMorph.TabIndex = 2;
            this.lblHoleMorph.Text = "Morph :";
            // 
            // nudHoleThreshold
            // 
            this.nudHoleThreshold.Location = new System.Drawing.Point(95, 25);
            this.nudHoleThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudHoleThreshold.Name = "nudHoleThreshold";
            this.nudHoleThreshold.Size = new System.Drawing.Size(60, 23);
            this.nudHoleThreshold.TabIndex = 1;
            this.nudHoleThreshold.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // lblHoleThreshold
            // 
            this.lblHoleThreshold.AutoSize = true;
            this.lblHoleThreshold.Location = new System.Drawing.Point(15, 28);
            this.lblHoleThreshold.Name = "lblHoleThreshold";
            this.lblHoleThreshold.Size = new System.Drawing.Size(71, 15);
            this.lblHoleThreshold.TabIndex = 0;
            this.lblHoleThreshold.Text = "Threshold :";
            // 
            // gbHvRoiControl
            // 
            this.gbHvRoiControl.Controls.Add(this.btnClearHvResult);
            this.gbHvRoiControl.Controls.Add(this.btnRunHvMeasure);
            this.gbHvRoiControl.Controls.Add(this.btnClearHvRoi);
            this.gbHvRoiControl.Controls.Add(this.btnCreateHvRoi);
            this.gbHvRoiControl.Location = new System.Drawing.Point(12, 166);
            this.gbHvRoiControl.Name = "gbHvRoiControl";
            this.gbHvRoiControl.Size = new System.Drawing.Size(445, 60);
            this.gbHvRoiControl.TabIndex = 2;
            this.gbHvRoiControl.TabStop = false;
            this.gbHvRoiControl.Text = "ROI Control";
            // 
            // btnClearHvResult
            // 
            this.btnClearHvResult.Location = new System.Drawing.Point(330, 24);
            this.btnClearHvResult.Name = "btnClearHvResult";
            this.btnClearHvResult.Size = new System.Drawing.Size(95, 23);
            this.btnClearHvResult.TabIndex = 3;
            this.btnClearHvResult.Text = "Clear Result";
            this.btnClearHvResult.UseVisualStyleBackColor = true;
            // 
            // btnRunHvMeasure
            // 
            this.btnRunHvMeasure.Location = new System.Drawing.Point(225, 24);
            this.btnRunHvMeasure.Name = "btnRunHvMeasure";
            this.btnRunHvMeasure.Size = new System.Drawing.Size(95, 23);
            this.btnRunHvMeasure.TabIndex = 2;
            this.btnRunHvMeasure.Text = "Run Measure";
            this.btnRunHvMeasure.UseVisualStyleBackColor = true;
            // 
            // btnClearHvRoi
            // 
            this.btnClearHvRoi.Location = new System.Drawing.Point(120, 24);
            this.btnClearHvRoi.Name = "btnClearHvRoi";
            this.btnClearHvRoi.Size = new System.Drawing.Size(95, 23);
            this.btnClearHvRoi.TabIndex = 1;
            this.btnClearHvRoi.Text = "Clear ROI";
            this.btnClearHvRoi.UseVisualStyleBackColor = true;
            // 
            // btnCreateHvRoi
            // 
            this.btnCreateHvRoi.Location = new System.Drawing.Point(15, 24);
            this.btnCreateHvRoi.Name = "btnCreateHvRoi";
            this.btnCreateHvRoi.Size = new System.Drawing.Size(95, 23);
            this.btnCreateHvRoi.TabIndex = 0;
            this.btnCreateHvRoi.Text = "Create ROI";
            this.btnCreateHvRoi.UseVisualStyleBackColor = true;
            // 
            // gbHvResolution
            // 
            this.gbHvResolution.Controls.Add(this.lblHvMmPerPx);
            this.gbHvResolution.Controls.Add(this.tbHvResolution);
            this.gbHvResolution.Controls.Add(this.lblHvResolution);
            this.gbHvResolution.Location = new System.Drawing.Point(12, 90);
            this.gbHvResolution.Name = "gbHvResolution";
            this.gbHvResolution.Size = new System.Drawing.Size(445, 60);
            this.gbHvResolution.TabIndex = 1;
            this.gbHvResolution.TabStop = false;
            this.gbHvResolution.Text = "Resolution";
            // 
            // lblHvMmPerPx
            // 
            this.lblHvMmPerPx.AutoSize = true;
            this.lblHvMmPerPx.Location = new System.Drawing.Point(233, 26);
            this.lblHvMmPerPx.Name = "lblHvMmPerPx";
            this.lblHvMmPerPx.Size = new System.Drawing.Size(48, 15);
            this.lblHvMmPerPx.TabIndex = 4;
            this.lblHvMmPerPx.Text = "mm/px";
            // 
            // tbHvResolution
            // 
            this.tbHvResolution.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.tbHvResolution.Location = new System.Drawing.Point(120, 23);
            this.tbHvResolution.Name = "tbHvResolution";
            this.tbHvResolution.Size = new System.Drawing.Size(97, 23);
            this.tbHvResolution.TabIndex = 3;
            this.tbHvResolution.Text = "0.001";
            // 
            // lblHvResolution
            // 
            this.lblHvResolution.AutoSize = true;
            this.lblHvResolution.Location = new System.Drawing.Point(29, 26);
            this.lblHvResolution.Name = "lblHvResolution";
            this.lblHvResolution.Size = new System.Drawing.Size(74, 15);
            this.lblHvResolution.TabIndex = 2;
            this.lblHvResolution.Text = "Resolution :";
            // 
            // gbHvMeasureMode
            // 
            this.gbHvMeasureMode.Controls.Add(this.rbHvMeasureHorizontal);
            this.gbHvMeasureMode.Controls.Add(this.rbHvMeasureVertical);
            this.gbHvMeasureMode.Controls.Add(this.rbHvMeasureFree);
            this.gbHvMeasureMode.Location = new System.Drawing.Point(12, 6);
            this.gbHvMeasureMode.Name = "gbHvMeasureMode";
            this.gbHvMeasureMode.Size = new System.Drawing.Size(445, 66);
            this.gbHvMeasureMode.TabIndex = 0;
            this.gbHvMeasureMode.TabStop = false;
            this.gbHvMeasureMode.Text = "Measure Mode";
            // 
            // rbHvMeasureHorizontal
            // 
            this.rbHvMeasureHorizontal.AutoSize = true;
            this.rbHvMeasureHorizontal.Location = new System.Drawing.Point(301, 28);
            this.rbHvMeasureHorizontal.Name = "rbHvMeasureHorizontal";
            this.rbHvMeasureHorizontal.Size = new System.Drawing.Size(84, 19);
            this.rbHvMeasureHorizontal.TabIndex = 2;
            this.rbHvMeasureHorizontal.Text = "Horizontal";
            this.rbHvMeasureHorizontal.UseVisualStyleBackColor = true;
            // 
            // rbHvMeasureVertical
            // 
            this.rbHvMeasureVertical.AutoSize = true;
            this.rbHvMeasureVertical.Location = new System.Drawing.Point(170, 28);
            this.rbHvMeasureVertical.Name = "rbHvMeasureVertical";
            this.rbHvMeasureVertical.Size = new System.Drawing.Size(69, 19);
            this.rbHvMeasureVertical.TabIndex = 1;
            this.rbHvMeasureVertical.Text = "Vertical";
            this.rbHvMeasureVertical.UseVisualStyleBackColor = true;
            // 
            // rbHvMeasureFree
            // 
            this.rbHvMeasureFree.AutoSize = true;
            this.rbHvMeasureFree.Checked = true;
            this.rbHvMeasureFree.Location = new System.Drawing.Point(52, 28);
            this.rbHvMeasureFree.Name = "rbHvMeasureFree";
            this.rbHvMeasureFree.Size = new System.Drawing.Size(50, 19);
            this.rbHvMeasureFree.TabIndex = 0;
            this.rbHvMeasureFree.TabStop = true;
            this.rbHvMeasureFree.Text = "Free";
            this.rbHvMeasureFree.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1475, 885);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.gbImageInfo.ResumeLayout(false);
            this.gbImageInfo.PerformLayout();
            this.gbMeasure.ResumeLayout(false);
            this.gbMeasure.PerformLayout();
            this.gbMeasureMode.ResumeLayout(false);
            this.gbMeasureMode.PerformLayout();
            this.gbResolution.ResumeLayout(false);
            this.gbResolution.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.gbRoiResult.ResumeLayout(false);
            this.gbRoiResult.PerformLayout();
            this.gbRoi2Setting.ResumeLayout(false);
            this.gbRoi2Setting.PerformLayout();
            this.gbRoi1Setting.ResumeLayout(false);
            this.gbRoi1Setting.PerformLayout();
            this.gbRoiEdgeSetting.ResumeLayout(false);
            this.gbRoiEdgeSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRoiMinValidPointsPercent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRoiScanStep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRoiEdgeThresholdPercent)).EndInit();
            this.gbRoiControl.ResumeLayout(false);
            this.gbRoiResolution.ResumeLayout(false);
            this.gbRoiResolution.PerformLayout();
            this.gbRoiMeasureMode.ResumeLayout(false);
            this.gbRoiMeasureMode.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.gbHvResult.ResumeLayout(false);
            this.gbHvResult.PerformLayout();
            this.gbViewImage.ResumeLayout(false);
            this.gbViewImage.PerformLayout();
            this.gbVentSetting.ResumeLayout(false);
            this.gbVentSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVentThreshold)).EndInit();
            this.gbHoleSetting.ResumeLayout(false);
            this.gbHoleSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMaxArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMinArea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleMorph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHoleThreshold)).EndInit();
            this.gbHvRoiControl.ResumeLayout(false);
            this.gbHvResolution.ResumeLayout(false);
            this.gbHvResolution.PerformLayout();
            this.gbHvMeasureMode.ResumeLayout(false);
            this.gbHvMeasureMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbMeasure;
        private System.Windows.Forms.Label lblDistanceUm;
        private System.Windows.Forms.Label lblDistanceMm;
        private System.Windows.Forms.Label lblDistancePx;
        private System.Windows.Forms.Label lblPoint2;
        private System.Windows.Forms.Label lblPoint1;
        private System.Windows.Forms.GroupBox gbResolution;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.Label lblmm;
        private System.Windows.Forms.TextBox tbResolution;
        private System.Windows.Forms.GroupBox gbImageInfo;
        private System.Windows.Forms.Label lblImageSize;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Panel pnlViewer;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuLoadImage;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.GroupBox gbMeasureMode;
        private System.Windows.Forms.RadioButton rbMeasureYFixed;
        private System.Windows.Forms.RadioButton rbMeasureXFixed;
        private System.Windows.Forms.RadioButton rbMeasureFree;
        private System.Windows.Forms.ToolStripMenuItem menuPreprocess;
        private System.Windows.Forms.ToolStripMenuItem menuBinarySetting;
        private System.Windows.Forms.ToolStripMenuItem menuShowOriginal;
        private System.Windows.Forms.ToolStripMenuItem menuShowBinary;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox gbRoiEdgeSetting;
        private System.Windows.Forms.ComboBox cmbRoiEdgeSelection;
        private System.Windows.Forms.Label lblRoiEdgeSelection;
        private System.Windows.Forms.NumericUpDown nudRoiEdgeThresholdPercent;
        private System.Windows.Forms.Label lblRoiEdgeThreshold;
        private System.Windows.Forms.GroupBox gbRoiControl;
        private System.Windows.Forms.Button btnRunRoiMeasure;
        private System.Windows.Forms.Button btnClearRoi;
        private System.Windows.Forms.Button btnCreateRoi;
        private System.Windows.Forms.GroupBox gbRoiResolution;
        private System.Windows.Forms.Label lblRoiMmPerPx;
        private System.Windows.Forms.TextBox tbRoiResolution;
        private System.Windows.Forms.Label lblRoiResolution;
        private System.Windows.Forms.GroupBox gbRoiMeasureMode;
        private System.Windows.Forms.RadioButton rbRoiMeasureHorizontal;
        private System.Windows.Forms.RadioButton rbRoiMeasureVertical;
        private System.Windows.Forms.RadioButton rbRoiMeasureFree;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox gbRoi2Setting;
        private System.Windows.Forms.ComboBox cmbRoi2EdgePolarity;
        private System.Windows.Forms.ComboBox cmbRoi2ScanDirection;
        private System.Windows.Forms.Label lblRoi2EdgePolarity;
        private System.Windows.Forms.Label lblRoi2ScanDirection;
        private System.Windows.Forms.GroupBox gbRoi1Setting;
        private System.Windows.Forms.ComboBox cmbRoi1EdgePolarity;
        private System.Windows.Forms.ComboBox cmbRoi1ScanDirection;
        private System.Windows.Forms.Label lblRoi1EdgePolarity;
        private System.Windows.Forms.Label lblRoi1ScanDirection;
        private System.Windows.Forms.Label lblRoiMinValidPoints;
        private System.Windows.Forms.NumericUpDown nudRoiMinValidPointsPercent;
        private System.Windows.Forms.NumericUpDown nudRoiScanStep;
        private System.Windows.Forms.Label lblRoiScanStep;
        private System.Windows.Forms.GroupBox gbRoiResult;
        private System.Windows.Forms.Label lblRoiDistanceUm;
        private System.Windows.Forms.Label lblRoiDistanceMm;
        private System.Windows.Forms.Label lblRoiDistancePx;
        private System.Windows.Forms.Label lblRoi2Edge;
        private System.Windows.Forms.Label lblRoi1Edge;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripMenuItem menuSaveOriginalImage;
        private System.Windows.Forms.ToolStripMenuItem menuSaveBinaryImage;
        private System.Windows.Forms.ToolStripMenuItem menuSaveCurrentImage;
        private System.Windows.Forms.ToolStripMenuItem menuSaveCurrentOverlayImage;
        private System.Windows.Forms.GroupBox gbVentSetting;
        private System.Windows.Forms.GroupBox gbHoleSetting;
        private System.Windows.Forms.GroupBox gbHvRoiControl;
        private System.Windows.Forms.Button btnRunHvMeasure;
        private System.Windows.Forms.Button btnClearHvRoi;
        private System.Windows.Forms.Button btnCreateHvRoi;
        private System.Windows.Forms.GroupBox gbHvResolution;
        private System.Windows.Forms.Label lblHvMmPerPx;
        private System.Windows.Forms.TextBox tbHvResolution;
        private System.Windows.Forms.Label lblHvResolution;
        private System.Windows.Forms.Button btnClearHvResult;
        private System.Windows.Forms.CheckBox chkHvShowOverlay;

        // Hole Setting (신규)
        private System.Windows.Forms.NumericUpDown nudHoleThreshold;
        private System.Windows.Forms.Label lblHoleThreshold;
        private System.Windows.Forms.NumericUpDown nudHoleMorph;
        private System.Windows.Forms.Label lblHoleMorph;
        private System.Windows.Forms.NumericUpDown nudHoleMinArea;
        private System.Windows.Forms.Label lblHoleMinArea;
        private System.Windows.Forms.NumericUpDown nudHoleMaxArea;
        private System.Windows.Forms.Label lblHoleMaxArea;
        private System.Windows.Forms.ComboBox cmbHolePolarity;
        private System.Windows.Forms.Label lblHolePolarity;
        private System.Windows.Forms.ComboBox cmbHoleCenterMethod;
        private System.Windows.Forms.Label lblHoleCenterMethod;

        // Vent Setting (신규)
        private System.Windows.Forms.NumericUpDown nudVentThreshold;
        private System.Windows.Forms.Label lblVentThreshold;
        private System.Windows.Forms.ComboBox cmbVentThresholdMode;
        private System.Windows.Forms.Label lblVentThresholdMode;

        // View Image (신규 GroupBox + 컨트롤)
        private System.Windows.Forms.GroupBox gbViewImage;
        private System.Windows.Forms.Label lblViewImage;
        private System.Windows.Forms.ComboBox cmbHvViewImage;
        private System.Windows.Forms.GroupBox gbHvMeasureMode;
        private System.Windows.Forms.RadioButton rbHvMeasureHorizontal;
        private System.Windows.Forms.RadioButton rbHvMeasureVertical;
        private System.Windows.Forms.RadioButton rbHvMeasureFree;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox gbHvResult;
        private System.Windows.Forms.Label lblHvVentStatus;
        private System.Windows.Forms.Label lblHvHoleStatus;
        private System.Windows.Forms.Label lblHvDistanceUm;
        private System.Windows.Forms.Label lblHvDistanceMm;
        private System.Windows.Forms.Label lblHvDistancePx;
        private System.Windows.Forms.Label lblHvVentCenter;
        private System.Windows.Forms.Label lblHvHoleCenter;
        private System.Windows.Forms.ComboBox cmbRoiMeasureType;
        private System.Windows.Forms.Label lblRoiMeasureType;
    }
}

