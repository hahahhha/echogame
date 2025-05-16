using echogame.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame.Views
{
    //var image = Image.FromFile("../../Textures/darkness_mask.png");
    //var destRect = new Rectangle(-800, -400, image.Width, image.Height); // Подгоняем под размер формы
    //e.Graphics.DrawImage(image, destRect);
    public class DarknessView
    {
        private DarknessModel darknessModel;
        private Image texture;
        private RectangleF drawRect;

        public DarknessView(DarknessModel model, string path)
        {
            this.darknessModel = model;
            texture = Image.FromFile(path);
            drawRect = new RectangleF(darknessModel.Position, texture.Size);
        }

        public void Draw(Graphics g)
        {
            var newPos = new PointF(darknessModel.Position.X - texture.Width / 2,
                darknessModel.Position.Y - texture.Height / 2);
            drawRect = new RectangleF(newPos, texture.Size);
            g.DrawImage(texture, drawRect);
        }
    }
}
