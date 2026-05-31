using System.Resources;

namespace Image_ToukaMan
{
    public partial class Form1 : Form
    {
        private const int HistoryLimit = 20;
        private const int EraserSize = 16;
        private const int LineProtectionRadius = 3;

        private readonly List<Bitmap> undoHistory = [];
        private readonly List<Bitmap> redoHistory = [];
        private readonly Dictionary<int, float> zoomTable = new()
        {
            [25] = 0.25f,
            [50] = 0.5f,
            [100] = 1.0f,
            [200] = 2.0f,
            [300] = 3.0f,
            [400] = 4.0f
        };
        private readonly int[] zoomPresetPercents = [25, 50, 100, 200, 300, 400];
        private static readonly ResourceManager AppResourceManager = new("Image_ToukaMan.AppResources", typeof(Form1).Assembly);

        private ImageCanvas canvas = null!;
        private NumericUpDown alphaInput = null!;
        private NumericUpDown toleranceInput = null!;
        private TrackBar alphaTrackBar = null!;
        private TrackBar toleranceTrackBar = null!;
        private ComboBox zoomComboBox = null!;
        private ComboBox backgroundComboBox = null!;

        private Bitmap? currentBitmap;
        private string? currentFilePath;
        private bool isDirty;
        private EditMode currentMode = EditMode.Fill;
        private bool isRectangleDragging;
        private bool isErasing;
        private bool restoreOpaqueOnDrag;
        private Point dragStartPoint;
        private Point dragCurrentPoint;
        private int currentZoomPercent = 100;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeHostedControls();
            InitializeCanvas();
            WireCommands();
            ApplyAppIcon();
            LoadToolbarIcons();
            SetEditMode(EditMode.Fill);
            ApplyBackgroundSelection();
            ApplyZoomSelection();
            UpdateWindowTitle();
            UpdateUiState();
            statusLabel.Text = "画像を開くか貼り付けてください。";
        }

        private void InitializeHostedControls()
        {
            alphaInput = (NumericUpDown)alphaInputHost.Control;
            toleranceInput = (NumericUpDown)toleranceInputHost.Control;
            alphaTrackBar = (TrackBar)alphaTrackHost.Control;
            toleranceTrackBar = (TrackBar)toleranceTrackHost.Control;
            zoomComboBox = (ComboBox)zoomComboHost.Control;
            backgroundComboBox = (ComboBox)backgroundComboHost.Control;

            ConfigureNumberInput(alphaInput, 0);
            ConfigureNumberInput(toleranceInput, 0);
            ConfigureTrackBar(alphaTrackBar, 0);
            ConfigureTrackBar(toleranceTrackBar, 0);
            ConfigureHost(alphaInputHost, 56);
            ConfigureHost(toleranceInputHost, 56);
            ConfigureHost(alphaTrackHost, 130);
            ConfigureHost(toleranceTrackHost, 130);

            zoomComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            zoomComboBox.Width = 88;
            zoomComboBox.Items.AddRange(["25%", "50%", "100%", "200%", "300%", "400%"]);
            zoomComboBox.Text = "100%";
            ConfigureHost(zoomComboHost, 88);

            backgroundComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            backgroundComboBox.Width = 72;
            backgroundComboBox.Items.AddRange(["灰", "緑", "黒", "白"]);
            backgroundComboBox.SelectedItem = "灰";
            ConfigureHost(backgroundComboHost, 72);
        }

