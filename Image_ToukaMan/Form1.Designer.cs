namespace Image_ToukaMan
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            menuStrip1 = new MenuStrip();
            fileMenuItem = new ToolStripMenuItem();
            openMenuItem = new ToolStripMenuItem();
            saveMenuItem = new ToolStripMenuItem();
            saveAsMenuItem = new ToolStripMenuItem();
            saveCurrentZoomMenuItem = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            exitMenuItem = new ToolStripMenuItem();
            editMenuItem = new ToolStripMenuItem();
            undoMenuItem = new ToolStripMenuItem();
            redoMenuItem = new ToolStripMenuItem();
            toolStripSeparator9 = new ToolStripSeparator();
            copyMenuItem = new ToolStripMenuItem();
            pasteMenuItem = new ToolStripMenuItem();
            toolStripSeparator10 = new ToolStripSeparator();
            fillModeMenuItem = new ToolStripMenuItem();
            protectedFillModeMenuItem = new ToolStripMenuItem();
            rectangleModeMenuItem = new ToolStripMenuItem();
            colorModeMenuItem = new ToolStripMenuItem();
            eraserModeMenuItem = new ToolStripMenuItem();
            toolStripSeparator11 = new ToolStripSeparator();
            finishMenuItem = new ToolStripMenuItem();
            viewMenuItem = new ToolStripMenuItem();
            toolbarVisibleMenuItem = new ToolStripMenuItem();
            statusbarVisibleMenuItem = new ToolStripMenuItem();
            convertMenuItem = new ToolStripMenuItem();
            helpMenuItem = new ToolStripMenuItem();
            usageMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            openButton = new ToolStripButton();
            saveButton = new ToolStripButton();
            pasteButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            undoButton = new ToolStripButton();
            redoButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            fillModeButton = new ToolStripButton();
            protectedFillModeButton = new ToolStripButton();
            rectangleModeButton = new ToolStripButton();
            colorModeButton = new ToolStripButton();
            eraserModeButton = new ToolStripButton();
            finishButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            alphaLabel = new ToolStripLabel();
            alphaInputHost = new ToolStripControlHost(new NumericUpDown());
            alphaTrackHost = new ToolStripControlHost(new TrackBar());
            toolStripSeparator4 = new ToolStripSeparator();
            toleranceLabel = new ToolStripLabel();
            toleranceInputHost = new ToolStripControlHost(new NumericUpDown());
            toleranceTrackHost = new ToolStripControlHost(new TrackBar());
            toolStripSeparator5 = new ToolStripSeparator();
            zoomLabel = new ToolStripLabel();
            zoomComboHost = new ToolStripControlHost(new ComboBox());
            toolStripSeparator6 = new ToolStripSeparator();
            backgroundLabel = new ToolStripLabel();
            backgroundComboHost = new ToolStripControlHost(new ComboBox());
            statusStrip1 = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            modeStatusLabel = new ToolStripStatusLabel();
            canvasPanel = new Panel();
            canvasMenuStrip = new ContextMenuStrip(components);
            contextCopyMenuItem = new ToolStripMenuItem();
            contextFinishMenuItem = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            contextFlipHorizontalMenuItem = new ToolStripMenuItem();
            contextFlipVerticalMenuItem = new ToolStripMenuItem();
            contextRotateRightMenuItem = new ToolStripMenuItem();
            contextRotateLeftMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            canvasMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileMenuItem, editMenuItem, viewMenuItem, convertMenuItem, helpMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1320, 28);
            menuStrip1.TabIndex = 0;
            // 
            // fileMenuItem
            // 
            fileMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openMenuItem, saveAsMenuItem, saveCurrentZoomMenuItem, toolStripSeparator8, exitMenuItem });
            fileMenuItem.Name = "fileMenuItem";
            fileMenuItem.Size = new Size(82, 24);
            fileMenuItem.Text = "ファイル(&F)";
            // 
            // openMenuItem
            // 
            openMenuItem.Name = "openMenuItem";
            openMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openMenuItem.Size = new Size(239, 26);
            openMenuItem.Text = "開く(&O)...";
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveMenuItem.Size = new Size(239, 26);
            saveMenuItem.Text = "保存(&S)";
            // 
            // saveAsMenuItem
            // 
            saveAsMenuItem.Name = "saveAsMenuItem";
            saveAsMenuItem.Size = new Size(239, 26);
            saveAsMenuItem.Text = "名前を付けて保存(&A)...";
            // 
            // saveCurrentZoomMenuItem
            // 
            saveCurrentZoomMenuItem.Name = "saveCurrentZoomMenuItem";
            saveCurrentZoomMenuItem.Size = new Size(239, 26);
            saveCurrentZoomMenuItem.Text = "現在の拡大率で保存";
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(236, 6);
            // 
            // exitMenuItem
            // 
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.Size = new Size(239, 26);
            exitMenuItem.Text = "アプリケーションの終了(&X)";
            // 
            // editMenuItem
            // 
            editMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoMenuItem, redoMenuItem, toolStripSeparator9, copyMenuItem, pasteMenuItem, toolStripSeparator10, fillModeMenuItem, protectedFillModeMenuItem, rectangleModeMenuItem, colorModeMenuItem, eraserModeMenuItem, toolStripSeparator11, finishMenuItem });
            editMenuItem.Name = "editMenuItem";
            editMenuItem.Size = new Size(71, 24);
            editMenuItem.Text = "編集(&E)";
            // 
            // undoMenuItem
            // 
            undoMenuItem.Name = "undoMenuItem";
            undoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoMenuItem.Size = new Size(217, 26);
            undoMenuItem.Text = "元に戻す(&U)";
            // 
            // redoMenuItem
            // 
            redoMenuItem.Name = "redoMenuItem";
            redoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoMenuItem.Size = new Size(217, 26);
            redoMenuItem.Text = "やり直す(&R)";
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(214, 6);
            // 
            // copyMenuItem
            // 
            copyMenuItem.Name = "copyMenuItem";
            copyMenuItem.Size = new Size(217, 26);
            copyMenuItem.Text = "画像をコピー(&C)";
            // 
            // pasteMenuItem
            // 
            pasteMenuItem.Name = "pasteMenuItem";
            pasteMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            pasteMenuItem.Size = new Size(217, 26);
            pasteMenuItem.Text = "貼り付け(&P)";
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(214, 6);
            // 
            // fillModeMenuItem
            // 
            fillModeMenuItem.Name = "fillModeMenuItem";
            fillModeMenuItem.Size = new Size(240, 26);
            fillModeMenuItem.Text = "塗りつぶしモード(&F)";
            // 
            // protectedFillModeMenuItem
            // 
            protectedFillModeMenuItem.Name = "protectedFillModeMenuItem";
            protectedFillModeMenuItem.Size = new Size(240, 26);
            protectedFillModeMenuItem.Text = "主線保護塗りつぶし";
            // 
            // rectangleModeMenuItem
            // 
            rectangleModeMenuItem.Name = "rectangleModeMenuItem";
            rectangleModeMenuItem.Size = new Size(240, 26);
            rectangleModeMenuItem.Text = "四角形モード(&S)";
            // 
            // colorModeMenuItem
            // 
            colorModeMenuItem.Name = "colorModeMenuItem";
            colorModeMenuItem.Size = new Size(240, 26);
            colorModeMenuItem.Text = "色選択モード(&Z)";
            // 
            // eraserModeMenuItem
            // 
            eraserModeMenuItem.Name = "eraserModeMenuItem";
            eraserModeMenuItem.Size = new Size(240, 26);
            eraserModeMenuItem.Text = "消しゴムモード(&E)";
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new Size(237, 6);
            // 
            // finishMenuItem
            // 
            finishMenuItem.Name = "finishMenuItem";
            finishMenuItem.Size = new Size(240, 26);
            finishMenuItem.Text = "フィニッシュ(&H)";
            // 
            // viewMenuItem
            // 
            viewMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolbarVisibleMenuItem, statusbarVisibleMenuItem });
            viewMenuItem.Name = "viewMenuItem";
            viewMenuItem.Size = new Size(72, 24);
            viewMenuItem.Text = "表示(&V)";
            // 
            // toolbarVisibleMenuItem
            // 
            toolbarVisibleMenuItem.Checked = true;
            toolbarVisibleMenuItem.CheckOnClick = true;
            toolbarVisibleMenuItem.CheckState = CheckState.Checked;
            toolbarVisibleMenuItem.Name = "toolbarVisibleMenuItem";
            toolbarVisibleMenuItem.Size = new Size(186, 26);
            toolbarVisibleMenuItem.Text = "ツールバー(&T)";
            // 
            // statusbarVisibleMenuItem
            // 
            statusbarVisibleMenuItem.Checked = true;
            statusbarVisibleMenuItem.CheckOnClick = true;
            statusbarVisibleMenuItem.CheckState = CheckState.Checked;
            statusbarVisibleMenuItem.Name = "statusbarVisibleMenuItem";
            statusbarVisibleMenuItem.Size = new Size(186, 26);
            statusbarVisibleMenuItem.Text = "ステータスバー(&S)";
            //
            // convertMenuItem
            //
            convertMenuItem.Name = "convertMenuItem";
            convertMenuItem.Size = new Size(86, 24);
            convertMenuItem.Text = "画像変換(&C)";
            //
            // helpMenuItem
            //
            helpMenuItem.DropDownItems.AddRange(new ToolStripItem[] { usageMenuItem });
            helpMenuItem.Name = "helpMenuItem";
            helpMenuItem.Size = new Size(79, 24);
            helpMenuItem.Text = "ヘルプ(&H)";
            // 
            // usageMenuItem
            // 
            usageMenuItem.Name = "usageMenuItem";
            usageMenuItem.Size = new Size(134, 26);
            usageMenuItem.Text = "使い方";
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.ImageScalingSize = new Size(20, 20);
            toolStrip1.Items.AddRange(new ToolStripItem[] { openButton, saveButton, pasteButton, toolStripSeparator1, undoButton, redoButton, toolStripSeparator2, fillModeButton, protectedFillModeButton, rectangleModeButton, colorModeButton, eraserModeButton, finishButton, toolStripSeparator3, toleranceLabel, toleranceInputHost, toleranceTrackHost, toolStripSeparator4, alphaLabel, alphaInputHost, alphaTrackHost, toolStripSeparator5, zoomLabel, zoomComboHost, toolStripSeparator6, backgroundLabel, backgroundComboHost });
            toolStrip1.Location = new Point(0, 28);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(6, 2, 6, 2);
            toolStrip1.Size = new Size(1320, 63);
            toolStrip1.TabIndex = 1;
            // 
            // openButton
            // 
            openButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            openButton.Name = "openButton";
            openButton.Size = new Size(30, 56);
            openButton.Text = "開";
            // 
            // saveButton
            // 
            saveButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(30, 56);
            saveButton.Text = "保";
            // 
            // pasteButton
            // 
            pasteButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            pasteButton.Name = "pasteButton";
            pasteButton.Size = new Size(30, 56);
            pasteButton.Text = "貼";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 59);
            // 
            // undoButton
            // 
            undoButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            undoButton.Name = "undoButton";
            undoButton.Size = new Size(30, 56);
            undoButton.Text = "戻";
            // 
            // redoButton
            // 
            redoButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            redoButton.Name = "redoButton";
            redoButton.Size = new Size(30, 56);
            redoButton.Text = "進";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 59);
            // 
            // fillModeButton
            // 
            fillModeButton.CheckOnClick = true;
            fillModeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            fillModeButton.Name = "fillModeButton";
            fillModeButton.Size = new Size(30, 56);
            fillModeButton.Text = "塗";
            // 
            // protectedFillModeButton
            // 
            protectedFillModeButton.CheckOnClick = true;
            protectedFillModeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            protectedFillModeButton.Name = "protectedFillModeButton";
            protectedFillModeButton.Size = new Size(30, 56);
            protectedFillModeButton.Text = "保";
            // 
            // rectangleModeButton
            // 
            rectangleModeButton.CheckOnClick = true;
            rectangleModeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            rectangleModeButton.Name = "rectangleModeButton";
            rectangleModeButton.Size = new Size(30, 56);
            rectangleModeButton.Text = "矩";
            // 
            // colorModeButton
            // 
            colorModeButton.CheckOnClick = true;
            colorModeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            colorModeButton.Name = "colorModeButton";
            colorModeButton.Size = new Size(30, 56);
            colorModeButton.Text = "色";
            // 
            // eraserModeButton
            // 
            eraserModeButton.CheckOnClick = true;
            eraserModeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            eraserModeButton.Name = "eraserModeButton";
            eraserModeButton.Size = new Size(30, 56);
            eraserModeButton.Text = "消";
            // 
            // finishButton
            // 
            finishButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            finishButton.Name = "finishButton";
            finishButton.Size = new Size(30, 56);
            finishButton.Text = "仕";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 59);
            // 
            // alphaLabel
            // 
            alphaLabel.Name = "alphaLabel";
            alphaLabel.Size = new Size(117, 56);
            alphaLabel.Text = "不透明度(0-255)";
            // 
            // alphaInputHost
            // 
            alphaInputHost.Name = "alphaInputHost";
            alphaInputHost.Size = new Size(53, 56);
            alphaInputHost.Text = "0";
            // 
            // alphaTrackHost
            // 
            alphaTrackHost.Name = "alphaTrackHost";
            alphaTrackHost.Size = new Size(104, 56);
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 59);
            // 
            // toleranceLabel
            // 
            toleranceLabel.Name = "toleranceLabel";
            toleranceLabel.Size = new Size(132, 56);
            toleranceLabel.Text = "誤差許容度(0-255)";
            // 
            // toleranceInputHost
            // 
            toleranceInputHost.Name = "toleranceInputHost";
            toleranceInputHost.Size = new Size(53, 56);
            toleranceInputHost.Text = "0";
            // 
            // toleranceTrackHost
            // 
            toleranceTrackHost.Name = "toleranceTrackHost";
            toleranceTrackHost.Size = new Size(104, 56);
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 59);
            // 
            // zoomLabel
            // 
            zoomLabel.Name = "zoomLabel";
            zoomLabel.Size = new Size(69, 56);
            zoomLabel.Text = "拡大表示";
            // 
            // zoomComboHost
            // 
            zoomComboHost.Name = "zoomComboHost";
            zoomComboHost.Size = new Size(121, 56);
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 59);
            // 
            // backgroundLabel
            // 
            backgroundLabel.Name = "backgroundLabel";
            backgroundLabel.Size = new Size(69, 56);
            backgroundLabel.Text = "背景模様";
            // 
            // backgroundComboHost
            // 
            backgroundComboHost.Name = "backgroundComboHost";
            backgroundComboHost.Size = new Size(121, 56);
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { statusLabel, modeStatusLabel });
            statusStrip1.Location = new Point(0, 738);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1320, 22);
            statusStrip1.TabIndex = 2;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(1305, 16);
            statusLabel.Spring = true;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // modeStatusLabel
            // 
            modeStatusLabel.Name = "modeStatusLabel";
            modeStatusLabel.Size = new Size(0, 16);
            // 
            // canvasPanel
            // 
            canvasPanel.AutoScroll = true;
            canvasPanel.BackColor = SystemColors.AppWorkspace;
            canvasPanel.ContextMenuStrip = canvasMenuStrip;
            canvasPanel.Dock = DockStyle.Fill;
            canvasPanel.Location = new Point(0, 91);
            canvasPanel.Name = "canvasPanel";
            canvasPanel.Size = new Size(1320, 647);
            canvasPanel.TabIndex = 3;
            // 
            // canvasMenuStrip
            // 
            canvasMenuStrip.ImageScalingSize = new Size(20, 20);
            canvasMenuStrip.Items.AddRange(new ToolStripItem[] { contextCopyMenuItem, contextFinishMenuItem, toolStripSeparator7, contextFlipHorizontalMenuItem, contextFlipVerticalMenuItem, contextRotateRightMenuItem, contextRotateLeftMenuItem });
            canvasMenuStrip.Name = "canvasMenuStrip";
            canvasMenuStrip.Size = new Size(198, 154);
            // 
            // contextCopyMenuItem
            // 
            contextCopyMenuItem.Name = "contextCopyMenuItem";
            contextCopyMenuItem.Size = new Size(197, 24);
            contextCopyMenuItem.Text = "クリップボードにコピー";
            // 
            // contextFinishMenuItem
            // 
            contextFinishMenuItem.Name = "contextFinishMenuItem";
            contextFinishMenuItem.Size = new Size(197, 24);
            contextFinishMenuItem.Text = "フィニッシュを実施";
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(194, 6);
            // 
            // contextFlipHorizontalMenuItem
            // 
            contextFlipHorizontalMenuItem.Name = "contextFlipHorizontalMenuItem";
            contextFlipHorizontalMenuItem.Size = new Size(197, 24);
            contextFlipHorizontalMenuItem.Text = "左右を反転";
            // 
            // contextFlipVerticalMenuItem
            // 
            contextFlipVerticalMenuItem.Name = "contextFlipVerticalMenuItem";
            contextFlipVerticalMenuItem.Size = new Size(197, 24);
            contextFlipVerticalMenuItem.Text = "上下を反転";
            // 
            // contextRotateRightMenuItem
            // 
            contextRotateRightMenuItem.Name = "contextRotateRightMenuItem";
            contextRotateRightMenuItem.Size = new Size(197, 24);
            contextRotateRightMenuItem.Text = "90° 右に回転";
            // 
            // contextRotateLeftMenuItem
            // 
            contextRotateLeftMenuItem.Name = "contextRotateLeftMenuItem";
            contextRotateLeftMenuItem.Size = new Size(197, 24);
            contextRotateLeftMenuItem.Text = "90° 左に回転";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1320, 760);
            Controls.Add(canvasPanel);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(960, 640);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ninja_Gecho";
            KeyPreview = true;
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            canvasMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileMenuItem;
        private ToolStripMenuItem openMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem saveAsMenuItem;
        private ToolStripMenuItem saveCurrentZoomMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem editMenuItem;
        private ToolStripMenuItem undoMenuItem;
        private ToolStripMenuItem redoMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem copyMenuItem;
        private ToolStripMenuItem pasteMenuItem;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem fillModeMenuItem;
        private ToolStripMenuItem protectedFillModeMenuItem;
        private ToolStripMenuItem rectangleModeMenuItem;
        private ToolStripMenuItem colorModeMenuItem;
        private ToolStripMenuItem eraserModeMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem finishMenuItem;
        private ToolStripMenuItem viewMenuItem;
        private ToolStripMenuItem toolbarVisibleMenuItem;
        private ToolStripMenuItem statusbarVisibleMenuItem;
        private ToolStripMenuItem convertMenuItem;
        private ToolStripMenuItem helpMenuItem;
        private ToolStripMenuItem usageMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton openButton;
        private ToolStripButton saveButton;
        private ToolStripButton pasteButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton undoButton;
        private ToolStripButton redoButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton fillModeButton;
        private ToolStripButton protectedFillModeButton;
        private ToolStripButton rectangleModeButton;
        private ToolStripButton colorModeButton;
        private ToolStripButton eraserModeButton;
        private ToolStripButton finishButton;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripLabel alphaLabel;
        private ToolStripControlHost alphaInputHost;
        private ToolStripControlHost alphaTrackHost;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripLabel toleranceLabel;
        private ToolStripControlHost toleranceInputHost;
        private ToolStripControlHost toleranceTrackHost;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripLabel zoomLabel;
        private ToolStripControlHost zoomComboHost;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripLabel backgroundLabel;
        private ToolStripControlHost backgroundComboHost;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusLabel;
        private ToolStripStatusLabel modeStatusLabel;
        private Panel canvasPanel;
        private ContextMenuStrip canvasMenuStrip;
        private ToolStripMenuItem contextCopyMenuItem;
        private ToolStripMenuItem contextFinishMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem contextFlipHorizontalMenuItem;
        private ToolStripMenuItem contextFlipVerticalMenuItem;
        private ToolStripMenuItem contextRotateRightMenuItem;
        private ToolStripMenuItem contextRotateLeftMenuItem;
    }
}
