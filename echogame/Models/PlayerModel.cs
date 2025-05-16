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
        public Point Position { get; private set; }
        public Size PlayerSize { get; private set; }

        public PointF Center => new PointF(Position.X + PlayerSize.Width / 2, Position.Y + PlayerSize.Height / 2);
        private LevelModel levelModel;

        public int SpeedX { get; private set; }
        public int SpeedY { get; private set; }

        public event Action PositionChanged;
        //public event Action PlayerDied;

        public PlayerModel(LevelModel levelModel, Point startPosition, int cellsWidth, int cellsHeight, int speedX, int speedY)
        {
            Position = startPosition;
            PlayerSize = new Size(cellsWidth * levelModel.CellWidth, cellsHeight * levelModel.CellHeight);
            SpeedX = speedX;
            SpeedY = speedY;
            this.levelModel = levelModel;

            // пока тут подписываю
            //PlayerDied += () =>
            //{
            //    PlayerSize = new Size(0, 0);
            //};
        }

        public void Move(int deltaX, int deltaY)
        {
            Position = new Point(Position.X + deltaX, Position.Y + deltaY);
            PositionChanged.Invoke();
            Console.WriteLine("invoked");
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
            //...
        }

        public void Die()
        {
            //PlayerDied.Invoke();
        }
    }
}
