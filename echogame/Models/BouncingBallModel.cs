using System;
using System.Drawing;
using System.Linq;

namespace echogame.Models
{
    public class BouncingBallModel
    {
        public PointF Position { get; private set; }
        public PointF PreviousPosition { get; private set; }
        public SizeF BallSize { get; private set; }
        public float Radius => BallSize.Width / 2;
        public PointF CenterF => new PointF(Position.X + Radius, Position.Y + Radius);
        public Point Center => new Point((int)Position.X + (int)Radius, (int)Position.Y + (int)Radius);
        public Point PreviousCenter => new Point((int)PreviousPosition.X + (int)Radius, (int)PreviousPosition.Y + (int)Radius);
        public bool IsAlive { get; set; }
        public float SpeedX { get; private set; }
        public float SpeedY { get; private set; }
        public int CollisionsAmount { get; private set; }

        private readonly LevelManager levelManager;

        private float epsilon = 0.001f;
        private float bounceEnergyChange = 1;
        private int collisionsAmountLimit = 6;

        public event Action PositionChanged;
        public event Action BallDestroyed;

        public BouncingBallModel(PointF position, SizeF size, float speedX, float speedY, LevelManager levelmng)
        {
            Position = position;
            BallSize = size;
            SpeedX = speedX;
            SpeedY = speedY;
            IsAlive = true;
            levelManager = levelmng;
            PreviousPosition = position;
        }

        public void Update(float deltaTime)
        {
            if (!IsAlive) return;
            PositionChanged?.Invoke();
            var remainingTime = deltaTime;
            var maxSteps = 5;

            while (remainingTime > 0 && maxSteps-- > 0)
            {
                var stepTime = Math.Min(remainingTime, 0.016f);
                remainingTime -= stepTime;

                var newCenter = new PointF(
                    CenterF.X + SpeedX * stepTime,
                    CenterF.Y + SpeedY * stepTime
                );

                foreach (var wall in levelManager.CurrentLevel.Model.LevelState.WallsGrid)
                {
                    RectangleF wallRect = new RectangleF(wall.Position, wall.Size);
                    if (CheckCircleRectangleCollision(newCenter, Radius, wallRect, out var collisionNormal, out var penetrationDepth))
                    {
                        newCenter = new PointF(
                            newCenter.X + collisionNormal.X * penetrationDepth,
                            newCenter.Y + collisionNormal.Y * penetrationDepth
                        );

                        var dotProduct = SpeedX * collisionNormal.X + SpeedY * collisionNormal.Y;
                        SpeedX = (SpeedX - 2 * dotProduct * collisionNormal.X) * bounceEnergyChange;
                        SpeedY = (SpeedY - 2 * dotProduct * collisionNormal.Y) * bounceEnergyChange;

                        CollisionsAmount++;
                        SoundEngine.Play("ball_hit");
                        break;
                    }
                }
                PreviousPosition = Position;
                Position = new PointF(newCenter.X - Radius, newCenter.Y - Radius);
            }

            PositionChanged?.Invoke();

            if (CollisionsAmount >= collisionsAmountLimit)
            {
                Destroy();
            }
        }

        private bool CheckCircleRectangleCollision(PointF circleCenter, float radius, RectangleF rect, 
            out PointF collisionNormal, out float penetrationDepth)
        {
            collisionNormal = PointF.Empty;
            penetrationDepth = 0;

            var closestX = circleCenter.X;
            if (closestX < rect.Left) closestX = rect.Left;
            if (closestX > rect.Right) closestX = rect.Right;

            var closestY = circleCenter.Y;
            if (closestY < rect.Top) closestY = rect.Top;
            if (closestY > rect.Bottom) closestY = rect.Bottom;

            var distanceX = circleCenter.X - closestX;
            var distanceY = circleCenter.Y - closestY;
            var distanceSquared = distanceX * distanceX + distanceY * distanceY;

            if (distanceSquared > radius * radius + epsilon)
                return false;

            var distance = (float)Math.Sqrt(distanceSquared);
            penetrationDepth = radius - distance + epsilon;

            if (distance < epsilon)
            {
                var left = circleCenter.X < rect.Left + rect.Width / 2;
                var top = circleCenter.Y < rect.Top + rect.Height / 2;
                collisionNormal = new PointF(left ? -1 : 1, top ? -1 : 1);
            }
            else
            {
                collisionNormal = new PointF(distanceX / distance, distanceY / distance);
            }

            return true;
        }

        public void Destroy()
        {
            IsAlive = false;
            BallDestroyed?.Invoke();
        }
    }
}