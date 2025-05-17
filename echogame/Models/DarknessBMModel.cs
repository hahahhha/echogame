using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace echogame.Models
{
    public class DarknessBMModel
    {
        public PointF Position { get; private set; }
        public Size Size { get; private set; }
        public PointF Center => new PointF(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);

        private readonly PlayerModel player;
        private readonly BallsManager ballsManager;
        private readonly List<Lighter> lighters;
        private readonly int darkRadius;
        private static readonly Color bgColor = Color.Black;

        private BitmapData bmpData;
        private byte[] pixelBuffer;
        private int stride;

        public Bitmap Bitmap { get; private set; }

        public DarknessBMModel(Size size, PlayerModel player, BallsManager ballsManager,
                             List<Lighter> lighters, int darkRadius)
        {
            Position = new Point(0, 0);
            Size = size;
            this.player = player;
            this.ballsManager = ballsManager;
            this.lighters = lighters;
            this.darkRadius = darkRadius;

            this.player.PositionChanged += OnPlayerPositionChanged;
            this.ballsManager.AnyBallPositionChanged += OnAnyBallPositionChanged;

            InitializeBitmap();
        }

        public void InitializeBitmap()
        {
            Bitmap = new Bitmap(Size.Width, Size.Height, PixelFormat.Format32bppArgb);
            LockBitmap();
            try
            {
                ClearBitmap();
                // Первоначальная отрисовка света от всех фонарей
                foreach (var lighter in lighters)
                {
                    var region = GetObjectRegion(lighter.Model.Position, lighter.Model.Radius);
                    UpdateRegion(region);
                }
            }
            finally
            {
                UnlockBitmap();
            }
            OnPlayerPositionChanged();
        }

        private void LockBitmap()
        {
            bmpData = Bitmap.LockBits(
                new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            stride = bmpData.Stride;
            pixelBuffer = new byte[stride * Bitmap.Height];
            Marshal.Copy(bmpData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
        }

        private void UnlockBitmap()
        {
            if (bmpData != null)
            {
                Marshal.Copy(pixelBuffer, 0, bmpData.Scan0, pixelBuffer.Length);
                Bitmap.UnlockBits(bmpData);
                bmpData = null;
            }
        }

        private void ClearBitmap()
        {
            for (int i = 0; i < pixelBuffer.Length; i += 4)
            {
                pixelBuffer[i] = bgColor.B;
                pixelBuffer[i + 1] = bgColor.G;
                pixelBuffer[i + 2] = bgColor.R;
                pixelBuffer[i + 3] = 255;
            }
        }

        private void UpdateRegion(Rectangle region)
        {
            LockBitmap();
            try
            {
                for (int y = region.Top; y < region.Bottom; y++)
                {
                    for (int x = region.Left; x < region.Right; x++)
                    {
                        if (x >= 0 && x < Bitmap.Width && y >= 0 && y < Bitmap.Height)
                        {
                            int offset = y * stride + x * 4;
                            UpdatePixel(offset, x, y);
                        }
                    }
                }
            }
            finally
            {
                UnlockBitmap();
            }
        }

        private void UpdatePixel(int offset, int x, int y)
        {
            // Освещение от игрока
            float playerDistSq = (player.CenterF.X - x) * (player.CenterF.X - x) +
                               (player.CenterF.Y - y) * (player.CenterF.Y - y);
            int alpha = CalculateAlpha(playerDistSq, player.LightRadius);

            // Освещение от шаров
            foreach (var activeBall in ballsManager.ActiveBalls().Where(b => b.Model.IsAlive))
            {
                float ballDistSq = (activeBall.Model.Center.X - x) * (activeBall.Model.Center.X - x) +
                                  (activeBall.Model.Center.Y - y) * (activeBall.Model.Center.Y - y);
                alpha = Math.Min(alpha, CalculateAlpha(ballDistSq, 20));
            }

            // Освещение от фонарей
            foreach (var lighter in lighters)
            {
                float lighterDistSq = (lighter.Model.Center.X - x) * (lighter.Model.Center.X - x) +
                                     (lighter.Model.Center.Y - y) * (lighter.Model.Center.Y - y);
                alpha = Math.Min(alpha, CalculateAlpha(lighterDistSq, lighter.Model.Radius));
            }

            pixelBuffer[offset + 3] = (byte)alpha;
        }

        private void OnPlayerPositionChanged()
        {
            var region = GetUpdateRegion(player.Center, player.PreviousCenter);
            UpdateRegion(region);
        }

        public void OnAnyBallPositionChanged(BouncingBall ball)
        {
            var region = GetUpdateRegion(ball.Model.Center, ball.Model.PreviousCenter);
            LockBitmap();
            try
            {
                for (int y = region.Top; y < region.Bottom; y++)
                {
                    for (int x = region.Left; x < region.Right; x++)
                    {
                        if (x >= 0 && x < Bitmap.Width && y >= 0 && y < Bitmap.Height)
                        {
                            int offset = y * stride + x * 4;

                            if (!ball.Model.IsAlive && IsInRadius(x, y, ball.Model.PreviousCenter, darkRadius))
                            {
                                UpdatePixel(offset, x, y);
                                continue;
                            }

                            UpdatePixel(offset, x, y);
                        }
                    }
                }
            }
            finally
            {
                UnlockBitmap();
            }
        }

        private bool IsInRadius(int x, int y, Point center, float radius)
        {
            var dx = center.X - x;
            var dy = center.Y - y;
            return dx * dx + dy * dy <= radius * radius;
        }

        private Rectangle GetUpdateRegion(Point current, Point previous)
        {
            var minX = Math.Max(0, Math.Min(current.X, previous.X) - darkRadius - 1);
            var minY = Math.Max(0, Math.Min(current.Y, previous.Y) - darkRadius - 1);
            var maxX = Math.Min(Bitmap.Width, Math.Max(current.X, previous.X) + darkRadius + 1);
            var maxY = Math.Min(Bitmap.Height, Math.Max(current.Y, previous.Y) + darkRadius + 1);

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        private Rectangle GetObjectRegion(Point position, int radius)
        {
            return new Rectangle(
                Math.Max(0, position.X - radius - 1),
                Math.Max(0, position.Y - radius - 1),
                Math.Min(radius * 2 + 2, Bitmap.Width - position.X + radius + 1),
                Math.Min(radius * 2 + 2, Bitmap.Height - position.Y + radius + 1));
        }

        private static int CalculateAlpha(float distanceSquared, float radius)
        {
            var radiusSquared = radius * radius;

            if (distanceSquared <= radiusSquared / 16)
                return 0;

            if (distanceSquared <= radiusSquared)
            {
                var distance = (float)Math.Sqrt(distanceSquared);
                float t = (distance - radius / 4f) / (radius * 0.75f);
                t = t.Clamp(0f, 1f);
                float smoothT = (float)Math.Pow(t, 0.5f);
                return (int)(255 * smoothT);
            }

            return 255;
        }
    }
}