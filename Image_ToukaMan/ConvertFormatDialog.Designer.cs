namespace Image_ToukaMan
{
    partial class ConvertFormatDialog
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
            messageLabel = new Label();
            targetCountLabel = new Label();
            pngRadioButton = new RadioButton();
            jpegRadioButton = new RadioButton();
            bmpRadioButton = new RadioButton();
            okButton = new Button();
            cancelButton = new Button();
            SuspendLayout();
            //
            // messageLabel
            //
            messageLabel.AutoSize = true;
            messageLabel.Location = new Point(16, 14);
            messageLabel.Name = "messageLabel";
            messageLabel.Size = new Size(232, 20);
            messageLabel.TabIndex = 0;
            messageLabel.Text = "変換先の形式を選択してください。";
            //
            // targetCountLabel
            //
            targetCountLabel.AutoSize = true;
            targetCountLabel.ForeColor = SystemColors.GrayText;
            targetCountLabel.Location = new Point(16, 40);
            targetCountLabel.Name = "targetCountLabel";
            targetCountLabel.Size = new Size(95, 20);
            targetCountLabel.TabIndex = 1;
            targetCountLabel.Text = "変換対象: 0 件";
            //
            // pngRadioButton
            //
            pngRadioButton.AutoSize = true;
            pngRadioButton.Checked = true;
            pngRadioButton.Location = new Point(24, 76);
            pngRadioButton.Name = "pngRadioButton";
            pngRadioButton.Size = new Size(140, 24);
            pngRadioButton.TabIndex = 2;
            pngRadioButton.TabStop = true;
            pngRadioButton.Text = "PNG（透過を保持）";
            pngRadioButton.UseVisualStyleBackColor = true;
            //
            // jpegRadioButton
            //
            jpegRadioButton.AutoSize = true;
            jpegRadioButton.Location = new Point(24, 106);
            jpegRadioButton.Name = "jpegRadioButton";
            jpegRadioButton.Size = new Size(210, 24);
            jpegRadioButton.TabIndex = 3;
            jpegRadioButton.Text = "JPEG（透過部分は白になります）";
            jpegRadioButton.UseVisualStyleBackColor = true;
            //
            // bmpRadioButton
            //
            bmpRadioButton.AutoSize = true;
            bmpRadioButton.Location = new Point(24, 136);
            bmpRadioButton.Name = "bmpRadioButton";
            bmpRadioButton.Size = new Size(205, 24);
            bmpRadioButton.TabIndex = 4;
            bmpRadioButton.Text = "BMP（透過部分は白になります）";
            bmpRadioButton.UseVisualStyleBackColor = true;
            //
            // okButton
            //
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(176, 182);
            okButton.Name = "okButton";
            okButton.Size = new Size(96, 32);
            okButton.TabIndex = 5;
            okButton.Text = "変換開始";
            okButton.UseVisualStyleBackColor = true;
            //
            // cancelButton
            //
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(282, 182);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(96, 32);
            cancelButton.TabIndex = 6;
            cancelButton.Text = "キャンセル";
            cancelButton.UseVisualStyleBackColor = true;
            //
            // ConvertFormatDialog
            //
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(394, 230);
            Controls.Add(messageLabel);
            Controls.Add(targetCountLabel);
            Controls.Add(pngRadioButton);
            Controls.Add(jpegRadioButton);
            Controls.Add(bmpRadioButton);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConvertFormatDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "画像変換";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label messageLabel;
        private Label targetCountLabel;
        private RadioButton pngRadioButton;
        private RadioButton jpegRadioButton;
        private RadioButton bmpRadioButton;
        private Button okButton;
        private Button cancelButton;
    }
}
