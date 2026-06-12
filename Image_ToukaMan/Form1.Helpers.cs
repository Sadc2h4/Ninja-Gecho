using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Image_ToukaMan
{
    public partial class Form1
    {
        //-------------------------------------------------------------------------------
        // クリックした点と同じ色がつながっている範囲を透過する処理
        //-------------------------------------------------------------------------------
        private void FloodFill(Point startPoint, int targetAlpha)
        {
            if (currentBitmap is null)
            {
                return;
            }

            if (ShouldIgnoreTransparentClick(startPoint, targetAlpha))
            {
                statusLabel.Text = "透明部分のクリックは透過対象にしません。";
                return;
            }

            BeginImageChange();
            EditBitmapPixels(currentBitmap, pixels =>
            {
                var sourceColor = pixels.GetColor(startPoint.X, startPoint.Y);
                var visited = new bool[pixels.Width * pixels.Height];
                var queue = new Queue<Point>();
                queue.Enqueue(startPoint);

                while (queue.Count > 0)
                {
                    var point = queue.Dequeue();
                    if (point.X < 0 || point.Y < 0 || point.X >= pixels.Width || point.Y >= pixels.Height)
                    {
                        continue;
                    }

                    var index = (point.Y * pixels.Width) + point.X;
                    if (visited[index])
                    {
                        continue;
                    }

                    visited[index] = true;
                    if (!pixels.IsColorMatch(point.X, point.Y, sourceColor, Tolerance))
                    {
                        continue;
                    }

                    pixels.SetAlpha(point.X, point.Y, targetAlpha);
                    queue.Enqueue(new Point(point.X - 1, point.Y));
                    queue.Enqueue(new Point(point.X + 1, point.Y));
                    queue.Enqueue(new Point(point.X, point.Y - 1));
                    queue.Enqueue(new Point(point.X, point.Y + 1));
                }
            });

            RefreshCanvas();
        }

        //-------------------------------------------------------------------------------
        // 主線の線抜けを保護しながらクリック範囲を透過する処理
        //-------------------------------------------------------------------------------
        private void ProtectedFloodFill(Point startPoint, int targetAlpha)
        {
            if (currentBitmap is null)
            {
                return;
            }

            if (ShouldIgnoreTransparentClick(startPoint, targetAlpha))
            {
                statusLabel.Text = "透明部分のクリックは透過対象にしません。";
                return;
            }

            BeginImageChange();
            EditBitmapPixels(currentBitmap, pixels =>
            {
                var sourceColor = pixels.GetColor(startPoint.X, startPoint.Y);
                var protectedLineMask = CreateProtectedLineMask(pixels, sourceColor, Tolerance);
                var visited = new bool[pixels.Width * pixels.Height];
                var queue = new Queue<Point>();
                queue.Enqueue(startPoint);

                while (queue.Count > 0)
                {
                    var point = queue.Dequeue();
                    if (point.X < 0 || point.Y < 0 || point.X >= pixels.Width || point.Y >= pixels.Height)
                    {
                        continue;
                    }

                    var index = (point.Y * pixels.Width) + point.X;
                    if (visited[index] || protectedLineMask[index])
                    {
                        continue;
                    }

                    visited[index] = true;
                    if (!pixels.IsColorMatch(point.X, point.Y, sourceColor, Tolerance))
                    {
                        continue;
                    }

                    pixels.SetAlpha(point.X, point.Y, targetAlpha);
                    queue.Enqueue(new Point(point.X - 1, point.Y));
                    queue.Enqueue(new Point(point.X + 1, point.Y));
                    queue.Enqueue(new Point(point.X, point.Y - 1));
                    queue.Enqueue(new Point(point.X, point.Y + 1));
                }
            });

            RefreshCanvas();
        }

        //-------------------------------------------------------------------------------
        // クリックした点と同じ色の全ピクセルを透過する処理
        //-------------------------------------------------------------------------------
        private void ApplyColorSelection(Point point, int targetAlpha)
        {
            if (currentBitmap is null)
            {
                return;
            }

            if (ShouldIgnoreTransparentClick(point, targetAlpha))
            {
                statusLabel.Text = "透明部分のクリックは透過対象にしません。";
                return;
            }

            BeginImageChange();
            EditBitmapPixels(currentBitmap, pixels =>
            {
                var sourceColor = pixels.GetColor(point.X, point.Y);
                for (var y = 0; y < pixels.Height; y++)
                {
                    for (var x = 0; x < pixels.Width; x++)
                    {
                        if (pixels.IsColorMatch(x, y, sourceColor, Tolerance))
                        {
                            pixels.SetAlpha(x, y, targetAlpha);
                        }
                    }
                }
            });

            RefreshCanvas();
        }

        //-------------------------------------------------------------------------------
        // 指定した四角形の範囲を透過する処理
        //-------------------------------------------------------------------------------
        private void ApplyRectangle(Rectangle rect, int targetAlpha)
        {
            if (currentBitmap is null)
            {
                return;
            }

            BeginImageChange();
            EditBitmapPixels(currentBitmap, pixels =>
            {
                var clipped = Rectangle.Intersect(rect, new Rectangle(0, 0, pixels.Width, pixels.Height));
                for (var y = clipped.Top; y < clipped.Bottom; y++)
                {
                    for (var x = clipped.Left; x < clipped.Right; x++)
                    {
                        pixels.SetAlpha(x, y, targetAlpha);
                    }
                }
            });

            RefreshCanvas();
        }

        //-------------------------------------------------------------------------------
        // 消しゴムでこすった部分を透過する処理
        //-------------------------------------------------------------------------------
        private void ApplyBrush(Point point, int targetAlpha)
        {
            if (currentBitmap is null)
            {
                return;
            }

            EditBitmapPixels(currentBitmap, pixels =>
            {
                var half = EraserSize / 2;
                var left = Math.Max(0, point.X - half);
                var top = Math.Max(0, point.Y - half);
                var right = Math.Min(pixels.Width, left + EraserSize);
                var bottom = Math.Min(pixels.Height, top + EraserSize);

                for (var y = top; y < bottom; y++)
                {
                    for (var x = left; x < right; x++)
                    {
                        pixels.SetAlpha(x, y, targetAlpha);
                    }
                }
            });

            RefreshCanvas();
        }

        //-------------------------------------------------------------------------------
        // 透明部分のふちに残った半端なピクセルをなじませる処理
        //-------------------------------------------------------------------------------
        private void FinishEdges()
        {
            if (currentBitmap is null)
            {
                return;
            }

            BeginImageChange();
            using var source = CloneBitmap(currentBitmap);
            EditBitmapPixels(currentBitmap, targetPixels =>
            {
                ReadBitmapPixels(source, sourcePixels =>
                {
                    for (var y = 1; y < sourcePixels.Height - 1; y++)
                    {
                        for (var x = 1; x < sourcePixels.Width - 1; x++)
                        {
                            var center = sourcePixels.GetColor(x, y);
                            if (center.A == 0 || !HasTransparentNeighbor(sourcePixels, x, y))
                            {
                                continue;
                            }

                            var alpha = EstimateEdgeAlpha(sourcePixels, x, y);
                            targetPixels.SetAlpha(x, y, alpha);
                        }
                    }
                });
            });

            RefreshCanvas();
        }

        private void ApplyTransform(RotateFlipType type)
        {
            if (currentBitmap is null)
            {
                return;
            }

            BeginImageChange();
            currentBitmap.RotateFlip(type);
            RefreshCanvas();
        }

        private void ApplyZoomSelection()
        {
            SetZoomPercent(GetSelectedZoomPercent(), true);
        }

        private void ApplyBackgroundSelection()
        {
            canvas.AccentColor = backgroundComboBox.SelectedItem?.ToString() switch
            {
                "緑" => Color.FromArgb(52, 102, 64),
                "黒" => Color.FromArgb(42, 42, 42),
                "白" => Color.FromArgb(170, 170, 170),
                _ => Color.FromArgb(96, 96, 96)
            };
            UpdateCanvasPanelBackground();
            canvas.Invalidate();
        }

        private int GetSelectedZoomPercent()
        {
            var text = zoomComboBox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return currentZoomPercent;
            }

            return int.TryParse(text.Replace("%", string.Empty).Trim(), out var value) ? value : currentZoomPercent;
        }

        private void SetZoomPercent(int zoomPercent, bool snapToPreset)
        {
            var normalized = Math.Clamp(zoomPercent, 20, 400);
            currentZoomPercent = normalized;

            float zoom;
            if (snapToPreset && zoomTable.TryGetValue(normalized, out var presetZoom))
            {
                zoom = presetZoom;
            }
            else
            {
                zoom = normalized / 100f;
            }

            canvas.Zoom = zoom;
            zoomComboBox.Text = $"{normalized}%";
            UpdateCanvasSize();
        }

        private void RefreshCanvas(bool preserveScroll = true)
        {
            void Refresh()
            {
                canvas.Image = currentBitmap;
                canvas.SelectionRectangle = Rectangle.Empty;
                UpdateCanvasSize();
                UpdateWindowTitle();
                UpdateUiState();
                canvas.Invalidate();
            }

            if (preserveScroll)
            {
                PreserveCanvasScroll(Refresh);
                return;
            }

            Refresh();
            canvasPanel.AutoScrollPosition = Point.Empty;
        }

        private void UpdateCanvasSize()
        {
            if (currentBitmap is null)
            {
                if (canvas.Size != new Size(1, 1))
                {
                    canvas.Size = new Size(1, 1);
                }

                return;
            }

            var nextSize = new Size(
                Math.Max(1, (int)Math.Round(currentBitmap.Width * canvas.Zoom)),
                Math.Max(1, (int)Math.Round(currentBitmap.Height * canvas.Zoom)));

            if (canvas.Size != nextSize)
            {
                canvas.Size = nextSize;
            }
        }

        private void UpdateWindowTitle()
        {
            var name = string.IsNullOrWhiteSpace(currentFilePath) ? "無題" : Path.GetFileName(currentFilePath);
            Text = $"{name}{(isDirty ? "*" : string.Empty)} - Ninja_Gecho";
        }

        private void UpdateUiState()
        {
            var hasImage = currentBitmap is not null;
            var hasClipboardImage = Clipboard.ContainsImage() || Clipboard.GetDataObject()?.GetDataPresent("PNG") == true;
            saveMenuItem.Enabled = hasImage;
            saveAsMenuItem.Enabled = hasImage;
            copyMenuItem.Enabled = hasImage;
            finishMenuItem.Enabled = hasImage;
            saveCurrentZoomMenuItem.Enabled = hasImage;
            undoMenuItem.Enabled = undoHistory.Count > 0;
            redoMenuItem.Enabled = redoHistory.Count > 0;
            pasteMenuItem.Enabled = hasClipboardImage;
            saveButton.Enabled = hasImage;
            pasteButton.Enabled = hasClipboardImage;
            undoButton.Enabled = undoHistory.Count > 0;
            redoButton.Enabled = redoHistory.Count > 0;
            finishButton.Enabled = hasImage;
            fillModeButton.Enabled = hasImage;
            protectedFillModeButton.Enabled = hasImage;
            rectangleModeButton.Enabled = hasImage;
            colorModeButton.Enabled = hasImage;
            eraserModeButton.Enabled = hasImage;
        }

        private void UpdateCanvasPanelBackground()
        {
            canvasPanel.BackgroundImage?.Dispose();
            canvasPanel.BackgroundImage = CreateCheckerboardBitmap(
                24,
                ControlPaint.Light(canvas.AccentColor, 0.1f),
                canvas.AccentColor);
            canvasPanel.BackgroundImageLayout = ImageLayout.Tile;
        }

        private void UpdateStatusText(Point? imagePoint)
        {
            if (currentBitmap is null)
            {
                statusLabel.Text = "画像を開くか貼り付けてください。";
                return;
            }

            if (imagePoint is null || !IsInsideImage(imagePoint.Value))
            {
                statusLabel.Text = $"{currentBitmap.Width} x {currentBitmap.Height}";
                return;
            }

            var color = currentBitmap.GetPixel(imagePoint.Value.X, imagePoint.Value.Y);
            statusLabel.Text = $"({imagePoint.Value.X},{imagePoint.Value.Y}) A={color.A} RGB=({color.R},{color.G},{color.B})";
        }

        private bool ConfirmDiscardChanges()
        {
            if (!isDirty || currentBitmap is null)
            {
                return false;
            }

            return MessageBox.Show(this, "未保存の変更があります。破棄して続行しますか。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes;
        }

        private void ShowHelpMessage()
        {
            var text =
                "左クリックで透過または半透明化します。\r\n" +
                "塗りつぶしは連続領域、色選択は同色全体、四角形は範囲指定、消しゴムはドラッグ対応です。\r\n" +
                "四角形と消しゴムは Shift を押しながら使うと不透明(255)へ戻せます。\r\n" +
                "Ctrl+マウスホイールで拡大率を変更できます。\r\n" +
                "メニューの「画像変換」から、画像やフォルダのドラッグ&ドロップで PNG / JPEG / BMP への一括変換ができます。";

            MessageBox.Show(this, text, "使い方", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool IsInsideImage(Point point)
        {
            return currentBitmap is not null &&
                   point.X >= 0 &&
                   point.Y >= 0 &&
                   point.X < currentBitmap.Width &&
                   point.Y < currentBitmap.Height;
        }

        //-------------------------------------------------------------------------------
        // 透明部分のクリックでRGBだけが一致する別領域を巻き込まないよう判定する処理
        //-------------------------------------------------------------------------------
        private bool ShouldIgnoreTransparentClick(Point point, int targetAlpha)
        {
            return currentBitmap is not null &&
                   targetAlpha != 255 &&
                   IsInsideImage(point) &&
                   currentBitmap.GetPixel(point.X, point.Y).A == 0;
        }

        //-------------------------------------------------------------------------------
        // 再描画やキャンバスサイズ更新の前後でスクロール位置を維持する処理
        //-------------------------------------------------------------------------------
        private void PreserveCanvasScroll(Action updateAction)
        {
            var scrollX = -canvasPanel.AutoScrollPosition.X;
            var scrollY = -canvasPanel.AutoScrollPosition.Y;

            updateAction();

            if (scrollX <= 0 && scrollY <= 0)
            {
                return;
            }

            RestoreCanvasScroll(scrollX, scrollY);

            if (canvasPanel.IsHandleCreated)
            {
                BeginInvoke(() => RestoreCanvasScroll(scrollX, scrollY));
            }
        }

        //-------------------------------------------------------------------------------
        // AutoScroll の遅延レイアウト後にも指定位置へ戻す処理
        //-------------------------------------------------------------------------------
        private void RestoreCanvasScroll(int scrollX, int scrollY)
        {
            canvasPanel.AutoScrollPosition = new Point(
                Math.Clamp(scrollX, 0, canvasPanel.HorizontalScroll.Maximum),
                Math.Clamp(scrollY, 0, canvasPanel.VerticalScroll.Maximum));
        }

        private Point ClampToImage(Point point)
        {
            if (currentBitmap is null)
            {
                return point;
            }

            return new Point(
                Math.Clamp(point.X, 0, currentBitmap.Width - 1),
                Math.Clamp(point.Y, 0, currentBitmap.Height - 1));
        }

        private static Rectangle CreateRectangle(Point start, Point end)
        {
            var left = Math.Min(start.X, end.X);
            var top = Math.Min(start.Y, end.Y);
            var right = Math.Max(start.X, end.X) + 1;
            var bottom = Math.Max(start.Y, end.Y) + 1;
            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        //-------------------------------------------------------------------------------
        // 主線候補を膨張させた保護マスクを作成する処理
        //-------------------------------------------------------------------------------
        private static bool[] CreateProtectedLineMask(PixelBuffer pixels, Color sourceColor, int tolerance)
        {
            var sourceLuminance = GetLuminance(sourceColor);
            var lineMask = new bool[pixels.Width * pixels.Height];

            for (var y = 0; y < pixels.Height; y++)
            {
                for (var x = 0; x < pixels.Width; x++)
                {
                    var color = pixels.GetColor(x, y);
                    if (IsProtectedLineCandidate(sourceColor, sourceLuminance, color, tolerance))
                    {
                        lineMask[(y * pixels.Width) + x] = true;
                    }
                }
            }

            var protectedMask = new bool[lineMask.Length];
            for (var y = 0; y < pixels.Height; y++)
            {
                for (var x = 0; x < pixels.Width; x++)
                {
                    if (!lineMask[(y * pixels.Width) + x])
                    {
                        continue;
                    }

                    for (var yy = -LineProtectionRadius; yy <= LineProtectionRadius; yy++)
                    {
                        for (var xx = -LineProtectionRadius; xx <= LineProtectionRadius; xx++)
                        {
                            if ((xx * xx) + (yy * yy) > LineProtectionRadius * LineProtectionRadius)
                            {
                                continue;
                            }

                            var targetX = x + xx;
                            var targetY = y + yy;
                            if (targetX < 0 || targetY < 0 || targetX >= pixels.Width || targetY >= pixels.Height)
                            {
                                continue;
                            }

                            protectedMask[(targetY * pixels.Width) + targetX] = true;
                        }
                    }
                }
            }

            return protectedMask;
        }

        //-------------------------------------------------------------------------------
        // 主線として保護する色かどうかを判定する処理
        //-------------------------------------------------------------------------------
        private static bool IsProtectedLineCandidate(Color sourceColor, int sourceLuminance, Color targetColor, int tolerance)
        {
            if (targetColor.A <= 32 || IsColorMatch(sourceColor, targetColor, tolerance))
            {
                return false;
            }

            var targetLuminance = GetLuminance(targetColor);
            var maxDelta = Math.Max(
                Math.Abs(sourceColor.R - targetColor.R),
                Math.Max(Math.Abs(sourceColor.G - targetColor.G), Math.Abs(sourceColor.B - targetColor.B)));

            return targetLuminance < 96 ||
                   targetLuminance < sourceLuminance - 35 ||
                   (targetLuminance < 180 && maxDelta > tolerance + 80);
        }

        //-------------------------------------------------------------------------------
        // RGBの各成分が誤差許容範囲内か判定する処理
        //-------------------------------------------------------------------------------
        private static bool IsColorMatch(Color source, Color target, int tolerance)
        {
            return Math.Abs(source.R - target.R) <= tolerance &&
                   Math.Abs(source.G - target.G) <= tolerance &&
                   Math.Abs(source.B - target.B) <= tolerance;
        }

        //-------------------------------------------------------------------------------
        // 色の明るさを算出する処理
        //-------------------------------------------------------------------------------
        private static int GetLuminance(Color color)
        {
            return (int)Math.Round((color.R * 0.299) + (color.G * 0.587) + (color.B * 0.114));
        }

        //-------------------------------------------------------------------------------
        // Bitmap を 32bit ARGB の配列として編集する処理
        //-------------------------------------------------------------------------------
        private static void EditBitmapPixels(Bitmap bitmap, Action<PixelBuffer> editAction)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                var buffer = new byte[Math.Abs(data.Stride) * bitmap.Height];
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
                editAction(new PixelBuffer(buffer, bitmap.Width, bitmap.Height, data.Stride));
                Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        //-------------------------------------------------------------------------------
        // Bitmap を 32bit ARGB の配列として読み取る処理
        //-------------------------------------------------------------------------------
        private static void ReadBitmapPixels(Bitmap bitmap, Action<PixelBuffer> readAction)
        {
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                var buffer = new byte[Math.Abs(data.Stride) * bitmap.Height];
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
                readAction(new PixelBuffer(buffer, bitmap.Width, bitmap.Height, data.Stride));
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        //-------------------------------------------------------------------------------
        // 透明ピクセルに隣接しているかを判定する処理
        //-------------------------------------------------------------------------------
        private static bool HasTransparentNeighbor(PixelBuffer pixels, int x, int y)
        {
            for (var yy = -1; yy <= 1; yy++)
            {
                for (var xx = -1; xx <= 1; xx++)
                {
                    if (xx == 0 && yy == 0)
                    {
                        continue;
                    }

                    if (pixels.GetAlpha(x + xx, y + yy) == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //-------------------------------------------------------------------------------
        // ふちのピクセルに設定する不透明度を算出する処理
        //-------------------------------------------------------------------------------
        private static int EstimateEdgeAlpha(PixelBuffer pixels, int x, int y)
        {
            var transparentNeighbors = 0;
            for (var yy = -1; yy <= 1; yy++)
            {
                for (var xx = -1; xx <= 1; xx++)
                {
                    if (xx == 0 && yy == 0)
                    {
                        continue;
                    }

                    if (pixels.GetAlpha(x + xx, y + yy) == 0)
                    {
                        transparentNeighbors++;
                    }
                }
            }

            return Math.Clamp(255 - (transparentNeighbors * 32), 0, 255);
        }

        private static Bitmap CloneBitmap(Image source)
        {
            var bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.DrawImage(source, 0, 0, source.Width, source.Height);
            return bitmap;
        }

        private static Bitmap CreateCheckerboardBitmap(int cellSize, Color lightColor, Color darkColor)
        {
            var bitmap = new Bitmap(cellSize * 2, cellSize * 2, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            using var lightBrush = new SolidBrush(lightColor);
            using var darkBrush = new SolidBrush(darkColor);
            graphics.FillRectangle(lightBrush, 0, 0, bitmap.Width, bitmap.Height);
            graphics.FillRectangle(darkBrush, 0, 0, cellSize, cellSize);
            graphics.FillRectangle(darkBrush, cellSize, cellSize, cellSize, cellSize);
            return bitmap;
        }

        private void CopyBitmapWithTransparencyToClipboard(Bitmap source)
        {
            var bitmapCopy = CloneBitmap(source);
            using var pngStream = new MemoryStream();
            bitmapCopy.Save(pngStream, ImageFormat.Png);
            pngStream.Position = 0;

            var dataObject = new DataObject();
            dataObject.SetData("PNG", false, pngStream);
            dataObject.SetImage(bitmapCopy);
            Clipboard.SetDataObject(dataObject, true);
        }

        private Bitmap? GetBitmapFromClipboardPreservingTransparency()
        {
            IDataObject? dataObject;

            try
            {
                dataObject = Clipboard.GetDataObject();
            }
            catch
            {
                return null;
            }

            if (dataObject?.GetDataPresent("PNG") == true &&
                dataObject.GetData("PNG") is Stream pngStream)
            {
                using var buffer = new MemoryStream();
                pngStream.Position = 0;
                pngStream.CopyTo(buffer);
                buffer.Position = 0;
                using var image = Image.FromStream(buffer);
                return CloneBitmap(image);
            }

            if (dataObject?.GetDataPresent(DataFormats.Bitmap) == true &&
                dataObject.GetData(DataFormats.Bitmap) is Image imageFromClipboard)
            {
                return CloneBitmap(imageFromClipboard);
            }

            return null;
        }

        private void ClearHistory()
        {
            foreach (var bitmap in undoHistory)
            {
                bitmap.Dispose();
            }
            undoHistory.Clear();

            foreach (var bitmap in redoHistory)
            {
                bitmap.Dispose();
            }
            redoHistory.Clear();
        }

        private int SelectedAlpha => (int)alphaInput.Value;

        private int Tolerance => (int)toleranceInput.Value;

        private readonly struct PixelBuffer(byte[] buffer, int width, int height, int stride)
        {
            public int Width { get; } = width;

            public int Height { get; } = height;

            //-------------------------------------------------------------------------------
            // 指定座標の色を取得する処理
            //-------------------------------------------------------------------------------
            public Color GetColor(int x, int y)
            {
                var offset = GetOffset(x, y);
                return Color.FromArgb(buffer[offset + 3], buffer[offset + 2], buffer[offset + 1], buffer[offset]);
            }

            //-------------------------------------------------------------------------------
            // 指定座標の不透明度を取得する処理
            //-------------------------------------------------------------------------------
            public int GetAlpha(int x, int y)
            {
                return buffer[GetOffset(x, y) + 3];
            }

            //-------------------------------------------------------------------------------
            // 指定座標の不透明度を設定する処理
            //-------------------------------------------------------------------------------
            public void SetAlpha(int x, int y, int alpha)
            {
                buffer[GetOffset(x, y) + 3] = (byte)Math.Clamp(alpha, 0, 255);
            }

            //-------------------------------------------------------------------------------
            // 指定座標の色が基準色の誤差許容範囲内か判定する処理
            //-------------------------------------------------------------------------------
            public bool IsColorMatch(int x, int y, Color source, int tolerance)
            {
                var offset = GetOffset(x, y);
                return Math.Abs(buffer[offset + 2] - source.R) <= tolerance &&
                       Math.Abs(buffer[offset + 1] - source.G) <= tolerance &&
                       Math.Abs(buffer[offset] - source.B) <= tolerance;
            }

            //-------------------------------------------------------------------------------
            // 指定座標の配列位置を取得する処理
            //-------------------------------------------------------------------------------
            private int GetOffset(int x, int y)
            {
                return (y * Math.Abs(stride)) + (x * 4);
            }
        }
    }
}
