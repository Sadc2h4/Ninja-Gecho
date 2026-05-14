using System.Drawing.Drawing2D;

namespace Image_ToukaMan
{
    internal sealed class ImageCanvas : Control
    {
        private Bitmap? image;
        private float zoom = 1.0f;
        private Rectangle selectionRectangle;

        public ImageCanvas()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public Bitmap? Image
        {
            get => image;
            set
            {
                image = value;
                Invalidate();
            }
        }

        public float Zoom
        {
            get => zoom;
            set
            {
                zoom = Math.Max(0.1f, value);
                Invalidate();
            }
        }

        public Color AccentColor { get; set; } = Color.FromArgb(96, 96, 96);

        public Rectangle SelectionRectangle
        {
            get => selectionRectangle;
            set
            {
                selectionRectangle = value;
                Invalidate();
            }
        }

        public Point ClientToImage(Point point)
        {
            return new Point(
                (int)Math.Floor(point.X / zoom),
                (int)Math.Floor(point.Y / zoom));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawCheckerboard(e.Graphics);

            if (image is not null)
            {
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                e.Graphics.DrawImage(image, new Rectangle(0, 0, Width, Height));
            }

            if (!selectionRectangle.IsEmpty)
            {
                var rect = new Rectangle(
                    (int)Math.Round(selectionRectangle.X * zoom),
                    (int)Math.Round(selectionRectangle.Y * zoom),
                    Math.Max(1, (int)Math.Round(selectionRectangle.Width * zoom)),
                    Math.Max(1, (int)Math.Round(selectionRectangle.Height * zoom)));

                using var fillBrush = new SolidBrush(Color.FromArgb(70, Color.DeepSkyBlue));
                using var borderPen = new Pen(Color.DeepSkyBlue, 1) { DashStyle = DashStyle.Dash };
                e.Graphics.FillRectangle(fillBrush, rect);
                e.Graphics.DrawRectangle(borderPen, rect);
            }
        }

        private void DrawCheckerboard(Graphics graphics)
        {
            const int cell = 16;
            using var lightBrush = new SolidBrush(ControlPaint.Light(AccentColor, 0.1f));
            using var darkBrush = new SolidBrush(AccentColor);

            for (var y = 0; y < Height; y += cell)
            {
                for (var x = 0; x < Width; x += cell)
                {
                    var brush = ((x / cell) + (y / cell)) % 2 == 0 ? lightBrush : darkBrush;
                    graphics.FillRectangle(brush, x, y, cell, cell);
                }
            }
        }
    }
}
