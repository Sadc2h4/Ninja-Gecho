using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Image_ToukaMan
{
    public partial class Form1
    {
        private static readonly string[] SupportedImageExtensions =
        [
            ".png",
            ".jpg",
            ".jpeg",
            ".jfif",
            ".bmp",
            ".gif",
            ".tif",
            ".tiff",
            ".webp"
        ];

        private void Canvas_MouseDown(object? sender, MouseEventArgs e)
        {
            if (currentBitmap is null || e.Button != MouseButtons.Left)
            {
                return;
            }

            var imagePoint = canvas.ClientToImage(e.Location);
            if (!IsInsideImage(imagePoint))
            {
                return;
            }

            if (currentMode == EditMode.Rectangle)
            {
                isRectangleDragging = true;
                dragStartPoint = imagePoint;
                dragCurrentPoint = imagePoint;
                canvas.SelectionRectangle = CreateRectangle(dragStartPoint, dragCurrentPoint);
                return;
            }

            if (currentMode == EditMode.Eraser)
            {
                BeginImageChange();
                isErasing = true;
                restoreOpaqueOnDrag = ModifierKeys.HasFlag(Keys.Shift);
                ApplyBrush(imagePoint, restoreOpaqueOnDrag ? 255 : SelectedAlpha);
                return;
            }

            ExecutePointAction(imagePoint, ModifierKeys.HasFlag(Keys.Shift));
        }

        private void Canvas_MouseMove(object? sender, MouseEventArgs e)
        {
            var imagePoint = canvas.ClientToImage(e.Location);
            UpdateStatusText(imagePoint);

            if (currentBitmap is null)
            {
                return;
            }

            if (currentMode == EditMode.Rectangle && isRectangleDragging)
            {
                dragCurrentPoint = ClampToImage(imagePoint);
                canvas.SelectionRectangle = CreateRectangle(dragStartPoint, dragCurrentPoint);
                return;
            }

            if (currentMode == EditMode.Eraser && isErasing && e.Button == MouseButtons.Left && IsInsideImage(imagePoint))
            {
                ApplyBrush(imagePoint, restoreOpaqueOnDrag ? 255 : SelectedAlpha);
            }
        }

        private void Canvas_MouseUp(object? sender, MouseEventArgs e)
        {
            if (currentBitmap is null)
            {
                return;
            }

            if (currentMode == EditMode.Rectangle && isRectangleDragging)
            {
                isRectangleDragging = false;
                dragCurrentPoint = ClampToImage(canvas.ClientToImage(e.Location));
                var rect = CreateRectangle(dragStartPoint, dragCurrentPoint);
                canvas.SelectionRectangle = Rectangle.Empty;
                if (rect.Width > 0 && rect.Height > 0)
                {
                    ApplyRectangle(rect, ModifierKeys.HasFlag(Keys.Shift) ? 255 : SelectedAlpha);
                }
            }

            if (currentMode == EditMode.Eraser)
            {
                isErasing = false;
            }
        }

        private void Canvas_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (!ModifierKeys.HasFlag(Keys.Control))
            {
                return;
            }

            var nextZoom = e.Delta > 0 ? currentZoomPercent + 20 : currentZoomPercent - 20;
            SetZoomPercent(Math.Clamp(nextZoom, 20, 400), false);
        }

        private void ExecutePointAction(Point imagePoint, bool restoreOpaque)
        {
            switch (currentMode)
            {
                case EditMode.Fill:
                    FloodFill(imagePoint, restoreOpaque ? 255 : SelectedAlpha);
                    break;
                case EditMode.ProtectedFill:
                    ProtectedFloodFill(imagePoint, restoreOpaque ? 255 : SelectedAlpha);
                    break;
                case EditMode.ColorSelect:
                    ApplyColorSelection(imagePoint, restoreOpaque ? 255 : SelectedAlpha);
                    break;
            }
        }

        private void OpenImage()
        {
            if (ConfirmDiscardChanges())
            {
                return;
            }

            using var dialog = new OpenFileDialog
            {
                Filter = "画像ファイル|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff;*.webp|すべてのファイル|*.*",
                Title = "画像を開く"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                LoadImageFromFile(dialog.FileName);
            }
        }

        private void LoadImageFromFile(string filePath)
        {
            try
            {
                using var bitmap = LoadBitmapFromFile(filePath);
                LoadBitmap(bitmap, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"画像を読み込めませんでした。\r\n{ex.Message}", "読み込みエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //-------------------------------------------------------------------------------
        // 指定された画像ファイルを編集用のBitmapとして読み込む処理
        //-------------------------------------------------------------------------------
        private static Bitmap LoadBitmapFromFile(string filePath)
        {
            if (!IsSupportedImageFile(filePath))
            {
                throw new NotSupportedException("対応していない画像形式です。");
            }

            try
            {
                using var source = new Bitmap(filePath);
                return CloneBitmap(source);
            }
            catch when (ShouldTryWindowsImageCodec(filePath))
            {
                return LoadBitmapWithWindowsImageCodec(filePath);
            }
        }

        //-------------------------------------------------------------------------------
        // 対応している画像拡張子かどうかを判定する処理
        //-------------------------------------------------------------------------------
        internal static bool IsSupportedImageFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            return SupportedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        //-------------------------------------------------------------------------------
        // System.Drawingで読み込めない場合にWindows画像コーデックを試すか判定する処理
        //-------------------------------------------------------------------------------
        private static bool ShouldTryWindowsImageCodec(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            return SupportedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        //-------------------------------------------------------------------------------
        // Windows画像コーデックを使って画像ファイルをBitmapとして読み込む処理
        //-------------------------------------------------------------------------------
        private static Bitmap LoadBitmapWithWindowsImageCodec(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(
                stream,
                System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat,
                System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);

            var frame = decoder.Frames[0];
            var converted = new System.Windows.Media.Imaging.FormatConvertedBitmap();
            converted.BeginInit();
            converted.Source = frame;
            converted.DestinationFormat = System.Windows.Media.PixelFormats.Bgra32;
            converted.EndInit();
            converted.Freeze();

            var width = converted.PixelWidth;
            var height = converted.PixelHeight;
            var stride = width * 4;
            var pixels = new byte[stride * height];
            converted.CopyPixels(pixels, stride, 0);

            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            if (converted.DpiX > 0 && converted.DpiY > 0)
            {
                bitmap.SetResolution((float)converted.DpiX, (float)converted.DpiY);
            }

            var rect = new Rectangle(0, 0, width, height);
            var data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try
            {
                Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

            return bitmap;
        }

        private void LoadBitmap(Bitmap bitmap, string? filePath)
        {
            ClearHistory();
            currentBitmap?.Dispose();
            currentBitmap = CloneBitmap(bitmap);
            currentFilePath = filePath;
            isDirty = false;
            RefreshCanvas(false);
        }

        private void SaveImage()
        {
            if (currentBitmap is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(currentFilePath) || !string.Equals(Path.GetExtension(currentFilePath), ".png", StringComparison.OrdinalIgnoreCase))
            {
                SaveImageAs();
                return;
            }

            SaveToFile(currentFilePath);
        }

        private void SaveImageAs()
        {
            if (currentBitmap is null)
            {
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Filter = "PNG ファイル|*.png",
                DefaultExt = "png",
                AddExtension = true,
                FileName = string.IsNullOrWhiteSpace(currentFilePath) ? "transparent.png" : $"{Path.GetFileNameWithoutExtension(currentFilePath)}.png"
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveToFile(dialog.FileName);
            }
        }

        //-------------------------------------------------------------------------------
        // 現在の表示倍率でリサイズした画像をPNG保存する処理
        //-------------------------------------------------------------------------------
        private void SaveCurrentZoomImage()
        {
            if (currentBitmap is null)
            {
                return;
            }

            using var dialog = new SaveFileDialog
            {
                Filter = "PNG ファイル|*.png",
                DefaultExt = "png",
                AddExtension = true,
                FileName = CreateZoomSaveFileName()
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                using var scaledBitmap = CreateScaledBitmap(currentBitmap, canvas.Zoom);
                scaledBitmap.Save(dialog.FileName, ImageFormat.Png);
                statusLabel.Text = $"現在の拡大率({currentZoomPercent}%)で保存しました: {Path.GetFileName(dialog.FileName)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"保存に失敗しました。\r\n{ex.Message}", "保存エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveToFile(string filePath)
        {
            if (currentBitmap is null)
            {
                return;
            }

            try
            {
                currentBitmap.Save(filePath, ImageFormat.Png);
                currentFilePath = filePath;
                isDirty = false;
                UpdateWindowTitle();
                UpdateUiState();
                statusLabel.Text = $"保存しました: {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"保存に失敗しました。\r\n{ex.Message}", "保存エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //-------------------------------------------------------------------------------
        // ドロップされたファイル・フォルダ内の対応画像を選択した形式へ一括変換する処理
        //-------------------------------------------------------------------------------
        private void ConvertDroppedPaths(string[] droppedPaths)
        {
            var imageFiles = CollectConversionTargets(droppedPaths);

            if (imageFiles.Length == 0)
            {
                MessageBox.Show(this, "変換対象の画像が見つかりませんでした。", "画像変換", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ImageConvertFormat format;
            using (var dialog = new ConvertFormatDialog(imageFiles.Length))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                format = dialog.SelectedFormat;
            }

            var formatLabel = GetFormatLabel(format);
            var sameFormatExtensions = GetFormatExtensions(format);
            var targets = imageFiles
                .Where(target => !sameFormatExtensions.Contains(Path.GetExtension(target.FilePath), StringComparer.OrdinalIgnoreCase))
                .ToArray();

            if (targets.Length == 0)
            {
                MessageBox.Show(this, $"すべての画像が既に {formatLabel} 形式のため、変換対象はありませんでした。", "画像変換", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var outputFolderName = $"変換後_{formatLabel}";
            var saveExtension = GetFormatSaveExtension(format);
            var convertedCount = 0;
            var failedFiles = new List<string>();

            foreach (var (filePath, baseDirectory) in targets)
            {
                try
                {
                    var outputDirectory = Path.Combine(baseDirectory, outputFolderName);
                    Directory.CreateDirectory(outputDirectory);

                    using var bitmap = LoadBitmapFromFile(filePath);
                    SaveBitmapAsFormat(bitmap, CreateAvailableSavePath(filePath, outputDirectory, saveExtension), format);
                    convertedCount++;
                }
                catch
                {
                    failedFiles.Add(Path.GetFileName(filePath));
                }
            }

            statusLabel.Text = $"画像変換が完了しました: {convertedCount} 件";
            CloseConversionOverlay();

            if (failedFiles.Count == 0)
            {
                MessageBox.Show(this, $"{convertedCount} 件の画像を {formatLabel} に変換しました。\r\n保存先: {outputFolderName} フォルダ", "画像変換", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show(
                this,
                $"{convertedCount} 件の画像を {formatLabel} に変換しました。\r\n保存先: {outputFolderName} フォルダ\r\n失敗: {failedFiles.Count} 件\r\n\r\n{string.Join("\r\n", failedFiles.Take(10))}",
                "画像変換",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        //-------------------------------------------------------------------------------
        // ドロップされたパスから変換対象の画像と出力先の基準フォルダを集める処理
        //-------------------------------------------------------------------------------
        private static (string FilePath, string BaseDirectory)[] CollectConversionTargets(string[] droppedPaths)
        {
            var targets = new List<(string FilePath, string BaseDirectory)>();
            var seenFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var path in droppedPaths)
            {
                if (Directory.Exists(path))
                {
                    foreach (var file in Directory.EnumerateFiles(path).Where(IsSupportedImageFile))
                    {
                        if (seenFiles.Add(file))
                        {
                            targets.Add((file, path));
                        }
                    }

                    continue;
                }

                if (File.Exists(path) && IsSupportedImageFile(path) && seenFiles.Add(path))
                {
                    targets.Add((path, Path.GetDirectoryName(path) ?? string.Empty));
                }
            }

            return targets.OrderBy(target => target.FilePath, StringComparer.CurrentCultureIgnoreCase).ToArray();
        }

        //-------------------------------------------------------------------------------
        // 変換形式に対応する表示名を取得する処理
        //-------------------------------------------------------------------------------
        private static string GetFormatLabel(ImageConvertFormat format)
        {
            return format switch
            {
                ImageConvertFormat.Png => "PNG",
                ImageConvertFormat.Jpeg => "JPEG",
                ImageConvertFormat.Bmp => "BMP",
                _ => format.ToString()
            };
        }

        //-------------------------------------------------------------------------------
        // 変換形式と同一形式とみなす拡張子の一覧を取得する処理
        //-------------------------------------------------------------------------------
        private static string[] GetFormatExtensions(ImageConvertFormat format)
        {
            return format switch
            {
                ImageConvertFormat.Png => [".png"],
                ImageConvertFormat.Jpeg => [".jpg", ".jpeg", ".jfif"],
                ImageConvertFormat.Bmp => [".bmp"],
                _ => []
            };
        }

        //-------------------------------------------------------------------------------
        // 変換形式の保存時に使用する拡張子を取得する処理
        //-------------------------------------------------------------------------------
        private static string GetFormatSaveExtension(ImageConvertFormat format)
        {
            return format switch
            {
                ImageConvertFormat.Png => ".png",
                ImageConvertFormat.Jpeg => ".jpg",
                ImageConvertFormat.Bmp => ".bmp",
                _ => ".png"
            };
        }

        //-------------------------------------------------------------------------------
        // 指定形式でBitmapをファイルへ保存する処理（JPEG/BMPは透過部分を白で合成）
        //-------------------------------------------------------------------------------
        private static void SaveBitmapAsFormat(Bitmap bitmap, string filePath, ImageConvertFormat format)
        {
            switch (format)
            {
                case ImageConvertFormat.Jpeg:
                {
                    using var opaqueBitmap = CreateOpaqueBitmap(bitmap);
                    SaveJpeg(opaqueBitmap, filePath);
                    break;
                }
                case ImageConvertFormat.Bmp:
                {
                    using var opaqueBitmap = CreateOpaqueBitmap(bitmap);
                    opaqueBitmap.Save(filePath, ImageFormat.Bmp);
                    break;
                }
                default:
                    bitmap.Save(filePath, ImageFormat.Png);
                    break;
            }
        }

        //-------------------------------------------------------------------------------
        // 透過部分を白背景に合成した不透明Bitmapを作成する処理
        //-------------------------------------------------------------------------------
        private static Bitmap CreateOpaqueBitmap(Bitmap source)
        {
            var result = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
            result.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using var graphics = Graphics.FromImage(result);
            graphics.Clear(Color.White);
            graphics.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height));
            return result;
        }

        //-------------------------------------------------------------------------------
        // 品質指定付きでJPEG保存する処理
        //-------------------------------------------------------------------------------
        private static void SaveJpeg(Bitmap bitmap, string filePath)
        {
            var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            if (encoder is null)
            {
                bitmap.Save(filePath, ImageFormat.Jpeg);
                return;
            }

            using var parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, 90L);
            bitmap.Save(filePath, encoder, parameters);
        }

        //-------------------------------------------------------------------------------
        // 既存ファイルと重複しない保存先パスを作成する処理
        //-------------------------------------------------------------------------------
        private static string CreateAvailableSavePath(string sourcePath, string outputDirectory, string extension)
        {
            var baseName = Path.GetFileNameWithoutExtension(sourcePath);
            var basePath = Path.Combine(outputDirectory, $"{baseName}{extension}");
            if (!File.Exists(basePath))
            {
                return basePath;
            }

            for (var index = 1; ; index++)
            {
                var candidate = Path.Combine(outputDirectory, $"{baseName}_{index}{extension}");
                if (!File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        //-------------------------------------------------------------------------------
        // 現在の表示倍率保存に使うファイル名を作成する処理
        //-------------------------------------------------------------------------------
        private string CreateZoomSaveFileName()
        {
            var baseName = string.IsNullOrWhiteSpace(currentFilePath) ? "transparent" : Path.GetFileNameWithoutExtension(currentFilePath);
            return $"{baseName}_{currentZoomPercent}percent.png";
        }

        //-------------------------------------------------------------------------------
        // ピクセルの境界を保ったまま指定倍率のBitmapを作成する処理
        //-------------------------------------------------------------------------------
        private static Bitmap CreateScaledBitmap(Bitmap source, float zoom)
        {
            var width = Math.Max(1, (int)Math.Round(source.Width * zoom));
            var height = Math.Max(1, (int)Math.Round(source.Height * zoom));
            var scaledBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            scaledBitmap.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            var sourceRect = new Rectangle(0, 0, source.Width, source.Height);
            var targetRect = new Rectangle(0, 0, width, height);
            var sourceData = source.LockBits(sourceRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var targetData = scaledBitmap.LockBits(targetRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                unsafe
                {
                    var sourceBase = (byte*)sourceData.Scan0;
                    var targetBase = (byte*)targetData.Scan0;

                    for (var y = 0; y < height; y++)
                    {
                        var sourceY = Math.Min(source.Height - 1, (int)(y / zoom));
                        var sourceRow = sourceBase + (sourceY * sourceData.Stride);
                        var targetRow = targetBase + (y * targetData.Stride);

                        for (var x = 0; x < width; x++)
                        {
                            var sourceX = Math.Min(source.Width - 1, (int)(x / zoom));
                            var sourceOffset = sourceX * 4;
                            var targetOffset = x * 4;
                            targetRow[targetOffset] = sourceRow[sourceOffset];
                            targetRow[targetOffset + 1] = sourceRow[sourceOffset + 1];
                            targetRow[targetOffset + 2] = sourceRow[sourceOffset + 2];
                            targetRow[targetOffset + 3] = sourceRow[sourceOffset + 3];
                        }
                    }
                }
            }
            finally
            {
                source.UnlockBits(sourceData);
                scaledBitmap.UnlockBits(targetData);
            }

            return scaledBitmap;
        }

        private void CopyImageToClipboard()
        {
            if (currentBitmap is null)
            {
                return;
            }

            CopyBitmapWithTransparencyToClipboard(currentBitmap);
            statusLabel.Text = "現在の画像をクリップボードにコピーしました。";
            UpdateUiState();
        }

        private void PasteImageFromClipboard()
        {
            if (!Clipboard.ContainsImage() && Clipboard.GetDataObject()?.GetDataPresent("PNG") != true)
            {
                MessageBox.Show(this, "クリップボードに画像がありません。", "貼り付け", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (ConfirmDiscardChanges())
            {
                return;
            }

            using var bitmap = GetBitmapFromClipboardPreservingTransparency();
            if (bitmap is null)
            {
                return;
            }

            LoadBitmap(bitmap, null);
        }

        private void Undo()
        {
            if (currentBitmap is null || undoHistory.Count == 0)
            {
                return;
            }

            redoHistory.Add(CloneBitmap(currentBitmap));
            if (redoHistory.Count > HistoryLimit)
            {
                redoHistory[0].Dispose();
                redoHistory.RemoveAt(0);
            }

            currentBitmap.Dispose();
            currentBitmap = undoHistory[^1];
            undoHistory.RemoveAt(undoHistory.Count - 1);
            isDirty = true;
            RefreshCanvas();
        }

        private void Redo()
        {
            if (currentBitmap is null || redoHistory.Count == 0)
            {
                return;
            }

            undoHistory.Add(CloneBitmap(currentBitmap));
            if (undoHistory.Count > HistoryLimit)
            {
                undoHistory[0].Dispose();
                undoHistory.RemoveAt(0);
            }

            currentBitmap.Dispose();
            currentBitmap = redoHistory[^1];
            redoHistory.RemoveAt(redoHistory.Count - 1);
            isDirty = true;
            RefreshCanvas();
        }

        private void BeginImageChange()
        {
            if (currentBitmap is null)
            {
                return;
            }

            undoHistory.Add(CloneBitmap(currentBitmap));
            if (undoHistory.Count > HistoryLimit)
            {
                undoHistory[0].Dispose();
                undoHistory.RemoveAt(0);
            }

            foreach (var redo in redoHistory)
            {
                redo.Dispose();
            }

            redoHistory.Clear();
            isDirty = true;
            UpdateWindowTitle();
            UpdateUiState();
        }
    }
}
