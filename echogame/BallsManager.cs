using echogame.Controls;
using echogame.Models;
using echogame.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame
{
    public class BallsManager
    {
        private readonly List<BouncingBall> balls = new List<BouncingBall>();
        private readonly PlayerModel playerModel;
        private readonly LevelManager levelManager;

        public event Action<BouncingBall> AnyBallPositionChanged;

        public BallsManager(PlayerModel playerModel, LevelManager levelManager)
        {
            this.playerModel = playerModel;
            this.levelManager = levelManager;
        }

        public void CreateBullet(int speedX, int speedY)
        {
            if (balls.Count(b => b.Model.IsAlive) >= 2) return;
            var model = new BouncingBallModel(
                playerModel.Position,
                new Size(10, 10),
                speedX,
                speedY,
                levelManager);

            var view = new BouncingBallView(model);
            var controller = new BouncingBallController(model);

            var ball = new BouncingBall(model, view, controller);
            balls.Add(ball);
        }

        //private void RemoveBullet(BouncingBall bullet)
        //{
        //    balls.Remove(bullet);
        //    BulletDestroyed?.Invoke(bullet);
        //}

        public void Update(float deltaTime)
        {
            foreach (var ball in balls)
            {
                ball.Controller.Update(deltaTime);
                AnyBallPositionChanged?.Invoke(ball);
                
            }
        }

        public IEnumerable<BouncingBall> ActiveBalls()
        {
            return balls.Where(ball => ball.Model.IsAlive);
        }

        public void DrawAllBalls(Graphics g)
        {
            foreach (var ball in balls)
            {
                ball.View.Draw(g);
            }
        }

        public void Clear()
        {
            balls.Clear();
        }
    }
}