        private static void ConfigureNumberInput(NumericUpDown input, int value)
        {
            input.Minimum = 0;
            input.Maximum = 255;
            input.Value = value;
            input.Width = 56;
            input.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void ConfigureTrackBar(TrackBar trackBar, int value)
        {
            trackBar.Minimum = 0;
            trackBar.Maximum = 255;
            trackBar.Value = value;
            trackBar.Width = 120;
            trackBar.TickStyle = TickStyle.None;
            trackBar.AutoSize = false;
            trackBar.Height = 24;
        }

        private static void ConfigureHost(ToolStripControlHost host, int width)
        {
            host.AutoSize = false;
            host.Width = width;
        }

        private void InitializeCanvas()
        {
            canvas = new ImageCanvas
            {
                Location = Point.Empty,
                Margin = Padding.Empty,
                ContextMenuStrip = canvasMenuStrip,
                AllowDrop = true
            };

            AllowDrop = true;
            canvasPanel.AllowDrop = true;
            canvasPanel.Controls.Add(canvas);
        }

        private void WireCommands()
        {
            openMenuItem.Click += (_, _) => OpenImage();
            saveMenuItem.Click += (_, _) => SaveImage();
            saveAsMenuItem.Click += (_, _) => SaveImageAs();
            saveCurrentZoomMenuItem.Click += (_, _) => SaveCurrentZoomImage();
            exitMenuItem.Click += (_, _) => Close();
            undoMenuItem.Click += (_, _) => Undo();
            redoMenuItem.Click += (_, _) => Redo();
            copyMenuItem.Click += (_, _) => CopyImageToClipboard();
            pasteMenuItem.Click += (_, _) => PasteImageFromClipboard();
            fillModeMenuItem.Click += (_, _) => SetEditMode(EditMode.Fill);
            protectedFillModeMenuItem.Click += (_, _) => SetEditMode(EditMode.ProtectedFill);
            rectangleModeMenuItem.Click += (_, _) => SetEditMode(EditMode.Rectangle);
            colorModeMenuItem.Click += (_, _) => SetEditMode(EditMode.ColorSelect);
            eraserModeMenuItem.Click += (_, _) => SetEditMode(EditMode.Eraser);
            finishMenuItem.Click += (_, _) => FinishEdges();
            usageMenuItem.Click += (_, _) => ShowHelpMessage();

            toolbarVisibleMenuItem.CheckedChanged += (_, _) => toolStrip1.Visible = toolbarVisibleMenuItem.Checked;
            statusbarVisibleMenuItem.CheckedChanged += (_, _) => statusStrip1.Visible = statusbarVisibleMenuItem.Checked;

            openButton.Click += (_, _) => OpenImage();
            saveButton.Click += (_, _) => SaveImage();
            pasteButton.Click += (_, _) => PasteImageFromClipboard();
            undoButton.Click += (_, _) => Undo();
            redoButton.Click += (_, _) => Redo();
            fillModeButton.Click += (_, _) => SetEditMode(EditMode.Fill);
            protectedFillModeButton.Click += (_, _) => SetEditMode(EditMode.ProtectedFill);
            rectangleModeButton.Click += (_, _) => SetEditMode(EditMode.Rectangle);
            colorModeButton.Click += (_, _) => SetEditMode(EditMode.ColorSelect);
            eraserModeButton.Click += (_, _) => SetEditMode(EditMode.Eraser);
            finishButton.Click += (_, _) => FinishEdges();

            contextCopyMenuItem.Click += (_, _) => CopyImageToClipboard();
            contextFinishMenuItem.Click += (_, _) => FinishEdges();
            contextFlipHorizontalMenuItem.Click += (_, _) => ApplyTransform(RotateFlipType.RotateNoneFlipX);
            contextFlipVerticalMenuItem.Click += (_, _) => ApplyTransform(RotateFlipType.RotateNoneFlipY);
            contextRotateRightMenuItem.Click += (_, _) => ApplyTransform(RotateFlipType.Rotate90FlipNone);
            contextRotateLeftMenuItem.Click += (_, _) => ApplyTransform(RotateFlipType.Rotate270FlipNone);

            alphaInput.ValueChanged += (_, _) => SyncAlphaFromInput();
            alphaTrackBar.ValueChanged += (_, _) => SyncAlphaFromTrack();
            toleranceInput.ValueChanged += (_, _) => SyncToleranceFromInput();
            toleranceTrackBar.ValueChanged += (_, _) => SyncToleranceFromTrack();
            zoomComboBox.SelectedIndexChanged += (_, _) => ApplyZoomSelection();
            zoomComboBox.Leave += (_, _) => ApplyZoomSelection();
            zoomComboBox.KeyDown += ZoomComboBox_KeyDown;
            backgroundComboBox.SelectedIndexChanged += (_, _) => ApplyBackgroundSelection();

            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            canvas.MouseLeave += (_, _) => UpdateStatusText(null);
            canvas.MouseWheel += Canvas_MouseWheel;
            canvasPanel.MouseWheel += Canvas_MouseWheel;
            toolStrip1.MouseWheel += Canvas_MouseWheel;
            menuStrip1.MouseWheel += Canvas_MouseWheel;
            statusStrip1.MouseWheel += Canvas_MouseWheel;

            canvasMenuStrip.Opening += CanvasMenuStrip_Opening;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;
            canvasPanel.DragEnter += Form1_DragEnter;
            canvasPanel.DragDrop += Form1_DragDrop;
            canvas.DragEnter += Form1_DragEnter;
            canvas.DragDrop += Form1_DragDrop;
            FormClosing += Form1_FormClosing;
            editMenuItem.DropDownOpening += (_, _) => UpdateModeChecks();
        }

        private void LoadToolbarIcons()
        {
            LoadToolbarButtonIcon(openButton, "OpenButtonIcon", "画像を開く");
            LoadToolbarButtonIcon(saveButton, "SaveButtonIcon", "画像を保存");
            LoadToolbarButtonIcon(pasteButton, "PasteButtonIcon", "クリップボードから貼り付け");
            LoadToolbarButtonIcon(undoButton, "UndoButtonIcon", "元に戻す");
            LoadToolbarButtonIcon(redoButton, "RedoButtonIcon", "やり直す");
            LoadToolbarButtonIcon(fillModeButton, "FillButtonIcon", "塗りつぶしモード");
            LoadToolbarButtonIcon(protectedFillModeButton, "ProtectedFillButtonIcon", "主線保護塗りつぶしモード");
            LoadToolbarButtonIcon(rectangleModeButton, "RectangleButtonIcon", "四角形モード");
            LoadToolbarButtonIcon(colorModeButton, "ColorButtonIcon", "色選択モード");
            LoadToolbarButtonIcon(eraserModeButton, "EraserButtonIcon", "消しゴムモード");
            LoadToolbarButtonIcon(finishButton, "FinishButtonIcon", "フィニッシュ");
        }

        private void ZoomComboBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            ApplyZoomSelection();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private static void LoadToolbarButtonIcon(ToolStripButton button, string resourceKey, string toolTip)
        {
            button.AutoSize = false;
            button.Size = new Size(28, 28);
            button.DisplayStyle = ToolStripItemDisplayStyle.None;
            button.Text = string.Empty;
            button.ToolTipText = toolTip;
            button.BackgroundImageLayout = ImageLayout.Zoom;

            if (AppResourceManager.GetObject(resourceKey) is not Bitmap resourceBitmap)
            {
                return;
            }

            button.BackgroundImage = new Bitmap(resourceBitmap);
        }

        private void ApplyAppIcon()
        {
            if (AppResourceManager.GetObject("AppIcon") is not Icon resourceIcon)
            {
                return;
            }

            Icon = (Icon)resourceIcon.Clone();
        }

        private void SyncAlphaFromInput()
        {
            if (alphaTrackBar.Value != (int)alphaInput.Value)
            {
                alphaTrackBar.Value = (int)alphaInput.Value;
            }
        }

        private void SyncAlphaFromTrack()
        {
            if (alphaInput.Value != alphaTrackBar.Value)
            {
                alphaInput.Value = alphaTrackBar.Value;
            }
        }

        private void SyncToleranceFromInput()
        {
            if (toleranceTrackBar.Value != (int)toleranceInput.Value)
            {
                toleranceTrackBar.Value = (int)toleranceInput.Value;
            }
        }

        private void SyncToleranceFromTrack()
        {
            if (toleranceInput.Value != toleranceTrackBar.Value)
            {
                toleranceInput.Value = toleranceTrackBar.Value;
            }
        }

        private void CanvasMenuStrip_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var hasImage = currentBitmap is not null;
            foreach (ToolStripItem item in canvasMenuStrip.Items)
            {
                if (item is ToolStripSeparator)
                {
                    continue;
                }

                item.Enabled = hasImage;
            }
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (ConfirmDiscardChanges())
            {
                e.Cancel = true;
            }
        }

