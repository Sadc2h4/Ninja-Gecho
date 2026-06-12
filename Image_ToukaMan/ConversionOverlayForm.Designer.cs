namespace Image_ToukaMan
{
    partial class ConversionOverlayForm
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
            closeButton = new Button();
            messageLabel = new Label();
            SuspendLayout();
            //
            // closeButton
            //
            closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            closeButton.BackColor = Color.Black;
            closeButton.Cursor = Cursors.Hand;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.Font = new Font("Yu Gothic UI", 16F, FontStyle.Bold);
            closeButton.ForeColor = Color.White;
            closeButton.Location = new Point(736, 8);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(48, 48);
            closeButton.TabIndex = 0;
            closeButton.TabStop = false;
            closeButton.Text = "×";
            closeButton.UseVisualStyleBackColor = false;
            closeButton.Click += CloseButton_Click;
            //
            // messageLabel
            //
            messageLabel.AllowDrop = true;
            messageLabel.Dock = DockStyle.Fill;
            messageLabel.Font = new Font("Yu Gothic UI", 14F, FontStyle.Bold);
            messageLabel.ForeColor = Color.White;
            messageLabel.Location = new Point(0, 0);
            messageLabel.Name = "messageLabel";
            messageLabel.Size = new Size(792, 458);
            messageLabel.TabIndex = 1;
            messageLabel.Text = "この画面にファイル、またはフォルダをドラッグアンドドロップすると\r\n画像の拡張子変換が可能です";
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messageLabel.DragDrop += ConversionOverlayForm_DragDrop;
            messageLabel.DragEnter += ConversionOverlayForm_DragEnter;
            //
            // ConversionOverlayForm
            //
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(792, 458);
            ControlBox = false;
            Controls.Add(closeButton);
            Controls.Add(messageLabel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ConversionOverlayForm";
            Opacity = 0.75D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "画像変換";
            DragDrop += ConversionOverlayForm_DragDrop;
            DragEnter += ConversionOverlayForm_DragEnter;
            ResumeLayout(false);
        }

        #endregion

        private Button closeButton;
        private Label messageLabel;
    }
}
