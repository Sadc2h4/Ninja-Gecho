namespace Image_ToukaMan
{
    //-------------------------------------------------------------------------------
    // 画像変換で使用する変換先の形式
    //-------------------------------------------------------------------------------
    public enum ImageConvertFormat
    {
        Png,
        Jpeg,
        Bmp
    }

    //-------------------------------------------------------------------------------
    // 画像変換の変換先形式を選択するダイアログ
    //-------------------------------------------------------------------------------
    public partial class ConvertFormatDialog : Form
    {
        public ConvertFormatDialog(int targetCount)
        {
            InitializeComponent();
            targetCountLabel.Text = $"変換対象: {targetCount} 件";
        }

        //-------------------------------------------------------------------------------
        // 選択されている変換先形式を取得する処理
        //-------------------------------------------------------------------------------
        public ImageConvertFormat SelectedFormat
        {
            get
            {
                if (jpegRadioButton.Checked)
                {
                    return ImageConvertFormat.Jpeg;
                }

                if (bmpRadioButton.Checked)
                {
                    return ImageConvertFormat.Bmp;
                }

                return ImageConvertFormat.Png;
            }
        }
    }
}