        private void Form1_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files &&
                files.Any(path => Directory.Exists(path) || IsSupportedImageFile(path)))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }

            if (e.Data?.GetDataPresent(DataFormats.Bitmap) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                if (Directory.Exists(files[0]))
                {
                    ConvertDroppedFolderToPng(files[0]);
                    return;
                }

                if (IsSupportedImageFile(files[0]))
                {
                    LoadImageFromFile(files[0]);
                }

                return;
            }

            if (e.Data?.GetData(DataFormats.Bitmap) is Image image)
            {
                using var bitmap = new Bitmap(image);
                LoadBitmap(bitmap, null);
            }
        }

        private void SetEditMode(EditMode mode)
        {
            currentMode = mode;
            UpdateModeChecks();
            modeStatusLabel.Text = $"モード: {GetModeLabel(mode)}";
            canvas.Cursor = Cursors.Cross;
        }

        private void UpdateModeChecks()
        {
            fillModeButton.Checked = currentMode == EditMode.Fill;
            protectedFillModeButton.Checked = currentMode == EditMode.ProtectedFill;
            rectangleModeButton.Checked = currentMode == EditMode.Rectangle;
            colorModeButton.Checked = currentMode == EditMode.ColorSelect;
            eraserModeButton.Checked = currentMode == EditMode.Eraser;
            fillModeMenuItem.Checked = currentMode == EditMode.Fill;
            protectedFillModeMenuItem.Checked = currentMode == EditMode.ProtectedFill;
            rectangleModeMenuItem.Checked = currentMode == EditMode.Rectangle;
            colorModeMenuItem.Checked = currentMode == EditMode.ColorSelect;
            eraserModeMenuItem.Checked = currentMode == EditMode.Eraser;
        }

        private static string GetModeLabel(EditMode mode)
        {
            return mode switch
            {
                EditMode.Fill => "塗りつぶし",
                EditMode.ProtectedFill => "主線保護塗りつぶし",
                EditMode.Rectangle => "四角形",
                EditMode.ColorSelect => "色選択",
                EditMode.Eraser => "消しゴム",
                _ => mode.ToString()
            };
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                PasteImageFromClipboard();
                return true;
            }

            if (keyData == (Keys.Control | Keys.S))
            {
                SaveImage();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearHistory();
            currentBitmap?.Dispose();
            base.OnFormClosed(e);
        }

        private enum EditMode
        {
            Fill,
            ProtectedFill,
            Rectangle,
            ColorSelect,
            Eraser
        }
    }
}
