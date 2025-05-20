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

        private LevelManager levelManager;

        public int SpeedX { get; private set; }
        public int SpeedY { get; private set; }


        public event Action PositionChanged;

        public PlayerModel(LevelManager levelManager, int cellsWidth, int cellsHeight, int speedX, int speedY)
        {
            PlayerSize = new Size(cellsWidth * levelManager.CurrentLevel.Model.CellWidth, 
                cellsHeight * levelManager.CurrentLevel.Model.CellHeight);
            SpeedX = speedX;
            SpeedY = speedY;
            this.levelManager = levelManager;
            SetPosition(levelManager.CurrentLevel.Model.PlayerStartPos);
            levelManager.LevelNumChanged += () => { SetPosition(levelManager.CurrentLevel.Model.PlayerStartPos); };
            PreviousPosition = Position;
            Move(0, 0);
        }

        private void SetPosition(Point p)
        {
            var deltaX = -Position.X + p.X;
            var deltaY = -Position.Y + p.Y;
            Move(deltaX, deltaY);
        }

        public void Move(int deltaX, int deltaY)
        {
            PreviousPosition = Position;
            Position = new Point(Position.X + deltaX, Position.Y + deltaY);
            PositionChanged?.Invoke();
            levelManager.OnPlayerPositionChanged(Collider);
            //Console.WriteLine("invoked");
        }

        public bool IsStepAvialable(Point nextPoint)
        {
            var nextPointRect = new Rectangle(nextPoint, PlayerSize);
            return levelManager.IsPlayerStepAvialable(nextPoint, PlayerSize);
        }
    }
}
