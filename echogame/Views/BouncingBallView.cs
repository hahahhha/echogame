using echogame.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame.Views
{
    public class BouncingBallView
    {
        private BouncingBallModel model;
        private Brush _brush = new SolidBrush(Color.White);

        public BouncingBallView(BouncingBallModel ballModel)
        {
            model = ballModel;
        }

        public void Draw(Graphics g)
        {
            if (model.IsAlive)
                g.FillEllipse(_brush, new RectangleF(model.Position, model.BallSize));
        }
    }
}
