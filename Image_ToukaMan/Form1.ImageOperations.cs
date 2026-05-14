using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Image_ToukaMan
{
    public partial class Form1
    {
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
                Filter = "画像ファイル|*.png;*.bmp;*.jpg;*.jpeg;*.gif;*.tif;*.tiff|すべてのファイル|*.*",
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
                using var bitmap = new Bitmap(filePath);
                LoadBitmap(bitmap, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"画像を読み込めませんでした。\r\n{ex.Message}", "読み込みエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
