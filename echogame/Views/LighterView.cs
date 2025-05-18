using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using echogame.Models;

namespace echogame.Views
{
    public class LighterView
    {
        private readonly LighterModel model;
        public readonly Image Texture;

        public LighterView(LighterModel model, string pathToTexture)
        {
            this.model = model;
            this.Texture = Image.FromFile(pathToTexture);
        }

        public void Draw(Graphics g)
        {
            if (!model.IsAlive) return;
            g.DrawImage(Texture, model.Position);
        }
    }
}
