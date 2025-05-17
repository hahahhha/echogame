using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using echogame.Models;

namespace echogame.Views
{
    public class DarknessBMView
    {
        private readonly PlayerModel player;
        private readonly DarknessBMModel model;

        public DarknessBMView(PlayerModel player, DarknessBMModel model)
        {
            this.player = player;
            this.model = model;
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(model.Bitmap, 0, 0);
        }
    }
}
