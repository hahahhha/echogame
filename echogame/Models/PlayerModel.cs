using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame.Models
{
    public class PlayerModel
    {
        // model не знает ни про control, ни про view
        public readonly int LightRadius = 50;

        public Point Position { get; private set; }
        public Point PreviousPosition { get; private set; }
        public Size PlayerSize { get; private set; }

        public PointF CenterF => new PointF(Position.X + PlayerSize.Width / 2, Position.Y + PlayerSize.Height / 2);
        public Point Center => new Point(Position.X + PlayerSize.Width / 2, Position.Y + PlayerSize.Height / 2);
        public Point PreviousCenter => new Point(PreviousPosition.X + PlayerSize.Width / 2, PreviousPosition.Y + PlayerSize.Height / 2);
        public Rectangle Collider => new Rectangle(Position, PlayerSize);

        private LevelModel levelModel;

        public int SpeedX { get; private set; }
        public int SpeedY { get; private set; }


        public event Action PositionChanged;

        public PlayerModel(LevelModel levelModel, Point startPosition, int cellsWidth, int cellsHeight, int speedX, int speedY)
        {
            Position = startPosition;
            PlayerSize = new Size(cellsWidth * levelModel.CellWidth, cellsHeight * levelModel.CellHeight);
            SpeedX = speedX;
            SpeedY = speedY;
            this.levelModel = levelModel;
            PreviousPosition = Position;
            Move(0, 0);
        }

        public void Move(int deltaX, int deltaY)
        {
            PreviousPosition = Position;
            Position = new Point(Position.X + deltaX, Position.Y + deltaY);
            PositionChanged?.Invoke();
            //Console.WriteLine("invoked");
        }

        public bool IsStepAvialable(Point nextPoint)
        {
            var nextPointRect = new Rectangle(nextPoint, PlayerSize);
            foreach (var wallPosition in levelModel.LevelState.WallsPositions)
            {
                var wallCell = new Rectangle(wallPosition, new Size(levelModel.CellWidth, levelModel.CellHeight));
                if (wallCell.IntersectsWith(nextPointRect))
                    return false;
            }
            return true;
        }

        public void Die()
        {

        }
    }
}
