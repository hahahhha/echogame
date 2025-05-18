using echogame.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace echogame.Views
{
    public class LevelView
    {
        private readonly LevelModel levelModel;

        private Bitmap levelBitmap;
        private readonly object bufferLockObj = new object();

        public LevelView(LevelModel levelModel)
        {
            this.levelModel = levelModel;
            this.levelModel.LevelChanged += UpdateLevelBitmap;
            BuildLevelBuffer();
        }

        private void BuildLevelBuffer()
        {
            lock (bufferLockObj)
            {
                if (levelBitmap != null)
                    levelBitmap.Dispose();
                var toDraw = levelModel.LevelState.ImagesToDrawAtPoints;

                levelBitmap = new Bitmap(800, 600, PixelFormat.Format32bppArgb);

                using (var g = Graphics.FromImage(levelBitmap))
                {
                    foreach (var point in toDraw.Keys)
                    {
                        g.DrawImage(toDraw[point], point.X, point.Y, levelModel.CellWidth, levelModel.CellHeight);
                    }
                }
            }
           
        }

        public void UpdateLevelBitmap()
        {
            BuildLevelBuffer();
        }

        public void Draw(Graphics g)
        {
            if (levelBitmap != null)
            {
                g.DrawImage(levelBitmap, 0, 0);
            }
        }
    }
}
