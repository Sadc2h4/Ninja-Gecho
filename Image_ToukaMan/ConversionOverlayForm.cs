namespace Image_ToukaMan
{
    //-------------------------------------------------------------------------------
    // 画像変換の導線を示す半透明スクリーンを表示するフォーム
    //-------------------------------------------------------------------------------
    public partial class ConversionOverlayForm : Form
    {
        private readonly Form ownerForm;
        private readonly Action<string[]> convertAction;

        public ConversionOverlayForm(Form ownerForm, Action<string[]> convertAction)
        {
            this.ownerForm = ownerForm;
            this.convertAction = convertAction;
            InitializeComponent();
        }

        //-------------------------------------------------------------------------------
        // 表示時にオーナーへ重ね、位置・サイズの追従用イベントを購読する処理
        //-------------------------------------------------------------------------------
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateOverlayBounds();
            ownerForm.LocationChanged += OwnerForm_BoundsChanged;
            ownerForm.SizeChanged += OwnerForm_BoundsChanged;
        }

        //-------------------------------------------------------------------------------
        // 閉じる際にオーナーのイベント購読を解除する処理
        //-------------------------------------------------------------------------------
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ownerForm.LocationChanged -= OwnerForm_BoundsChanged;
            ownerForm.SizeChanged -= OwnerForm_BoundsChanged;
            base.OnFormClosed(e);
        }

        private void OwnerForm_BoundsChanged(object? sender, EventArgs e)
        {
            UpdateOverlayBounds();
        }

        //-------------------------------------------------------------------------------
        // オーナーのクライアント領域全面に重なるよう位置とサイズを合わせる処理
        //-------------------------------------------------------------------------------
        private void UpdateOverlayBounds()
        {
            if (ownerForm.WindowState == FormWindowState.Minimized)
            {
                Visible = false;
                return;
            }

            Visible = true;
            Bounds = ownerForm.RectangleToScreen(ownerForm.ClientRectangle);
        }

        private void CloseButton_Click(object? sender, EventArgs e)
        {
            Close();
        }

        //-------------------------------------------------------------------------------
        // Escキーでもスクリーンを閉じられるようにする処理
        //-------------------------------------------------------------------------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        //-------------------------------------------------------------------------------
        // スクリーンへのドラッグを受け付けるか判定する処理
        //-------------------------------------------------------------------------------
        private void ConversionOverlayForm_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files &&
                files.Any(path => Directory.Exists(path) || Form1.IsSupportedImageFile(path)))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        //-------------------------------------------------------------------------------
        // ドロップされたファイル・フォルダを変換処理へ引き渡す処理
        //-------------------------------------------------------------------------------
        private void ConversionOverlayForm_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                convertAction(files);
            }
        }
    }
}
