using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace echogame.Models
{
    public class LighterModel
    {
        public int Radius { get; private set; }
        public Point Position { get; private set; }
        public Point Center => new Point(Position.X / 2 + Radius, Position.Y / 2 + Radius);

        public LighterModel(int radius, Point position)
        {
            Radius = radius;
            Position = position;
        }
    }
}
