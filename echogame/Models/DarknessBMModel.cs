using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace echogame.Models
{
    public class DarknessBMModel : IDisposable
    {
        public bool IsActive { get; set; } = true;

        public PointF Position { get; private set; }
        public Size Size { get; private set; }
        public PointF Center => new PointF(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);

        private readonly PlayerModel player;
        private readonly BallsManager ballsManager;
        private static readonly Color bgColor = Color.Black;

        private List<Lighter> lighters;
        private BitmapData bmpData;
        private byte[] pixelBuffer;
        private int stride;
        private readonly object bitmapLock = new object();

        private LevelManager levelManager;

        public Bitmap Bitmap { get; private set; }

        public DarknessBMModel(Size size, PlayerModel player, BallsManager ballsManager, LevelManager levelManager)
        {
            Position = new Point(0, 0);
            Size = size;
            this.player = player;
            this.ballsManager = ballsManager;
            this.levelManager = levelManager;
            this.lighters = levelManager.GetCurrentLevelLightersMng().GetActiveLighters();
            levelManager.LevelChanged += ForceRedraw;
            InitializeBitmap();

            this.player.PositionChanged += OnPlayerPositionChanged;
            this.ballsManager.AnyBallPositionChanged += OnAnyBallPositionChanged;
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
                    var region = GetObjectRegion(lighter.Model.Center, lighter.Model.LightRadius);
                    UpdateRegionInternal(region);
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
            Monitor.Enter(bitmapLock);
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
            if (Monitor.IsEntered(bitmapLock))
            {
                Monitor.Exit(bitmapLock);
            }
        }

        public void ClearBitmap()
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
                UpdateRegionInternal(region);
            }
            finally
            {
                UnlockBitmap();
            }
        }

        private void UpdateRegionInternal(Rectangle region)
        {
            for (int y = region.Top; y < region.Bottom; y++)
            {
                for (int x = region.Left; x < region.Right; x++)
                {
                    if (x >= 0 && x < Bitmap.Width && y >= 0 && y < Bitmap.Height)
                    {
                        int offset = y * stride + x * 4;
                        UpdatePixelInternal(offset, x, y);
                    }
                }
            }
        }

        public void OnPlayerPositionChanged()
        {
            if (!IsActive) return;
            var currentRegion = GetLightRegion(player.CenterF, player.LightRadius);
            var previousRegion = GetLightRegion(player.PreviousCenter, player.LightRadius);
            UpdateCombinedRegion(currentRegion, previousRegion);
        }

        public void OnAnyBallPositionChanged(BouncingBall ball)
        {
            if (!IsActive) return;
            var currentRegion = GetLightRegion(ball.Model.CenterF, ball.Model.LightRadius);
            var previousRegion = GetLightRegion(ball.Model.PreviousCenter, ball.Model.LightRadius);
            UpdateCombinedRegion(currentRegion, previousRegion);
        }

        private void UpdateCombinedRegion(Rectangle region1, Rectangle region2)
        {
            // Объединяем две области для перерисовки
            var left = Math.Min(region1.Left, region2.Left);
            var top = Math.Min(region1.Top, region2.Top);
            var right = Math.Max(region1.Right, region2.Right);
            var bottom = Math.Max(region1.Bottom, region2.Bottom);

            var combined = Rectangle.FromLTRB(left, top, right, bottom);
            UpdateRegion(combined);
        }

        private void UpdatePixelInternal(int offset, int x, int y)
        {
            var alpha = 255; // Начинаем с полной темноты

            // 1. Освещение от игрока
            var playerDistSq = CalculateDistanceSquared(x, y, player.CenterF);
            alpha = Math.Min(alpha, CalculateAlpha(playerDistSq, player.LightRadius));

            // 2. Освещение от шаров
            foreach (var ball in ballsManager.ActiveBalls().Where(b => b.Model.IsAlive))
            {
                var ballDistSq = CalculateDistanceSquared(x, y, ball.Model.CenterF);
                alpha = Math.Min(alpha, CalculateAlpha(ballDistSq, ball.Model.LightRadius));
            }

            // 3. Освещение от фонарей
            foreach (var lighter in lighters)
            {
                var lighterDistSq = CalculateDistanceSquared(x, y, lighter.Model.Center);
                alpha = Math.Min(alpha, CalculateAlpha(lighterDistSq, lighter.Model.LightRadius));
            }

            pixelBuffer[offset + 3] = (byte)alpha;
        }

        private float CalculateDistanceSquared(int x, int y, PointF center)
        {
            return (center.X - x) * (center.X - x) + (center.Y - y) * (center.Y - y);
        }

        private Rectangle GetLightRegion(PointF center, float radius)
        {
            var left = (int)Math.Max(0, center.X - radius);
            var top = (int)Math.Max(0, center.Y - radius);
            var right = (int)Math.Min(Bitmap.Width, center.X + radius);
            var bottom = (int)Math.Min(Bitmap.Height, center.Y + radius);

            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        private Rectangle GetObjectRegion(Point position, int radius)
        {
            return GetLightRegion(new PointF(position.X, position.Y), radius);
        }

        private static int CalculateAlpha(float distanceSquared, float radius)
        {
            var radiusSquared = radius * radius;

            if (distanceSquared <= radiusSquared / 16)
                return 0;

            if (distanceSquared <= radiusSquared)
            {
                var distance = (float)Math.Sqrt(distanceSquared);
                var t = (distance - radius / 4f) / (radius * 0.75f);
                t = t.Clamp(0f, 1f);
                var smoothT = (float)Math.Pow(t, 0.5f);
                return (int)(255 * smoothT);
            }

            return 255;
        }

        public void ForceRedraw()
        {
            if (!IsActive) return;
            lighters = levelManager.GetCurrentLevelLightersMng().GetActiveLighters();
            LockBitmap();
            try
            {
                ClearBitmap();

                // Перерисовываем все источники света
                foreach (var lighter in lighters)
                {
                    var region = GetObjectRegion(lighter.Model.Center, lighter.Model.LightRadius);
                    UpdateRegionInternal(region);
                }

                // Перерисовываем игрока
                var playerRegion = GetLightRegion(player.CenterF, player.LightRadius);
                UpdateRegionInternal(playerRegion);

                // Перерисовываем все мячи
                foreach (var ball in ballsManager.ActiveBalls().Where(b => b.Model.IsAlive))
                {
                    var ballRegion = GetLightRegion(ball.Model.CenterF, ball.Model.LightRadius);
                    UpdateRegionInternal(ballRegion);
                }
            }
            finally
            {
                UnlockBitmap();
            }
            Console.WriteLine("forced redraw");
        }

        public void Dispose()
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }
        }
    }
}