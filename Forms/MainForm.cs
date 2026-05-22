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
    public partial class MainForm : Form
    {
        private Bitmap _loadedImage = null;
        private Bitmap _originalImage = null;
        private Bitmap _binaryImage = null;

        private int _binaryThreshold = 128;
        private bool _binaryInverse = false;

        // Viewer 표시용 변수
        private float _zoom = 1.0f;
        private PointF _panOffset = new PointF(0, 0);

        // Pan 이동용 변수
        private bool _isPanning = false;
        private Point _lastMousePoint;

        // Resolution 관리
        private double _resolutionMmPerPx = 0.001;
        private string _lastValidResolutionText = "0.001";
        private bool _isRestoringResolutionText = false;

        // ROI Resolution 관리
        private double _roiResolutionMmPerPx = 0.001;
        private string _lastValidRoiResolutionText = "0.001";
        private bool _isRestoringRoiResolutionText = false;

        // Hole / Vent Resolution 관리
        private double _hvResolutionMmPerPx = 0.001;
        private string _lastValidHvResolutionText = "0.001";
        private bool _isRestoringHvResolutionText = false;

        // Viewer Hover Info
        private bool _isViewerHoverValid = false;
        private Point _viewerHoverPixel = Point.Empty;

        #region ROI Edge Measure - State

        private Rectangle? _roi1Rect = null;
        private Rectangle? _roi2Rect = null;

        private bool _isRoiEditing = false;
        private int _activeRoiIndex = 0; // 1 or 2
        private RoiHitType _activeRoiHitType = RoiHitType.None;
        private Point _roiEditStartImagePoint = Point.Empty;
        private Rectangle _roiEditStartRect = Rectangle.Empty;

        private const int RoiMinWidth = 5;
        private const int RoiMinHeight = 5;
        private const int RoiHandleSize = 7;

        #endregion

        #region Hole Vent Measure - State

        private Rectangle? _holeRoiRect = null;
        private Rectangle? _ventRoiRect = null;

        private int _selectedHvRoiIndex = 0; // 0=None, 1=Hole, 2=Vent

        private bool _isHvRoiEditing = false;
        private int _activeHvRoiIndex = 0; // 1=Hole, 2=Vent
        private RoiHitType _activeHvRoiHitType = RoiHitType.None;
        private Point _hvRoiEditStartImagePoint = Point.Empty;
        private Rectangle _hvRoiEditStartRect = Rectangle.Empty;

        private PointF? _holeCenter = null;
        private PointF? _ventCenter = null;

        private HoleDetectResult _holeDetectResult = null;
        private VentDetectResult _ventDetectResult = null;

        private PointF? _hvMeasureCalcPoint1 = null;
        private PointF? _hvMeasureCalcPoint2 = null;

        private double _hvDistancePx = 0.0;
        private double _hvDistanceMm = 0.0;
        private double _hvDistanceUm = 0.0;

        // Blob Preview Images
        private Bitmap _holeBinaryPreview = null;
        private Bitmap _holeBinaryDetectPreview = null;
        private Bitmap _ventBinaryPreview = null;
        private Bitmap _ventBinaryDetectPreview = null;

        // View Image Mode
        private HvViewImageMode _hvViewMode = HvViewImageMode.Original;

        #endregion

        #region ROI Edge Measure - Result State

        private RoiEdgeResult _roi1EdgeResult = null;
        private RoiEdgeResult _roi2EdgeResult = null;

        private PointF? _roiMeasurePoint1 = null;
        private PointF? _roiMeasurePoint2 = null;
        private PointF? _roiMeasureCalcPoint1 = null;
        private PointF? _roiMeasureCalcPoint2 = null;

        private double _roiDistancePx = 0.0;
        private double _roiDistanceMm = 0.0;
        private double _roiDistanceUm = 0.0;

        #endregion

        private int _selectedRoiIndex = 0; // 0=None, 1=ROI1, 2=ROI2
        private RoiMeasureType _roiMeasureType = RoiMeasureType.DualRoi;

        // enum / result model class는 Models 폴더로 분리했습니다.
        // - MainFormEnums.cs
        // - EdgeCandidate.cs
        // - RoiEdgeResult.cs
        // - HoleDetectResult.cs
        // - CircleFitResult.cs
        // - VentDetectResult.cs

        private ViewerMode _viewerMode = ViewerMode.None;

        // 사용자가 실제 클릭한 픽셀 좌표
        private Point? _measurePoint1Raw = null;
        private Point? _measurePoint2Raw = null;

        // 우클릭 메뉴
        private ContextMenuStrip _viewerContextMenu = null;
        private ToolStripMenuItem _menuMeasureDistance = null;
        private ToolStripMenuItem _menuClearMeasure = null;
        private ToolStripMenuItem _menuFitImage = null;

        public MainForm()
        {
            InitializeComponent();

            InitViewer();
            InitRoiEdgeControls();
            InitHoleVentControls();
            InitContextMenu();
            InitEvents();
        }

        private void InitContextMenu()
        {
            _viewerContextMenu = new ContextMenuStrip();

            _menuMeasureDistance = new ToolStripMenuItem("치수 측정");
            _menuClearMeasure = new ToolStripMenuItem("측정 초기화");
            _menuFitImage = new ToolStripMenuItem("화면 맞춤");

            _viewerContextMenu.Items.Add(_menuMeasureDistance);
            _viewerContextMenu.Items.Add(_menuClearMeasure);
            _viewerContextMenu.Items.Add(new ToolStripSeparator());
            _viewerContextMenu.Items.Add(_menuFitImage);

            pnlViewer.ContextMenuStrip = _viewerContextMenu;
        }

        private void InitEvents()
        {
            menuLoadImage.Click += MenuLoadImage_Click;
            menuExit.Click += MenuExit_Click;

            // 저장 메뉴 이벤트
            menuSaveOriginalImage.Click += MenuSaveOriginalImage_Click;
            menuSaveBinaryImage.Click += MenuSaveBinaryImage_Click;
            menuSaveCurrentImage.Click += MenuSaveCurrentImage_Click;
            menuSaveCurrentOverlayImage.Click += MenuSaveCurrentOverlayImage_Click;

            // 전처리 메뉴 이벤트
            menuBinarySetting.Click += MenuBinarySetting_Click;
            menuShowOriginal.Click += MenuShowOriginal_Click;
            menuShowBinary.Click += MenuShowBinary_Click;

            pnlViewer.Paint += PnlViewer_Paint;
            pnlViewer.MouseWheel += PnlViewer_MouseWheel;
            pnlViewer.MouseDown += PnlViewer_MouseDown;
            pnlViewer.MouseMove += PnlViewer_MouseMove;
            pnlViewer.MouseUp += PnlViewer_MouseUp;
            pnlViewer.MouseEnter += PnlViewer_MouseEnter;
            pnlViewer.MouseLeave += PnlViewer_MouseLeave;
            pnlViewer.Resize += PnlViewer_Resize;

            _menuMeasureDistance.Click += MenuMeasureDistance_Click;
            _menuClearMeasure.Click += MenuClearMeasure_Click;
            _menuFitImage.Click += MenuFitImage_Click;

            rbMeasureFree.CheckedChanged += MeasureMode_CheckedChanged;
            rbMeasureXFixed.CheckedChanged += MeasureMode_CheckedChanged;
            rbMeasureYFixed.CheckedChanged += MeasureMode_CheckedChanged;

            tbResolution.KeyDown += TbResolution_KeyDown;
            tbResolution.Leave += TbResolution_Leave;

            btnCreateRoi.Click += BtnCreateRoi_Click;
            btnClearRoi.Click += BtnClearRoi_Click;
            btnRunRoiMeasure.Click += BtnRunRoiMeasure_Click;
            btnClearHvResult.Click += BtnClearHvResult_Click;

            // ROI Edge Measure 이벤트
            cmbRoiMeasureType.SelectedIndexChanged += CmbRoiMeasureType_SelectedIndexChanged;

            rbRoiMeasureFree.CheckedChanged += RoiMeasureMode_CheckedChanged;
            rbRoiMeasureVertical.CheckedChanged += RoiMeasureMode_CheckedChanged;
            rbRoiMeasureHorizontal.CheckedChanged += RoiMeasureMode_CheckedChanged;

            tbRoiResolution.KeyDown += TbRoiResolution_KeyDown;
            tbRoiResolution.Leave += TbRoiResolution_Leave;

            // Hole / Vent Measure 이벤트
            btnCreateHvRoi.Click += BtnCreateHvRoi_Click;
            btnClearHvRoi.Click += BtnClearHvRoi_Click;
            btnRunHvMeasure.Click += BtnRunHvMeasure_Click;

            cmbHvViewImage.SelectedIndexChanged += CmbHvViewImage_SelectedIndexChanged;
            cmbHvViewImage.DrawItem += CmbHvViewImage_DrawItem;
            chkHvShowOverlay.CheckedChanged += ChkHvShowOverlay_CheckedChanged;

            cmbHolePolarity.SelectedIndexChanged += HvSettingChanged;
            cmbHoleCenterMethod.SelectedIndexChanged += HvSettingChanged;
            nudHoleThreshold.ValueChanged += NudHoleValueChanged_ForLivePreview;
            nudHoleMorph.ValueChanged += NudHoleValueChanged_ForLivePreview;
            cmbHolePolarity.SelectedIndexChanged += NudHoleValueChanged_ForLivePreview;
            nudVentThreshold.ValueChanged += NudVentValueChanged_ForLivePreview;
            cmbVentThresholdMode.SelectedIndexChanged += HvSettingChanged;
            cmbVentThresholdMode.SelectedIndexChanged += NudVentValueChanged_ForLivePreview;

            rbHvMeasureFree.CheckedChanged += HvMeasureMode_CheckedChanged;
            rbHvMeasureVertical.CheckedChanged += HvMeasureMode_CheckedChanged;
            rbHvMeasureHorizontal.CheckedChanged += HvMeasureMode_CheckedChanged;

            tbHvResolution.KeyDown += TbHvResolution_KeyDown;
            tbHvResolution.Leave += TbHvResolution_Leave;
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            DisposeImages();
            base.OnFormClosed(e);
        }
    }
}